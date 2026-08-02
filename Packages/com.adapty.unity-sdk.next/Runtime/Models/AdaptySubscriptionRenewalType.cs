//
//  AdaptySubscriptionRenewalType.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

using System.Runtime.Serialization;

namespace AdaptySDK
{
    public enum AdaptySubscriptionRenewalType
    {
        [EnumMember(Value = "prepaid")]
        Prepaid,
        [EnumMember(Value = "autorenewable")]
        Autorenewable,

        /// <summary>
        /// The renewal kind is not one this SDK version knows.
        /// </summary>
        /// <remarks>
        /// Appended last on purpose: the members above keep the numeric values they had,
        /// and no member of this type is both non-nullable and optional, so this can never
        /// become the value of a missing field.
        /// </remarks>
        Unknown,
    }
}