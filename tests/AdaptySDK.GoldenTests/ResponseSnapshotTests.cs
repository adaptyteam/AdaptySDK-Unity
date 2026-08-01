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
