using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The unit a subscription period is counted in.
    /// </summary>
    [Preserve]
    public enum AdaptySubscriptionPeriodUnit
    {
        /// <summary>
        /// Days.
        /// </summary>
        [EnumMember(Value = "day")]
        Day = 0,
        /// <summary>
        /// Weeks.
        /// </summary>
        [EnumMember(Value = "week")]
        Week = 1,
        /// <summary>
        /// Months.
        /// </summary>
        [EnumMember(Value = "month")]
        Month = 2,
        /// <summary>
        /// Years.
        /// </summary>
        [EnumMember(Value = "year")]
        Year = 3,
        /// <summary>
        /// The store reported a unit the contract does not list. One of the two enums that keep an unknown value, because the contract lists <c>"unknown"</c> among theirs.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 4
    }
}
