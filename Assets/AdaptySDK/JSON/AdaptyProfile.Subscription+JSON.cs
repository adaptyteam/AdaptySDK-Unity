﻿//
//  AdaptyProfile.Subscription+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//
using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyProfile
    {
        public partial class Subscription
        {
            internal Subscription(JSONObject jsonNode)
            {
                IsActive = jsonNode.GetBoolean("is_active");
                VendorProductId = jsonNode.GetString("vendor_product_id");
                Store = jsonNode.GetString("store");
                ActivatedAt = jsonNode.GetDateTime("activated_at");
                RenewedAt = jsonNode.GetDateTimeIfPresent("renewed_at");
                ExpiresAt = jsonNode.GetDateTimeIfPresent("expires_at");
                StartsAt = jsonNode.GetDateTimeIfPresent("starts_at");
                IsLifetime = jsonNode.GetBoolean("is_lifetime");
                ActiveIntroductoryOfferType = jsonNode.GetStringIfPresent("active_introductory_offer_type");
                ActivePromotionalOfferType = jsonNode.GetStringIfPresent("active_promotional_offer_type");
                ActivePromotionalOfferId = jsonNode.GetStringIfPresent("active_promotional_offer_id");
                OfferId = jsonNode.GetStringIfPresent("offer_id");
                WillRenew = jsonNode.GetBoolean("will_renew");
                IsInGracePeriod = jsonNode.GetBoolean("is_in_grace_period");
                UnsubscribedAt = jsonNode.GetDateTimeIfPresent("unsubscribed_at");
                BillingIssueDetectedAt = jsonNode.GetDateTimeIfPresent("billing_issue_detected_at");
                IsSandbox = jsonNode.GetBoolean("is_sandbox");
                VendorTransactionId = jsonNode.GetString("vendor_transaction_id");
                VendorOriginalTransactionId = jsonNode.GetString("vendor_original_transaction_id");
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
        internal static AdaptyProfile.Subscription GetSubscription(this JSONNode node, string aKey)
             => new AdaptyProfile.Subscription(GetObject(node, aKey));

        internal static AdaptyProfile.Subscription GetSubscriptionIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyProfile.Subscription(obj);
        }

        internal static IDictionary<string, AdaptyProfile.Subscription> GetSubscriptionDictionaryIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj == null) return null;
            var result = new Dictionary<string, AdaptyProfile.Subscription>();
            foreach (var item in obj)
            {
                var value = item.Value;
                if (!value.IsObject) throw new Exception($"Value by key: {item.Key} is not Object");
                result.Add(item.Key, new AdaptyProfile.Subscription(value.AsObject));
            }
            return result;
        }
    }
}