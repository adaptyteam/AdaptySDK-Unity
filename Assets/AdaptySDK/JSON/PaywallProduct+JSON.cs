//
//  PaywallProduct+JSON.cs
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
        public partial class PaywallProduct
        {

            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("vendor_product_id", VendorProductId);
                node.Add("introductory_offer_eligibility", IntroductoryOfferEligibility.ToJSON());
                node.Add("timestamp", _Version);
#if UNITY_IOS
                if (PromotionalOfferId is not null) node.Add("promotional_offer_id", PromotionalOfferId);
#endif
                node.Add("variation_id", VariationId);
                node.Add("paywall_ab_test_name", PaywallABTestName);
                node.Add("paywall_name", PaywallName);
                if (_PayloadData is not null) node.Add("payload_data", _PayloadData);
                return node;
            }

            internal PaywallProduct(JSONObject jsonNode)
            {
                VendorProductId = jsonNode.GetString("vendor_product_id");
                IntroductoryOfferEligibility = jsonNode.GetEligibility("introductory_offer_eligibility");
                _Version = jsonNode.GetInteger("timestamp");
                _PayloadData = jsonNode.GetStringIfPresent("payload_data");
#if UNITY_IOS
                PromotionalOfferId = jsonNode.GetStringIfPresent("promotional_offer_id");
                RegionCode = jsonNode.GetStringIfPresent("region_code");
                IsFamilyShareable = jsonNode.GetBoolean("is_family_shareable");
                SubscriptionGroupIdentifier = jsonNode.GetStringIfPresent("subscription_group_identifier");
                Discounts = jsonNode.GetProductDiscountListIfPresent("discounts") ?? new List<ProductDiscount>();
#else
                PromotionalOfferId = null;
                RegionCode = null;
                IsFamilyShareable = false;
                SubscriptionGroupIdentifier = null;
                Discounts = new List<ProductDiscount>();
#endif
                VariationId = jsonNode.GetString("variation_id");
                PaywallABTestName = jsonNode.GetString("paywall_ab_test_name");
                PaywallName = jsonNode.GetString("paywall_name");

                LocalizedDescription = jsonNode.GetString("localized_description");
                LocalizedTitle = jsonNode.GetString("localized_title");
                Price = jsonNode.GetDouble("price");
                CurrencyCode = jsonNode.GetStringIfPresent("currency_code");
                CurrencySymbol = jsonNode.GetStringIfPresent("currency_symbol");
                SubscriptionPeriod = jsonNode.GetSubscriptionPeriodIfPresent("subscription_period");
                IntroductoryDiscount = jsonNode.GetProductDiscountIfPresent("introductory_discount");
                LocalizedPrice = jsonNode.GetStringIfPresent("localized_price");
                LocalizedSubscriptionPeriod = jsonNode.GetStringIfPresent("localized_subscription_period");
#if UNITY_ANDROID
                AndroidFreeTrialPeriod = jsonNode.GetSubscriptionPeriodIfPresent("localized_free_trial_period");
                AndroidLocalizedFreeTrialPeriod = jsonNode.GetStringIfPresent("free_trial_period");
#else
                AndroidFreeTrialPeriod = null;
                AndroidLocalizedFreeTrialPeriod = null;
#endif
            }
        }
    }
}


namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.PaywallProduct GetPaywallProduct(this JSONNode node, string aKey)
             => new Adapty.PaywallProduct(GetObject(node, aKey));

        internal static Adapty.PaywallProduct GetPaywallProductIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.PaywallProduct(obj);
        }

        internal static IList<Adapty.PaywallProduct> GetPaywallProductList(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<Adapty.PaywallProduct>();
            foreach (var item in array.Children)
            {
                if (!item.IsObject) throw new Exception($"Value by index: {result.Count} is not Object");
                result.Add(new Adapty.PaywallProduct(item.AsObject));
            }
            return result;
        }
    }
}