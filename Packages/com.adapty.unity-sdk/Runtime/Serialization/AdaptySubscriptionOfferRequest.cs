using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// How a request writes a subscription offer back to the native side - reduced to its
    /// identifier and nested as subscription.offer.offer_identifier, the same envelope for the
    /// paywall and promoted product requests.
    /// </summary>
    [DataContract]
    [Preserve]
    internal sealed class AdaptySubscriptionOfferRequest
    {
        internal AdaptySubscriptionOfferRequest(AdaptySubscriptionOffer offer)
        {
            Offer = new OfferEnvelope(new OfferIdentifier(offer.Identifier, offer.Type));
        }

        [DataMember(Name = "offer", IsRequired = true)]
        [Preserve]
        private OfferEnvelope Offer { get; }

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
