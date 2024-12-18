//
//  AdaptyPaywall.ViewConfiguration+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPaywall
    {
        internal partial class ViewConfiguration
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("paywall_builder_id", PaywallBuilderId);
                node.Add("lang", Lang);
                return node;
            }

            internal ViewConfiguration(JSONObject jsonNode)
            {
                PaywallBuilderId = jsonNode.GetString("paywall_builder_id");
                Lang = jsonNode.GetString("lang");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPaywall.ViewConfiguration GetAdaptyPaywallViewConfiguration(this JSONNode node, string aKey)
             => new AdaptyPaywall.ViewConfiguration(GetObject(node, aKey));

        internal static AdaptyPaywall.ViewConfiguration GetAdaptyPaywallViewConfigurationIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyPaywall.ViewConfiguration(obj);
        }
    }
}