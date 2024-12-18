//
//  AdaptySubscriptionPhase.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 11.09.2023.
//

namespace AdaptySDK
{
    public partial class AdaptySubscriptionPhase
    {
        public readonly AdaptyPrice Price;

        /// An integer that indicates the number of periods the product discount is available.
        public readonly int NumberOfPeriods;

        /// The payment mode for this product discount.
        public readonly AdaptyPaymentMode PaymentMode;

        /// A [Adapty.Period] object that defines the period for the product discount.
        public readonly AdaptySubscriptionPeriod SubscriptionPeriod;

        /// The formatted subscription period of the discount for the user's localization.
        ///
        /// [Nullable]
        public readonly string LocalizedSubscriptionPeriod;

        /// The formatted number of periods of the discount for the user's localization.
        ///
        /// [Nullable]
        public readonly string LocalizedNumberOfPeriods;

        public override string ToString() => $"{nameof(Price)}: {Price}, " +
                   $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                   $"{nameof(NumberOfPeriods)}: {NumberOfPeriods}, " +
                   $"{nameof(PaymentMode)}: {PaymentMode}, " +
                   $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
                   $"{nameof(LocalizedNumberOfPeriods)}: {LocalizedNumberOfPeriods}";
    }
}