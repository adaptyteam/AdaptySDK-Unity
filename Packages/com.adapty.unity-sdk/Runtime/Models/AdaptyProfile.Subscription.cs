//
//  AdaptyProfile.Subscription.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    public partial class AdaptyProfile
    {
        [DataContract]
        public class Subscription
        {
            private Subscription() { }

            /// The store of the purchase. The possible values are: app_store, play_store , adapty.
            [DataMember(Name = "store", IsRequired = true)]
            public readonly string Store;

            /// The identifier of the product in the App Store Connect.
            [DataMember(Name = "vendor_product_id", IsRequired = true)]
            public readonly string VendorProductId;

            /// Transaction id from the App Store.
            [DataMember(Name = "vendor_transaction_id", IsRequired = true)]
            public readonly string VendorTransactionId;

            /// Original transaction id from the App Store.
            /**
            * For auto-renewable subscription, this will be the id of the first transaction in the subscription.
            */
            [DataMember(Name = "vendor_original_transaction_id", IsRequired = true)]
            public readonly string VendorOriginalTransactionId;

            /// Whether the subscription is active.
            [DataMember(Name = "is_active", IsRequired = true)]
            public readonly bool IsActive;

            /// Whether the subscription is active for a lifetime (no expiration date).
            /**
            * If set to true you shouldn't check expires_at , or you could just check isActive.
            */
            [DataMember(Name = "is_lifetime", IsRequired = true)]
            public readonly bool IsLifetime;

            /// The time when the subscription was activated.
            [DataMember(Name = "activated_at", IsRequired = true)]
            public readonly DateTime ActivatedAt;

            /// The time when the subscription was renewed.
            [DataMember(Name = "renewed_at")]
            public readonly DateTime? RenewedAt; // nullable

            /// The time when the subscription will expire (could be in the past and could be null for lifetime access).
            [DataMember(Name = "expires_at")]
            public readonly DateTime? ExpiresAt; // nullable

            /// The time when the subscription has started (could be in the future).
            [DataMember(Name = "starts_at")]
            public readonly DateTime? StartsAt; // nullable

            /// The time when the auto-renewable subscription was cancelled.
            /**
            * Subscription can still be active, it just means that auto-renewal turned off.
            * Will be set to null if the user reactivates the subscription.
            */
            [DataMember(Name = "unsubscribed_at")]
            public readonly DateTime? UnsubscribedAt; // nullable

            /// The time when billing issue was detected (Apple was not able to charge the card).
            /**
            * Subscription can still be active. Will be set to null if the charge will be made.
            */
            [DataMember(Name = "billing_issue_detected_at")]
            public readonly DateTime? BillingIssueDetectedAt; // nullable

            /// Whether the auto-renewable subscription is in the grace period.
            [DataMember(Name = "is_in_grace_period", IsRequired = true)]
            public readonly bool IsInGracePeriod;

            /// Whether the product was purchased in the sandbox environment.
            [DataMember(Name = "is_sandbox", IsRequired = true)]
            public readonly bool IsSandbox;

            /// Whether the purchase was refunded.
            [DataMember(Name = "is_refund", IsRequired = true)]
            public readonly bool IsRefund;

            /// Whether the auto-renewable subscription is set to renew.
            [DataMember(Name = "will_renew", IsRequired = true)]
            public readonly bool WillRenew;

            /// The type of active introductory offer.
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            [DataMember(Name = "active_introductory_offer_type")]
            public readonly string ActiveIntroductoryOfferType; // nullable

            /// The type of active promotional offer.
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            [DataMember(Name = "active_promotional_offer_type")]
            public readonly string ActivePromotionalOfferType; // nullable

            [DataMember(Name = "active_promotional_offer_id")]
            public readonly string ActivePromotionalOfferId; // nullable

            [DataMember(Name = "offer_id")]
            public readonly string OfferId; // nullable

            /// The reason why the subscription was cancelled.
            /**
            * Possible values are: voluntarily_cancelled, billing_error, refund, price_increase, product_was_not_available, unknown.
            */
            [DataMember(Name = "cancellation_reason")]
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