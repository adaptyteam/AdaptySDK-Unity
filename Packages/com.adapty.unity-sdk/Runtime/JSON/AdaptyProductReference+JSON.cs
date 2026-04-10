//
//  AdaptyProductReference+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyProductReference
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("vendor_product_id", VendorProductId);
            node.Add("adapty_product_id", _AdaptyProductId);
            if (PromotionalOfferId != null)
            {
                node.Add("promotional_offer_id", PromotionalOfferId);
            }

            if (WinBackOfferId != null)
            {
                node.Add("win_back_offer_id", WinBackOfferId);
            }

            if (AndroidBasePlanId != null)
            {
                node.Add("base_plan_id", AndroidBasePlanId);
            }

            if (AndroidOfferId != null)
            {
                node.Add("offer_id", AndroidOfferId);
            }

            return node;
        }

        internal AdaptyProductReference(JSONObject jsonNode)
        {
            VendorProductId = jsonNode.GetString("vendor_product_id");
            _AdaptyProductId = jsonNode.GetString("adapty_product_id");
            PromotionalOfferId = jsonNode.GetStringIfPresent("promotional_offer_id");
            WinBackOfferId = jsonNode.GetStringIfPresent("win_back_offer_id");
            AndroidBasePlanId = jsonNode.GetStringIfPresent("base_plan_id");
            AndroidOfferId = jsonNode.GetStringIfPresent("offer_id");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyProductReference GetAdaptyProductReference(
            this JSONNode node,
            string aKey
        ) => new AdaptyProductReference(JSONNodeExtensions.GetObject(node, aKey));

        internal static AdaptyProductReference GetAdaptyProductReferenceIfPresent(
            this JSONNode node,
            string aKey
        )
        {
            var obj = JSONNodeExtensions.GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyProductReference(obj);
        }
    }
}
