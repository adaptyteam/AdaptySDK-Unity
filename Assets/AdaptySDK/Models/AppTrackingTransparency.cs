using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum AppTrackingTransparency
        {
            NotDetermined,
            Restricted,
            Denied,
            Authorized
        }

        public static AppTrackingTransparency AppTrackingTransparencyFromJSON(JSONNode response)
        {
            return AppTrackingTransparencyFromString(response);
        }

        public static AppTrackingTransparency AppTrackingTransparencyFromString(string value)
        {
            switch (value)
            {
                case "not_determined":
                case "notDetermined":
                    return AppTrackingTransparency.NotDetermined;
                case "restricted":
                    return AppTrackingTransparency.Restricted;
                case "denied":
                    return AppTrackingTransparency.Denied;
                case "authorized":
                    return AppTrackingTransparency.Authorized;
            }

            throw new Exception($"AppTrackingTransparency unknown value: {value}");
        }

        public static string AppTrackingTransparencyToString(this AppTrackingTransparency value)
        {
            switch (value)
            {
                case AppTrackingTransparency.NotDetermined:
                    return "not_determined";
                case AppTrackingTransparency.Restricted:
                    return "restricted";
                case AppTrackingTransparency.Denied:
                    return "denied";
                case AppTrackingTransparency.Authorized:
                    return "authorized";
            }
            throw new Exception($"AppTrackingTransparency unknown value: {value}");
        }
    }
}