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

    public partial class AdaptyPaywall
    {
        /// An `AdaptyPlacement` object, that contains information about the placement of the paywall.
        public readonly AdaptyPlacement Placement;

        public readonly string InstanceIdentity;

        /// A paywall name configured in Adapty Dashboard.
        public readonly string Name;

        /// The identifier of the variation, used to attribute purchases to the paywall.
        public readonly string VariationId;

        /// The custom JSON formatted data configured in Adapty Dashboard.
        /// (String representation)
        public readonly AdaptyRemoteConfig RemoteConfig; // nullable

        /// If `true`, it is possible to use Adapty Paywall Builder.
        /// Read more here: https://docs.adapty.io/docs/paywall-builder-getting-started
        public bool HasViewConfiguration
        {
            get { return _ViewConfiguration != null; }
        }
        private readonly ViewConfiguration _ViewConfiguration;

        private readonly IList<ProductReference> _Products;
        private readonly int _ResponseCreatedAt;
        private readonly string _PayloadData; // nullable
        private readonly string _WebPurchaseUrl; // nullable
        private readonly string _RequestLocale;

        /// Array of related products ids.
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

        /// Array of related product references.
        public IList<ProductReference> Products => _Products;

        /// Array of related product identifiers.
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
        public int Revision => Placement.Revision;

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
