//
//  AdaptySubscriptionPhase+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptySubscriptionPhase
    {
        internal AdaptySubscriptionPhase(JSONObject jsonNode)
        {
            Price = jsonNode.GetAdaptyPrice("price");
            NumberOfPeriods = jsonNode.GetInteger("number_of_periods");
            PaymentMode = jsonNode.GetAdaptyPaymentMode("payment_mode");
            SubscriptionPeriod = jsonNode.GetAdaptySubscriptionPeriod("subscription_period");
            LocalizedSubscriptionPeriod = jsonNode.GetStringIfPresent("localized_subscription_period");
            LocalizedNumberOfPeriods = jsonNode.GetStringIfPresent("localized_number_of_periods");
        }
    }

}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySubscriptionPhase GetAdaptySubscriptionPhase(this JSONNode node, string aKey)
             => new AdaptySubscriptionPhase(GetObject(node, aKey));

        internal static AdaptySubscriptionPhase GetAdaptySubscriptionPhaseIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptySubscriptionPhase(obj);
        }

        internal static IList<AdaptySubscriptionPhase> GetAdaptySubscriptionPhaseListIfPresent(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<AdaptySubscriptionPhase>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new AdaptySubscriptionPhase(item.AsObject));
            }
            return result;
        }
    }
}