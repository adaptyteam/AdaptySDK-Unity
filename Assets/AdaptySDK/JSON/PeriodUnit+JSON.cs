//
//  PeriodUnit.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.PeriodUnit value)
        {
            switch (value)
            {
                case Adapty.PeriodUnit.Day: return "day";
                case Adapty.PeriodUnit.Week: return "week";
                case Adapty.PeriodUnit.Month: return "month";
                case Adapty.PeriodUnit.Year: return "year";
                default: return "unknown";
            }
        }

        internal static Adapty.PeriodUnit GetPeriodUnit(this JSONNode node, string aKey)
            => GetString(node, aKey).ToPeriodUnit();
        internal static Adapty.PeriodUnit? GetPeriodUnitIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToPeriodUnit();

        internal static Adapty.PeriodUnit ToPeriodUnit(this string value)
        {
            switch (value)
            {
                case "day": return Adapty.PeriodUnit.Day;
                case "week": return Adapty.PeriodUnit.Week;
                case "month": return Adapty.PeriodUnit.Month;
                case "year": return Adapty.PeriodUnit.Year;
                default: return Adapty.PeriodUnit.Unknown;
            }
        }
    }
}