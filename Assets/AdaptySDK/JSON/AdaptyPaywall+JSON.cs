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

            node.Add("placement", Placement.ToJSONNode());
            node.Add("paywall_id", InstanceIdentity);
            node.Add("paywall_name", Name);
            node.Add("variation_id", VariationId);
            node.Add("response_created_at", _ResponseCreatedAt);
            node.Add("request_locale", _RequestLocale);

            if (RemoteConfig != null)
            {
                node.Add("remote_config", RemoteConfig.ToJSONNode());
            }

            if (_WebPurchaseUrl != null)
            {
                node.Add("web_purchase_url", _WebPurchaseUrl);
            }

            if (_ViewConfiguration != null)
            {
                node.Add("paywall_builder", _ViewConfiguration.ToJSONNode());
            }

            var products = new JSONArray();
            foreach (var item in _Products)
            {
                products.Add(item.ToJSONNode());
            }

            node.Add("products", products);

            if (_PayloadData != null)
            {
                node.Add("payload_data", _PayloadData);
            }

            return node;
        }

        internal AdaptyPaywall(JSONObject jsonNode)
        {
            Placement = jsonNode.GetPlacement("placement");
            InstanceIdentity = jsonNode.GetString("paywall_id");
            Name = jsonNode.GetString("paywall_name");
            _ResponseCreatedAt = jsonNode.GetLong("response_created_at");
            VariationId = jsonNode.GetString("variation_id");
            RemoteConfig = jsonNode.GetRemoteConfigIfPresent("remote_config");
            _ViewConfiguration = jsonNode.GetAdaptyPaywallViewConfigurationIfPresent(
                "paywall_builder"
            );
            _Products = jsonNode.GetAdaptyPaywallProductReferenceList("products");
            _PayloadData = jsonNode.GetStringIfPresent("payload_data");
            _WebPurchaseUrl = jsonNode.GetStringIfPresent("web_purchase_url");
            _RequestLocale = jsonNode.GetStringIfPresent("request_locale");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPaywall GetPaywall(this JSONNode node) =>
            new AdaptyPaywall(GetObject(node));

        internal static AdaptyPaywall GetPaywall(this JSONNode node, string aKey) =>
            new AdaptyPaywall(GetObject(node, aKey));

        internal static AdaptyPaywall GetPaywallIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyPaywall(obj);
        }
    }
}
