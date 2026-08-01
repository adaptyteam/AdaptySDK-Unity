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

        public AdaptyPurchaseParameters(
            AdaptySubscriptionUpdateParameters subscriptionUpdateParams = null,
            bool? isOfferPersonalized = null
        )
        {
            SubscriptionUpdateParams = subscriptionUpdateParams;
            IsOfferPersonalized = isOfferPersonalized;
        }

        public override string ToString() =>
            $"{nameof(SubscriptionUpdateParams)}: {SubscriptionUpdateParams}, "
            + $"{nameof(IsOfferPersonalized)}: {IsOfferPersonalized}";
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
                _parameters.IsOfferPersonalized
            );
            return this;
        }

        public AdaptyPurchaseParametersBuilder SetIsOfferPersonalized(bool? isOfferPersonalized)
        {
            _parameters = new AdaptyPurchaseParameters(
                _parameters.SubscriptionUpdateParams,
                isOfferPersonalized
            );
            return this;
        }

        public AdaptyPurchaseParameters Build()
        {
            return _parameters;
        }
    }
}
