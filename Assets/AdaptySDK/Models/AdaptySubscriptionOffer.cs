//
//  AdaptySubscriptionOffer.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 12.12.2024.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    public partial class AdaptySubscriptionOffer
    {
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