//
//  AdaptyUIDialogActionType+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//
using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyUIDialogActionType GetAdaptyUIDialogActionType(this JSONNode node) =>
            GetString(node).ToAdaptyUIDialogActionType();
        internal static AdaptyUIDialogActionType GetAdaptyUIDialogActionType(this JSONNode node, string aKey) =>
            GetString(node, aKey).ToAdaptyUIDialogActionType();
        internal static AdaptyUIDialogActionType? GetAdaptyUIDialogActionTypeIfPresent(this JSONNode node, string aKey) =>
            GetStringIfPresent(node, aKey)?.ToAdaptyUIDialogActionType();
        private static AdaptyUIDialogActionType ToAdaptyUIDialogActionType(this string value) =>
            value switch
            {
                "secondary" => AdaptyUIDialogActionType.Secondary,
                _ => AdaptyUIDialogActionType.Default
            };
    }
}