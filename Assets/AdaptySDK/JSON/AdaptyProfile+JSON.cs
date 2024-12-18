//
//  AdaptyProfile+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyProfile
    {
        internal AdaptyProfile(JSONObject jsonNode)
        {
            ProfileId = jsonNode.GetString("profile_id");
            SegmentId = jsonNode.GetString("segment_hash");
            CustomerUserId = jsonNode.GetStringIfPresent("customer_user_id");
            CustomAttributes = jsonNode.GetDictionaryIfPresent("custom_attributes") ?? new Dictionary<string, dynamic>();
            AccessLevels = jsonNode.GetAccessLevelDictionaryIfPresent("paid_access_levels") ?? new Dictionary<string, AccessLevel>();
            Subscriptions = jsonNode.GetSubscriptionDictionaryIfPresent("subscriptions") ?? new Dictionary<string, Subscription>();
            NonSubscriptions = jsonNode.GetNonSubscriptionDictionaryIfPresent("non_subscriptions") ?? new Dictionary<string, IList<NonSubscription>>();
            Version = jsonNode.GetInteger("timestamp");
            IsTestUser = jsonNode.GetBoolean("is_test_user");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyProfile GetAdaptyProfile(this JSONNode node)
             => new AdaptyProfile(GetObject(node));
        internal static AdaptyProfile GetAdaptyProfile(this JSONNode node, string aKey)
             => new AdaptyProfile(GetObject(node, aKey));

        internal static AdaptyProfile GetAdaptyProfileIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyProfile(obj);
        }
    }
}