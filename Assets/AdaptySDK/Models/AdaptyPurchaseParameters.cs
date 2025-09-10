//
//  AdaptyPurchaseParameters.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    public partial class AdaptyPurchaseParameters
    {
        public readonly AdaptySubscriptionUpdateParameters SubscriptionUpdateParams; // Android Only, nullable
        public readonly bool? IsOfferPersonalized; // Android Only, nullable
        public readonly string ObfuscatedAccountId; // Android Only, nullable
        public readonly string ObfuscatedProfileId; // Android Only, nullable

        public override string ToString() =>
            $"{nameof(SubscriptionUpdateParams)}: {SubscriptionUpdateParams}, "
            + $"{nameof(IsOfferPersonalized)}: {IsOfferPersonalized}, "
            + $"{nameof(ObfuscatedAccountId)}: {ObfuscatedAccountId}, "
            + $"{nameof(ObfuscatedProfileId)}: {ObfuscatedProfileId}";
    }
}
