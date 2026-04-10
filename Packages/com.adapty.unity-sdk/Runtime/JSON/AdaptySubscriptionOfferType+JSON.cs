//
//  AdaptyLogLevel+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static JSONNode ToJSONNode(this AdaptySubscriptionOfferType value) =>
            value switch
            {
                AdaptySubscriptionOfferType.Introductory => "introductory",
                AdaptySubscriptionOfferType.Promotional => "promotional",
                AdaptySubscriptionOfferType.WinBack => "win_back",
                _ => "unknown",
            };

        internal static AdaptySubscriptionOfferType GetAdaptySubscriptionOfferType(this JSONNode node, string aKey) =>
            GetString(node, aKey).ToAdaptySubscriptionOfferType();

        internal static AdaptySubscriptionOfferType? GetdaptySubscriptionOfferTypeIfPresent(this JSONNode node, string aKey) =>
            GetStringIfPresent(node, aKey)?.ToAdaptySubscriptionOfferType();

        private static AdaptySubscriptionOfferType ToAdaptySubscriptionOfferType(this string value) =>
            value switch
            {
                "introductory" => AdaptySubscriptionOfferType.Introductory,
                "promotional" => AdaptySubscriptionOfferType.Promotional,
                "win_back" => AdaptySubscriptionOfferType.WinBack,
                _ => throw new Exception($"AdaptySubscriptionOfferType unknown value: {value}"),
            };
    }
}