//
//  SubscriptionDetails+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class SubscriptionDetails
        {
            internal SubscriptionDetails(JSONObject jsonNode)
            {
                SubscriptionGroupIdentifier = jsonNode.GetStringIfPresent("subscription_group_identifier");
#if UNITY_ANDROID
                AndroidIntroductoryOfferEligibility = jsonNode.GetEligibility("introductory_offer_eligibility");
                AndroidBasePlanId = jsonNode.GetStringIfPresent("android_base_plan_id");
                AndroidOfferId = jsonNode.GetStringIfPresent("android_offer_id");
                AndroidOfferTags = jsonNode.GetStringListIfPresent("android_offer_tags") ?? new List<string>();
#else
                AndroidIntroductoryOfferEligibility = null;
                AndroidBasePlanId = null;
                AndroidOfferId = null;
                AndroidOfferTags = new List<string>();
#endif
                IntroductoryOffer = jsonNode.GetSubscriptionPhaseListIfPresent("introductory_offer_phases") ?? new List<SubscriptionPhase>();
                PromotionalOffer = jsonNode.GetSubscriptionPhaseIfPresent("promotional_offer");
                RenewalType = jsonNode.GetRenewalTypeIfPresent("renewal_type") ?? Adapty.RenewalType.Autorenewable;
                SubscriptionPeriod = jsonNode.GetSubscriptionPeriod("subscription_period");
                LocalizedSubscriptionPeriod = jsonNode.GetStringIfPresent("localized_subscription_period");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.SubscriptionDetails GetSubscriptionDetails(this JSONNode node, string aKey)
             => new Adapty.SubscriptionDetails(GetObject(node, aKey));

        internal static Adapty.SubscriptionDetails GetSubscriptionDetailsIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.SubscriptionDetails(obj);
        }
    }
}