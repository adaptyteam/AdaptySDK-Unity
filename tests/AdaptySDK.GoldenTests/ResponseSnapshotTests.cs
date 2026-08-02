using System;
using AdaptySDK.SimpleJSON;
using NUnit.Framework;

namespace AdaptySDK.GoldenTests
{
    /// <summary>
    /// Parses a fixture with the current JSON layer and pins the resulting model state.
    /// </summary>
    [TestFixture]
    public class ResponseSnapshotTests
    {
        [TestCase("profile-full")]
        [TestCase("profile-minimal")]
        public void Profile(string fixture)
        {
            var profile = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetAdaptyProfile();
            Snapshots.Matches(fixture, ModelSnapshot.Render(profile));
        }

        [TestCase("flow-full")]
        [TestCase("flow-minimal")]
        public void Flow(string fixture)
        {
            var flow = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetFlow();
            Snapshots.Matches(fixture, ModelSnapshot.Render(flow));
        }

        [TestCase("onboarding-full")]
        [TestCase("onboarding-minimal")]
        public void Onboarding(string fixture)
        {
            var onboarding = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetOnboarding();
            Snapshots.Matches(fixture, ModelSnapshot.Render(onboarding));
        }

        [TestCase("onboarding-analytics-started")]
        [TestCase("onboarding-analytics-screen-completed")]
        [TestCase("onboarding-analytics-screen-completed-bare")]
        [TestCase("onboarding-analytics-unknown")]
        public void OnboardingAnalyticsEvent(string fixture)
        {
            var node = JSONNode.Parse(Snapshots.LoadResponse(fixture));
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(node.GetOnboardingsAnalyticsEvent("event"))
            );
        }

        [TestCase("installation-determined")]
        [TestCase("installation-determined-minimal")]
        [TestCase("installation-not-determined")]
        [TestCase("installation-not-available")]
        public void InstallationStatus(string fixture)
        {
            var status = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetInstallationStatus();
            Snapshots.Matches(fixture, ModelSnapshot.Render(status));
        }

        [TestCase("error-full")]
        [TestCase("error-minimal")]
        public void Error(string fixture)
        {
            var error = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetAdaptyError("error");
            Snapshots.Matches(fixture, ModelSnapshot.Render(error));
        }

        [TestCase("user-action-full")]
        [TestCase("user-action-minimal")]
        public void UserAction(string fixture)
        {
            var node = JSONNode.Parse(Snapshots.LoadResponse(fixture));
            Snapshots.Matches(fixture, ModelSnapshot.Render(node.GetAdaptyUIUserAction("action")));
        }

        /// <summary>
        /// The remote config's dashboard JSON is parsed lazily by a public property, not by the
        /// response parser, so it needs its own case.
        /// </summary>
        [TestCase("flow-full")]
        public void RemoteConfigDictionary(string fixture)
        {
            var flow = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetFlow();
            Snapshots.Matches(
                fixture + "-remote-config-dictionary",
                ModelSnapshot.Render(flow.RemoteConfig.Dictionary)
            );
        }

        [TestCase("products-full")]
        public void PaywallProducts(string fixture)
        {
            var products = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetAdaptyPaywallProductList();
            Snapshots.Matches(fixture, ModelSnapshot.Render(products));
        }

        [TestCase("purchase-result-success")]
        [TestCase("purchase-result-pending")]
        [TestCase("purchase-result-cancelled")]
        public void PurchaseResult(string fixture)
        {
            var result = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetAdaptyPurchaseResult();
            Snapshots.Matches(fixture, ModelSnapshot.Render(result));
        }

        /// <summary>
        /// day/month/year are optional per the contract; a partially filled picker used to throw.
        /// </summary>
        [TestCase("onboarding-date-picker-full")]
        [TestCase("onboarding-date-picker-partial")]
        [TestCase("onboarding-select")]
        public void OnboardingStateUpdated(string fixture)
        {
            var parameters = JSONNode
                .Parse(Snapshots.LoadResponse(fixture))
                .GetOnboardingsStateUpdatedParams("action");
            Snapshots.Matches(fixture, ModelSnapshot.Render(parameters));
        }

        /// <summary>
        /// Required fields must fail loudly rather than parse into nulls — the property the
        /// migration has to preserve.
        /// </summary>
        [TestCase("profile-full", "profile_id")]
        [TestCase("profile-full", "segment_hash")]
        [TestCase("profile-full", "timestamp")]
        [TestCase("profile-full", "is_test_user")]
        public void ProfileRejectsMissingRequiredField(string fixture, string field)
        {
            var node = JSONNode.Parse(Snapshots.LoadResponse(fixture)).AsObject;
            node.Remove(field);

            Assert.Throws<Exception>(() => node.GetAdaptyProfile());
        }

        [TestCase("flow-full", "placement")]
        [TestCase("flow-full", "flow_id")]
        [TestCase("flow-full", "flow_name")]
        [TestCase("flow-full", "variation_id")]
        [TestCase("flow-full", "response_created_at")]
        public void FlowRejectsMissingRequiredField(string fixture, string field)
        {
            var node = JSONNode.Parse(Snapshots.LoadResponse(fixture)).AsObject;
            node.Remove(field);

            Assert.Throws<Exception>(() => node.GetFlow());
        }
    }
}
