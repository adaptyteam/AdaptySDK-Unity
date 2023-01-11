//
//  AttributionSource+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.AttributionSource value)
            => value switch
            {
                Adapty.AttributionSource.Adjust => "adjust",
                Adapty.AttributionSource.Appsflyer => "appsflyer",
                Adapty.AttributionSource.Branch => "branch",
                Adapty.AttributionSource.AppleSearchAds => "apple_search_ads",
                Adapty.AttributionSource.Custom => "custom",
                _ => "custom",
            };

        internal static Adapty.AttributionSource GetAttributionSource(this JSONNode node, string aKey)
            => GetString(node, aKey).ToAttributionSource();

        internal static Adapty.AttributionSource? GetAttributionSourceIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToAttributionSource();

        internal static Adapty.AttributionSource ToAttributionSource(this string value)
            => value switch
            {
                "adjust" => Adapty.AttributionSource.Adjust,
                "appsflyer" => Adapty.AttributionSource.Appsflyer,
                "branch" => Adapty.AttributionSource.Branch,
                "apple_search_ads" => Adapty.AttributionSource.AppleSearchAds,
                _ => Adapty.AttributionSource.Custom,
            };
    }
}