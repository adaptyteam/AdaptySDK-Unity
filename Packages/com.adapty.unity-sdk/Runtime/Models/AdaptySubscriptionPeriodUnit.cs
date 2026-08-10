using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptySubscriptionPeriodUnit
    {
        [EnumMember(Value = "day")]
        Day = 0,
        [EnumMember(Value = "week")]
        Week = 1,
        [EnumMember(Value = "month")]
        Month = 2,
        [EnumMember(Value = "year")]
        Year = 3,
        [EnumMember(Value = "unknown")]
        Unknown = 4
    }
}