//
//  AdaptyWebPresentation.cs
//  AdaptySDK
//

namespace AdaptySDK
{
    /// <summary>
    /// Controls how web content (paywalls, external URLs in onboarding) is presented.
    /// </summary>
    public enum AdaptyWebPresentation
    {
        /// <summary>
        /// Open in the default external browser (outside the app).
        /// </summary>
        ExternalBrowser,

        /// <summary>
        /// Open in an in-app browser/web view.
        /// </summary>
        InAppBrowser,
    }
}
