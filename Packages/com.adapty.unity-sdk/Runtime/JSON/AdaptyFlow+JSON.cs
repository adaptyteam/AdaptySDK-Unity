//
//  AdaptyFlow+JSON.cs
//  AdaptySDK
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyFlow
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();

            node.Add("placement", Placement.ToJSONNode());
            node.Add("flow_id", InstanceIdentity);
            node.Add("flow_name", Name);
            node.Add("variation_id", VariationId);
            node.Add("response_created_at", _ResponseCreatedAt);

            if (FlowVersionId != null)
            {
                node.Add("flow_version_id", FlowVersionId);
            }

            if (RemoteConfigs.Count > 0)
            {
                var remoteConfigs = new JSONArray();
                foreach (var item in RemoteConfigs)
                {
                    remoteConfigs.Add(item.ToJSONNode());
                }
                node.Add("remote_configs", remoteConfigs);
            }

            var variations = new JSONArray();
            foreach (var item in Paywalls)
            {
                variations.Add(item.ToJSONNode());
            }

            node.Add("variations", variations);

            if (_PayloadData != null)
            {
                node.Add("payload_data", _PayloadData);
            }

            return node;
        }

        internal AdaptyFlow(JSONObject jsonNode)
        {
            Placement = jsonNode.GetPlacement("placement");
            InstanceIdentity = jsonNode.GetString("flow_id");
            Name = jsonNode.GetString("flow_name");
            VariationId = jsonNode.GetString("variation_id");
            _ResponseCreatedAt = jsonNode.GetLong("response_created_at");
            FlowVersionId = jsonNode.GetStringIfPresent("flow_version_id");
            RemoteConfigs = jsonNode.GetRemoteConfigList("remote_configs");
            Paywalls = jsonNode.GetAdaptyFlowPaywallList("variations");
            _PayloadData = jsonNode.GetStringIfPresent("payload_data");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyFlow GetFlow(this JSONNode node) => new AdaptyFlow(GetObject(node));

        internal static AdaptyFlow GetFlow(this JSONNode node, string aKey) =>
            new AdaptyFlow(GetObject(node, aKey));

        internal static AdaptyFlow GetFlowIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyFlow(obj);
        }
    }
}
