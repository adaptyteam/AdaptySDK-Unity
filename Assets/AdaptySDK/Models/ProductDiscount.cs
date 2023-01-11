//
//  ProductDiscount.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class ProductDiscount
        {
            /// The discount price of the product in the user's local currency.
            public readonly double Price;
            /// An identifier of the discount offer for the product.
            ///
            /// [Nullable]
            public readonly string Identifier;
            /// A [Adapty.Period] object that defines the period for the product discount.
            public readonly SubscriptionPeriod SubscriptionPeriod;
            /// An integer that indicates the number of periods the product discount is available.
            public readonly int NumberOfPeriods;
            /// The payment mode for this product discount.
            public readonly PaymentMode PaymentMode;
            /// The formatted price of the discount for the user's localization.
            ///
            /// [Nullable]
            public readonly string LocalizedPrice;
            /// The formatted subscription period of the discount for the user's localization.
            ///
            /// [Nullable]
            public readonly string LocalizedSubscriptionPeriod;
            /// The formatted number of periods of the discount for the user's localization.
            ///
            /// [Nullable]
            public readonly string LocalizedNumberOfPeriods;

            public override string ToString()
            {
                return $"{nameof(Price)}: {Price}, " +
                       $"{nameof(Identifier)}: {Identifier}, " +
                       $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                       $"{nameof(NumberOfPeriods)}: {NumberOfPeriods}, " +
                       $"{nameof(PaymentMode)}: {PaymentMode}, " +
                       $"{nameof(LocalizedPrice)}: {LocalizedPrice}, " +
                       $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
                       $"{nameof(LocalizedNumberOfPeriods)}: {LocalizedNumberOfPeriods}";
            }
        }
    }
}