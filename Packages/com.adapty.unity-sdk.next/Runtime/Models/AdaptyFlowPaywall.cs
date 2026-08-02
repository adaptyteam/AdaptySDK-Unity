//
//  AdaptyFlowPaywall.cs
//  AdaptySDK
//

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Represents a paywall variation of an <see cref="AdaptyFlow"/>.
    /// </summary>
    /// <remarks>
    /// A flow paywall is a set of products that can be displayed to users within a flow.
    /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
    /// </remarks>
    [DataContract]
    public partial class AdaptyFlowPaywall
    {
        private AdaptyFlowPaywall() { }

        /// <summary>
        /// An <see cref="AdaptyPlacement"/> object that contains information about the placement of the paywall.
        /// </summary>
        [DataMember(Name = "placement", IsRequired = true)]
        public readonly AdaptyPlacement Placement;

        /// <summary>
        /// A unique identifier for this paywall instance.
        /// </summary>
        [DataMember(Name = "paywall_id", IsRequired = true)]
        public readonly string InstanceIdentity;

        /// <summary>
        /// The paywall name configured in the Adapty Dashboard.
        /// </summary>
        [DataMember(Name = "paywall_name", IsRequired = true)]
        public readonly string Name;

        /// <summary>
        /// The identifier of the variation, used to attribute purchases to the paywall.
        /// </summary>
        [DataMember(Name = "variation_id", IsRequired = true)]
        public readonly string VariationId;

        [DataMember(Name = "products", IsRequired = true)]
        internal readonly IList<ProductReference> _Products;
        [DataMember(Name = "web_purchase_url")]
        private readonly string _WebPurchaseUrl; // nullable

        /// <summary>
        /// Array of vendor product IDs (App Store or Google Play product identifiers) associated with this paywall.
        /// </summary>
        public IList<string> VendorProductIds
        {
            get
            {
                var list = new List<string>();
                foreach (var item in _Products)
                {
                    list.Add(item.VendorProductId);
                }
                return list;
            }
        }

        /// <summary>
        /// Array of product identifiers associated with this paywall.
        /// </summary>
        public IList<AdaptyProductIdentifier> ProductIdentifiers
        {
            get
            {
                var list = new List<AdaptyProductIdentifier>();
                foreach (var product in _Products)
                {
                    list.Add(product.ToAdaptyProductIdentifier());
                }
                return list;
            }
        }

        public override string ToString() =>
            $"{nameof(Placement)}: {Placement}, "
            + $"{nameof(InstanceIdentity)}: {InstanceIdentity}, "
            + $"{nameof(Name)}: {Name}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(_Products)}: {_Products}, "
            + $"{nameof(_WebPurchaseUrl)}: {_WebPurchaseUrl}";
    }
}
