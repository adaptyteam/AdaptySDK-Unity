//
//  AdaptyPaywallProduct+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPaywallProduct
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("vendor_product_id", VendorProductId);
            node.Add("adapty_product_id", AdaptyProductId);
            node.Add("access_level_id", AccessLevelId);
            node.Add("product_type", ProductType);
            node.Add("paywall_variation_id", PaywallVariationId);
            node.Add("paywall_ab_test_name", PaywallABTestName);
            node.Add("paywall_name", PaywallName);
            node.Add("paywall_product_index", PaywallProductIndex);

            if (_WebPurchaseUrl != null)
            {
                node.Add("web_purchase_url", _WebPurchaseUrl);
            }

            if (_PayloadData != null)
            {
                node.Add("payload_data", _PayloadData);
            }

            var offer = Subscription?.Offer;
            if (offer != null)
            {
                var subNode = new JSONObject();
                if (offer.Identifier != null)
                {
                    subNode.Add("id", offer.Identifier);
                }

                subNode.Add("type", offer.Type.ToJSONNode());
                node.Add("subscription_offer_identifier", subNode);
            }

            return node;
        }

        internal AdaptyPaywallProduct(JSONObject jsonNode)
        {
            VendorProductId = jsonNode.GetString("vendor_product_id");
            AdaptyProductId = jsonNode.GetString("adapty_product_id");
            AccessLevelId = jsonNode.GetString("access_level_id");
            ProductType = jsonNode.GetString("product_type");
            PaywallVariationId = jsonNode.GetString("paywall_variation_id");
            PaywallABTestName = jsonNode.GetString("paywall_ab_test_name");
            PaywallName = jsonNode.GetString("paywall_name");
            LocalizedDescription = jsonNode.GetString("localized_description");
            LocalizedTitle = jsonNode.GetString("localized_title");
#if UNITY_IOS
            IsFamilyShareable = jsonNode.GetBoolean("is_family_shareable");
#else
            IsFamilyShareable = false;
#endif
            RegionCode = jsonNode.GetStringIfPresent("region_code");
            Price = jsonNode.GetAdaptyPrice("price");
            Subscription = jsonNode.GetAdaptySubscriptionIfPresent("subscription");
            PaywallProductIndex = jsonNode.GetInteger("paywall_product_index");
            _PayloadData = jsonNode.GetStringIfPresent("payload_data");
            _WebPurchaseUrl = jsonNode.GetStringIfPresent("web_purchase_url");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPaywallProduct GetAdaptyPaywallProduct(
            this JSONNode node,
            string aKey
        ) => new AdaptyPaywallProduct(GetObject(node, aKey));

        internal static AdaptyPaywallProduct GetAdaptyPaywallProductIfPresent(
            this JSONNode node,
            string aKey
        )
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyPaywallProduct(obj);
        }

        internal static IList<AdaptyPaywallProduct> GetAdaptyPaywallProductList(this JSONNode node)
        {
            var array = GetArrayIfPresent(node);
            return GetAdaptyPaywallProductList(array);
        }

        internal static IList<AdaptyPaywallProduct> GetAdaptyPaywallProductList(
            this JSONNode node,
            string aKey
        )
        {
            var array = GetArrayIfPresent(node, aKey);
            return GetAdaptyPaywallProductList(array);
        }

        private static IList<AdaptyPaywallProduct> GetAdaptyPaywallProductList(this JSONArray array)
        {
            if (array is null)
                return null;
            var result = new List<AdaptyPaywallProduct>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject)
                    throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new AdaptyPaywallProduct(item.AsObject));
            }
            return result;
        }
    }
}
