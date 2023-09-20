﻿//
//  AndroidSubscriptionUpdateParameters+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class AndroidSubscriptionUpdateParameters
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("old_sub_vendor_product_id", OldSubVendorProductId);
                node.Add("replacement_mode", ReplacementMode.ToJSON());
                return node;
            }
        }
    }
}