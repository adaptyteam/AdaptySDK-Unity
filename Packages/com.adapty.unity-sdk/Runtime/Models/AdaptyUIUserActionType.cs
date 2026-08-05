//
//  AdaptyUIUserActionType.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK {
    [Preserve]
    public enum AdaptyUIUserActionType {
        [EnumMember(Value = "close")]
        Close,
        [EnumMember(Value = "system_back")]
        SystemBack,
        [EnumMember(Value = "open_url")]
        OpenUrl,
        [EnumMember(Value = "custom")]
        Custom,

        /// <summary>
        /// The action is not one this SDK version knows.
        /// </summary>
        /// <remarks>
        /// Appended last on purpose: the members above keep the numeric values they had,
        /// and no member of this type is both non-nullable and optional, so this can never
        /// become the value of a missing field.
        /// </remarks>
        Unknown,
    }
}