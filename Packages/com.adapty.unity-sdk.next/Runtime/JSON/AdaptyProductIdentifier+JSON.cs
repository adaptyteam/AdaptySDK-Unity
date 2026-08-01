//
//  AdaptyProductIdentifier+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyProductIdentifier
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("vendor_product_id", VendorProductId);
            node.Add("adapty_product_id", _AdaptyProductId);

            if (!string.IsNullOrEmpty(BasePlanId))
            {
                node.Add("base_plan_id", BasePlanId);
            }

            return node;
        }

        internal AdaptyProductIdentifier(JSONObject jsonNode)
        {
            VendorProductId = jsonNode.GetString("vendor_product_id");
            _AdaptyProductId = jsonNode.GetString("adapty_product_id");
            BasePlanId = jsonNode.GetStringIfPresent("base_plan_id");
        }
    }
}
