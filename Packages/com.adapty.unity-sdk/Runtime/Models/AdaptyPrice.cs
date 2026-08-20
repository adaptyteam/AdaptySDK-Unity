using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{

    /// <summary>
    /// A price as the store reports it: the amount, and the strings to show it with.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyPrice
    {
        private AdaptyPrice() { }

        /// <summary>
        /// Discount price of a product in a local currency.
        /// </summary>
        [DataMember(Name = "amount", IsRequired = true)]
        public readonly double Amount;

        /// <summary>
        /// The currency code of the locale used to format the price of the product.
        /// ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "currency_code")]
        public readonly string CurrencyCode;

        /// <summary>
        /// The currency symbol of the locale used to format the price of the product.
        /// ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "currency_symbol")]
        public readonly string CurrencySymbol;

        /// <summary>
        /// A formatted price of a discount for a user's locale.
        ///
        /// [Nullable]
        /// </summary>
        [DataMember(Name = "localized_string")]
        public readonly string LocalizedString;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() => $"{nameof(Amount)}: {Amount}, " +
                   $"{nameof(CurrencyCode)}: {CurrencyCode}, " +
                   $"{nameof(CurrencySymbol)}: {CurrencySymbol}, " +
                   $"{nameof(LocalizedString)}: {LocalizedString}";
    }

}
