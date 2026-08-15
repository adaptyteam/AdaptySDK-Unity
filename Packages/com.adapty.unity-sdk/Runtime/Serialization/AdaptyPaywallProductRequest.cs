using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// What the native side expects when a product is handed back to it - a strict subset of what
    /// it sent, with the subscription reduced to its offer identifier.
    /// </summary>
    /// <remarks>
    /// The model itself cannot express this: a member carries one contract name for both
    /// directions, so serializing <see cref="AdaptyPaywallProduct"/> as-is would return the whole
    /// product, price and localization included.
    /// </remarks>
    [DataContract]
    [Preserve]
    internal sealed class AdaptyPaywallProductRequest
    {
        internal AdaptyPaywallProductRequest(AdaptyPaywallProduct product)
        {
            VendorProductId = product.VendorProductId;
            AdaptyProductId = product.AdaptyProductId;
            AccessLevelId = product.AccessLevelId;
            ProductType = product.ProductType;
            PaywallVariationId = product.PaywallVariationId;
            PaywallABTestName = product.PaywallABTestName;
            PaywallName = product.PaywallName;
            PaywallProductIndex = product.PaywallProductIndex;
            WebPurchaseUrl = product.WebPurchaseUrl;
            PayloadData = product.PayloadData;

            var offer = product.Subscription?.Offer;
            if (offer != null)
            {
                Subscription = new AdaptySubscriptionOfferRequest(offer);
            }
        }

        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        [Preserve]
        private string VendorProductId { get; }

        [DataMember(Name = "adapty_product_id", IsRequired = true)]
        [Preserve]
        private string AdaptyProductId { get; }

        [DataMember(Name = "access_level_id", IsRequired = true)]
        [Preserve]
        private string AccessLevelId { get; }

        [DataMember(Name = "product_type", IsRequired = true)]
        [Preserve]
        private string ProductType { get; }

        [DataMember(Name = "paywall_variation_id", IsRequired = true)]
        [Preserve]
        private string PaywallVariationId { get; }

        [DataMember(Name = "paywall_ab_test_name", IsRequired = true)]
        [Preserve]
        private string PaywallABTestName { get; }

        [DataMember(Name = "paywall_name", IsRequired = true)]
        [Preserve]
        private string PaywallName { get; }

        [DataMember(Name = "paywall_product_index", IsRequired = true)]
        [Preserve]
        private int PaywallProductIndex { get; }

        [DataMember(Name = "web_purchase_url")]
        [Preserve]
        private string WebPurchaseUrl { get; }

        [DataMember(Name = "payload_data")]
        [Preserve]
        private string PayloadData { get; }

        [DataMember(Name = "subscription")]
        [Preserve]
        private AdaptySubscriptionOfferRequest Subscription { get; }
    }
}
