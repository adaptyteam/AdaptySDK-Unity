using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class PurchaserInfo
        {
            /// An identifier of the user in Adapty
            public readonly string ProfileId;

            /// An identifier of the user in your system.
            ///
            /// [Nullable]
            public readonly string CustomerUserId;

            /// The keys are access level identifiers configured by you in Adapty Dashboard.
            /// The values are [AdaptyAccessLevelInfo] objects.
            /// Can be null if the customer has no access levels.
            public readonly Dictionary<string, AccessLevelInfo> AccessLevels;

            /// The keys are product ids from App Store Connect.
            /// The values are [AdaptySubscriptionInfo] objects.
            /// Can be null if the customer has no subscriptions.
            public readonly Dictionary<string, SubscriptionInfo> Subscriptions;

            /// The keys are product ids from App Store Connect.
            /// The values are array[] of [AdaptyNonSubscriptionInfo] objects.
            /// Can be null if the customer has no purchases.
            public readonly Dictionary<string, NonSubscriptionInfo[]> NonSubscriptions;

            internal PurchaserInfo(JSONNode response)
            {
                ProfileId = response["profile_id"] ?? response["id"];
                CustomerUserId = response["customer_user_id"];

                AccessLevels = new Dictionary<string, AccessLevelInfo>();
                Subscriptions = new Dictionary<string, SubscriptionInfo>();
                NonSubscriptions = new Dictionary<string, NonSubscriptionInfo[]>();

                var _accessLevels = response["access_levels"] ?? response["paid_access_levels"];
                foreach (var item in _accessLevels)
                {
                    var value = AccessLevelInfoInfoFromJSON(item.Value);
                    if (value != null)
                    {
                        AccessLevels.Add(item.Key, value);
                    }
                }

                var _subscriptions = response["subscriptions"];
                foreach (var item in _subscriptions)
                {
                    var value = SubscriptionInfoFromJSON(item.Value);
                    if (value != null)
                    {
                        Subscriptions.Add(item.Key, value);
                    }
                }

                var _nonSubscriptions = response["non_subscriptions"];
                foreach (var item in _nonSubscriptions)
                {
                    var list = new List<NonSubscriptionInfo>();
                    foreach (var subitem in item.Value)
                    {
                        var value = NonSubscriptionInfoFromJSON(subitem);
                        if (value != null) {
                            list.Add(value);
                        }
                    }
                    if (list.Count > 0)
                    {
                        NonSubscriptions.Add(item.Key, list.ToArray());
                    }
                }
            }

            public override string ToString()
            {
                return $"{nameof(ProfileId)}: {ProfileId}, " +
                       $"{nameof(CustomerUserId)}: {CustomerUserId}, " +
                       $"{nameof(AccessLevels)}: {AccessLevels}, " +
                       $"{nameof(Subscriptions)}: {Subscriptions}, " +
                       $"{nameof(NonSubscriptions)}: {NonSubscriptions}";
            }
        }


        public static PurchaserInfo PurchaserInfoFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull || !response.IsObject) return null;
            try
            {
                return new PurchaserInfo(response);
            } catch (Exception e)
            {
                Debug.LogError($"Exception on decoding PurchaserInfo: {e} source: {response}");
                return null;
            }
        }
    }
}
