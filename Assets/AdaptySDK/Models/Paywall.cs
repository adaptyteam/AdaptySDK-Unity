//
//  Paywall.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class Paywall
        {
            /// The identifier of the paywall, configured in Adapty Dashboard.
            public readonly string PlacementId;

            private readonly string _InstanceIdentity;

            /// Paywall name
            public readonly string Name;

            /// Paywall A/B test name
            public readonly string ABTestName;

            /// The identifier of the variation, used to attribute purchases to the paywall.
            public readonly string VariationId;

            /// The current revision (version) of the paywall.
            /// Every change within the paywall creates a new revision.
            public readonly int Revision;

            /// If `true`, it is possible to use Adapty Paywall Builder.
            /// Read more here: https://docs.adapty.io/docs/paywall-builder-getting-started
            public readonly bool HasViewConfiguration;

            /// And identifier of a paywall locale.
            public readonly string Locale;

            /// The custom JSON formatted data configured in Adapty Dashboard.
            /// (String representation)
            public readonly string RemoteConfigString; //nullable

            /// An array of ProductModel objects related to this paywall.
            private readonly IList<ProductReference> _Products;

            private readonly int _Version;

            private readonly string _PayloadData;

            /// Array of related products ids.
            public IList<string> VendorProductIds
            {
                get
                {
                    var list = new List<string>();
                    foreach (var item in _Products)
                    {
                        list.Add(item.VendorId);
                    }
                    return list;
                }
            }

            /// A custom dictionary configured in Adapty Dashboard for this paywall (same as `remoteConfigString`)
            public IDictionary<string, dynamic> RemoteConfig
            {
                get
                {
                    if (string.IsNullOrEmpty(RemoteConfigString)) return null;
                    return JSONNode.Parse(RemoteConfigString).GetDictionary();
                }
            }

            public override string ToString() => $"{nameof(PlacementId)}: {PlacementId}, " +
                       $"{nameof(_InstanceIdentity)}: {_InstanceIdentity}, " +
                       $"{nameof(Name)}: {Name}, " +
                       $"{nameof(ABTestName)}: {ABTestName}, " +
                       $"{nameof(VariationId)}: {VariationId}, " +
                       $"{nameof(Revision)}: {Revision}, " +
                       $"{nameof(Locale)}: {Locale}, " +
                       $"{nameof(_Products)}: {_Products}, " +
                       $"{nameof(_Version)}: {_Version}";
        }
    }
}