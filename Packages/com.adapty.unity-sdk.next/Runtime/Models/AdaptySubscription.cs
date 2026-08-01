//
//  AdaptySubscription.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public partial class AdaptySubscription
    {
        /// The identifier of the subscription group to which the subscription belongs.
        ///
        /// [Nullable]
        public readonly string GroupIdentifier;

        /// A ProductSubscriptionPeriodModel object.
        /// The period details for products that are subscriptions.
        ///
        public readonly AdaptySubscriptionPeriod Period;

        /// Localized subscription period of the product.
        ///
        /// [Nullable]
        public readonly string LocalizedPeriod;

        public readonly AdaptySubscriptionOffer Offer;

        public readonly AdaptySubscriptionRenewalType RenewalType;
        public readonly string BasePlanId; //nullable


        public override string ToString() =>
            $"{nameof(GroupIdentifier)}: {GroupIdentifier}, " +
            $"{nameof(Period)}: {Period}, " +
            $"{nameof(LocalizedPeriod)}: {LocalizedPeriod}, " +
            $"{nameof(Offer)}: {Offer}, " +
            $"{nameof(RenewalType)}: {RenewalType}, " +
            $"{nameof(BasePlanId)}: {BasePlanId}";
    }
}