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
         => value switch
         {
             Adapty.PeriodUnit.Day => "day",
             Adapty.PeriodUnit.Week => "week",
             Adapty.PeriodUnit.Month => "month",
             Adapty.PeriodUnit.Year => "year",
             _ => "unknown",
         };

        internal static Adapty.PeriodUnit GetPeriodUnit(this JSONNode node, string aKey)
            => GetString(node, aKey).ToPeriodUnit();
        internal static Adapty.PeriodUnit? GetPeriodUnitIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToPeriodUnit();

        internal static Adapty.PeriodUnit ToPeriodUnit(this string value)
            => value switch
            {
                "day" => Adapty.PeriodUnit.Day,
                "week" => Adapty.PeriodUnit.Week,
                "month" => Adapty.PeriodUnit.Month,
                "year" => Adapty.PeriodUnit.Year,
                _ => Adapty.PeriodUnit.Unknown,
            };
    }
}