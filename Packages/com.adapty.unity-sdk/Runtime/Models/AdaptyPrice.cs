//
//  AdaptyPrice.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 08.09.2023.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{

    [DataContract]
    [Preserve]
    public partial class AdaptyPrice
    {
        private AdaptyPrice() { }

        /// Discount price of a product in a local currency.
        [DataMember(Name = "amount", IsRequired = true)]
        public readonly double Amount;

        /// The currency code of the locale used to format the price of the product.
        /// ///
        /// [Nullable]
        [DataMember(Name = "currency_code")]
        public readonly string CurrencyCode;

        /// The currency symbol of the locale used to format the price of the product.
        /// ///
        /// [Nullable]
        [DataMember(Name = "currency_symbol")]
        public readonly string CurrencySymbol;

        /// A formatted price of a discount for a user's locale.
        ///
        /// [Nullable]
        [DataMember(Name = "localized_string")]
        public readonly string LocalizedString;

        public override string ToString() => $"{nameof(Amount)}: {Amount}, " +
                   $"{nameof(CurrencyCode)}: {CurrencyCode}, " +
                   $"{nameof(CurrencySymbol)}: {CurrencySymbol}, " +
                   $"{nameof(LocalizedString)}: {LocalizedString}";
    }

}