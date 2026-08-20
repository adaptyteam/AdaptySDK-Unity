using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <remarks>
    /// The contract nests the identifier and the type inside <c>offer_identifier</c> while the
    /// model keeps them flat, so this one is built by
    /// <c>AdaptySDK.Serialization.AdaptyConverterSubscriptionOffer</c> rather than from member
    /// annotations.
    /// </remarks>
    /// <summary>
    /// A discounted offer on a subscription, and the phases it runs through.
    /// </summary>
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

            // No platform check: the converter is the only caller and already reads offer_tags on
            // Android alone, so off it this is null on the way in.
            OfferTags = offerTags is null ? null : new ReadOnlyCollection<string>(offerTags);
        }

        /// <summary>
        /// The offer id the store knows it by. Null for an introductory offer on iOS, which has none.
        /// </summary>
        public readonly string Identifier;

        /// <summary>
        /// Which kind of offer this is.
        /// </summary>
        public readonly AdaptySubscriptionOfferType Type;

        /// <summary>
        /// The phases the offer runs through, in order.
        /// </summary>
        public readonly IReadOnlyList<AdaptySubscriptionPhase> Phases;
        /// <summary>
        /// Android only. The tags Google Play carries on the offer. Null on iOS.
        /// </summary>
        public readonly IReadOnlyList<string> OfferTags;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Identifier)}: {Identifier}, " +
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Phases)}: {Phases}, " +
            $"{nameof(OfferTags)}: {OfferTags}";
    }
}
