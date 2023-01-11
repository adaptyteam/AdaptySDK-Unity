//
//  BackendProduct+JSON.dart
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using AdaptySDK.SimpleJSON;
using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        internal partial class BackendProduct
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("vendor_product_id", VendorId);
                node.Add("introductory_offer_eligibility", IntroductoryOfferEligibility.ToJSON());
                node.Add("timestamp", _Version);
#if UNITY_IOS
                node.Add("promotional_offer_eligibility", PromotionalOfferEligibility);
                if (PromotionalOfferId is not null) node.Add("promotional_offer_id", PromotionalOfferId);
#endif
                return node;
            }

            internal BackendProduct(JSONObject node)
            {
                VendorId = node.GetString("vendor_product_id");
                IntroductoryOfferEligibility = node.GetEligibility("introductory_offer_eligibility");
                _Version = node.GetInteger("timestamp");
#if UNITY_IOS
                PromotionalOfferEligibility = node.GetBooleanIfPresent("promotional_offer_eligibility") ?? false;
                PromotionalOfferId = node.GetStringIfPresent("promotional_offer_id");
#else
                PromotionalOfferEligibility = false;
                PromotionalOfferId = null;
#endif
            }
        }
    }
}



namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static IList<Adapty.BackendProduct> GetBackendProductList(this JSONNode node, string aKey)
        {
            var result = new List<Adapty.BackendProduct>();
            foreach (var item in GetArray(node, aKey).Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new Adapty.BackendProduct(item.AsObject));
            }
            return result;
        }
    }
}