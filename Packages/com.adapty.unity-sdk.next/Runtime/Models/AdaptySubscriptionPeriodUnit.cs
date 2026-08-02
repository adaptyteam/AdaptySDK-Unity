//
//  AdaptySubscriptionUnit.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Runtime.Serialization;

namespace AdaptySDK
{
    public enum AdaptySubscriptionPeriodUnit
    {
        [EnumMember(Value = "day")]
        Day,
        [EnumMember(Value = "week")]
        Week,
        [EnumMember(Value = "month")]
        Month,
        [EnumMember(Value = "year")]
        Year,
        [EnumMember(Value = "unknown")]
        Unknown
    }
}