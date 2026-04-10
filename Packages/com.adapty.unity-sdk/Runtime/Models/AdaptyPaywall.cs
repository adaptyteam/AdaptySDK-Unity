//
//  AdaptyPaywall.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    /// <summary>
    /// Represents a paywall configuration in Adapty.
    /// </summary>
    /// <remarks>
    /// A paywall is a set of products that can be displayed to users. It contains information about the placement, products, and visual configuration.
    /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
    /// </remarks>
    public partial class AdaptyPaywall
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

        /// <summary>
        /// The custom JSON formatted data configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// This can be null if no remote config is configured for the paywall.
        /// </remarks>
        public readonly AdaptyRemoteConfig RemoteConfig; // nullable

        /// <summary>
        /// Indicates whether it is possible to use Adapty Paywall Builder for this paywall.
        /// </summary>
        /// <remarks>
        /// If <c>true</c>, you can use <see cref="AdaptyUI.CreatePaywallView(AdaptyPaywall, AdaptyUICreatePaywallViewParameters, Action{AdaptyUIPaywallView, AdaptyError})"/> to create a native paywall view.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
        /// </remarks>
        public bool HasViewConfiguration
        {
            get { return _ViewConfiguration != null; }
        }
        private readonly ViewConfiguration _ViewConfiguration;

        private readonly IList<ProductReference> _Products;
        private readonly long _ResponseCreatedAt;
        private readonly string _PayloadData; // nullable
        private readonly string _WebPurchaseUrl; // nullable
        private readonly string _RequestLocale;

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
        /// Array of product references associated with this paywall.
        /// </summary>
        public IList<ProductReference> Products => _Products;

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

        /// The identifier of the paywall, configured in Adapty Dashboard.
        [System.Obsolete("Use Placement.Id instead")]
        public string PlacementId => Placement.Id;

        [System.Obsolete("Use Placement.AudienceName instead")]
        public string AudienceName => Placement.AudienceName;

        /// Paywall A/B test name
        [System.Obsolete("Use Placement.ABTestName instead")]
        public string ABTestName => Placement.ABTestName;

        /// The current revision (version) of the paywall.
        /// Every change within the paywall creates a new revision.
        [System.Obsolete("Use Placement.Revision instead")]
        public long Revision => Placement.Revision;

        [System.Obsolete("Use RemoteConfig.Data instead")]
        public string RemoteConfigString => RemoteConfig.Data;

        [System.Obsolete("Use RemoteConfig.Locale instead")]
        public string Locale => RemoteConfig?.Locale;

        public override string ToString() =>
            $"{nameof(Placement)}: {Placement}, "
            + $"{nameof(InstanceIdentity)}: {InstanceIdentity}, "
            + $"{nameof(Name)}: {Name}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(HasViewConfiguration)}: {HasViewConfiguration}, "
            + $"{nameof(RemoteConfig)}: {RemoteConfig}, "
            + $"{nameof(_Products)}: {_Products}, "
            + $"{nameof(_ResponseCreatedAt)}: {_ResponseCreatedAt}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}, "
            + $"{nameof(_WebPurchaseUrl)}: {_WebPurchaseUrl}, "
            + $"{nameof(_RequestLocale)}: {_RequestLocale}";
    }
}
