//
//  SubscriptionPhase+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class SubscriptionPhase
        {
            internal SubscriptionPhase(JSONObject jsonNode)
            {
                Price = jsonNode.GetPrice("price");
                Identifier = jsonNode.GetStringIfPresent("identifier");
                NumberOfPeriods = jsonNode.GetInteger("number_of_periods");
                PaymentMode = jsonNode.GetPaymentModeIfPresent("payment_mode") ?? Adapty.PaymentMode.Unknown;
                SubscriptionPeriod = jsonNode.GetSubscriptionPeriod("subscription_period");
                LocalizedSubscriptionPeriod = jsonNode.GetStringIfPresent("localized_subscription_period");
                LocalizedNumberOfPeriods = jsonNode.GetStringIfPresent("localized_number_of_periods");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.SubscriptionPhase GetSubscriptionPhase(this JSONNode node, string aKey)
             => new Adapty.SubscriptionPhase(GetObject(node, aKey));

        internal static Adapty.SubscriptionPhase GetSubscriptionPhaseIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.SubscriptionPhase(obj);
        }

        internal static IList<Adapty.SubscriptionPhase> GetSubscriptionPhaseListIfPresent(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<Adapty.SubscriptionPhase>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new Adapty.SubscriptionPhase(item.AsObject));
            }
            return result;
        }
    }
}