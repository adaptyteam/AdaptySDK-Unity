//
//  AdaptyWebPresentation.cs
//  AdaptySDK
//

using System.Runtime.Serialization;

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
        [EnumMember(Value = "browser_out_app")]
        ExternalBrowser,

        /// <summary>
        /// Open in an in-app browser/web view.
        /// </summary>
        [EnumMember(Value = "browser_in_app")]
        InAppBrowser,

        /// <summary>
        /// The presentation is not one this SDK version knows.
        /// </summary>
        /// <remarks>
        /// Appended last on purpose: the members above keep the numeric values they had,
        /// and no member of this type is both non-nullable and optional, so this can never
        /// become the value of a missing field.
        /// </remarks>
        Unknown,
    }
}
