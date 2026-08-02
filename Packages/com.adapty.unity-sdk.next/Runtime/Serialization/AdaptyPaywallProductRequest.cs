//
//  AdaptyPaywallProductRequest.cs
//  AdaptySDK
//

using System.Runtime.Serialization;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// What the native side expects when a product is handed back to it - a strict subset of what
    /// it sent, with the subscription offer flattened to an identifier.
    /// </summary>
    /// <remarks>
    /// The model itself cannot express this: a member carries one contract name for both
    /// directions, so serializing <see cref="AdaptyPaywallProduct"/> as-is would return the whole
    /// product, price and localization included.
    /// </remarks>
    [DataContract]
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
                Offer = new OfferIdentifier(offer.Identifier, offer.Type);
            }
        }

        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        private string VendorProductId { get; }

        [DataMember(Name = "adapty_product_id", IsRequired = true)]
        private string AdaptyProductId { get; }

        [DataMember(Name = "access_level_id", IsRequired = true)]
        private string AccessLevelId { get; }

        [DataMember(Name = "product_type", IsRequired = true)]
        private string ProductType { get; }

        [DataMember(Name = "paywall_variation_id", IsRequired = true)]
        private string PaywallVariationId { get; }

        [DataMember(Name = "paywall_ab_test_name", IsRequired = true)]
        private string PaywallABTestName { get; }

        [DataMember(Name = "paywall_name", IsRequired = true)]
        private string PaywallName { get; }

        [DataMember(Name = "paywall_product_index", IsRequired = true)]
        private int PaywallProductIndex { get; }

        [DataMember(Name = "web_purchase_url")]
        private string WebPurchaseUrl { get; }

        [DataMember(Name = "payload_data")]
        private string PayloadData { get; }

        [DataMember(Name = "subscription_offer_identifier")]
        private OfferIdentifier Offer { get; }

        [DataContract]
        private sealed class OfferIdentifier
        {
            internal OfferIdentifier(string id, AdaptySubscriptionOfferType type)
            {
                Id = id;
                Type = type;
            }

            [DataMember(Name = "id")]
            private string Id { get; }

            [DataMember(Name = "type", IsRequired = true)]
            private AdaptySubscriptionOfferType Type { get; }
        }
    }
}
