//
//  AdaptyPaywall+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;


    public partial class AdaptyPaywall
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("developer_id", PlacementId);
            node.Add("paywall_id", _InstanceIdentity);
            node.Add("paywall_name", Name);
            node.Add("ab_test_name", ABTestName);
            node.Add("variation_id", VariationId);
            node.Add("revision", Revision);
            node.Add("response_created_at", _Version);

            if (RemoteConfigString != null)
            {
                var remoteConfig = new JSONObject();
                remoteConfig.Add("lang", Locale);
                remoteConfig.Add("data", RemoteConfigString);
                node.Add("remote_config", remoteConfig);
            }

            node.Add("paywall_builder", _ViewConfiguration.ToJSONNode());

            var products = new JSONArray();
            foreach (var item in _Products)
            {
                products.Add(item.ToJSONNode());
            }
            node.Add("products", products);

            if (_PayloadData != null) node.Add("payload_data", _PayloadData);

            return node;
        }

        internal AdaptyPaywall(JSONObject jsonNode)
        {
            PlacementId = jsonNode.GetString("developer_id");
            _InstanceIdentity = jsonNode.GetString("paywall_id");
            Name = jsonNode.GetString("paywall_name");
            _Version = jsonNode.GetInteger("response_created_at");
            Revision = jsonNode.GetInteger("revision");
            VariationId = jsonNode.GetString("variation_id");
            ABTestName = jsonNode.GetString("ab_test_name");
            var remoteConfig = jsonNode.GetObjectIfPresent("remote_config");
            if (remoteConfig != null)
            {
                Locale = remoteConfig.GetString("lang");
                RemoteConfigString = remoteConfig.GetStringIfPresent("data");
            }
            else
            {
                Locale = null;
                RemoteConfigString = null;
            }

            _ViewConfiguration = jsonNode.GetAdaptyPaywallViewConfigurationIfPresent("paywall_builder");
            _Products = jsonNode.GetAdaptyPaywallProductReferenceList("products");
            _PayloadData = jsonNode.GetStringIfPresent("payload_data");
        }
    }

}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPaywall GetPaywall(this JSONNode node)
            => new AdaptyPaywall(GetObject(node));
        internal static AdaptyPaywall GetPaywall(this JSONNode node, string aKey)
             => new AdaptyPaywall(GetObject(node, aKey));

        internal static AdaptyPaywall GetPaywallIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyPaywall(obj);
        }
    }
}