//
//  RenewalType+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.RenewalType value)
        {
            switch (value)
            {
                case Adapty.RenewalType.Prepaid: return "prepaid";
                case Adapty.RenewalType.Autorenewable: return "autorenewable";
                default: return "autorenewable";
            }
        }

        internal static Adapty.RenewalType GetRenewalType(this JSONNode node, string aKey)
            => GetString(node, aKey).ToRenewalType();

        internal static Adapty.RenewalType? GetRenewalTypeIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToRenewalType();

        internal static Adapty.RenewalType ToRenewalType(this string value)
        {
            switch (value)
            {
                case "prepaid": return Adapty.RenewalType.Prepaid;
                case "autorenewable": return Adapty.RenewalType.Autorenewable;
                default: return Adapty.RenewalType.Autorenewable;
            }
        }
    }
}