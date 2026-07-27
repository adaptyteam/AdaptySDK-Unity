//
//  AdaptyFlowPaywall+JSON.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyFlowPaywall
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();

            node.Add("placement", Placement.ToJSONNode());
            node.Add("paywall_id", InstanceIdentity);
            node.Add("paywall_name", Name);
            node.Add("variation_id", VariationId);

            var products = new JSONArray();
            foreach (var item in _Products)
            {
                products.Add(item.ToJSONNode());
            }

            node.Add("products", products);

            if (_WebPurchaseUrl != null)
            {
                node.Add("web_purchase_url", _WebPurchaseUrl);
            }

            return node;
        }

        internal AdaptyFlowPaywall(JSONObject jsonNode)
        {
            Placement = jsonNode.GetPlacement("placement");
            InstanceIdentity = jsonNode.GetString("paywall_id");
            Name = jsonNode.GetString("paywall_name");
            VariationId = jsonNode.GetString("variation_id");
            _Products = jsonNode.GetAdaptyFlowPaywallProductReferenceList("products");
            _WebPurchaseUrl = jsonNode.GetStringIfPresent("web_purchase_url");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyFlowPaywall GetAdaptyFlowPaywall(this JSONNode node) =>
            new AdaptyFlowPaywall(GetObject(node));

        internal static AdaptyFlowPaywall GetAdaptyFlowPaywall(this JSONNode node, string aKey) =>
            new AdaptyFlowPaywall(GetObject(node, aKey));

        internal static AdaptyFlowPaywall GetAdaptyFlowPaywallIfPresent(
            this JSONNode node,
            string aKey
        )
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyFlowPaywall(obj);
        }

        internal static IList<AdaptyFlowPaywall> GetAdaptyFlowPaywallList(
            this JSONNode node,
            string aKey
        )
        {
            var array = GetArray(node, aKey);
            var result = new List<AdaptyFlowPaywall>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject)
                    throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new AdaptyFlowPaywall(item.AsObject));
            }
            return result;
        }
    }
}
