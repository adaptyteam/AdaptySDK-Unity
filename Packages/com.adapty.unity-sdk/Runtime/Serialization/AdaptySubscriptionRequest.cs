using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// How a request writes a subscription back to the native side - reduced to its billing plan
    /// (subscription.pricing_terms.billing_plan_id) and its offer identifier
    /// (subscription.offer.offer_identifier), the same envelope for the paywall and promoted
    /// product requests.
    /// </summary>
    /// <remarks>
    /// The plan has to go back: without it iOS buys the product up front, whatever plan the
    /// product was offered on.
    /// </remarks>
    [DataContract]
    [Preserve]
    internal sealed class AdaptySubscriptionRequest
    {
        private AdaptySubscriptionRequest(string billingPlan, AdaptySubscriptionOffer offer)
        {
            if (billingPlan != null)
            {
                PricingTerms = new PricingTermsEnvelope(billingPlan);
            }
            if (offer != null)
            {
                Offer = new OfferEnvelope(new OfferIdentifier(offer.Identifier, offer.Type));
            }
        }

        /// <summary>
        /// Null when the subscription has neither a plan nor an offer, so the request carries no
        /// subscription key at all.
        /// </summary>
        internal static AdaptySubscriptionRequest From(AdaptySubscription subscription)
        {
            var billingPlan = subscription?.PricingTerms?.BillingPlan.RawValue;
            var offer = subscription?.Offer;

            return billingPlan == null && offer == null
                ? null
                : new AdaptySubscriptionRequest(billingPlan, offer);
        }

        [DataMember(Name = "pricing_terms")]
        [Preserve]
        private PricingTermsEnvelope PricingTerms { get; }

        [DataMember(Name = "offer")]
        [Preserve]
        private OfferEnvelope Offer { get; }

        [DataContract]
        private sealed class PricingTermsEnvelope
        {
            internal PricingTermsEnvelope(string billingPlan)
            {
                BillingPlan = billingPlan;
            }

            [DataMember(Name = "billing_plan_id", IsRequired = true)]
            [Preserve]
            private string BillingPlan { get; }
        }

        [DataContract]
        private sealed class OfferEnvelope
        {
            internal OfferEnvelope(OfferIdentifier identifier)
            {
                Identifier = identifier;
            }

            [DataMember(Name = "offer_identifier", IsRequired = true)]
            [Preserve]
            private OfferIdentifier Identifier { get; }
        }

        [DataContract]
        private sealed class OfferIdentifier
        {
            internal OfferIdentifier(string id, AdaptySubscriptionOfferType type)
            {
                Id = id;
                Type = type;
            }

            [DataMember(Name = "id")]
            [Preserve]
            private string Id { get; }

            [DataMember(Name = "type", IsRequired = true)]
            [Preserve]
            private AdaptySubscriptionOfferType Type { get; }
        }
    }
}
