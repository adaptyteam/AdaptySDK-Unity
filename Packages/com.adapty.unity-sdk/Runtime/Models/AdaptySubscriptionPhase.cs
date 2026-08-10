//
//  AdaptySubscriptionPhase.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 11.09.2023.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscriptionPhase
    {
        private AdaptySubscriptionPhase() { }

        [DataMember(Name = "price", IsRequired = true)]
        public readonly AdaptyPrice Price;

        /// An integer that indicates the number of periods the product discount is available.
        [DataMember(Name = "number_of_periods", IsRequired = true)]
        public readonly int NumberOfPeriods;

        /// The payment mode for this product discount.
        [DataMember(Name = "payment_mode", IsRequired = true)]
        public readonly AdaptyPaymentMode PaymentMode;

        /// A [Adapty.Period] object that defines the period for the product discount.
        [DataMember(Name = "subscription_period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod SubscriptionPeriod;

        /// The formatted subscription period of the discount for the user's localization.
        ///
        /// [Nullable]
        [DataMember(Name = "localized_subscription_period")]
        public readonly string LocalizedSubscriptionPeriod;

        /// The formatted number of periods of the discount for the user's localization.
        ///
        /// [Nullable]
        [DataMember(Name = "localized_number_of_periods")]
        public readonly string LocalizedNumberOfPeriods;

        public override string ToString() => $"{nameof(Price)}: {Price}, " +
                   $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                   $"{nameof(NumberOfPeriods)}: {NumberOfPeriods}, " +
                   $"{nameof(PaymentMode)}: {PaymentMode}, " +
                   $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
                   $"{nameof(LocalizedNumberOfPeriods)}: {LocalizedNumberOfPeriods}";
    }
}