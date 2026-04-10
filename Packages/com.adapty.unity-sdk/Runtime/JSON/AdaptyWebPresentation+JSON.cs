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
