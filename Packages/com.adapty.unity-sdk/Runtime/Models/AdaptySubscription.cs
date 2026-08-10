//
//  AdaptySubscription.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscription
    {
        private AdaptySubscription() { }

        /// The identifier of the subscription group to which the subscription belongs.
        ///
        /// [Nullable]
        #if UNITY_IOS
        [DataMember(Name = "group_identifier", IsRequired = true)]
#endif
        public readonly string GroupIdentifier;

        /// A ProductSubscriptionPeriodModel object.
        /// The period details for products that are subscriptions.
        ///
        [DataMember(Name = "period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod Period;

        /// Localized subscription period of the product.
        ///
        /// [Nullable]
        [DataMember(Name = "localized_period")]
        public readonly string LocalizedPeriod;

        [DataMember(Name = "offer")]
        public readonly AdaptySubscriptionOffer Offer;

        #if UNITY_ANDROID
        [DataMember(Name = "renewal_type", IsRequired = true)]
#endif
        public readonly AdaptySubscriptionRenewalType RenewalType =
            AdaptySubscriptionRenewalType.Autorenewable;
        #if UNITY_ANDROID
        [DataMember(Name = "base_plan_id", IsRequired = true)]
#endif
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