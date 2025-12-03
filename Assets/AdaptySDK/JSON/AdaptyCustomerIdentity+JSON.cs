//
//  AdaptyCustomerIdentity+JSON.cs
//  AdaptySDK
//
//  Created by AI Assistant on 14.01.2025.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyCustomerIdentity
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();

            if (IosAppAccountToken != Guid.Empty)
            {
                node.Add(_CustomerIdentityKeys.IosAppAccountToken, IosAppAccountToken.ToString());
            }

            if (!string.IsNullOrEmpty(AndroidObfuscatedAccountId))
            {
                node.Add(
                    _CustomerIdentityKeys.AndroidObfuscatedAccountId,
                    AndroidObfuscatedAccountId
                );
            }

            return node;
        }
    }

    internal static class _CustomerIdentityKeys
    {
        internal const string IosAppAccountToken = "app_account_token";
        internal const string AndroidObfuscatedAccountId = "obfuscated_account_id";
    }
}
