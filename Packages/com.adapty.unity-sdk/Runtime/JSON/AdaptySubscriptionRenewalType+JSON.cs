//
//  AdaptySubscriptionRenewalType+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySubscriptionRenewalType GetAdaptySubscriptionRenewalType(this JSONNode node, string aKey)
            => GetString(node, aKey).ToAdaptySubscriptionRenewalType();

        internal static AdaptySubscriptionRenewalType? GetAdaptySubscriptionRenewalTypeIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToAdaptySubscriptionRenewalType();

        private static AdaptySubscriptionRenewalType ToAdaptySubscriptionRenewalType(this string value)
        {
            switch (value)
            {
                case "prepaid": return AdaptySubscriptionRenewalType.Prepaid;
                case "autorenewable": return AdaptySubscriptionRenewalType.Autorenewable;
                default: return AdaptySubscriptionRenewalType.Autorenewable;
            }
        }
    }
}