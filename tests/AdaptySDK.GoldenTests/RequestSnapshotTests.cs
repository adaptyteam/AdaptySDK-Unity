using AdaptySDK.SimpleJSON;
using NUnit.Framework;

namespace AdaptySDK.GoldenTests
{
    /// <summary>
    /// Pins what the SDK sends to the native side. Snapshots are canonicalised (keys sorted), so
    /// they compare the payload rather than the order it was written in.
    /// </summary>
    [TestFixture]
    public class RequestSnapshotTests
    {
        [Test]
        public void SubscriptionUpdateParameters()
        {
            var parameters = new AdaptySubscriptionUpdateParameters(
                "com.adapty.sample.monthly",
                AdaptySubscriptionUpdateReplacementMode.ChargeProratedPrice
            );

            Snapshots.Matches(
                "request-subscription-update",
                Snapshots.Canonical(parameters.ToJSONNode().ToString())
            );
        }

        [Test]
        public void PurchaseParametersFull()
        {
            var parameters = new AdaptyPurchaseParametersBuilder()
                .SetSubscriptionUpdateParams(
                    new AdaptySubscriptionUpdateParameters(
                        "com.adapty.sample.monthly",
                        AdaptySubscriptionUpdateReplacementMode.Deferred
                    )
                )
                .SetIsOfferPersonalized(true)
                .Build();

            Snapshots.Matches(
                "request-purchase-parameters-full",
                Snapshots.Canonical(parameters.ToJSONNode().ToString())
            );
        }

        [Test]
        public void PurchaseParametersEmpty()
        {
            var parameters = new AdaptyPurchaseParametersBuilder().Build();

            Snapshots.Matches(
                "request-purchase-parameters-empty",
                Snapshots.Canonical(parameters.ToJSONNode().ToString())
            );
        }

        [Test]
        public void Configuration() =>
            Snapshots.Matches(
                "request-configuration",
                Snapshots.Canonical(Samples.Configuration().ToJSONNode().ToString())
            );

        [Test]
        public void ConfigurationWithEmptyIdentity() =>
            Snapshots.Matches(
                "request-configuration-empty-identity",
                Snapshots.Canonical(
                    Samples.ConfigurationWithEmptyIdentity().ToJSONNode().ToString()
                )
            );

        [Test]
        public void ConfigurationWithDefaultCluster() =>
            Snapshots.Matches(
                "request-configuration-default-cluster",
                Snapshots.Canonical(
                    Samples.ConfigurationWithDefaultCluster().ToJSONNode().ToString()
                )
            );

        [Test]
        public void ProfileParameters() =>
            Snapshots.Matches(
                "request-profile-parameters",
                Snapshots.Canonical(Samples.ProfileParameters().ToJSONNode().ToString())
            );

        [Test]
        public void DialogConfiguration() =>
            Snapshots.Matches(
                "request-dialog-configuration",
                Snapshots.Canonical(Samples.DialogConfiguration().ToJSONNode().ToString())
            );

        [Test]
        public void DialogConfigurationMinimal() =>
            Snapshots.Matches(
                "request-dialog-configuration-minimal",
                Snapshots.Canonical(Samples.DialogConfigurationMinimal().ToJSONNode().ToString())
            );

        [Test]
        public void FetchPolicyDefault() =>
            Snapshots.Matches(
                "request-fetch-policy-default",
                Snapshots.Canonical(Samples.FetchPolicyDefault().ToJSONNode().ToString())
            );

        [Test]
        public void FetchPolicyWithMaxAge() =>
            Snapshots.Matches(
                "request-fetch-policy-max-age",
                Snapshots.Canonical(Samples.FetchPolicyWithMaxAge().ToJSONNode().ToString())
            );

        /// <summary>
        /// Read then write: a product goes back to the native side as a subset of what arrived,
        /// so the fixture is the only honest source for one.
        /// </summary>
        [TestCase("products-full", 0, "request-product-with-offer")]
        [TestCase("products-full", 1, "request-product-plain")]
        public void PaywallProduct(string fixture, int index, string snapshot)
        {
            var products = JSONNode
                .Parse(Snapshots.LoadResponse(fixture))
                .GetAdaptyPaywallProductList();

            Snapshots.Matches(
                snapshot,
                Snapshots.Canonical(products[index].ToJSONNode().ToString())
            );
        }

        [Test]
        public void ProductIdentifier() =>
            Snapshots.Matches(
                "request-product-identifier",
                Snapshots.Canonical(Samples.ProductIdentifier().ToJSONNode().ToString())
            );

        [Test]
        public void ProductIdentifierWithoutBasePlan() =>
            Snapshots.Matches(
                "request-product-identifier-no-base-plan",
                Snapshots.Canonical(
                    Samples.ProductIdentifierWithoutBasePlan().ToJSONNode().ToString()
                )
            );

        /// <summary>
        /// A flow is handed back to the native side as it arrived, so reading a fixture and
        /// writing it again is the whole contract for it.
        /// </summary>
        [TestCase("flow-full")]
        [TestCase("flow-minimal")]
        public void FlowRoundTrip(string fixture)
        {
            var flow = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetFlow();
            Snapshots.Matches(
                "request-" + fixture,
                Snapshots.Canonical(flow.ToJSONNode().ToString())
            );
        }

        [TestCase("onboarding-full")]
        [TestCase("onboarding-minimal")]
        public void OnboardingRoundTrip(string fixture)
        {
            var onboarding = JSONNode.Parse(Snapshots.LoadResponse(fixture)).GetOnboarding();
            Snapshots.Matches(
                "request-" + fixture,
                Snapshots.Canonical(onboarding.ToJSONNode().ToString())
            );
        }

        [Test]
        public void CustomAssets() =>
            Snapshots.Matches(
                "request-custom-assets",
                Snapshots.Canonical(Samples.CustomAssets().ToJSONNode().ToString())
            );
    }
}
