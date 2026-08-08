// Same bridge constraint as EventDispatchTests: the transport needs the no-op bridge.
#if !UNITY_IOS && !UNITY_ANDROID

using System;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The legacy onboarding events are dispatched from a method of their own, split out of the
    /// main switch so that its deprecation warnings stay in one place. These pin what that move has
    /// to preserve: every id reaching its own listener method with a built model, and nothing
    /// escaping on the way out.
    /// </summary>
    [TestFixture]
    [Obsolete("Covers the legacy onboarding API, which is deprecated in favor of Flows.")]
    public class LegacyOnboardingDispatchTests
    {
        private const string View =
            "\"view\":{\"id\":\"view-1\",\"placement_id\":\"placement-1\",\"variation_id\":\"variation-1\"}";
        private const string Meta =
            "\"meta\":{\"onboarding_id\":\"onboarding-1\",\"screen_cid\":\"screen-1\",\"screen_index\":2,\"total_screens\":5}";

        private Listener _listener;

        [SetUp]
        public void Setup()
        {
            _listener = new Listener();
            Adapty.SetOnboardingsEventsListener(_listener);
        }

        [TearDown]
        public void TearDown() => Adapty.SetOnboardingsEventsListener(null);

        [Test]
        public void DidFinishLoadingCarriesTheViewAndTheMeta()
        {
            Adapty.OnMessage("onboarding_did_finish_loading", "{" + View + "," + Meta + "}");

            Assert.That(_listener.Called, Is.EqualTo("did_finish_loading"));
            Assert.That(_listener.View.Id, Is.EqualTo("view-1"));
            Assert.That(_listener.View.PlacementId, Is.EqualTo("placement-1"));
            Assert.That(_listener.Meta.OnboardingId, Is.EqualTo("onboarding-1"));
            Assert.That(_listener.Meta.ScreenIndex, Is.EqualTo(2));
            Assert.That(_listener.Meta.ScreensTotal, Is.EqualTo(5));
        }

        [Test]
        public void DidFailWithErrorCarriesTheError()
        {
            Adapty.OnMessage(
                "onboarding_did_fail_with_error",
                "{" + View + ",\"error\":{\"adapty_code\":1004,\"message\":\"No purchases\"}}"
            );

            Assert.That(_listener.Called, Is.EqualTo("did_fail_with_error"));
            Assert.That(_listener.View.Id, Is.EqualTo("view-1"));
            Assert.That(_listener.Error.Code, Is.EqualTo(AdaptyErrorCode.NoPurchasesToRestore));
            Assert.That(_listener.Error.Message, Is.EqualTo("No purchases"));
        }

        /// <summary>
        /// Three ids that share a payload shape and differ only in the method they must reach -
        /// the case the move is most likely to get wrong.
        /// </summary>
        [TestCase("onboarding_on_close_action", "close_action")]
        [TestCase("onboarding_on_paywall_action", "paywall_action")]
        [TestCase("onboarding_on_custom_action", "custom_action")]
        public void ActionEventsReachTheirOwnMethod(string id, string expected)
        {
            Adapty.OnMessage(id, "{" + View + "," + Meta + ",\"action_id\":\"act-1\"}");

            Assert.That(_listener.Called, Is.EqualTo(expected));
            Assert.That(_listener.ActionId, Is.EqualTo("act-1"));
            Assert.That(_listener.Meta.ScreenClientId, Is.EqualTo("screen-1"));
        }

        [Test]
        public void AnalyticsEventsArriveTyped()
        {
            Adapty.OnMessage(
                "onboarding_on_analytics_action",
                "{" + View + "," + Meta + ",\"event\":{\"name\":\"onboarding_started\"}}"
            );

            Assert.That(_listener.Called, Is.EqualTo("analytics_event"));
            Assert.That(
                _listener.AnalyticsEvent,
                Is.TypeOf<AdaptyOnboardingsAnalyticsEventOnboardingStarted>()
            );
        }

        /// <summary>
        /// The one case that reads the same object twice - the element id off the raw action, the
        /// params through the converter.
        /// </summary>
        [Test]
        public void StateUpdatedCarriesTheElementIdAndTheParams()
        {
            Adapty.OnMessage(
                "onboarding_on_state_updated_action",
                "{"
                    + View
                    + ","
                    + Meta
                    + ",\"action\":{\"element_id\":\"plan\",\"element_type\":\"select\","
                    + "\"value\":{\"id\":\"plan-1\",\"value\":\"monthly\",\"label\":\"Monthly\"}}}"
            );

            Assert.That(_listener.Called, Is.EqualTo("state_updated"));
            Assert.That(_listener.ElementId, Is.EqualTo("plan"));
            Assert.That(_listener.Params, Is.TypeOf<AdaptyOnboardingsSelectParams>());
        }

        [Test]
        public void AThrowingListenerIsContained()
        {
            _listener.Throw = true;

            Assert.That(
                () =>
                    Adapty.OnMessage(
                        "onboarding_did_finish_loading",
                        "{" + View + "," + Meta + "}"
                    ),
                Throws.Nothing
            );
        }

        [TestCase("{}", TestName = "missing view and meta")]
        [TestCase("{\"view\":{}}", TestName = "view missing required fields")]
        [TestCase("not json at all", TestName = "malformed json")]
        public void BrokenPayloadsAreContained(string json)
        {
            Assert.That(
                () => Adapty.OnMessage("onboarding_did_finish_loading", json),
                Throws.Nothing
            );
            Assert.That(_listener.Called, Is.Null, "a broken payload reached the listener");
        }

        [Test]
        public void EventsWithoutAListenerAreIgnored()
        {
            Adapty.SetOnboardingsEventsListener(null);

            Assert.That(
                () =>
                    Adapty.OnMessage(
                        "onboarding_did_finish_loading",
                        "{" + View + "," + Meta + "}"
                    ),
                Throws.Nothing
            );
        }

        private sealed class Listener : IAdaptyOnboardingsEventsListener
        {
            internal string Called;
            internal AdaptyUIOnboardingView View;
            internal AdaptyUIOnboardingMeta Meta;
            internal AdaptyError Error;
            internal string ActionId;
            internal string ElementId;
            internal AdaptyOnboardingsStateUpdatedParams Params;
            internal AdaptyOnboardingsAnalyticsEvent AnalyticsEvent;
            internal bool Throw;

            private void Record(string called, AdaptyUIOnboardingView view)
            {
                Called = called;
                View = view;
                if (Throw)
                    throw new InvalidOperationException("the app's own bug");
            }

            public void OnboardingViewDidFailWithError(
                AdaptyUIOnboardingView view,
                AdaptyError error
            )
            {
                Error = error;
                Record("did_fail_with_error", view);
            }

            public void OnboardingViewDidFinishLoading(
                AdaptyUIOnboardingView view,
                AdaptyUIOnboardingMeta meta
            )
            {
                Meta = meta;
                Record("did_finish_loading", view);
            }

            public void OnboardingViewOnCloseAction(
                AdaptyUIOnboardingView view,
                AdaptyUIOnboardingMeta meta,
                string actionId
            )
            {
                Meta = meta;
                ActionId = actionId;
                Record("close_action", view);
            }

            public void OnboardingViewOnPaywallAction(
                AdaptyUIOnboardingView view,
                AdaptyUIOnboardingMeta meta,
                string actionId
            )
            {
                Meta = meta;
                ActionId = actionId;
                Record("paywall_action", view);
            }

            public void OnboardingViewOnCustomAction(
                AdaptyUIOnboardingView view,
                AdaptyUIOnboardingMeta meta,
                string actionId
            )
            {
                Meta = meta;
                ActionId = actionId;
                Record("custom_action", view);
            }

            public void OnboardingViewOnStateUpdatedAction(
                AdaptyUIOnboardingView view,
                AdaptyUIOnboardingMeta meta,
                string elementId,
                AdaptyOnboardingsStateUpdatedParams @params
            )
            {
                Meta = meta;
                ElementId = elementId;
                Params = @params;
                Record("state_updated", view);
            }

            public void OnboardingViewOnAnalyticsEvent(
                AdaptyUIOnboardingView view,
                AdaptyUIOnboardingMeta meta,
                AdaptyOnboardingsAnalyticsEvent analyticsEvent
            )
            {
                Meta = meta;
                AnalyticsEvent = analyticsEvent;
                Record("analytics_event", view);
            }
        }
    }
}

#endif
