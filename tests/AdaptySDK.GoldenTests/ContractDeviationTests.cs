using System;
using AdaptySDK.SimpleJSON;
using NUnit.Framework;

namespace AdaptySDK.GoldenTests
{
    /// <summary>
    /// Places where the parser and cross_platform.yaml disagree. These tests pin what the code
    /// does today, so the migration is a deliberate decision per case rather than an accident.
    /// </summary>
    [TestFixture]
    public class ContractDeviationTests
    {
        private const string ProductWithOffer =
            "{\"vendor_product_id\":\"p\",\"adapty_product_id\":\"a\",\"access_level_id\":\"premium\","
            + "\"product_type\":\"subscription\",\"paywall_product_index\":0,\"paywall_variation_id\":\"v\","
            + "\"paywall_ab_test_name\":\"t\",\"paywall_name\":\"n\",\"localized_description\":\"d\","
            + "\"localized_title\":\"t\",\"is_family_shareable\":false,"
            + "\"price\":{\"amount\":1.0},"
            + "\"subscription\":{\"group_identifier\":\"g\",\"period\":{\"unit\":\"month\",\"number_of_units\":1},"
            + "\"renewal_type\":\"autorenewable\",\"base_plan_id\":\"b\",\"offer\":__OFFER__}}";

        /// <summary>
        /// cross_platform.yaml:1225 lists `phases` as required for AdaptySubscriptionOffer, but the
        /// parser reads it with GetAdaptySubscriptionPhaseListIfPresent and accepts its absence.
        /// </summary>
        [Test]
        public void OfferWithoutPhasesIsAcceptedAlthoughContractRequiresThem()
        {
            var json = ProductWithOffer.Replace(
                "__OFFER__",
                "{\"offer_identifier\":{\"id\":\"o\",\"type\":\"introductory\"}}"
            );

            var product = ParseProduct(json);

            Assert.That(product.Subscription, Is.Not.Null);
            Assert.That(product.Subscription.Offer, Is.Not.Null);
        }

        /// <summary>
        /// The same offer with phases present, for contrast.
        /// </summary>
        [Test]
        public void OfferWithPhasesParses()
        {
            var json = ProductWithOffer.Replace(
                "__OFFER__",
                "{\"offer_identifier\":{\"id\":\"o\",\"type\":\"introductory\"},"
                    + "\"phases\":[{\"price\":{\"amount\":0.0},\"number_of_periods\":1,"
                    + "\"payment_mode\":\"free_trial\",\"subscription_period\":{\"unit\":\"week\",\"number_of_units\":1}}]}"
            );

            var product = ParseProduct(json);

            Assert.That(product.Subscription.Offer, Is.Not.Null);
        }

        /// <summary>
        /// AdaptySubscriptionPeriod.Unit and AdaptySubscriptionOffer.PaymentMode declare "unknown"
        /// in the contract, and the C# enums already carry it — no exception is expected.
        /// </summary>
        [Test]
        public void ContractDeclaredUnknownEnumValuesParse()
        {
            var json = ProductWithOffer.Replace("\"unit\":\"month\"", "\"unit\":\"unknown\"")
                .Replace(
                    "__OFFER__",
                    "{\"offer_identifier\":{\"id\":\"o\",\"type\":\"introductory\"},"
                        + "\"phases\":[{\"price\":{\"amount\":0.0},\"number_of_periods\":1,"
                        + "\"payment_mode\":\"unknown\",\"subscription_period\":{\"unit\":\"unknown\",\"number_of_units\":1}}]}"
                );

            var product = ParseProduct(json);

            Assert.That(product.Subscription.Period.Unit, Is.EqualTo(AdaptySubscriptionPeriodUnit.Unknown));
        }

        /// <summary>
        /// Enum values outside the contract still throw today. The Newtonsoft layer is expected to
        /// return Unknown instead, so this test marks the behaviour that will change.
        /// </summary>
        [Test]
        public void EnumValueOutsideContractThrowsToday()
        {
            var json = ProductWithOffer.Replace(
                "__OFFER__",
                "{\"offer_identifier\":{\"id\":\"o\",\"type\":\"brand_new_offer_type\"}}"
            );

            Assert.Throws<Exception>(() => ParseProduct(json));
        }

        private static AdaptyPaywallProduct ParseProduct(string json) =>
            JSONNode.Parse("{\"product\":" + json + "}").GetAdaptyPaywallProduct("product");
    }
}
