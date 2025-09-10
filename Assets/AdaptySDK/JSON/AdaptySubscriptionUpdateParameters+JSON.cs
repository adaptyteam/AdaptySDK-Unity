//
//  AdaptySubscriptionUpdateParameters+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptySubscriptionUpdateParameters
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("old_sub_vendor_product_id", OldSubVendorProductId);
            node.Add("replacement_mode", ReplacementMode.ToJSONNode());
            return node;
        }
    }
}
