//
//  AdaptyFlowPaywall.ProductReference+JSON.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyFlowPaywall
    {
        public partial class ProductReference
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("vendor_product_id", VendorProductId);
                node.Add("adapty_product_id", AdaptyProductId);
                node.Add("access_level_id", AccessLevelId);
                node.Add("product_type", ProductType);

                if (FlowProductId != null)
                    node.Add("flow_product_id", FlowProductId);

#if UNITY_ANDROID
                if (AndroidBasePlanId != null)
                    node.Add("base_plan_id", AndroidBasePlanId);
                if (AndroidOfferId != null)
                    node.Add("offer_id", AndroidOfferId);
#endif

#if UNITY_IOS
                if (PromotionalOfferId != null)
                    node.Add("promotional_offer_id", PromotionalOfferId);
                if (WinBackOfferId != null)
                    node.Add("win_back_offer_id", WinBackOfferId);
#endif
                return node;
            }

            internal ProductReference(JSONObject jsonNode)
            {
                VendorProductId = jsonNode.GetString("vendor_product_id");
                AdaptyProductId = jsonNode.GetString("adapty_product_id");
                AccessLevelId = jsonNode.GetString("access_level_id");
                ProductType = jsonNode.GetString("product_type");
                FlowProductId = jsonNode.GetStringIfPresent("flow_product_id");

#if UNITY_ANDROID
                AndroidBasePlanId = jsonNode.GetStringIfPresent("base_plan_id");
                AndroidOfferId = jsonNode.GetStringIfPresent("offer_id");
#else
                AndroidBasePlanId = null;
                AndroidOfferId = null;
#endif

#if UNITY_IOS
                PromotionalOfferId = jsonNode.GetStringIfPresent("promotional_offer_id");
                WinBackOfferId = jsonNode.GetStringIfPresent("win_back_offer_id");
#else
                PromotionalOfferId = null;
                WinBackOfferId = null;
#endif
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyFlowPaywall.ProductReference GetAdaptyFlowPaywallProductReference(
            this JSONNode node,
            string aKey
        ) => new AdaptyFlowPaywall.ProductReference(GetObject(node, aKey));

        internal static AdaptyFlowPaywall.ProductReference GetAdaptyFlowPaywallProductReferenceIfPresent(
            this JSONNode node,
            string aKey
        )
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyFlowPaywall.ProductReference(obj);
        }

        internal static IList<AdaptyFlowPaywall.ProductReference> GetAdaptyFlowPaywallProductReferenceList(
            this JSONNode node,
            string aKey
        )
        {
            var array = GetArray(node, aKey);
            var result = new List<AdaptyFlowPaywall.ProductReference>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject)
                    throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new AdaptyFlowPaywall.ProductReference(item.AsObject));
            }
            return result;
        }
    }
}
