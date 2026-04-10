//
//  AdaptyUIIOSPresentationStyle+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2024.
//

using System;

namespace AdaptySDK
{
    public static partial class AdaptyUIIOSPresentationStyleExtensions
    {
        public static string ToJSONNode(this AdaptyUIIOSPresentationStyle value) =>
            value switch
            {
                AdaptyUIIOSPresentationStyle.FullScreen => "full_screen",
                AdaptyUIIOSPresentationStyle.PageSheet => "page_sheet",
                _ => throw new Exception($"AdaptyUIIOSPresentationStyle unknown value: {value}"),
            };
    }
}
