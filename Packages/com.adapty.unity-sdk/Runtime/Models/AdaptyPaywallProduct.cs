//
//  AdaptyPaywallProduct.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Represents a product available for purchase in a paywall.
    /// </summary>
    /// <remarks>
    /// This class contains all information about a product including pricing, subscription details, and metadata.
    /// Read more at <see href="https://adapty.io/docs/product">Adapty Documentation</see>
    /// </remarks>
    [DataContract]
    [Preserve]
    public partial class AdaptyPaywallProduct
    {
        private AdaptyPaywallProduct() { }

        /// <summary>
        /// The unique identifier of the product in the App Store or Google Play Store.
        /// </summary>
        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        public readonly string VendorProductId;

        /// <summary>
        /// The unique identifier of the product in Adapty.
        /// </summary>
        [DataMember(Name = "adapty_product_id", IsRequired = true)]
        public readonly string AdaptyProductId;

        /// <summary>
        /// The identifier of the product within the flow.
        /// </summary>
        /// <remarks>
        /// This can be null if the product does not belong to a flow.
        /// </remarks>
        [DataMember(Name = "flow_product_id")]
        public readonly string FlowProductId; //nullable

        /// <summary>
        /// The identifier of the access level configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// When a user purchases this product, they will be granted access to this access level.
        /// </remarks>
        [DataMember(Name = "access_level_id", IsRequired = true)]
        public readonly string AccessLevelId;

        /// <summary>
        /// The type of the product (e.g., "consumable", "non_consumable", "subscription").
        /// </summary>
        [DataMember(Name = "product_type", IsRequired = true)]
        public readonly string ProductType;

        /// <summary>
        /// The identifier of the variation, used to attribute purchases to the paywall.
        /// </summary>
        [DataMember(Name = "paywall_variation_id", IsRequired = true)]
        public readonly string PaywallVariationId;

        /// <summary>
        /// The parent A/B test name associated with this product.
        /// </summary>
        [DataMember(Name = "paywall_ab_test_name", IsRequired = true)]
        public readonly string PaywallABTestName;

        /// <summary>
        /// The parent paywall name associated with this product.
        /// </summary>
        [DataMember(Name = "paywall_name", IsRequired = true)]
        public readonly string PaywallName;

        /// <summary>
        /// A localized description of the product.
        /// </summary>
        [DataMember(Name = "localized_description", IsRequired = true)]
        public readonly string LocalizedDescription;

        /// <summary>
        /// The localized name of the product.
        /// </summary>
        [DataMember(Name = "localized_title", IsRequired = true)]
        public readonly string LocalizedTitle;

        /// <summary>
        /// Indicates whether the product is available for family sharing in App Store Connect (iOS only).
        /// </summary>
        #if UNITY_IOS
        [DataMember(Name = "is_family_shareable", IsRequired = true)]
#endif
        public readonly bool IsFamilyShareable;

        /// <summary>
        /// The product locale region code.
        /// </summary>
        /// <remarks>
        /// This can be null if the region code is not available.
        /// </remarks>
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
        public readonly AdaptySubscription Subscription; //nullable

        /// <summary>
        /// The index of the product in the paywall (0-based).
        /// </summary>
        [DataMember(Name = "paywall_product_index", IsRequired = true)]
        public readonly int PaywallProductIndex;

        [DataMember(Name = "payload_data")]
        private readonly string _PayloadData;
        [DataMember(Name = "web_purchase_url")]
        private readonly string _WebPurchaseUrl;

        internal string PayloadData => _PayloadData;
        internal string WebPurchaseUrl => _WebPurchaseUrl;

        public override string ToString() =>
            $"{nameof(VendorProductId)}: {VendorProductId}, "
            + $"{nameof(AdaptyProductId)}: {AdaptyProductId}, "
            + $"{nameof(FlowProductId)}: {FlowProductId}, "
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
