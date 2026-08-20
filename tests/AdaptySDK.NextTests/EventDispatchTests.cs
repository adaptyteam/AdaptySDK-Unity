// The bridge is chosen by the same #if the SDK uses: off the editor it is a real P/Invoke or
// AndroidJavaClass with nothing behind it on a desktop test host. These fixtures drive the
// transport end to end, so they need the no-op bridge; device coverage is a separate stage.
// The platform-dependent payloads themselves - the custom asset paths - are pinned per platform
// by the request snapshots.
#if !UNITY_IOS && !UNITY_ANDROID

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using AdaptySDK.Noop;
using AdaptySDK.TestSupport;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Events arrive from native code. On iOS that is a reverse-P/Invoke callback with nothing
    /// behind it, so the dispatcher's first duty is to never let anything escape - and its second
    /// is to hand the listener a fully built model.
    /// </summary>
    [TestFixture]
    public class EventDispatchTests
    {
        private Listener _listener;

        [SetUp]
        public void Setup()
        {
            _listener = new Listener();
            Adapty.SetEventListener(_listener);
        }

        [TearDown]
        public void TearDown() => Adapty.SetEventListener(null);

        [Test]
        public void ProfileUpdatesReachTheListener()
        {
            Adapty.OnMessage(
                "did_load_latest_profile",
                "{\"profile\":" + Snapshots.LoadResponse("profile-minimal") + "}"
            );

            Assert.That(_listener.Profile, Is.Not.Null);
            Assert.That(_listener.Profile.ProfileId, Is.Not.Null);
        }

        /// <summary>
        /// The wire is UTC and the public API is local, on this path too: an event is a document
        /// parsed before anything typed is built out of it.
        /// </summary>
        [Test]
        public void ProfileDatesReachTheListenerAsLocalTime()
        {
            Adapty.OnMessage(
                "did_load_latest_profile",
                "{\"profile\":" + Snapshots.LoadResponse("profile-full") + "}"
            );

            Assert.That(_listener.Profile, Is.Not.Null);

            var premium = _listener.Profile.AccessLevels["premium"];

            Assert.Multiple(() =>
            {
                Assert.That(
                    premium.ActivatedAt.Kind,
                    Is.EqualTo(DateTimeKind.Local),
                    "a date reached the listener in the wrong zone"
                );
                Assert.That(
                    premium.ActivatedAt.ToUniversalTime(),
                    Is.EqualTo(new DateTime(2026, 1, 15, 9, 30, 0, DateTimeKind.Utc)),
                    "the zone was right and the instant was not"
                );
            });
        }

        /// <summary>
        /// Every way a payload can be wrong, none of which may reach the caller as an exception.
        /// </summary>
        [TestCase("not json at all", TestName = "malformed json")]
        [TestCase("[]", TestName = "not an object")]
        [TestCase("{}", TestName = "missing payload")]
        [TestCase("{\"profile\":null}", TestName = "null payload")]
        [TestCase("{\"profile\":{}}", TestName = "payload missing required fields")]
        [TestCase("{\"profile\":\"a string\"}", TestName = "payload of the wrong shape")]
        public void BrokenPayloadsAreContained(string json)
        {
            Assert.That(() => Adapty.OnMessage("did_load_latest_profile", json), Throws.Nothing);
            Assert.That(_listener.Profile, Is.Null, "a broken payload reached the listener");
        }

        /// <summary>
        /// The listener is the app's code, and it throwing is the app's bug - but it happens on the
        /// same callback, so it cannot be allowed to take the process down either.
        /// </summary>
        [Test]
        public void AThrowingListenerIsContained()
        {
            _listener.Throw = true;

            Assert.That(
                () =>
                    Adapty.OnMessage(
                        "did_load_latest_profile",
                        "{\"profile\":" + Snapshots.LoadResponse("profile-minimal") + "}"
                    ),
                Throws.Nothing
            );
        }

        [Test]
        public void UnknownEventIdsAreIgnored() =>
            Assert.That(
                () => Adapty.OnMessage("something_from_a_newer_native_sdk", "{}"),
                Throws.Nothing
            );

        [TestCase(null)]
        [TestCase("")]
        public void EmptyPayloadsAreIgnored(string json) =>
            Assert.That(() => Adapty.OnMessage("did_load_latest_profile", json), Throws.Nothing);

        /// <summary>
        /// The analytic event's params are the third payload the contract leaves untyped, and the
        /// only one that reaches the app through the dispatcher rather than through a model. It has
        /// to arrive as the CLR graph of doubles that 3.x handed over, not as Newtonsoft's own
        /// shapes.
        /// </summary>
        [Test]
        public void AnalyticEventParamsArriveAsALooseGraph()
        {
            var flows = new FlowsListener();
            Adapty.SetFlowsEventsListener(flows);

            try
            {
                Adapty.OnMessage(
                    "flow_view_did_receive_analytic_event",
                    "{\"view\":{\"id\":\"v\",\"placement_id\":\"p\",\"variation_id\":\"var\"},"
                        + "\"name\":\"purchase_started\","
                        + "\"params\":{\"count\":7,\"nested\":{\"k\":1},\"list\":[1,2],"
                        + "\"released_at\":\"2026-07-30T10:00:00.000Z\"}}"
                );

                Assert.That(flows.Params, Is.Not.Null, "the event never reached the listener");
                Assert.Multiple(() =>
                {
                    Assert.That(flows.Params["count"], Is.EqualTo(7d).And.TypeOf<double>());
                    Assert.That(flows.Params["nested"], Is.TypeOf<Dictionary<string, object>>());
                    Assert.That(flows.Params["list"], Is.TypeOf<List<object>>());

                    // An untyped payload is not the place to recognise dates: the app gets back
                    // what was sent, character for character.
                    Assert.That(
                        flows.Params["released_at"],
                        Is.EqualTo("2026-07-30T10:00:00.000Z").And.TypeOf<string>()
                    );
                });
            }
            finally
            {
                Adapty.SetFlowsEventsListener(null);
            }
        }

        /// <summary>
        /// <c>respond</c> is a delegate the SDK hands to app code, and the app may invoke it from
        /// any thread - an OS permission callback rarely arrives on the main one. The answer it
        /// produces is a request, and on Android the bridge is JNI, which a thread Unity did not
        /// attach cannot enter - so the send has to reach the bridge from the main thread, whatever
        /// thread the app answered on.
        /// </summary>
        [Test]
        public void RespondFromABackgroundThreadReachesTheBridgeOnTheMainThread()
        {
            var pump = new PumpingContext();
            var previous = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(pump);

            try
            {
                Adapty.InitializeTransport();

                string sentMethod = null;
                string sentRequest = null;
                var sentOnThread = -1;
                AdaptyNoop.Handler = (method, request) =>
                {
                    sentMethod = method;
                    sentRequest = request;
                    sentOnThread = Thread.CurrentThread.ManagedThreadId;
                    return "{\"success\":true}";
                };

                var handler = new PermissionHandler();
                Adapty.SetSystemRequestsHandler(handler);

                Adapty.OnMessage(
                    "flow_view_did_ask_permission",
                    "{\"view\":{\"id\":\"v\",\"placement_id\":\"p\",\"variation_id\":\"var\"},"
                        + "\"event_id\":\"evt-1\",\"permission\":\"camera\"}"
                );

                Assert.That(handler.Respond, Is.Not.Null, "the request never reached the handler");

                var worker = new Thread(() => handler.Respond(true, "os said yes"));
                worker.Start();
                worker.Join();

                Assert.That(sentMethod, Is.Null, "the answer reached the bridge from the worker thread");

                pump.RunAll();

                Assert.Multiple(() =>
                {
                    Assert.That(sentMethod, Is.EqualTo("flow_view_did_answer_permission"));
                    Assert.That(sentRequest, Does.Contain("\"status\":\"granted\""));
                    Assert.That(sentRequest, Does.Contain("\"detail\":\"os said yes\""));
                    Assert.That(sentOnThread, Is.EqualTo(Thread.CurrentThread.ManagedThreadId));
                });
            }
            finally
            {
                Adapty.SetSystemRequestsHandler(null);
                AdaptyNoop.Handler = null;
                SynchronizationContext.SetSynchronizationContext(previous);

                // Re-capture, so the pump this test made does not stay the SDK's main thread.
                Adapty.InitializeTransport();
            }
        }

        /// <summary>
        /// The observer-mode reports are the other delegates handed to app code, and a billing
        /// implementation answers on its own threads. Same rule, same route.
        /// </summary>
        [Test]
        public void AnObserverReportFromABackgroundThreadReachesTheBridgeOnTheMainThread()
        {
            var pump = new PumpingContext();
            var previous = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(pump);

            try
            {
                Adapty.InitializeTransport();

                string sentMethod = null;
                var sentOnThread = -1;
                AdaptyNoop.Handler = (method, request) =>
                {
                    sentMethod = method;
                    sentOnThread = Thread.CurrentThread.ManagedThreadId;
                    return "{\"success\":true}";
                };

                var resolver = new Resolver();
                Adapty.SetObserverModeResolver(resolver);

                Adapty.OnMessage(
                    "flow_view_observer_did_initiate_restore",
                    "{\"view\":{\"id\":\"v\",\"placement_id\":\"p\",\"variation_id\":\"var\"},"
                        + "\"event_id\":\"evt-2\"}"
                );

                Assert.That(resolver.StartRestore, Is.Not.Null, "the event never reached the resolver");

                var worker = new Thread(() => resolver.StartRestore());
                worker.Start();
                worker.Join();

                Assert.That(sentMethod, Is.Null, "the report reached the bridge from the worker thread");

                pump.RunAll();

                Assert.Multiple(() =>
                {
                    Assert.That(sentMethod, Is.EqualTo("observer_restore_did_start"));
                    Assert.That(sentOnThread, Is.EqualTo(Thread.CurrentThread.ManagedThreadId));
                });
            }
            finally
            {
                Adapty.SetObserverModeResolver(null);
                AdaptyNoop.Handler = null;
                SynchronizationContext.SetSynchronizationContext(previous);
                Adapty.InitializeTransport();
            }
        }

        private sealed class PumpingContext : SynchronizationContext
        {
            private readonly ConcurrentQueue<KeyValuePair<SendOrPostCallback, object>> _queue =
                new ConcurrentQueue<KeyValuePair<SendOrPostCallback, object>>();

            public override void Post(SendOrPostCallback d, object state) =>
                _queue.Enqueue(new KeyValuePair<SendOrPostCallback, object>(d, state));

            internal void RunAll()
            {
                while (_queue.TryDequeue(out var work))
                {
                    work.Key(work.Value);
                }
            }
        }

        private sealed class PermissionHandler : IAdaptyUISystemRequestsHandler
        {
            internal Action<bool, string> Respond;

            public void FlowViewDidAskPermission(
                AdaptyUIFlowView view,
                string permission,
                IReadOnlyDictionary<string, string> customArgs,
                Action<bool, string> respond
            ) => Respond = respond;

            public void FlowViewDidRequestAppReview(AdaptyUIFlowView view) { }
        }

        private sealed class Resolver : IAdaptyUIObserverModeResolver
        {
            internal Action StartRestore;

            public void FlowViewDidInitiatePurchase(
                AdaptyUIFlowView view,
                AdaptyPaywallProduct product,
                Action onStartPurchase,
                Action onFinishPurchase
            ) { }

            public void FlowViewDidInitiateRestore(
                AdaptyUIFlowView view,
                Action onStartRestore,
                Action onFinishRestore
            ) => StartRestore = onStartRestore;
        }

        private sealed class FlowsListener : IAdaptyFlowsEventsListener
        {
            internal IReadOnlyDictionary<string, object> Params;

            public void FlowViewDidReceiveAnalyticEvent(
                AdaptyUIFlowView view,
                string name,
                IReadOnlyDictionary<string, object> @params
            ) => Params = @params;

            public void FlowViewDidAppear(AdaptyUIFlowView view) { }

            public void FlowViewDidDisappear(AdaptyUIFlowView view) { }

            public void FlowViewDidPerformAction(AdaptyUIFlowView view, AdaptyUIUserAction action) { }

            public void FlowViewDidSelectProduct(AdaptyUIFlowView view, string productId) { }

            public void FlowViewDidStartPurchase(
                AdaptyUIFlowView view,
                AdaptyPaywallProduct product
            ) { }

            public void FlowViewDidFinishPurchase(
                AdaptyUIFlowView view,
                AdaptyPaywallProduct product,
                AdaptyPurchaseResult purchasedResult
            ) { }

            public void FlowViewDidFailPurchase(
                AdaptyUIFlowView view,
                AdaptyPaywallProduct product,
                AdaptyError error
            ) { }

            public void FlowViewDidStartRestore(AdaptyUIFlowView view) { }

            public void FlowViewDidFinishRestore(AdaptyUIFlowView view, AdaptyProfile profile) { }

            public void FlowViewDidFailRestore(AdaptyUIFlowView view, AdaptyError error) { }

            public void FlowViewDidReceiveError(AdaptyUIFlowView view, AdaptyError error) { }

            public void FlowViewDidFailLoadingProducts(AdaptyUIFlowView view, AdaptyError error) { }

            public void FlowViewDidFinishWebPaymentNavigation(
                AdaptyUIFlowView view,
                AdaptyPaywallProduct product,
                AdaptyError error
            ) { }
        }

        private sealed class Listener : IAdaptyEventListener
        {
            internal AdaptyProfile Profile;
            internal bool Throw;

            public void OnLoadLatestProfile(AdaptyProfile profile)
            {
                if (Throw)
                {
                    throw new InvalidOperationException("the app's own bug");
                }

                Profile = profile;
            }

            public void OnInstallationDetailsSuccess(AdaptyInstallationDetails details) { }

            public void OnInstallationDetailsFail(AdaptyError error) { }
        }
    }
}

#endif
