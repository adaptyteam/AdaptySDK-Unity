using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {

        public static DateTime DateTimeFromJSON(JSONNode response)
        {
            return DateTimeFromString(response);
        }

        public static DateTime? NullableDateTimeFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull) return null;
            return DateTimeFromJSON(response);
        }

        public static DateTime DateTimeFromString(string value)
        {
            try
            {
                return DateTime.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding DateTimeFromString: {e} source: \"{value}\"");
                throw e;
            }
        }

        public static DateTime? NullableDateTimeFromString(string value)
        {
            if (value == null) return null;
            return DateTimeFromString(value);
        }
    }
}