//
//  AdaptyFlowPaywall.cs
//  AdaptySDK
//

using System.Collections.Generic;

namespace AdaptySDK
{
    /// <summary>
    /// Represents a paywall variation of an <see cref="AdaptyFlow"/>.
    /// </summary>
    /// <remarks>
    /// A flow paywall is a set of products that can be displayed to users within a flow.
    /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
    /// </remarks>
    public partial class AdaptyFlowPaywall
    {
        /// <summary>
        /// An <see cref="AdaptyPlacement"/> object that contains information about the placement of the paywall.
        /// </summary>
        public readonly AdaptyPlacement Placement;

        /// <summary>
        /// A unique identifier for this paywall instance.
        /// </summary>
        public readonly string InstanceIdentity;

        /// <summary>
        /// The paywall name configured in the Adapty Dashboard.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The identifier of the variation, used to attribute purchases to the paywall.
        /// </summary>
        public readonly string VariationId;

        internal readonly IList<ProductReference> _Products;
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
