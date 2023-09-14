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
        {
            switch (value)
            {
                case Adapty.IOSAppTrackingTransparencyStatus.NotDetermined: return "not_determined";
                case Adapty.IOSAppTrackingTransparencyStatus.Restricted: return "restricted";
                case Adapty.IOSAppTrackingTransparencyStatus.Denied: return "denied";
                case Adapty.IOSAppTrackingTransparencyStatus.Authorized: return "authorized";
                default: throw new Exception($"IOSAppTrackingTransparencyStatus unknown value: {value}");
            }
        }

        internal static Adapty.IOSAppTrackingTransparencyStatus GetIOSAppTrackingTransparencyStatus(this JSONNode node, string aKey)
            => GetString(node, aKey).ToIOSAppTrackingTransparencyStatus();

        internal static Adapty.IOSAppTrackingTransparencyStatus? GetIOSAppTrackingTransparencyStatusIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToIOSAppTrackingTransparencyStatus();

        internal static Adapty.IOSAppTrackingTransparencyStatus ToIOSAppTrackingTransparencyStatus(this string value)
        {
            switch (value)
            {
                case "not_determined": return Adapty.IOSAppTrackingTransparencyStatus.NotDetermined;
                case "restricted": return Adapty.IOSAppTrackingTransparencyStatus.Restricted;
                case "denied": return Adapty.IOSAppTrackingTransparencyStatus.Denied;
                case "authorized": return Adapty.IOSAppTrackingTransparencyStatus.Authorized;
                default: throw new Exception($"IOSAppTrackingTransparencyStatus unknown value: {value}");
            }
        }
    }
}