//
//  AdaptyPurchaseResultType.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

using System.Runtime.Serialization;

namespace AdaptySDK
{
    public enum AdaptyPurchaseResultType
    {
        [EnumMember(Value = "pending")]
        Pending,
        [EnumMember(Value = "user_cancelled")]
        UserCancelled,
        [EnumMember(Value = "success")]
        Success,

        /// <summary>
        /// The purchase outcome is not one this SDK version knows.
        /// </summary>
        /// <remarks>
        /// Appended last on purpose: the members above keep the numeric values they had,
        /// and no member of this type is both non-nullable and optional, so this can never
        /// become the value of a missing field.
        /// </remarks>
        Unknown,
    }
}