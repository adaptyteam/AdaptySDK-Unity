//
//  AdaptyUIUserActionType+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//
using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyUIUserActionType GetAdaptyUIUserActionType(this JSONNode node, string aKey) =>
            GetString(node, aKey).ToAAdaptyUIUserActionType();

        internal static AdaptyUIUserActionType? GetAdaptyUIUserActionTypeIfPresent(this JSONNode node, string aKey) =>
            GetStringIfPresent(node, aKey)?.ToAAdaptyUIUserActionType();

        private static AdaptyUIUserActionType ToAAdaptyUIUserActionType(this string value) =>
            value switch
            {
                "close" => AdaptyUIUserActionType.Close,
                "system_back" => AdaptyUIUserActionType.SystemBack,
                "open_url" => AdaptyUIUserActionType.OpenUrl,
                "custom" => AdaptyUIUserActionType.Custom,
                _ => throw new Exception($"AdaptyUIUserActionType unknown value: {value}"),
            };
    }
}