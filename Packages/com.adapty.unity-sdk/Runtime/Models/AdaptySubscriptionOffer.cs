using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AdaptySDK
{
    /// <remarks>
    /// The contract nests the identifier and the type inside <c>offer_identifier</c> while the
    /// model keeps them flat, so this one is built by
    /// <c>AdaptySDK.Serialization.AdaptyConverterSubscriptionOffer</c> rather than from member
    /// annotations.
    /// </remarks>
    [Preserve]
    public sealed class AdaptySubscriptionOffer
    {
        internal AdaptySubscriptionOffer(
            string identifier,
            AdaptySubscriptionOfferType type,
            IList<AdaptySubscriptionPhase> phases,
            IList<string> offerTags
        )
        {
            Identifier = identifier;
            Type = type;
            Phases = new ReadOnlyCollection<AdaptySubscriptionPhase>(phases);
#if UNITY_ANDROID
            OfferTags = offerTags is null ? null : new ReadOnlyCollection<string>(offerTags);
#else
            OfferTags = null;
#endif
        }

        public readonly string Identifier;

        public readonly AdaptySubscriptionOfferType Type;

        public readonly IReadOnlyList<AdaptySubscriptionPhase> Phases;
        public readonly IReadOnlyList<string> OfferTags;

        public override string ToString() => 
            $"{nameof(Identifier)}: {Identifier}, " +
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Phases)}: {Phases}, " +
            $"{nameof(OfferTags)}: {OfferTags}";
    }
}