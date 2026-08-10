using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Controls how web content (paywalls, external URLs in onboarding) is presented.
    /// </summary>
    [Preserve]
    public enum AdaptyWebPresentation
    {
        /// <summary>
        /// Open in the default external browser (outside the app).
        /// </summary>
        [EnumMember(Value = "browser_out_app")]
        ExternalBrowser = 0,

        /// <summary>
        /// Open in an in-app browser/web view.
        /// </summary>
        [EnumMember(Value = "browser_in_app")]
        InAppBrowser = 1,
    }
}
