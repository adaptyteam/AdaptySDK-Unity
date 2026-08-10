using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyRefundPreference
    {
        [EnumMember(Value = "no_preference")]
        NoPreference = 0,
        [EnumMember(Value = "grant")]
        Grant = 1,
        [EnumMember(Value = "decline")]
        Decline = 2,
    }
}
