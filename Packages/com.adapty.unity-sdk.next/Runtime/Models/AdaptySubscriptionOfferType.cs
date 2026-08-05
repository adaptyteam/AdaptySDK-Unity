//
//  AdaptySubscriptionOfferType.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptySubscriptionOfferType
    {
        [EnumMember(Value = "introductory")]
        Introductory,
        [EnumMember(Value = "promotional")]
        Promotional,
        [EnumMember(Value = "win_back")]
        WinBack,
        [EnumMember(Value = "code")]
        Code, // iOS Only

        /// <summary>
        /// The offer kind is not one this SDK version knows.
        /// </summary>
        /// <remarks>
        /// Appended last on purpose: the members above keep the numeric values they had,
        /// and no member of this type is both non-nullable and optional, so this can never
        /// become the value of a missing field.
        /// </remarks>
        [EnumMember(Value = "unknown")]
        Unknown,
    }
}
