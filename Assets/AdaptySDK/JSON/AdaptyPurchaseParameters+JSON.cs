//
//  AdaptyPurchaseParameters+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPurchaseParameters
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            if (SubscriptionUpdateParams != null)
                node.Add("subscription_update_params", SubscriptionUpdateParams.ToJSONNode());
            if (IsOfferPersonalized.HasValue)
                node.Add("is_offer_personalized", IsOfferPersonalized.Value);
            if (ObfuscatedAccountId != null)
                node.Add("obfuscated_account_id", ObfuscatedAccountId);
            if (ObfuscatedProfileId != null)
                node.Add("obfuscated_profile_id", ObfuscatedProfileId);
            return node;
        }
    }
}
