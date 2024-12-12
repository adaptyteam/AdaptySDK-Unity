//
//  AdaptySubscriptionPeriodUnit+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySubscriptionPeriodUnit GetAdaptySubscriptionPeriodUnit(this JSONNode node, string aKey)
            => GetString(node, aKey).ToAdaptySubscriptionPeriodUnit();

        internal static AdaptySubscriptionPeriodUnit? GetAdaptySubscriptionPeriodUnitIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToAdaptySubscriptionPeriodUnit();

        internal static AdaptySubscriptionPeriodUnit ToAdaptySubscriptionPeriodUnit(this string value)
        {
            switch (value)
            {
                case "day": return AdaptySubscriptionPeriodUnit.Day;
                case "week": return AdaptySubscriptionPeriodUnit.Week;
                case "month": return AdaptySubscriptionPeriodUnit.Month;
                case "year": return AdaptySubscriptionPeriodUnit.Year;
                default: return AdaptySubscriptionPeriodUnit.Unknown;
            }
        }
    }
}