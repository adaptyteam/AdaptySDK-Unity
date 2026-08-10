using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyUIIOSPresentationStyle
    {
        [EnumMember(Value = "full_screen")]
        FullScreen = 0,
        [EnumMember(Value = "page_sheet")]
        PageSheet = 1,
    }
}
