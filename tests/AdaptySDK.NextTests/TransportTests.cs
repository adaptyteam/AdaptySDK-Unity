// The bridge is chosen by the same #if the SDK uses: off the editor it is a real P/Invoke or
// AndroidJavaClass with nothing behind it on a desktop test host. These fixtures drive the
// transport end to end, so they need the no-op bridge; device coverage is a separate stage.
// The platform-dependent payloads themselves - the custom asset paths - are pinned per platform
// by the request snapshots.
#if !UNITY_IOS && !UNITY_ANDROID

using System;
using System.Collections.Generic;
using AdaptySDK.TestSupport;
using AdaptySDK.Noop;
using AdaptySDK.Serialization;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// What actually crosses the bridge. The public methods assemble a request key by key, so the
    /// only way to see the result is from the far side of the transport.
    /// </summary>
    [TestFixture]
    public class TransportTests
    {
        private string _method;
        private string _request;
        private string _reply;

        [SetUp]
        public void Setup()
        {
            _method = null;
            _request = null;
            _reply = "{\"success\":true}";

            AdaptyNoop.Handler = (method, request) =>
            {
                _method = method;
                _request = request;
                return _reply;
            };
        }

        [TearDown]
        public void TearDown() => AdaptyNoop.Handler = null;

        [Test]
        public void ActivateSendsTheConfiguration()
        {
            Adapty.Activate(Samples.Configuration(), _ => { });

            Assert.That(_method, Is.EqualTo("activate"));
            Snapshots.Matches("transport-activate", Snapshots.Canonical(_request));
        }

        [Test]
        public void GetFlowSendsThePlacementAndPolicy()
        {
            _reply = "{\"success\":" + Snapshots.LoadResponse("flow-minimal") + "}";

            AdaptyFlow received = null;
            Adapty.GetFlow(
                "onboarding",
                AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                    TimeSpan.FromSeconds(90)
                ),
                TimeSpan.FromSeconds(5),
                (flow, _) => received = flow
            );

            Assert.That(_method, Is.EqualTo("get_flow"));
            Assert.That(received, Is.Not.Null, "the reply was not mapped back to a model");
            Assert.That(received.InstanceIdentity, Is.Not.Null);
            Snapshots.Matches("transport-get-flow", Snapshots.Canonical(_request));
        }

        /// <summary>
        /// The optional view parameters are merged into the request rather than nested, so this
        /// pins the flattening the annotated model now performs.
        /// </summary>
        [Test]
        public void CreateFlowViewFlattensItsOptionalParameters()
        {
            var flow = AdaptyJson.Deserialize<AdaptyFlow>(Snapshots.LoadResponse("flow-minimal"));

            AdaptyUI.CreateFlowView(
                flow,
                new AdaptyUICreateFlowViewParameters()
                    .SetLocale("es")
                    .SetCustomLayoutId("tablet_wide")
                    .SetLoadTimeout(TimeSpan.FromSeconds(12))
                    .SetPreloadProducts(true)
                    .SetCustomTags(new Dictionary<string, string> { { "NAME", "Ada" } })
                    .SetCustomTimers(
                        new Dictionary<string, DateTime>
                        {
                            { "OFFER_END", new DateTime(2026, 7, 30, 10, 0, 0, DateTimeKind.Utc) },
                        }
                    )
                    .SetCustomAssets(Samples.CustomAssets())
                    .SetEnableSafeAreaPaddings(false),
                (_, __) => { }
            );

            Assert.That(_method, Is.EqualTo("adapty_ui_create_flow_view"));
            Snapshots.Matches("transport-create-flow-view", Snapshots.Canonical(_request));
        }

        [Test]
        public void UpdateExternalAttributionSendsEveryValueKind()
        {
            Adapty.UpdateExternalAttribution(
                new Dictionary<string, object>
                {
                    { "status", "organic" },
                    { "clicks", 3 },
                    { "cost", 1.5 },
                    { "is_retargeting", false },
                    { "install_time", new DateTime(2026, 7, 30, 10, 0, 0, DateTimeKind.Utc) },
                    { "campaign", null },
                    { "tags", new List<string> { "a", "b" } },
                    { "nested", new Dictionary<string, object> { { "k", "v" } } },
                },
                AdaptyExternalAttributionProvider.Appsflyer,
                _ => { }
            );

            Assert.That(_method, Is.EqualTo("update_external_attribution_data"));
            Snapshots.Matches("transport-update-external-attribution", Snapshots.Canonical(_request));
        }

        /// <summary>
        /// The dictionary overload is the one public method that encodes an argument before it can
        /// build a request, so it is the one place a serialization failure could escape the
        /// transport's guard and be thrown at the caller instead of reported to the handler.
        /// </summary>
        [Test]
        public void UpdateExternalAttributionReportsAGraphItCannotEncode()
        {
            var loop = new Dictionary<string, object>();
            loop["self"] = loop;

            AdaptyError reported = null;

            Assert.DoesNotThrow(
                () =>
                    Adapty.UpdateExternalAttribution(
                        loop,
                        AdaptyExternalAttributionProvider.Custom,
                        error => reported = error
                    )
            );

            Assert.Multiple(() =>
            {
                Assert.That(reported, Is.Not.Null, "the completion handler was never called");
                Assert.That(reported?.Code, Is.EqualTo(AdaptyErrorCode.WrongParam));
                Assert.That(_method, Is.Null, "nothing should have reached the bridge");
            });
        }

        /// <summary>
        /// A wrong argument the SDK can see before sending — a null a request cannot carry — is
        /// reported through the completion handler with the code the native side answers a wrong
        /// argument with, and nothing reaches the bridge.
        /// </summary>
        [Test]
        public void ANullArgumentReportsWrongParamAndSendsNothing()
        {
            var reported = new List<AdaptyError>();

            Adapty.GetFlow(null, null, null, (_, error) => reported.Add(error));
            Adapty.GetFlowForDefaultAudience(null, null, (_, error) => reported.Add(error));
            Adapty.SetIntegrationIdentifier(null, "af-1", error => reported.Add(error));
            Adapty.UpdateExternalAttribution("{}", null, error => reported.Add(error));

            Assert.Multiple(() =>
            {
                Assert.That(reported, Has.Count.EqualTo(4), "a completion handler was never called");
                Assert.That(
                    reported,
                    Has.All.Matches<AdaptyError>(error => error?.Code == AdaptyErrorCode.WrongParam)
                );
                Assert.That(_method, Is.Null, "nothing should have reached the bridge");
            });
        }


        /// <summary>
        /// One case per migrated public method, so a call site cannot change what it sends - or
        /// which shape it sends a model in - without a snapshot moving.
        /// </summary>
        /// <remarks>
        /// Compiling under three symbol sets proves nothing about method names, request keys or
        /// which DTO a call site picked. Only the payload does.
        /// </remarks>
        [TestCaseSource(nameof(Requests))]
        public void RequestPayload(string name, string method, Action send)
        {
            send();

            Assert.That(_method, Is.EqualTo(method));
            Snapshots.Matches("transport-" + name, Snapshots.Canonical(_request));
        }

        private static IEnumerable<TestCaseData> Requests()
        {
            var flow = AdaptyJson.Deserialize<AdaptyFlow>(Snapshots.LoadResponse("flow-minimal"));
            var products = AdaptyJson.Deserialize<IList<AdaptyPaywallProduct>>(
                Snapshots.LoadResponse("products-full")
            );
            var withOffer = products[0];
            var withoutOffer = products[1];
            var paywall = AdaptyJson
                .Deserialize<AdaptyFlow>(Snapshots.LoadResponse("flow-full"))
                .Paywalls[0];
            var promoted = AdaptyJson.Deserialize<AdaptyPromotedProduct>(
                Snapshots.LoadResponse("promoted-product-full")
            );
            var flowView = AdaptyJson.Deserialize<AdaptyUIFlowView>(
                "{\"id\":\"view-1\",\"placement_id\":\"onboarding\",\"variation_id\":\"variation-0001\"}"
            );

            TestCaseData Case(string name, string method, Action send) =>
                new TestCaseData(name, method, send).SetName($"{{m}}({name})");

            // The three product paths: the response model is 17 fields, the request a strict subset
            // with a synthesized offer identifier, so these guard the DTO being used at all.
            yield return Case(
                "make-purchase-with-offer",
                "make_purchase",
                () => Adapty.MakePurchase(withOffer, (_, __) => { })
            );
            yield return Case(
                "make-purchase-without-offer",
                "make_purchase",
                () => Adapty.MakePurchase(withoutOffer, (_, __) => { })
            );
            yield return Case(
                "make-promoted-purchase",
                "make_promoted_purchase",
                () => Adapty.MakePromotedPurchase(promoted, (_, __) => { })
            );
            yield return Case(
                "create-web-paywall-url-product",
                "create_web_paywall_url",
                () => Adapty.CreateWebPaywallUrl(withOffer, (_, __) => { })
            );
            yield return Case(
                "open-web-paywall-product",
                "open_web_paywall",
                () => Adapty.OpenWebPaywall(withOffer, AdaptyWebPresentation.InAppBrowser, _ => { })
            );

            yield return Case(
                "create-web-paywall-url-paywall",
                "create_web_paywall_url",
                () => Adapty.CreateWebPaywallUrl(paywall, (_, __) => { })
            );
            yield return Case(
                "get-paywall-products",
                "get_paywall_products",
                () => Adapty.GetPaywallProducts(flow, (_, __) => { })
            );
            yield return Case(
                "log-show-flow",
                "log_show_flow",
                () => Adapty.LogShowFlow(flow, _ => { })
            );
            yield return Case(
                "identify",
                "identify",
                () => Adapty.Identify("user-1", Guid.Empty, "obfuscated-1", _ => { })
            );
            yield return Case("logout", "logout", () => Adapty.Logout(_ => { }));
            yield return Case(
                "get-profile",
                "get_profile",
                () => Adapty.GetProfile((_, __) => { })
            );
            yield return Case(
                "update-profile",
                "update_profile",
                () => Adapty.UpdateProfile(Samples.ProfileParameters(), _ => { })
            );
            yield return Case(
                "set-log-level",
                "set_log_level",
                () => Adapty.SetLogLevel(AdaptyLogLevel.Verbose, _ => { })
            );
            yield return Case(
                "set-fallback",
                "set_fallback",
                () => Adapty.SetFallback("fallback.json", _ => { })
            );
            yield return Case(
                "set-integration-identifier",
                "set_integration_identifiers",
                () => Adapty.SetIntegrationIdentifier("appsflyer_id", "af-1", _ => { })
            );
            yield return Case(
                "report-transaction",
                "report_transaction",
                () => Adapty.ReportTransaction("txn-1", "variation-0001", _ => { })
            );
            yield return Case(
                "restore-purchases",
                "restore_purchases",
                () => Adapty.RestorePurchases((_, __) => { })
            );
            // iOS-only, and the reason they are here is the Editor rather than the payload: they
            // used to take an #else that reported a null error, which a caller cannot tell from
            // success, instead of reaching the bridge that says the SDK is not available here.
            yield return Case(
                "update-collecting-refund-data-consent",
                "update_collecting_refund_data_consent",
                () => Adapty.UpdateAppStoreCollectingRefundDataConsent(true, _ => { })
            );
            yield return Case(
                "update-refund-preference",
                "update_refund_preference",
                () => Adapty.UpdateAppStoreRefundPreference(AdaptyRefundPreference.Grant, _ => { })
            );
            yield return Case(
                "present-code-redemption-sheet",
                "present_code_redemption_sheet",
                () => Adapty.PresentCodeRedemptionSheet(_ => { })
            );

            yield return Case(
                "open-url",
                "adapty_ui_open_url",
                () => AdaptyUI.OpenUrl("https://adapty.io", AdaptyWebPresentation.ExternalBrowser, _ => { })
            );
            yield return Case(
                "present-flow-view",
                "adapty_ui_present_flow_view",
                () => AdaptyUI.PresentFlowView(flowView, _ => { })
            );
            yield return Case(
                "dismiss-flow-view",
                "adapty_ui_dismiss_flow_view",
                () => AdaptyUI.DismissFlowView(flowView, _ => { })
            );
            yield return Case(
                "show-dialog",
                "adapty_ui_show_dialog",
                () => AdaptyUI.ShowDialog(flowView, Samples.DialogConfiguration(), (_, __) => { })
            );
        }

        /// <summary>
        /// A reply with neither member is malformed, not an empty success: reporting it as a
        /// default would mean "not premium" or "the purchase did not happen".
        /// </summary>
        [TestCase("{}")]
        [TestCase("{\"unrelated\":1}")]
        [TestCase("{\"success\":null}")]
        [TestCase("[]")]
        public void RepliesWithoutSuccessAreDecodingErrors(string reply)
        {
            _reply = reply;

            AdaptyError fromValueType = null;
            var activated = true;
            Adapty.IsActivated((value, error) => (activated, fromValueType) = (value, error));

            AdaptyError fromReferenceType = null;
            AdaptyProfile profile = null;
            Adapty.GetProfile((value, error) => (profile, fromReferenceType) = (value, error));

            Assert.Multiple(() =>
            {
                Assert.That(fromValueType?.Code, Is.EqualTo(AdaptyErrorCode.DecodingFailed));
                Assert.That(activated, Is.False);
                Assert.That(fromReferenceType?.Code, Is.EqualTo(AdaptyErrorCode.DecodingFailed));
                Assert.That(profile, Is.Null);
            });
        }

        /// <summary>
        /// A native error comes back as an AdaptyError, not as an exception.
        /// </summary>
        [Test]
        public void ErrorRepliesAreMappedNotThrown()
        {
            _reply =
                "{\"error\":{\"adapty_code\":2003,\"message\":\"not found\",\"detail\":\"d\"}}";

            AdaptyError received = null;
            Adapty.GetProfile((_, error) => received = error);

            Assert.That(received, Is.Not.Null);
            Assert.That(received.Code, Is.EqualTo(AdaptyErrorCode.BadRequest));
            Assert.That(received.Message, Is.EqualTo("not found"));
        }

        /// <summary>
        /// A reply the SDK cannot read is an error too - it must never escape the callback, which
        /// on iOS is a reverse-P/Invoke boundary.
        /// </summary>
        [TestCase("not json at all")]
        [TestCase("{\"success\":{\"flow_id\":\"only-this\"}}")]
        public void MalformedRepliesBecomeDecodingErrors(string reply)
        {
            _reply = reply;

            AdaptyError received = null;
            AdaptyFlow value = null;
            Assert.That(
                () => Adapty.GetFlow("onboarding", (flow, error) => (value, received) = (flow, error)),
                Throws.Nothing
            );

            Assert.That(value, Is.Null);
            Assert.That(received, Is.Not.Null);
            Assert.That(received.Code, Is.EqualTo(AdaptyErrorCode.DecodingFailed));
        }
    }
}

#endif
