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
            var remoteConfig = new JSONObject();
            remoteConfig.Add("lang", Locale);
            if (RemoteConfigString != null) remoteConfig.Add("data", RemoteConfigString);

            var node = new JSONObject();
            node.Add("developer_id", PlacementId);
            node.Add("paywall_id", _InstanceIdentity);
            node.Add("paywall_name", Name);
            node.Add("ab_test_name", ABTestName);
            node.Add("variation_id", VariationId);
            node.Add("revision", Revision);
            node.Add("use_paywall_builder", HasViewConfiguration);
            node.Add("remote_config", remoteConfig);
            var products = new JSONArray();
            foreach (var item in _Products)
            {
                products.Add(item.ToJSONNode());
            }
            node.Add("products", products);
            node.Add("paywall_updated_at", _Version);
            if (_PayloadData != null) node.Add("payload_data", _PayloadData);

            return node;
        }

        internal AdaptyPaywall(JSONObject jsonNode)
        {
            PlacementId = jsonNode.GetString("developer_id");
            _InstanceIdentity = jsonNode.GetString("paywall_id");
            Name = jsonNode.GetString("paywall_name");
            ABTestName = jsonNode.GetString("ab_test_name");
            VariationId = jsonNode.GetString("variation_id");
            Revision = jsonNode.GetInteger("revision");
            HasViewConfiguration = jsonNode.GetBooleanIfPresent("use_paywall_builder") ?? false;

            var remoteConfig = jsonNode.GetObject("remote_config");
            Locale = remoteConfig.GetString("lang");
            RemoteConfigString = remoteConfig.GetStringIfPresent("data");

            _Products = jsonNode.GetProductReferenceList("products");
            _Version = jsonNode.GetInteger("paywall_updated_at");
            _PayloadData = jsonNode.GetStringIfPresent("payload_data");
        }
    }

}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
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