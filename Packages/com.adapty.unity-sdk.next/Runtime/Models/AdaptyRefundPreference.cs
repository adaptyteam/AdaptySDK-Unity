//
//  AdaptyRefundPreference.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 19.03.2025.
//

using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyRefundPreference
    {
        [EnumMember(Value = "no_preference")]
        NoPreference,
        [EnumMember(Value = "grant")]
        Grant,
        [EnumMember(Value = "decline")]
        Decline,
    }

    [Preserve]
    public static partial class AdaptyRefundPreferenceExtensions
    {
        public static string ToJSONNode(this AdaptyRefundPreference value) =>
            value switch
            {
                AdaptyRefundPreference.NoPreference => "no_preference",
                AdaptyRefundPreference.Grant => "grant",
                AdaptyRefundPreference.Decline => "decline",
                _ => throw new Exception($"AdaptyRefundPreference unknown value: {value}"),
            };
    }
}
