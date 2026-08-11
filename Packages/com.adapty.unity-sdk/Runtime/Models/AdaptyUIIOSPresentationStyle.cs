using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// iOS only. How a flow view is presented. Ignored on Android.
    /// </summary>
    [Preserve]
    public enum AdaptyUIIOSPresentationStyle
    {
        /// <summary>
        /// Covers the screen.
        /// </summary>
        [EnumMember(Value = "full_screen")]
        FullScreen = 0,
        /// <summary>
        /// A sheet over the current screen, which the user can swipe down.
        /// </summary>
        [EnumMember(Value = "page_sheet")]
        PageSheet = 1,
    }
}
