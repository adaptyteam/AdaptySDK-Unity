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

        public AdaptyPurchaseParameters(
            AdaptySubscriptionUpdateParameters subscriptionUpdateParams = null,
            bool? isOfferPersonalized = null,
            string obfuscatedAccountId = null,
            string obfuscatedProfileId = null
        )
        {
            SubscriptionUpdateParams = subscriptionUpdateParams;
            IsOfferPersonalized = isOfferPersonalized;
            ObfuscatedAccountId = obfuscatedAccountId;
            ObfuscatedProfileId = obfuscatedProfileId;
        }

        public override string ToString() =>
            $"{nameof(SubscriptionUpdateParams)}: {SubscriptionUpdateParams}, "
            + $"{nameof(IsOfferPersonalized)}: {IsOfferPersonalized}, "
            + $"{nameof(ObfuscatedAccountId)}: {ObfuscatedAccountId}, "
            + $"{nameof(ObfuscatedProfileId)}: {ObfuscatedProfileId}";
    }

    public class AdaptyPurchaseParametersBuilder
    {
        private AdaptyPurchaseParameters _parameters = new AdaptyPurchaseParameters();

        public AdaptyPurchaseParametersBuilder SetSubscriptionUpdateParams(
            AdaptySubscriptionUpdateParameters subscriptionUpdateParams
        )
        {
            _parameters = new AdaptyPurchaseParameters(
                subscriptionUpdateParams,
                _parameters.IsOfferPersonalized,
                _parameters.ObfuscatedAccountId,
                _parameters.ObfuscatedProfileId
            );
            return this;
        }

        public AdaptyPurchaseParametersBuilder SetIsOfferPersonalized(bool? isOfferPersonalized)
        {
            _parameters = new AdaptyPurchaseParameters(
                _parameters.SubscriptionUpdateParams,
                isOfferPersonalized,
                _parameters.ObfuscatedAccountId,
                _parameters.ObfuscatedProfileId
            );
            return this;
        }

        public AdaptyPurchaseParametersBuilder SetObfuscatedAccountId(string obfuscatedAccountId)
        {
            _parameters = new AdaptyPurchaseParameters(
                _parameters.SubscriptionUpdateParams,
                _parameters.IsOfferPersonalized,
                obfuscatedAccountId,
                _parameters.ObfuscatedProfileId
            );
            return this;
        }

        public AdaptyPurchaseParametersBuilder SetObfuscatedProfileId(string obfuscatedProfileId)
        {
            _parameters = new AdaptyPurchaseParameters(
                _parameters.SubscriptionUpdateParams,
                _parameters.IsOfferPersonalized,
                _parameters.ObfuscatedAccountId,
                obfuscatedProfileId
            );
            return this;
        }

        public AdaptyPurchaseParameters Build()
        {
            return _parameters;
        }
    }
}
