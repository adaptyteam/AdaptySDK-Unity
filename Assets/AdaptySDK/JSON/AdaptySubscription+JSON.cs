//
//  AdaptySubscription+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptySubscription
    {
        internal AdaptySubscription(JSONObject jsonNode)
        {
#if UNITY_IOS
            GroupIdentifier = jsonNode.GetString("group_identifier");
#else
            GroupIdentifier = null;
#endif
            Period = jsonNode.GetAdaptySubscriptionPeriod("period");
            LocalizedPeriod = jsonNode.GetStringIfPresent("localized_period");
            Offer = jsonNode.GetAdaptySubscriptionOfferIfPresent("offer");
            RenewalType = jsonNode.GetAdaptySubscriptionRenewalTypeIfPresent("renewal_type") ?? AdaptySubscriptionRenewalType.Autorenewable;
            BasePlanId = jsonNode.GetStringIfPresent("base_plan_id");

#if UNITY_ANDROID
            RenewalType = jsonNode.GetAdaptySubscriptionRenewalType("renewal_type");
            BasePlanId = jsonNode.GetString("base_plan_id");
#else
            RenewalType = AdaptySubscriptionRenewalType.Autorenewable;
            BasePlanId = null;
#endif

        }
    }

}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySubscription GetAdaptySubscription(this JSONNode node, string aKey)
             => new AdaptySubscription(GetObject(node, aKey));

        internal static AdaptySubscription GetAdaptySubscriptionIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptySubscription(obj);
        }
    }
}