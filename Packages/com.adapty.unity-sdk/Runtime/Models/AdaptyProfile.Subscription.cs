using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    public sealed partial class AdaptyProfile
    {
        /// <summary>
        /// One subscription of a profile, as the store and Adapty currently see it.
        /// </summary>
        [DataContract]
        public sealed class Subscription
        {
            private Subscription() { }

            /// <summary>
            /// The store of the purchase. The possible values are: app_store, play_store , adapty.
            /// </summary>
            [DataMember(Name = "store", IsRequired = true)]
            public readonly string Store;

            /// <summary>
            /// The identifier of the product in the App Store Connect.
            /// </summary>
            [DataMember(Name = "vendor_product_id", IsRequired = true)]
            public readonly string VendorProductId;

            /// <summary>
            /// Transaction id from the App Store.
            /// </summary>
            [DataMember(Name = "vendor_transaction_id", IsRequired = true)]
            public readonly string VendorTransactionId;

            /// <summary>
            /// Original transaction id from the App Store.
            /// </summary>
            /**
            * For auto-renewable subscription, this will be the id of the first transaction in the subscription.
            */
            [DataMember(Name = "vendor_original_transaction_id", IsRequired = true)]
            public readonly string VendorOriginalTransactionId;

            /// <summary>
            /// Whether the subscription is active.
            /// </summary>
            [DataMember(Name = "is_active", IsRequired = true)]
            public readonly bool IsActive;

            /// <summary>
            /// Whether the subscription is active for a lifetime (no expiration date).
            /// </summary>
            /**
            * If set to true you shouldn't check expires_at , or you could just check isActive.
            */
            [DataMember(Name = "is_lifetime", IsRequired = true)]
            public readonly bool IsLifetime;

            /// <summary>
            /// The time when the subscription was activated.
            /// </summary>
            [DataMember(Name = "activated_at", IsRequired = true)]
            public readonly DateTime ActivatedAt;

            /// <summary>
            /// The time when the subscription was renewed.
            /// </summary>
            [DataMember(Name = "renewed_at")]
            public readonly DateTime? RenewedAt; // nullable

            /// <summary>
            /// The time when the subscription will expire (could be in the past and could be null for lifetime access).
            /// </summary>
            [DataMember(Name = "expires_at")]
            public readonly DateTime? ExpiresAt; // nullable

            /// <summary>
            /// The time when the subscription has started (could be in the future).
            /// </summary>
            [DataMember(Name = "starts_at")]
            public readonly DateTime? StartsAt; // nullable

            /// <summary>
            /// The time when the auto-renewable subscription was cancelled.
            /// </summary>
            /**
            * Subscription can still be active, it just means that auto-renewal turned off.
            * Will be set to null if the user reactivates the subscription.
            */
            [DataMember(Name = "unsubscribed_at")]
            public readonly DateTime? UnsubscribedAt; // nullable

            /// <summary>
            /// The time when billing issue was detected (Apple was not able to charge the card).
            /// </summary>
            /**
            * Subscription can still be active. Will be set to null if the charge will be made.
            */
            [DataMember(Name = "billing_issue_detected_at")]
            public readonly DateTime? BillingIssueDetectedAt; // nullable

            /// <summary>
            /// Whether the auto-renewable subscription is in the grace period.
            /// </summary>
            [DataMember(Name = "is_in_grace_period", IsRequired = true)]
            public readonly bool IsInGracePeriod;

            /// <summary>
            /// Whether the product was purchased in the sandbox environment.
            /// </summary>
            [DataMember(Name = "is_sandbox", IsRequired = true)]
            public readonly bool IsSandbox;

            /// <summary>
            /// Whether the purchase was refunded.
            /// </summary>
            [DataMember(Name = "is_refund", IsRequired = true)]
            public readonly bool IsRefund;

            /// <summary>
            /// Whether the auto-renewable subscription is set to renew.
            /// </summary>
            [DataMember(Name = "will_renew", IsRequired = true)]
            public readonly bool WillRenew;

            /// <summary>
            /// The type of active introductory offer.
            /// </summary>
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            [DataMember(Name = "active_introductory_offer_type")]
            public readonly string ActiveIntroductoryOfferType; // nullable

            /// <summary>
            /// The type of active promotional offer.
            /// </summary>
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            [DataMember(Name = "active_promotional_offer_type")]
            public readonly string ActivePromotionalOfferType; // nullable

            /// <summary>
            /// The promotional offer in force right now, when one is.
            /// </summary>
            [DataMember(Name = "active_promotional_offer_id")]
            public readonly string ActivePromotionalOfferId; // nullable

            /// <summary>
            /// The offer the current period was bought with, when there was one.
            /// </summary>
            [DataMember(Name = "offer_id")]
            public readonly string OfferId; // nullable

            /// <summary>
            /// The reason why the subscription was cancelled.
            /// </summary>
            /**
            * Possible values are: voluntarily_cancelled, billing_error, refund, price_increase, product_was_not_available, unknown.
            */
            [DataMember(Name = "cancellation_reason")]
            public readonly string CancellationReason; // nullable

            /// <summary>
            /// A description for logs and the debugger. The format is not part of the contract —
            /// read the members rather than parsing it.
            /// </summary>
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