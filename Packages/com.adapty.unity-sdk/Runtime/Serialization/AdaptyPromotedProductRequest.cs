using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// What the native side expects when a promoted product is handed back to it - a strict subset
    /// of what it sent, with the subscription reduced to its offer identifier.
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
            VendorProductId = product.VendorProductId;
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

        [DataMember(Name = "payload_data")]
        [Preserve]
        private string PayloadData { get; }

        [DataMember(Name = "subscription")]
        [Preserve]
        private AdaptySubscriptionOfferRequest Subscription { get; }
    }
}
