//
//  DeferredProduct+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class DeferredProduct
        {
            internal DeferredProduct(JSONObject jsonNode)
            {
                VendorProductId = jsonNode.GetString("vendor_product_id");
                PromotionalOfferId = jsonNode.GetStringIfPresent("promotional_offer_id");
                LocalizedDescription = jsonNode.GetString("localized_description");
                LocalizedTitle = jsonNode.GetString("localized_title");
                Price = jsonNode.GetDouble("price");
                CurrencyCode = jsonNode.GetStringIfPresent("currency_code");
                CurrencySymbol = jsonNode.GetStringIfPresent("currency_symbol");
                RegionCode = jsonNode.GetStringIfPresent("region_code");
                IsFamilyShareable = jsonNode.GetBoolean("is_family_shareable");
                SubscriptionPeriod = jsonNode.GetSubscriptionPeriodIfPresent("subscription_period");
                IntroductoryDiscount = jsonNode.GetProductDiscountIfPresent("introductory_discount");
                SubscriptionGroupIdentifier = jsonNode.GetStringIfPresent("subscription_group_identifier");
                Discounts = jsonNode.GetProductDiscountListIfPresent("discounts") ?? new List<ProductDiscount>();
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
        internal static Adapty.DeferredProduct GetDeferredProduct(this JSONNode node, string aKey)
             => new Adapty.DeferredProduct(GetObject(node, aKey));

        internal static Adapty.DeferredProduct GetDeferredProductIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.DeferredProduct(obj);
        }
    }
}