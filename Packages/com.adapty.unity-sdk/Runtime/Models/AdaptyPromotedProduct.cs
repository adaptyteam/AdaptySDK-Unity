using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// A product for an App Store promoted in-app purchase. iOS only.
    /// </summary>
    /// <remarks>
    /// Arrives through <see cref="IAdaptyEventListener.OnReceivePromotedPurchase(AdaptyPromotedProduct)"/>
    /// when the user starts a purchase from the App Store product page. Hand it to
    /// <see cref="Adapty.MakePromotedPurchase(AdaptyPromotedProduct, System.Action{AdaptyPurchaseResult, AdaptyError})"/>
    /// to complete the purchase.
    /// </remarks>
    [DataContract]
    [Preserve]
    public sealed class AdaptyPromotedProduct
    {
        private AdaptyPromotedProduct() { }

        /// <summary>
        /// The unique identifier of the product in the App Store.
        /// </summary>
        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        public readonly string VendorProductId;

        /// <summary>
        /// The description of the product, localized by the storefront the user's device is
        /// connected to.
        /// </summary>
        [DataMember(Name = "localized_description", IsRequired = true)]
        public readonly string LocalizedDescription;

        /// <summary>
        /// The name of the product, localized by the storefront the user's device is connected to.
        /// </summary>
        [DataMember(Name = "localized_title", IsRequired = true)]
        public readonly string LocalizedTitle;

        /// <summary>
        /// Whether the product is available for family sharing in App Store Connect (iOS only).
        /// </summary>
#if UNITY_IOS
        [DataMember(Name = "is_family_shareable", IsRequired = true)]
#endif
        public readonly bool IsFamilyShareable;

        /// <summary>
        /// The region code of the locale used to format the price of the product.
        /// </summary>
        [DataMember(Name = "region_code")]
        public readonly string RegionCode;

        /// <summary>
        /// The object that represents the main price for the product.
        /// </summary>
        [DataMember(Name = "price", IsRequired = true)]
        public readonly AdaptyPrice Price;

        /// <summary>
        /// Detailed information about the subscription, including introductory offers, promotional offers, etc.
        /// </summary>
        /// <remarks>
        /// This is null for non-subscription products.
        /// </remarks>
        [DataMember(Name = "subscription")]
        public readonly AdaptySubscription Subscription;

        [DataMember(Name = "payload_data")]
        private readonly string _PayloadData;

        internal string PayloadData => _PayloadData;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(VendorProductId)}: {VendorProductId}, "
            + $"{nameof(LocalizedDescription)}: {LocalizedDescription}, "
            + $"{nameof(LocalizedTitle)}: {LocalizedTitle}, "
            + $"{nameof(IsFamilyShareable)}: {IsFamilyShareable}, "
            + $"{nameof(RegionCode)}: {RegionCode}, "
            + $"{nameof(Price)}: {Price}, "
            + $"{nameof(Subscription)}: {Subscription}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}";
    }
}
