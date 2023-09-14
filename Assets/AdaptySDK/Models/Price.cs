//
//  Price.cs
//  Adapty
//
//  Created by Aleksei Valiano on 08.09.2023.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class Price
        {
            /// Discount price of a product in a local currency.
            public readonly double Amount;

            /// The currency code of the locale used to format the price of the product.
            /// ///
            /// [Nullable]
            public readonly string CurrencyCode;

            /// The currency symbol of the locale used to format the price of the product.
            /// ///
            /// [Nullable]
            public readonly string CurrencySymbol;

            /// A formatted price of a discount for a user's locale.
            ///
            /// [Nullable]
            public readonly string LocalizedString;

            public override string ToString() => $"{nameof(Amount)}: {Amount}, " +
                       $"{nameof(CurrencyCode)}: {CurrencyCode}, " +
                       $"{nameof(CurrencySymbol)}: {CurrencySymbol}, " +
                       $"{nameof(LocalizedString)}: {LocalizedString}";
        }
    }
}