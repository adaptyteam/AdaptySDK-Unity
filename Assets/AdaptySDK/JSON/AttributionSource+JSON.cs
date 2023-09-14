//
//  AttributionSource+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.AttributionSource value)
        {
            switch (value)
            {
                case Adapty.AttributionSource.Adjust: return "adjust";
                case Adapty.AttributionSource.Appsflyer: return "appsflyer";
                case Adapty.AttributionSource.Branch: return "branch";
                case Adapty.AttributionSource.AppleSearchAds: return "apple_search_ads";
                case Adapty.AttributionSource.Custom: return "custom";
                default: return "custom";
            }
        }

        internal static Adapty.AttributionSource GetAttributionSource(this JSONNode node, string aKey)
            => GetString(node, aKey).ToAttributionSource();

        internal static Adapty.AttributionSource? GetAttributionSourceIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToAttributionSource();

        internal static Adapty.AttributionSource ToAttributionSource(this string value)
        {
            switch (value)
            {
                case "adjust": return Adapty.AttributionSource.Adjust;
                case "appsflyer": return Adapty.AttributionSource.Appsflyer;
                case "branch": return Adapty.AttributionSource.Branch;
                case "apple_search_ads": return Adapty.AttributionSource.AppleSearchAds;
                default: return Adapty.AttributionSource.Custom;
            }
        }
    }
}