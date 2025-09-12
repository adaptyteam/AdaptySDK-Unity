//
//  AdaptyPurchaseParameters+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPurchaseParameters
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();

            if (SubscriptionUpdateParams != null)
            {
                node.Add(_Keys.SubscriptionUpdateParams, SubscriptionUpdateParams.ToJSONNode());
            }

            if (IsOfferPersonalized.HasValue)
            {
                node.Add(_Keys.IsOfferPersonalized, IsOfferPersonalized.Value);
            }

            if (ObfuscatedAccountId != null)
            {
                node.Add(_Keys.ObfuscatedAccountId, ObfuscatedAccountId);
            }

            if (ObfuscatedProfileId != null)
            {
                node.Add(_Keys.ObfuscatedProfileId, ObfuscatedProfileId);
            }

            return node;
        }
    }

    internal static class _Keys
    {
        internal const string SubscriptionUpdateParams = "subscription_update_params";
        internal const string IsOfferPersonalized = "is_offer_personalized";
        internal const string ObfuscatedAccountId = "obfuscated_account_id";
        internal const string ObfuscatedProfileId = "obfuscated_profile_id";
    }
}
