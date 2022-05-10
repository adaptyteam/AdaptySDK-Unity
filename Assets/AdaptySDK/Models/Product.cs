using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class Product
        {
            /// Unique identifier of the product.
            public readonly string VendorProductId;

            /// Eligibility of user for introductory offer.
            public readonly bool IntroductoryOfferEligibility;

            /// Eligibility of user for promotional offer.
            public readonly bool PromotionalOfferEligibility;

            /// Id of the offer, provided by Adapty for this specific user.
            ///
            /// [Nullable]
            public readonly string PromotionalOfferId;

            /// The identifier of the variation, used to attribute purchases to the paywall.
            ///
            /// [Nullable]
            public readonly string VariationId;

            /// A description of the product.
            public readonly string LocalizedDescription;

            /// The name of the product.
            public readonly string LocalizedTitle;

            /// The cost of the product in the local currency.
            public readonly double? Price;

            /// Product locale currency code.
            ///
            /// [Nullable]
            public readonly string CurrencyCode;

            /// Product locale currency symbol.
            ///
            /// [Nullable]
            public readonly string CurrencySymbol;

            /// Product locale region code.
            ///
            /// [Nullable]
            public readonly string RegionCode;

            /// A ProductSubscriptionPeriodModel object.
            /// The period details for products that are subscriptions.
            ///
            /// [Nullable]
            public readonly Period SubscriptionPeriod;

            /// A ProductDiscountModel object, containing introductory price information for the product.
            ///
            /// [Nullable]
            public readonly ProductDiscount IntroductoryDiscount;

            /// The identifier of the subscription group to which the subscription belongs.
            ///
            /// [Nullable]
            public readonly string SubscriptionGroupIdentifier;

            /// An array of [Adapty.ProductDiscount] discount offers available for the product.
            public readonly ProductDiscount[] Discounts;

            /// Localized price of the product.
            ///
            /// [Nullable]
            public readonly string LocalizedPrice;

            /// Localized subscription period of the product.
            ///
            /// [Nullable]
            public readonly string LocalizedSubscriptionPeriod;

            /// Parent A/B test name
            public readonly string PaywallABTestName;

            /// Indicates whether the product is available for family sharing in App Store Connect.
            public readonly bool IsFamilyShareable;

            /// Parent paywall name
            public readonly string PaywallName;

            /// The duration of the trial period. (Android only)
            public readonly Period FreeTrialPeriod;

            /// Localized trial period of the product. (Android only)
            public readonly string LocalizedFreeTrialPeriod;

            internal Product(JSONNode response)
            {
                VendorProductId = response["vendor_product_id"];
                IntroductoryOfferEligibility = response["introductory_offer_eligibility"];
                PromotionalOfferEligibility = response["promotional_offer_eligibility"];
                PromotionalOfferId = response["promotional_offer_id"];
                VariationId = response["variation_id"];
                LocalizedDescription = response["localized_description"];
                LocalizedTitle = response["localized_title"];
                var price = response["price"];
                this.Price = (price == null || price.IsNull || !price.IsNumber) ? null : price;
                CurrencyCode = response["currency_code"];
                CurrencySymbol = response["currency_symbol"];
                RegionCode = response["region_code"];
                SubscriptionPeriod = PeriodFromJSON( response["subscription_period"]);
                IntroductoryDiscount = ProductDiscountFromJSON(response["introductory_discount"]);
                var discounts = response["discounts"];
                if (discounts != null && !discounts.IsNull && discounts.IsArray) {
                    var _discounts = new List<ProductDiscount>();
                    foreach (var item in discounts)
                    {
                        var value = ProductDiscountFromJSON(item);
                        if (value != null)
                        {
                            _discounts.Add(value);
                        }
                    }
                    this.Discounts = _discounts.ToArray();
                } 
                SubscriptionGroupIdentifier = response["subscription_group_identifier"];
                LocalizedPrice = response["localized_price"];
                LocalizedSubscriptionPeriod = response["localized_subscription_period"];
                PaywallABTestName = response["paywall_ab_test_name"] ?? response["paywall_a_b_test_name"];
                PaywallName = response["paywall_name"];
                IsFamilyShareable = response["is_family_shareable"];
                FreeTrialPeriod = PeriodFromJSON(response["free_trial_period"]);
                LocalizedFreeTrialPeriod = response["localized_free_trial_period"];

            }

            public override string ToString()
            {
                return $"{nameof(VendorProductId)}: {VendorProductId}, " +
                       $"{nameof(IntroductoryOfferEligibility)}: {IntroductoryOfferEligibility}, " +
                       $"{nameof(PromotionalOfferEligibility)}: {PromotionalOfferEligibility}, " +
                       $"{nameof(PromotionalOfferId)}: {PromotionalOfferId}, " +
                       $"{nameof(VariationId)}: {VariationId}, " +
                       $"{nameof(LocalizedDescription)}: {LocalizedDescription}, " +
                       $"{nameof(LocalizedTitle)}: {LocalizedTitle}, " +
                       $"{nameof(Price)}: {Price}, " +
                       $"{nameof(CurrencyCode)}: {CurrencyCode}, " +
                       $"{nameof(CurrencySymbol)}: {CurrencySymbol}, " +
                       $"{nameof(RegionCode)}: {RegionCode}, " +
                       $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                       $"{nameof(IntroductoryDiscount)}: {IntroductoryDiscount}, " +
                       $"{nameof(Discounts)}: {Discounts}, " +
                       $"{nameof(SubscriptionGroupIdentifier)}: {SubscriptionGroupIdentifier}, " +
                       $"{nameof(LocalizedPrice)}: {LocalizedPrice}, " +
                       $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
                       $"{nameof(PaywallABTestName)}: {PaywallABTestName}, " +
                       $"{nameof(PaywallName)}: {PaywallName}, " +
                       $"{nameof(IsFamilyShareable)}: {IsFamilyShareable}, " +
                       $"{nameof(FreeTrialPeriod)}: {FreeTrialPeriod}, " +
                       $"{nameof(LocalizedFreeTrialPeriod)}: {LocalizedFreeTrialPeriod}";
            }
        }

        public static Product ProductFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull || !response.IsObject) return null;
            try { 
                return new Product(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding Product: {e} source: {response}");
                return null;
            }
        }
    }
}