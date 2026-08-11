using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Where a web paywall opens.
    /// </summary>
    [Preserve]
    public enum AdaptyWebPresentation
    {
        /// <summary>
        /// The device's browser app, leaving your app.
        /// </summary>
        [EnumMember(Value = "browser_out_app")]
        ExternalBrowser = 0,

        /// <summary>
        /// A browser presented over your app, which stays in the foreground.
        /// </summary>
        [EnumMember(Value = "browser_in_app")]
        InAppBrowser = 1,
    }
}
