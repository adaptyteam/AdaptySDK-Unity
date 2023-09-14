//
//  ProductReference+JSON.cs
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
        internal partial class ProductReference
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("vendor_product_id", VendorId);
#if UNITY_ANDROID
                node.Add("base_plan_id", AndroidBasePlanId);
                node.Add("offer_id", AndroidOfferId);
#endif

#if UNITY_IOS
                if (IOSDiscountId != null) node.Add("promotional_offer_id", IOSDiscountId);
#endif
                return node;
            }

            internal ProductReference(JSONObject jsonNode)
            {
                VendorId = jsonNode.GetString("vendor_product_id");
#if UNITY_ANDROID
                AndroidBasePlanId = jsonNode.GetStringIfPresent("base_plan_id");
                AndroidOfferId = jsonNode.GetStringIfPresent("offer_id");
#else
                AndroidBasePlanId = null;
                AndroidOfferId = null;
#endif

#if UNITY_IOS
                IOSDiscountId = jsonNode.GetStringIfPresent("promotional_offer_id");
#else
                IOSDiscountId = null;
#endif
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.ProductReference GetProductReference(this JSONNode node, string aKey)
             => new Adapty.ProductReference(GetObject(node, aKey));

        internal static Adapty.ProductReference GetProductReferenceIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.ProductReference(obj);
        }

        internal static IList<Adapty.ProductReference> GetProductReferenceList(this JSONNode node, string aKey)
        {
            var array = GetArray(node, aKey);
            var result = new List<Adapty.ProductReference>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new Adapty.ProductReference(item.AsObject));
            }
            return result;
        }
    }
}