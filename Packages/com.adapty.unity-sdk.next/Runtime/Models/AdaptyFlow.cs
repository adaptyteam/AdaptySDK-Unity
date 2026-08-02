//
//  AdaptyFlow.cs
//  AdaptySDK
//

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Represents a flow configuration in Adapty.
    /// </summary>
    /// <remarks>
    /// A flow is a set of paywall variations that can be displayed to users. It contains information about the placement, paywalls, and remote configs.
    /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
    /// </remarks>
    [DataContract]
    public partial class AdaptyFlow
    {
        private AdaptyFlow() { }

        /// <summary>
        /// An <see cref="AdaptyPlacement"/> object that contains information about the placement of the flow.
        /// </summary>
        [DataMember(Name = "placement", IsRequired = true)]
        public readonly AdaptyPlacement Placement;

        /// <summary>
        /// A unique identifier for this flow instance.
        /// </summary>
        [DataMember(Name = "flow_id", IsRequired = true)]
        public readonly string InstanceIdentity;

        /// <summary>
        /// The flow name configured in the Adapty Dashboard.
        /// </summary>
        [DataMember(Name = "flow_name", IsRequired = true)]
        public readonly string Name;

        /// <summary>
        /// The identifier of the variation, used to attribute purchases to the flow.
        /// </summary>
        [DataMember(Name = "variation_id", IsRequired = true)]
        public readonly string VariationId;

        /// <summary>
        /// The identifier of the flow version.
        /// </summary>
        /// <remarks>
        /// This can be null if the version identifier is not available.
        /// </remarks>
        [DataMember(Name = "flow_version_id")]
        public readonly string FlowVersionId; // nullable

        /// <summary>
        /// Array of custom JSON formatted data configured in the Adapty Dashboard, one entry per locale.
        /// </summary>
        [DataMember(Name = "remote_configs")]
        public readonly IList<AdaptyRemoteConfig> RemoteConfigs = new List<AdaptyRemoteConfig>();

        private bool ShouldSerializeRemoteConfigs() => RemoteConfigs.Count > 0;

        /// <summary>
        /// The first custom JSON formatted data configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// This can be null if no remote config is configured for the flow. Use <see cref="RemoteConfigs"/> to access configs for a specific locale.
        /// </remarks>
        public AdaptyRemoteConfig RemoteConfig
        {
            get { return RemoteConfigs.Count > 0 ? RemoteConfigs[0] : null; }
        }

        /// <summary>
        /// Array of paywall variations associated with this flow.
        /// </summary>
        [DataMember(Name = "variations", IsRequired = true)]
        public readonly IList<AdaptyFlowPaywall> Paywalls;

        [DataMember(Name = "response_created_at", IsRequired = true)]
        private readonly long _ResponseCreatedAt;
        [DataMember(Name = "payload_data")]
        private readonly string _PayloadData; // nullable

        /// <summary>
        /// Array of vendor product IDs (App Store or Google Play product identifiers) aggregated across all paywall variations of this flow.
        /// </summary>
        public IList<string> VendorProductIds
        {
            get
            {
                var list = new List<string>();
                var seen = new HashSet<string>();
                foreach (var paywall in Paywalls)
                {
                    foreach (var item in paywall.VendorProductIds)
                    {
                        if (seen.Add(item))
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Array of product identifiers aggregated across all paywall variations of this flow.
        /// </summary>
        public IList<AdaptyProductIdentifier> ProductIdentifiers
        {
            get
            {
                var list = new List<AdaptyProductIdentifier>();
                var seen = new HashSet<AdaptyProductIdentifier>();
                foreach (var paywall in Paywalls)
                {
                    foreach (var item in paywall.ProductIdentifiers)
                    {
                        if (seen.Add(item))
                        {
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
        }

        public override string ToString() =>
            $"{nameof(Placement)}: {Placement}, "
            + $"{nameof(InstanceIdentity)}: {InstanceIdentity}, "
            + $"{nameof(Name)}: {Name}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(FlowVersionId)}: {FlowVersionId}, "
            + $"{nameof(RemoteConfigs)}: {RemoteConfigs}, "
            + $"{nameof(Paywalls)}: {Paywalls}, "
            + $"{nameof(_ResponseCreatedAt)}: {_ResponseCreatedAt}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}";
    }
}
