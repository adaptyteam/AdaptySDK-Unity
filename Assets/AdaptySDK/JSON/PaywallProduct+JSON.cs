//
//  PaywallProduct+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class PaywallProduct
        {

            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("vendor_product_id", VendorProductId);
#if UNITY_IOS
                if (SubscriptionDetails?.PromotionalOfferId != null) node.Add("promotional_offer_id", SubscriptionDetails?.PromotionalOfferId);
#endif
                node.Add("paywall_variation_id", PaywallVariationId);
                node.Add("paywall_ab_test_name", PaywallABTestName);
                node.Add("paywall_name", PaywallName);
                if (_PayloadData != null) node.Add("payload_data", _PayloadData);
                return node;
            }

            internal PaywallProduct(JSONObject jsonNode)
            {
                VendorProductId = jsonNode.GetString("vendor_product_id");
                LocalizedDescription = jsonNode.GetString("localized_description");
                LocalizedTitle = jsonNode.GetString("localized_title");
                RegionCode = jsonNode.GetStringIfPresent("region_code");
                IsFamilyShareable = jsonNode.GetBooleanIfPresent("is_family_shareable") ?? false;
                PaywallVariationId = jsonNode.GetString("paywall_variation_id");
                PaywallABTestName = jsonNode.GetString("paywall_ab_test_name");
                PaywallName = jsonNode.GetString("paywall_name");
                Price = jsonNode.GetPrice("price");
                SubscriptionDetails = jsonNode.GetSubscriptionDetailsIfPresent("subscription_details");
                _PayloadData = jsonNode.GetStringIfPresent("payload_data");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.PaywallProduct GetPaywallProduct(this JSONNode node, string aKey)
             => new Adapty.PaywallProduct(GetObject(node, aKey));

        internal static Adapty.PaywallProduct GetPaywallProductIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.PaywallProduct(obj);
        }

        internal static IList<Adapty.PaywallProduct> GetPaywallProductList(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<Adapty.PaywallProduct>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new Adapty.PaywallProduct(item.AsObject));
            }
            return result;
        }
    }
}