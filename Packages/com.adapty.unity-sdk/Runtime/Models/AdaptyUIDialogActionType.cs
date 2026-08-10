using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyUIDialogActionType {
        [EnumMember(Value = "primary")]
        Primary = 0,
        [EnumMember(Value = "secondary")]
        Secondary = 1,
    }
}