//
//  IOSAppTrackingTransparencyStatus+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.IOSAppTrackingTransparencyStatus value)
            => value switch
            {
                Adapty.IOSAppTrackingTransparencyStatus.NotDetermined => "not_determined",
                Adapty.IOSAppTrackingTransparencyStatus.Restricted => "restricted",
                Adapty.IOSAppTrackingTransparencyStatus.Denied => "denied",
                Adapty.IOSAppTrackingTransparencyStatus.Authorized => "authorized",
                _ => throw new Exception($"IOSAppTrackingTransparencyStatus unknown value: {value}"),
            };

        internal static Adapty.IOSAppTrackingTransparencyStatus GetIOSAppTrackingTransparencyStatus(this JSONNode node, string aKey)
            => GetString(node, aKey).ToIOSAppTrackingTransparencyStatus();
        internal static Adapty.IOSAppTrackingTransparencyStatus? GetIOSAppTrackingTransparencyStatusIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToIOSAppTrackingTransparencyStatus();

        internal static Adapty.IOSAppTrackingTransparencyStatus ToIOSAppTrackingTransparencyStatus(this string value)
            => value switch
            {
                "not_determined" => Adapty.IOSAppTrackingTransparencyStatus.NotDetermined,
                "restricted" => Adapty.IOSAppTrackingTransparencyStatus.Restricted,
                "denied" => Adapty.IOSAppTrackingTransparencyStatus.Denied,
                "authorized" => Adapty.IOSAppTrackingTransparencyStatus.Authorized,
                _ => throw new Exception($"IOSAppTrackingTransparencyStatus unknown value: {value}"),
            };
    }
}