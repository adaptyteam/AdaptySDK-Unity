//
//  ProductDiscount+JSON.cs
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
        public partial class ProductDiscount
        {
            internal ProductDiscount(JSONObject jsonNode)
            {
                Price = jsonNode.GetDouble("price");
#if UNITY_IOS
                Identifier = jsonNode.GetStringIfPresent("identifier");
                PaymentMode = jsonNode.GetPaymentMode("payment_mode");
                LocalizedNumberOfPeriods = jsonNode.GetStringIfPresent("localized_number_of_periods");

#else
                Identifier = null;
                PaymentMode = PaymentMode.Unknown;
                LocalizedNumberOfPeriods = null;
#endif
                SubscriptionPeriod = jsonNode.GetSubscriptionPeriod("subscription_period");
                NumberOfPeriods = jsonNode.GetInteger("number_of_periods");
                LocalizedPrice = jsonNode.GetStringIfPresent("localized_price");
                LocalizedSubscriptionPeriod = jsonNode.GetStringIfPresent("localized_subscription_period");
            }
        }
    }
}


namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.ProductDiscount GetProductDiscount(this JSONNode node, string aKey)
             => new Adapty.ProductDiscount(GetObject(node, aKey));

        internal static Adapty.ProductDiscount GetProductDiscountIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.ProductDiscount(obj);
        }

        internal static IList<Adapty.ProductDiscount> GetProductDiscountListIfPresent(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<Adapty.ProductDiscount>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new Adapty.ProductDiscount(item.AsObject));
            }
            return result;
        }
    }
}