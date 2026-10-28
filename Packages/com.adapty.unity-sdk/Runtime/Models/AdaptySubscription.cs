using System.Runtime.Serialization;
using UnityEngine.Scripting;

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
        /// <remarks>
        /// On iOS this is the period of the whole commitment: a year for a plan paid monthly. The
        /// time between payments is <see cref="AdaptySubscriptionPricingTerms.BillingPeriod"/>.
        /// </remarks>
        [DataMember(Name = "period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod Period;

        /// <summary>
        /// Localized subscription period of the product — on iOS, of the whole commitment, like
        /// <see cref="Period"/>.
        ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "localized_period")]
        public readonly string LocalizedPeriod;

        /// <summary>
        /// iOS only. How the subscription is billed: its plan, each payment, and the commitment.
        /// Null on Android.
        /// </summary>
        /// <remarks>
        /// A plan other than <see cref="AdaptySubscriptionBillingPlan.UpFront"/> is offered only on
        /// iOS 26.4+ in an app built with Xcode 26.5+, in a storefront that offers it; anywhere
        /// else the product comes with its up-front terms and is bought up front. The purchase
        /// fails with <see cref="AdaptyErrorCode.BillingPlanUnavailable"/> only when StoreKit no
        /// longer offers the plan by the time the product is bought.
        /// </remarks>
#if UNITY_IOS
        [DataMember(Name = "pricing_terms", IsRequired = true)]
#endif
        public readonly AdaptySubscriptionPricingTerms PricingTerms;

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
            $"{nameof(PricingTerms)}: {PricingTerms}, " +
            $"{nameof(Offer)}: {Offer}, " +
            $"{nameof(RenewalType)}: {RenewalType}, " +
            $"{nameof(BasePlanId)}: {BasePlanId}";
    }
}
