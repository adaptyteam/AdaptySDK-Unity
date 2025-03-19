//
//  AdaptyRefundPreference+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 19.03.2025.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static JSONNode ToJSONNode(this AdaptyRefundPreference value) =>
            value switch
            {
                AdaptyRefundPreference.NoPreference => "no_preference",
                AdaptyRefundPreference.Grant => "grant",
                AdaptyRefundPreference.Decline => "decline",
                _ => throw new Exception($"AdaptyRefundPreference unknown value: {value}"),
            };

    }
}