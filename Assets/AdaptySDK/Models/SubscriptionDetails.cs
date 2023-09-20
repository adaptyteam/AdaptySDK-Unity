//
//  SubscriptionDetails.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class SubscriptionDetails
        {
            /// The identifier of the subscription group to which the subscription belongs.
            ///
            /// [Nullable]
            public readonly string SubscriptionGroupIdentifier;

            internal readonly Eligibility? AndroidIntroductoryOfferEligibility; //nullable
            public readonly string AndroidBasePlanId; //nullable
            public readonly string AndroidOfferId; //nullable

            public readonly IList<string> AndroidOfferTags;
            public readonly IList<SubscriptionPhase> IntroductoryOffer;
            public readonly SubscriptionPhase PromotionalOffer; //nullable

            public readonly RenewalType RenewalType;

            /// A ProductSubscriptionPeriodModel object.
            /// The period details for products that are subscriptions.
            ///
            /// [Nullable]
            public readonly SubscriptionPeriod SubscriptionPeriod;

            /// Localized subscription period of the product.
            ///
            /// [Nullable]
            public readonly string LocalizedSubscriptionPeriod;

            /// Eligibility of user for promotional offer.
            public bool PromotionalOfferEligibility => PromotionalOffer != null;

            /// Id of the offer, provided by Adapty for this specific user.
            ///
            /// [Nullable]
            public string PromotionalOfferId => PromotionalOffer?.Identifier;

            public override string ToString() => $"{nameof(SubscriptionGroupIdentifier)}: {SubscriptionGroupIdentifier}, " +
                       $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
                       $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
                       $"{nameof(IntroductoryOffer)}: {IntroductoryOffer}, " +
                       $"{nameof(PromotionalOffer)}: {PromotionalOffer}, " +
                       $"{nameof(AndroidOfferId)}: {AndroidOfferId}, " +
                       $"{nameof(AndroidBasePlanId)}: {AndroidBasePlanId}, " +
                       $"{nameof(AndroidOfferTags)}: {AndroidOfferTags}, " +
                       $"{nameof(RenewalType)}: {RenewalType}, " +
                       $"{nameof(AndroidIntroductoryOfferEligibility)}: {AndroidIntroductoryOfferEligibility}";
        }
    }
}