//
//  DeferredProduct.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class DeferredProduct
        {
            /// Unique identifier of the product.
            public readonly string VendorProductId;

            /// Eligibility of user for promotional offer.
            public bool PromotionalOfferEligibility
            {
                get
                {
                    return PromotionalOfferId != null;
                }
            }

            /// Id of the offer, provided by Adapty for this specific user.
            ///
            /// [Nullable]
            public readonly string PromotionalOfferId;

            /// A description of the product.
            public readonly string LocalizedDescription;

            /// The name of the product.
            public readonly string LocalizedTitle;

            /// The cost of the product in the local currency.
            public readonly double Price;

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

            /// Indicates whether the product is available for family sharing in App Store Connect.
            public readonly bool IsFamilyShareable;

            /// A ProductSubscriptionPeriodModel object.
            /// The period details for products that are subscriptions.
            ///
            /// [Nullable]
            public readonly SubscriptionPeriod SubscriptionPeriod;

            /// A ProductDiscountModel object, containing introductory price information for the product.
            ///
            /// [Nullable]
            public readonly ProductDiscount IntroductoryDiscount;

            /// The identifier of the subscription group to which the subscription belongs.
            ///
            /// [Nullable]
            public readonly string SubscriptionGroupIdentifier;

            /// An list of [Adapty.ProductDiscount] discount offers available for the product.
            public readonly IList<ProductDiscount> Discounts;

            /// Localized price of the product.
            ///
            /// [Nullable]
            public readonly string LocalizedPrice;

            /// Localized subscription period of the product.
            ///
            /// [Nullable]
            public readonly string LocalizedSubscriptionPeriod;

            public override string ToString()
            {
                return $"{nameof(VendorProductId)}: {VendorProductId}, " +
                       $"{nameof(PromotionalOfferEligibility)}: {PromotionalOfferEligibility}, " +
                       $"{nameof(PromotionalOfferId)}: {PromotionalOfferId}, " +
                       $"{nameof(LocalizedDescription)}: {LocalizedDescription}, " +
                       $"{nameof(LocalizedTitle)}: {LocalizedTitle}, " +
                       $"{nameof(Price)}: {Price}, " +
                       $"{nameof(CurrencyCode)}: {CurrencyCode}, " +
                       $"{nameof(CurrencySymbol)}: {CurrencySymbol}, " +
                       $"{nameof(RegionCode)}: {RegionCode}, " +
                       $"{nameof(IsFamilyShareable)}: {IsFamilyShareable}, " +
                       $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                       $"{nameof(IntroductoryDiscount)}: {IntroductoryDiscount}, " +
                       $"{nameof(SubscriptionGroupIdentifier)}: {SubscriptionGroupIdentifier}, " +
                       $"{nameof(Discounts)}: {Discounts}, " +
                       $"{nameof(LocalizedPrice)}: {LocalizedPrice}, " +
                       $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}";
            }
        }
    }
}
