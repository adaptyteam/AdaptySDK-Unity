using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// What a product's subscription looks like: its period, its offer, and how it renews.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscription
    {
        private AdaptySubscription() { }

        /// <summary>
        /// The identifier of the subscription group to which the subscription belongs.
        ///
        /// [Nullable]
        /// </summary>
        #if UNITY_IOS
        [DataMember(Name = "group_identifier", IsRequired = true)]
#endif
        public readonly string GroupIdentifier;

        /// <summary>
        /// A ProductSubscriptionPeriodModel object.
        /// The period details for products that are subscriptions.
        ///
        /// </summary>
        [DataMember(Name = "period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod Period;

        /// <summary>
        /// Localized subscription period of the product.
        ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "localized_period")]
        public readonly string LocalizedPeriod;

        /// <summary>
        /// The discounted offer attached to this subscription, or null when it is at full price.
        /// </summary>
        [DataMember(Name = "offer")]
        public readonly AdaptySubscriptionOffer Offer;

        /// <summary>
        /// Android only. Whether the subscription renews by itself.
        /// </summary>
        #if UNITY_ANDROID
        [DataMember(Name = "renewal_type", IsRequired = true)]
#endif
        public readonly AdaptySubscriptionRenewalType RenewalType =
            AdaptySubscriptionRenewalType.Autorenewable;
        /// <summary>
        /// Android only. The Google Play base plan this subscription is on. Null on iOS.
        /// </summary>
        #if UNITY_ANDROID
        [DataMember(Name = "base_plan_id", IsRequired = true)]
#endif
        public readonly string BasePlanId;


        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(GroupIdentifier)}: {GroupIdentifier}, " +
            $"{nameof(Period)}: {Period}, " +
            $"{nameof(LocalizedPeriod)}: {LocalizedPeriod}, " +
            $"{nameof(Offer)}: {Offer}, " +
            $"{nameof(RenewalType)}: {RenewalType}, " +
            $"{nameof(BasePlanId)}: {BasePlanId}";
    }
}