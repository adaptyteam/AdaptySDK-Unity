using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum AttributionNetwork
        {
            Adjust,
            Appsflyer,
            Branch,
            AppleSearchAds,
            Custom
        }

        public static AttributionNetwork AttributionNetworkFromJSON(JSONNode response)
        {
            return AttributionNetworkFromString(response);
        }

        public static AttributionNetwork AttributionNetworkFromString(string value)
        {
            if (value == null) return AttributionNetwork.Custom;
            switch (value)
            {
                case "adjust":
                    return AttributionNetwork.Adjust;
                case "appsflyer":
                    return AttributionNetwork.Appsflyer;
                case "branch":
                    return AttributionNetwork.Branch;
                case "apple_search_ads":
                case "appleSearchAds":
                    return AttributionNetwork.AppleSearchAds;
                default:
                    return AttributionNetwork.Custom;
            }
        }

        public static string AttributionNetworkToString(this AttributionNetwork value)
        {
            switch (value)
            {
                case AttributionNetwork.Adjust:
                    return "adjust";
                case AttributionNetwork.Appsflyer:
                    return "appsflyer";
                case AttributionNetwork.Branch:
                    return "branch";
                case AttributionNetwork.AppleSearchAds:
                    return "apple_search_ads";
                case AttributionNetwork.Custom:
                default:
                    return "custom";
            }
        }
    }
}
