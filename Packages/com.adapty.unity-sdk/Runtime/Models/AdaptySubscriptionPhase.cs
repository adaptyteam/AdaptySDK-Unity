using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// One stretch of an offer at one price: a free trial, then a discounted period, then the rest.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscriptionPhase
    {
        private AdaptySubscriptionPhase() { }

        /// <summary>
        /// What the phase costs. Zero for a free trial.
        /// </summary>
        [DataMember(Name = "price", IsRequired = true)]
        public readonly AdaptyPrice Price;

        /// <summary>
        /// An integer that indicates the number of periods the product discount is available.
        /// </summary>
        [DataMember(Name = "number_of_periods", IsRequired = true)]
        public readonly int NumberOfPeriods;

        /// <summary>
        /// The payment mode for this product discount.
        /// </summary>
        [DataMember(Name = "payment_mode", IsRequired = true)]
        public readonly AdaptyPaymentMode PaymentMode;

        /// <summary>
        /// A [Adapty.Period] object that defines the period for the product discount.
        /// </summary>
        [DataMember(Name = "subscription_period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod SubscriptionPeriod;

        /// <summary>
        /// The formatted subscription period of the discount for the user's localization.
        ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "localized_subscription_period")]
        public readonly string LocalizedSubscriptionPeriod;

        /// <summary>
        /// The formatted number of periods of the discount for the user's localization.
        ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "localized_number_of_periods")]
        public readonly string LocalizedNumberOfPeriods;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() => $"{nameof(Price)}: {Price}, " +
                   $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                   $"{nameof(NumberOfPeriods)}: {NumberOfPeriods}, " +
                   $"{nameof(PaymentMode)}: {PaymentMode}, " +
                   $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
                   $"{nameof(LocalizedNumberOfPeriods)}: {LocalizedNumberOfPeriods}";
    }
}