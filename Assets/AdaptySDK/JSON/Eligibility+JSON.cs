//
//  Eligibility+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.Eligibility value)
        {
            switch (value)
            {
                case Adapty.Eligibility.Ineligible: return "ineligible";
                case Adapty.Eligibility.Eligible: return "eligible";
                case Adapty.Eligibility.Unknown: return "unknown";
                default: return "unknown";
            }
        }

        internal static Adapty.Eligibility GetEligibility(this JSONNode node, string aKey)
            => GetString(node, aKey).ToEligibility();
        internal static Adapty.Eligibility? GetEligibilityIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToEligibility();

        internal static Adapty.Eligibility ToEligibility(this string value)
        {
            switch (value)
            {
                case "ineligible": return Adapty.Eligibility.Ineligible;
                case "eligible": return Adapty.Eligibility.Eligible;
                case "unknown": return Adapty.Eligibility.Unknown;
                default: return Adapty.Eligibility.Unknown;
            }
        }
    }
}