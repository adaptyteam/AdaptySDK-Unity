using System;
using System.Globalization;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static DateTime GetDateTime(this JSONNode node, string aKey) => node.GetString(aKey).ToDateTime();
        internal static DateTime? GetDateTimeIfPresent(this JSONNode node, string aKey)
        {
            var str = node.GetStringIfPresent(aKey);
            if (str is null) return null;
            return str.ToDateTime();
        }

        private static DateTime ToDateTime(this string value)
        {
            try
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new Exception($"Exception on decoding DateTime from string: {e} source: \"{value}\"");
            }
        }
    }
}