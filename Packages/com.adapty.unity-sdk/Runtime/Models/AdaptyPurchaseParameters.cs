//
//  AdaptyPurchaseParameters.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public sealed class AdaptyPurchaseParameters
    {
        [DataMember(Name = "subscription_update_params")]
        public readonly AdaptySubscriptionUpdateParameters SubscriptionUpdateParams; // Android Only, nullable
        [DataMember(Name = "is_offer_personalized")]
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

    [Preserve]
    public sealed class AdaptyPurchaseParametersBuilder
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
