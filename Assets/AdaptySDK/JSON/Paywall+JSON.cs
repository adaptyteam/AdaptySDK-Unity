﻿//
//  Paywall+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class Paywall
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("developer_id", Id);
                node.Add("paywall_name", Name);
                node.Add("ab_test_name", ABTestName);
                node.Add("variation_id", VariationId);
                node.Add("revision", Revision);
                node.Add("custom_payload", RemoteConfigString);
                var products = new JSONArray();
                foreach (var item in _Products)
                {
                    products.Add(item.ToJSONNode());
                }
                node.Add("products", products);
                node.Add("paywall_updated_at", _Version);
                node.Add("payload_data", _PayloadData);
                return node;
            }

            internal Paywall(JSONObject jsonNode)
            {
                Id = jsonNode.GetString("developer_id");
                Name = jsonNode.GetString("paywall_name");
                ABTestName = jsonNode.GetString("ab_test_name");
                VariationId = jsonNode.GetString("variation_id");
                Revision = jsonNode.GetInteger("revision");

                RemoteConfigString = jsonNode.GetStringIfPresent("custom_payload");
                _Products = jsonNode.GetBackendProductList("products");
                _Version = jsonNode.GetInteger("paywall_updated_at");
                _PayloadData = jsonNode.GetStringIfPresent("payload_data");
            }
        }
    }
}


namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Paywall GetPaywall(this JSONNode node, string aKey)
             => new Adapty.Paywall(GetObject(node, aKey));

        internal static Adapty.Paywall GetPaywallIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.Paywall(obj);
        }
    }
}