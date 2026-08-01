//
//  AdaptyProfile.Subscription.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    public partial class AdaptyProfile
    {
        public partial class Subscription
        {
            /// The store of the purchase. The possible values are: app_store, play_store , adapty.
            public readonly string Store;

            /// The identifier of the product in the App Store Connect.
            public readonly string VendorProductId;

            /// Transaction id from the App Store.
            public readonly string VendorTransactionId;

            /// Original transaction id from the App Store.
            /**
            * For auto-renewable subscription, this will be the id of the first transaction in the subscription.
            */
            public readonly string VendorOriginalTransactionId;

            /// Whether the subscription is active.
            public readonly bool IsActive;

            /// Whether the subscription is active for a lifetime (no expiration date).
            /**
            * If set to true you shouldn't check expires_at , or you could just check isActive.
            */
            public readonly bool IsLifetime;

            /// The time when the subscription was activated.
            public readonly DateTime ActivatedAt;

            /// The time when the subscription was renewed.
            public readonly DateTime? RenewedAt; // nullable

            /// The time when the subscription will expire (could be in the past and could be null for lifetime access).
            public readonly DateTime? ExpiresAt; // nullable

            /// The time when the subscription has started (could be in the future).
            public readonly DateTime? StartsAt; // nullable

            /// The time when the auto-renewable subscription was cancelled.
            /**
            * Subscription can still be active, it just means that auto-renewal turned off.
            * Will be set to null if the user reactivates the subscription.
            */
            public readonly DateTime? UnsubscribedAt; // nullable

            /// The time when billing issue was detected (Apple was not able to charge the card).
            /**
            * Subscription can still be active. Will be set to null if the charge will be made.
            */
            public readonly DateTime? BillingIssueDetectedAt; // nullable

            /// Whether the auto-renewable subscription is in the grace period.
            public readonly bool IsInGracePeriod;

            /// Whether the product was purchased in the sandbox environment.
            public readonly bool IsSandbox;

            /// Whether the purchase was refunded.
            public readonly bool IsRefund;

            /// Whether the auto-renewable subscription is set to renew.
            public readonly bool WillRenew;

            /// The type of active introductory offer.
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            public readonly string ActiveIntroductoryOfferType; // nullable

            /// The type of active promotional offer.
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            public readonly string ActivePromotionalOfferType; // nullable

            public readonly string ActivePromotionalOfferId; // nullable

            public readonly string OfferId; // nullable

            /// The reason why the subscription was cancelled.
            /**
            * Possible values are: voluntarily_cancelled, billing_error, refund, price_increase, product_was_not_available, unknown.
            */
            public readonly string CancellationReason; // nullable

            public override string ToString() => $"{nameof(IsActive)}: {IsActive}, " +
                       $"{nameof(VendorProductId)}: {VendorProductId}, " +
                       $"{nameof(Store)}: {Store}, " +
                       $"{nameof(ActivatedAt)}: {ActivatedAt}, " +
                       $"{nameof(RenewedAt)}: {RenewedAt}, " +
                       $"{nameof(ExpiresAt)}: {ExpiresAt}, " +
                       $"{nameof(StartsAt)}: {StartsAt}, " +
                       $"{nameof(IsLifetime)}: {IsLifetime}, " +
                       $"{nameof(ActiveIntroductoryOfferType)}: {ActiveIntroductoryOfferType}, " +
                       $"{nameof(ActivePromotionalOfferType)}: {ActivePromotionalOfferType}, " +
                       $"{nameof(ActivePromotionalOfferId)}: {ActivePromotionalOfferId}, " +
                       $"{nameof(WillRenew)}: {WillRenew}, " +
                       $"{nameof(IsInGracePeriod)}: {IsInGracePeriod}, " +
                       $"{nameof(UnsubscribedAt)}: {UnsubscribedAt}, " +
                       $"{nameof(BillingIssueDetectedAt)}: {BillingIssueDetectedAt}, " +
                       $"{nameof(IsSandbox)}: {IsSandbox}, " +
                       $"{nameof(VendorTransactionId)}: {VendorTransactionId}, " +
                       $"{nameof(VendorOriginalTransactionId)}: {VendorOriginalTransactionId}, " +
                       $"{nameof(CancellationReason)}: {CancellationReason}, " +
                       $"{nameof(IsRefund)}: {IsRefund}";
        }
    }
}