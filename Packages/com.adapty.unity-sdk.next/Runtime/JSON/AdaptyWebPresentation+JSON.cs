//
//  AdaptyWebPresentation+JSON.cs
//  AdaptySDK
//

using System;

namespace AdaptySDK
{
    public static partial class AdaptyWebPresentationExtensions
    {
        public static string ToJSONNode(this AdaptyWebPresentation value) =>
            value switch
            {
                AdaptyWebPresentation.ExternalBrowser => "browser_out_app",
                AdaptyWebPresentation.InAppBrowser => "browser_in_app",
                _ => throw new Exception($"AdaptyWebPresentation unknown value: {value}"),
            };
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyWebPresentation GetAdaptyWebPresentation(this JSONNode node) =>
            GetString(node).ToAdaptyWebPresentation();

        internal static AdaptyWebPresentation GetAdaptyWebPresentation(this JSONNode node, string aKey) =>
            GetString(node, aKey).ToAdaptyWebPresentation();

        internal static AdaptyWebPresentation? GetAdaptyWebPresentationIfPresent(this JSONNode node, string aKey) =>
            GetStringIfPresent(node, aKey)?.ToAdaptyWebPresentation();

        private static AdaptyWebPresentation ToAdaptyWebPresentation(this string value) =>
            value switch
            {
                "browser_out_app" => AdaptyWebPresentation.ExternalBrowser,
                "browser_in_app" => AdaptyWebPresentation.InAppBrowser,
                _ => throw new Exception($"AdaptyWebPresentation unknown value: {value}"),
            };
    }
}
