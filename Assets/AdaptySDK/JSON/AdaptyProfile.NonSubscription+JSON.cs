//
//  AdaptyProfile.NonSubscription+JSON.cs
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
        public partial class NonSubscription
        {
            internal NonSubscription(JSONObject jsonNode)
            {
                PurchaseId = jsonNode.GetString("purchase_id");
                Store = jsonNode.GetString("store");
                VendorProductId = jsonNode.GetString("vendor_product_id");
                VendorTransactionId = jsonNode.GetStringIfPresent("vendor_transaction_id");
                PurchasedAt = jsonNode.GetDateTime("purchased_at");
                IsConsumable = jsonNode.GetBoolean("is_consumable");
                IsSandbox = jsonNode.GetBoolean("is_sandbox");
                IsRefund = jsonNode.GetBoolean("is_refund");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyProfile.NonSubscription GetNonSubscription(this JSONNode node, string aKey)
             => new AdaptyProfile.NonSubscription(GetObject(node, aKey));

        internal static AdaptyProfile.NonSubscription GetNonSubscriptionIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyProfile.NonSubscription(obj);
        }

        internal static IDictionary<string, IList<AdaptyProfile.NonSubscription>> GetNonSubscriptionDictionaryIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj == null) return null;
            var result = new Dictionary<string, IList<AdaptyProfile.NonSubscription>>();
            foreach (var item in obj)
            {
                var array = item.Value;
                if (!array.IsArray) throw new Exception($"Value by key: {item.Key} is not Array");
                var list = new List<AdaptyProfile.NonSubscription>();
                foreach (var value in array.Children)
                {
                    if (!value.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                    list.Add(new AdaptyProfile.NonSubscription(value.AsObject));
                }
                result.Add(item.Key, list);
            }
            return result;
        }
    }
}