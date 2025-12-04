//
//  AdaptyPaywallProduct.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    /// <summary>
    /// Represents a product available for purchase in a paywall.
    /// </summary>
    /// <remarks>
    /// This class contains all information about a product including pricing, subscription details, and metadata.
    /// Read more at <see href="https://adapty.io/docs/product">Adapty Documentation</see>
    /// </remarks>
    public partial class AdaptyPaywallProduct
    {
        /// <summary>
        /// The unique identifier of the product in the App Store or Google Play Store.
        /// </summary>
        public readonly string VendorProductId;

        /// <summary>
        /// The unique identifier of the product in Adapty.
        /// </summary>
        public readonly string AdaptyProductId;

        /// <summary>
        /// The identifier of the access level configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// When a user purchases this product, they will be granted access to this access level.
        /// </remarks>
        public readonly string AccessLevelId;

        /// <summary>
        /// The type of the product (e.g., "consumable", "non_consumable", "subscription").
        /// </summary>
        public readonly string ProductType;

        /// <summary>
        /// The identifier of the variation, used to attribute purchases to the paywall.
        /// </summary>
        public readonly string PaywallVariationId;

        /// <summary>
        /// The parent A/B test name associated with this product.
        /// </summary>
        public readonly string PaywallABTestName;

        /// <summary>
        /// The parent paywall name associated with this product.
        /// </summary>
        public readonly string PaywallName;

        /// <summary>
        /// A localized description of the product.
        /// </summary>
        public readonly string LocalizedDescription;

        /// <summary>
        /// The localized name of the product.
        /// </summary>
        public readonly string LocalizedTitle;

        /// <summary>
        /// Indicates whether the product is available for family sharing in App Store Connect (iOS only).
        /// </summary>
        public readonly bool IsFamilyShareable;

        /// <summary>
        /// The product locale region code.
        /// </summary>
        /// <remarks>
        /// This can be null if the region code is not available.
        /// </remarks>
        public readonly string RegionCode;

        /// <summary>
        /// The object that represents the main price for the product.
        /// </summary>
        public readonly AdaptyPrice Price;

        /// <summary>
        /// Detailed information about the subscription, including introductory offers, promotional offers, etc.
        /// </summary>
        /// <remarks>
        /// This is null for non-subscription products.
        /// </remarks>
        public readonly AdaptySubscription Subscription; //nullable

        /// <summary>
        /// The index of the product in the paywall (0-based).
        /// </summary>
        public readonly int PaywallProductIndex;

        private readonly string _PayloadData;
        private readonly string _WebPurchaseUrl;

        public override string ToString() =>
            $"{nameof(VendorProductId)}: {VendorProductId}, "
            + $"{nameof(AdaptyProductId)}: {AdaptyProductId}, "
            + $"{nameof(AccessLevelId)}: {AccessLevelId}, "
            + $"{nameof(ProductType)}: {ProductType}, "
            + $"{nameof(LocalizedDescription)}: {LocalizedDescription}, "
            + $"{nameof(LocalizedTitle)}: {LocalizedTitle}, "
            + $"{nameof(RegionCode)}: {RegionCode}, "
            + $"{nameof(IsFamilyShareable)}: {IsFamilyShareable}, "
            + $"{nameof(PaywallVariationId)}: {PaywallVariationId}, "
            + $"{nameof(PaywallABTestName)}: {PaywallABTestName}, "
            + $"{nameof(PaywallName)}: {PaywallName}, "
            + $"{nameof(Price)}: {Price}, "
            + $"{nameof(Subscription)}: {Subscription}, "
            + $"{nameof(PaywallProductIndex)}: {PaywallProductIndex}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}, "
            + $"{nameof(_WebPurchaseUrl)}: {_WebPurchaseUrl}";
    }
}
