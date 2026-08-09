using AdaptySDK.TestSupport;
using AdaptySDK.Serialization;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Outgoing payloads: the same objects serialized through Newtonsoft have to produce what the
    /// native side received before.
    /// </summary>
    [TestFixture]
    public class RequestParityTests
    {
        /// <summary>
        /// The contract declares a format for this one - <c>YYYY-MM-dd</c> - and it is built by
        /// hand rather than through the date converter, which is where padding gets lost.
        /// </summary>
        /// <remarks>
        /// The sample the snapshots use is Ada Lovelace's birthday, whose month and day are both
        /// two digits, so no snapshot can tell a padded writer from an unpadded one. This picks a
        /// date where it shows.
        /// </remarks>
        [Test]
        public void BirthdayCarriesTheContractFormat()
        {
            var parameters = new AdaptyProfileParameters.Builder()
                .SetBirthday(new System.DateTime(1990, 3, 7))
                .Build();

            Assert.That(
                AdaptyJson.Serialize(parameters),
                Does.Contain("\"birthday\":\"1990-03-07\"")
            );
        }

        [Test]
        public void SubscriptionUpdateParameters()
        {
            var parameters = new AdaptySubscriptionUpdateParameters(
                "com.adapty.sample.monthly",
                AdaptySubscriptionUpdateReplacementMode.ChargeProratedPrice
            );

            Snapshots.Matches(
                "request-subscription-update",
                Snapshots.Canonical(AdaptyJson.Serialize(parameters))
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
                Snapshots.Canonical(AdaptyJson.Serialize(parameters))
            );
        }

        [Test]
        public void PurchaseParametersEmpty()
        {
            var parameters = new AdaptyPurchaseParametersBuilder().Build();

            Snapshots.Matches(
                "request-purchase-parameters-empty",
                Snapshots.Canonical(AdaptyJson.Serialize(parameters))
            );
        }

        [Test]
        public void Configuration() =>
            Snapshots.Matches(
                "request-configuration",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.Configuration()))
            );

        [Test]
        public void ConfigurationWithEmptyIdentity() =>
            Snapshots.Matches(
                "request-configuration-empty-identity",
                Snapshots.Canonical(
                    AdaptyJson.Serialize(Samples.ConfigurationWithEmptyIdentity())
                )
            );

        [Test]
        public void ConfigurationWithDefaultCluster() =>
            Snapshots.Matches(
                "request-configuration-default-cluster",
                Snapshots.Canonical(
                    AdaptyJson.Serialize(Samples.ConfigurationWithDefaultCluster())
                )
            );

        [Test]
        public void ProfileParameters() =>
            Snapshots.Matches(
                "request-profile-parameters",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.ProfileParameters()))
            );

        [Test]
        public void DialogConfiguration() =>
            Snapshots.Matches(
                "request-dialog-configuration",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.DialogConfiguration()))
            );

        [Test]
        public void DialogConfigurationMinimal() =>
            Snapshots.Matches(
                "request-dialog-configuration-minimal",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.DialogConfigurationMinimal()))
            );

        [Test]
        public void FetchPolicyDefault() =>
            Snapshots.Matches(
                "request-fetch-policy-default",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.FetchPolicyDefault()))
            );

        [Test]
        public void FetchPolicyWithMaxAge() =>
            Snapshots.Matches(
                "request-fetch-policy-max-age",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.FetchPolicyWithMaxAge()))
            );

        [TestCase("products-full", 0, "request-product-with-offer")]
        [TestCase("products-full", 1, "request-product-plain")]
        public void PaywallProduct(string fixture, int index, string snapshot)
        {
            var products = AdaptyJson.Deserialize<System.Collections.Generic.IList<AdaptyPaywallProduct>>(
                Snapshots.LoadResponse(fixture)
            );

            Snapshots.Matches(
                snapshot,
                Snapshots.Canonical(
                    AdaptyJson.Serialize(new AdaptyPaywallProductRequest(products[index]))
                )
            );
        }

        [Test]
        public void ProductIdentifier() =>
            Snapshots.Matches(
                "request-product-identifier",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.ProductIdentifier()))
            );

        [Test]
        public void ProductIdentifierWithoutBasePlan() =>
            Snapshots.Matches(
                "request-product-identifier-no-base-plan",
                Snapshots.Canonical(
                    AdaptyJson.Serialize(Samples.ProductIdentifierWithoutBasePlan())
                )
            );

        /// <summary>
        /// The same request, from an empty base plan rather than none — the identifier normalizes it
        /// at construction, so the two share an approved file.
        /// </summary>
        [Test]
        public void ProductIdentifierWithEmptyBasePlan() =>
            Snapshots.Matches(
                "request-product-identifier-no-base-plan",
                Snapshots.Canonical(
                    AdaptyJson.Serialize(Samples.ProductIdentifierWithEmptyBasePlan())
                )
            );

        [TestCase("flow-full")]
        [TestCase("flow-minimal")]
        public void FlowRoundTrip(string fixture)
        {
            var flow = AdaptyJson.Deserialize<AdaptyFlow>(Snapshots.LoadResponse(fixture));
            Snapshots.Matches("request-" + fixture, Snapshots.Canonical(AdaptyJson.Serialize(flow)));
        }

        [TestCase("onboarding-full")]
        [TestCase("onboarding-minimal")]
        public void OnboardingRoundTrip(string fixture)
        {
            var onboarding = AdaptyJson.Deserialize<AdaptyOnboarding>(
                Snapshots.LoadResponse(fixture)
            );
            Snapshots.Matches(
                "request-" + fixture,
                Snapshots.Canonical(AdaptyJson.Serialize(onboarding))
            );
        }

        [Test]
        public void CustomAssets() =>
            Snapshots.Matches(
                "request-custom-assets",
                Snapshots.Canonical(AdaptyJson.Serialize(Samples.CustomAssets()))
            );
    }
}
