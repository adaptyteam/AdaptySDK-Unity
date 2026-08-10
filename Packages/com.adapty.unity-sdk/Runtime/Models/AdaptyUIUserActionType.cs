using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK {
    [Preserve]
    public enum AdaptyUIUserActionType {
        [EnumMember(Value = "close")]
        Close = 0,
        [EnumMember(Value = "system_back")]
        SystemBack = 1,
        [EnumMember(Value = "open_url")]
        OpenUrl = 2,
        [EnumMember(Value = "custom")]
        Custom = 3,
    }
}