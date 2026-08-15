using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

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
    [Preserve]
    public sealed class AdaptyFlow
    {
        private AdaptyFlow() => Freeze();

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
        public readonly string FlowVersionId;

        /// <summary>
        /// Array of custom JSON formatted data configured in the Adapty Dashboard, one entry per locale.
        /// </summary>
        [DataMember(Name = "remote_configs")]
        private readonly List<AdaptyRemoteConfig> _RemoteConfigs = new List<AdaptyRemoteConfig>();

        /// <summary>
        /// The remote configs of the flow, one per localization. Empty when none is configured;
        /// <see cref="RemoteConfig"/> is the first of them.
        /// </summary>
        [Preserve]
        public IReadOnlyList<AdaptyRemoteConfig> RemoteConfigs { get; private set; }

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
        private readonly List<AdaptyFlowPaywall> _Paywalls;

        /// <summary>
        /// The paywall variations this flow offers.
        /// </summary>
        [Preserve]
        public IReadOnlyList<AdaptyFlowPaywall> Paywalls { get; private set; }

        /// <summary>
        /// The renderer's custom-layout schema. Opaque to the app: carried only so a flow handed
        /// back to the native side keeps it.
        /// </summary>
        [DataMember(Name = "ui_schema")]
        private readonly JObject _UiSchema;

        [DataMember(Name = "response_created_at", IsRequired = true)]
        private readonly long _ResponseCreatedAt;
        [DataMember(Name = "payload_data")]
        private readonly string _PayloadData;

        [Preserve]
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => Freeze();

        private void Freeze()
        {
            RemoteConfigs = new ReadOnlyCollection<AdaptyRemoteConfig>(_RemoteConfigs);
            Paywalls =
                _Paywalls is null
                    ? null
                    : new ReadOnlyCollection<AdaptyFlowPaywall>(_Paywalls);
        }

        /// <summary>
        /// Array of vendor product IDs (App Store or Google Play product identifiers) aggregated across all paywall variations of this flow.
        /// </summary>
        public IReadOnlyList<string> VendorProductIds
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
                return new ReadOnlyCollection<string>(list);
            }
        }

        /// <summary>
        /// Array of product identifiers aggregated across all paywall variations of this flow.
        /// </summary>
        public IReadOnlyList<AdaptyProductIdentifier> ProductIdentifiers
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
                return new ReadOnlyCollection<AdaptyProductIdentifier>(list);
            }
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Placement)}: {Placement}, "
            + $"{nameof(InstanceIdentity)}: {InstanceIdentity}, "
            + $"{nameof(Name)}: {Name}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(FlowVersionId)}: {FlowVersionId}, "
            + $"{nameof(RemoteConfigs)}: {RemoteConfigs}, "
            + $"{nameof(Paywalls)}: {Paywalls}, "
            + $"{nameof(_UiSchema)}: {_UiSchema?.ToString(Newtonsoft.Json.Formatting.None)}, "
            + $"{nameof(_ResponseCreatedAt)}: {_ResponseCreatedAt}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}";
    }
}
