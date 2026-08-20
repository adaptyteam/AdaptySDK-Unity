using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How a purchase ended.
    /// </summary>
    [Preserve]
    public enum AdaptyPurchaseResultType
    {
        /// <summary>
        /// The store is waiting on something — Ask to Buy, or a payment method that settles later. The profile updates when it resolves, so wait rather than retrying.
        /// </summary>
        [EnumMember(Value = "pending")]
        Pending = 0,
        /// <summary>
        /// The user dismissed the store's sheet. Not a failure to report.
        /// </summary>
        [EnumMember(Value = "user_cancelled")]
        UserCancelled = 1,
        /// <summary>
        /// The purchase went through; the updated profile is on the result.
        /// </summary>
        [EnumMember(Value = "success")]
        Success = 2,
    }
}
