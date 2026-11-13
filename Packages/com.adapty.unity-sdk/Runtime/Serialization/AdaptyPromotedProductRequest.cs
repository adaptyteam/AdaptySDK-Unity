using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// What the native side expects when a promoted product is handed back to it - a strict subset
    /// of what it sent, with the subscription reduced to its billing plan and offer identifier.
    /// </summary>
    /// <remarks>
    /// The model itself cannot express this: a member carries one contract name for both
    /// directions, so serializing <see cref="AdaptyPromotedProduct"/> as-is would return the whole
    /// product, price and localization included.
    /// </remarks>
    [DataContract]
    [Preserve]
    internal sealed class AdaptyPromotedProductRequest
    {
        internal AdaptyPromotedProductRequest(AdaptyPromotedProduct product)
        {
            PurchaseIntentId = product.PurchaseIntentId;
            VendorProductId = product.VendorProductId;
            PayloadData = product.PayloadData;
            Subscription = AdaptySubscriptionRequest.From(product.Subscription);
        }

        // iOS needs it to find the StoreKit purchase intent; it is absent only if the event lacked it.
        [DataMember(Name = "purchase_intent_id")]
        [Preserve]
        private string PurchaseIntentId { get; }

        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        [Preserve]
        private string VendorProductId { get; }

        [DataMember(Name = "payload_data")]
        [Preserve]
        private string PayloadData { get; }

        [DataMember(Name = "subscription")]
        [Preserve]
        private AdaptySubscriptionRequest Subscription { get; }
    }
}
