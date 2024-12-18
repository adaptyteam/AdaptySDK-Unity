//
//  AdaptyPurchaseResultType+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {

        internal static AdaptyPurchaseResultType GetAdaptyPurchaseResultType(this JSONNode node) =>
            GetString(node).ToAdaptyPurchaseResultType();
        internal static AdaptyPurchaseResultType GetAdaptyPurchaseResultType(this JSONNode node, string aKey) =>
            GetString(node, aKey).ToAdaptyPurchaseResultType();
        internal static AdaptyPurchaseResultType? GetAdaptyPurchaseResultTypeIfPresent(this JSONNode node, string aKey) =>
            GetStringIfPresent(node, aKey)?.ToAdaptyPurchaseResultType();

        private static AdaptyPurchaseResultType ToAdaptyPurchaseResultType(this string value) =>
            value switch
            {
                "pending" => AdaptyPurchaseResultType.Pending,
                "user_cancelled" => AdaptyPurchaseResultType.UserCancelled,
                "success" => AdaptyPurchaseResultType.Success,
                _ => throw new Exception($"AdaptyPurchaseResultType unknown value: {value}"),
            };
    }
}