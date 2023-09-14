//
//  Profile+JSON.cs
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
        public partial class Profile
        {
            internal Profile(JSONObject jsonNode)
            {
                ProfileId = jsonNode.GetString("profile_id");
                CustomerUserId = jsonNode.GetStringIfPresent("customer_user_id");
                CustomAttributes = jsonNode.GetDictionaryIfPresent("custom_attributes") ?? new Dictionary<string, dynamic>();
                AccessLevels = jsonNode.GetAccessLevelDictionaryIfPresent("paid_access_levels") ?? new Dictionary<string, AccessLevel>();
                Subscriptions = jsonNode.GetSubscriptionDictionaryIfPresent("subscriptions") ?? new Dictionary<string, Subscription>();
                NonSubscriptions = jsonNode.GetNonSubscriptionDictionaryIfPresent("non_subscriptions") ?? new Dictionary<string, IList<NonSubscription>>();
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Profile GetProfile(this JSONObject obj)
           => new Adapty.Profile(obj);

        internal static Adapty.Profile GetProfile(this JSONNode node, string aKey)
           => new Adapty.Profile(GetObject(node, aKey));
    }
}