using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyPurchaseResultType
    {
        [EnumMember(Value = "pending")]
        Pending = 0,
        [EnumMember(Value = "user_cancelled")]
        UserCancelled = 1,
        [EnumMember(Value = "success")]
        Success = 2,
    }
}