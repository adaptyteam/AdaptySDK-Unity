using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptySubscriptionOfferType
    {
        [EnumMember(Value = "introductory")]
        Introductory = 0,
        [EnumMember(Value = "promotional")]
        Promotional = 1,
        [EnumMember(Value = "win_back")]
        WinBack = 2,
        [EnumMember(Value = "code")]
        Code = 3, // iOS Only
    }
}
