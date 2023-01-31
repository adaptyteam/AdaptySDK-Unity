//
//  AccessLevel+JSON.cs
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
        public partial class AccessLevel
        {
            internal AccessLevel(JSONObject jsonNode)
            {
                Id = jsonNode.GetString("id");
                IsActive = jsonNode.GetBoolean("is_active");
                VendorProductId = jsonNode.GetString("vendor_product_id");
                Store = jsonNode.GetString("store");
                ActivatedAt = jsonNode.GetDateTime("activated_at");
                RenewedAt = jsonNode.GetDateTimeIfPresent("renewed_at");
                ExpiresAt = jsonNode.GetDateTimeIfPresent("expires_at");
                IsLifetime = jsonNode.GetBoolean("is_lifetime");
                ActiveIntroductoryOfferType = jsonNode.GetStringIfPresent("active_introductory_offer_type");
                ActivePromotionalOfferType = jsonNode.GetStringIfPresent("active_promotional_offer_type");
                ActivePromotionalOfferId = jsonNode.GetStringIfPresent("active_promotional_offer_id");
                WillRenew = jsonNode.GetBoolean("will_renew");
                IsInGracePeriod = jsonNode.GetBoolean("is_in_grace_period");
                UnsubscribedAt = jsonNode.GetDateTimeIfPresent("unsubscribed_at");
                BillingIssueDetectedAt = jsonNode.GetDateTimeIfPresent("billing_issue_detected_at");
                StartsAt = jsonNode.GetDateTimeIfPresent("starts_at");
                CancellationReason = jsonNode.GetStringIfPresent("cancellation_reason");
                IsRefund = jsonNode.GetBoolean("is_refund");
            }
        }
    }
}


namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.AccessLevel GetAccessLevel(this JSONNode node, string aKey)
             => new Adapty.AccessLevel(GetObject(node, aKey));

        internal static Adapty.AccessLevel GetAccessLevelIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.AccessLevel(obj);
        }

        internal static IDictionary<string, Adapty.AccessLevel> GetAccessLevelDictionaryIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj == null) return null;
            var result = new Dictionary<string, Adapty.AccessLevel>();
            foreach (var item in obj)
            {
                var value = item.Value;
                if (!value.IsObject) throw new Exception($"Value by key: {item.Key} is not Object");
                result.Add(item.Key, new Adapty.AccessLevel(value.AsObject));
            }
            return result;
        }
    }
}