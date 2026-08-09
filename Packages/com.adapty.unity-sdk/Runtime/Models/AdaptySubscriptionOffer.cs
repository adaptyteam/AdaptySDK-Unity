//
//  AdaptySubscriptionOffer.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 12.12.2024.
//

using UnityEngine.Scripting;
using System.Collections.Generic;

namespace AdaptySDK
{
    /// <remarks>
    /// The contract nests the identifier and the type inside <c>offer_identifier</c> while the
    /// model keeps them flat, so this one is built by
    /// <c>AdaptySDK.Serialization.AdaptyConverterSubscriptionOffer</c> rather than from member
    /// annotations.
    /// </remarks>
    [Preserve]
    public partial class AdaptySubscriptionOffer
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
            Phases = phases;
#if UNITY_ANDROID
            OfferTags = offerTags;
#else
            OfferTags = null;
#endif
        }

        public readonly string Identifier;

        public readonly AdaptySubscriptionOfferType Type;

        public readonly IList<AdaptySubscriptionPhase> Phases;
        public readonly IList<string> OfferTags;

        public override string ToString() => 
            $"{nameof(Identifier)}: {Identifier}, " +
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Phases)}: {Phases}, " +
            $"{nameof(OfferTags)}: {OfferTags}";
    }
}