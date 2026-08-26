using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

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
            /// The identifier of the product in the store it was bought from.
            /// </summary>
            [DataMember(Name = "vendor_product_id", IsRequired = true)]
            public readonly string VendorProductId;

            /// <summary>
            /// The transaction id the store reported.
            /// </summary>
            [DataMember(Name = "vendor_transaction_id", IsRequired = true)]
            public readonly string VendorTransactionId;

            /// <summary>
            /// The original transaction id the store reported.
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
            /// The time when the subscription was renewed. Null when there has been no renewal.
            /// </summary>
            [DataMember(Name = "renewed_at")]
            public readonly DateTime? RenewedAt;

            /// <summary>
            /// The time when the subscription will expire (could be in the past and could be null for lifetime access).
            /// </summary>
            [DataMember(Name = "expires_at")]
            public readonly DateTime? ExpiresAt;

            /// <summary>
            /// The time when the subscription has started (could be in the future). Null when there is
            /// no start date.
            /// </summary>
            [DataMember(Name = "starts_at")]
            public readonly DateTime? StartsAt;

            /// <summary>
            /// The time when the auto-renewable subscription was cancelled.
            /// </summary>
            /**
            * Subscription can still be active, it just means that auto-renewal turned off.
            * Will be set to null if the user reactivates the subscription.
            */
            [DataMember(Name = "unsubscribed_at")]
            public readonly DateTime? UnsubscribedAt;

            /// <summary>
            /// The time a billing issue was detected — the store could not charge the payment method.
            /// </summary>
            /**
            * Subscription can still be active. Will be set to null if the charge will be made.
            */
            [DataMember(Name = "billing_issue_detected_at")]
            public readonly DateTime? BillingIssueDetectedAt;

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
            public readonly string ActiveIntroductoryOfferType;

            /// <summary>
            /// The type of active promotional offer.
            /// </summary>
            /**
            * Possible values are: free_trial, pay_as_you_go, pay_up_front.
            * If the value is not null, it means that the offer was applied during the current subscription period.
            */
            [DataMember(Name = "active_promotional_offer_type")]
            public readonly string ActivePromotionalOfferType;

            /// <summary>
            /// The App Store promotional offer in force right now. Null when there is none, and
            /// for a subscription bought on Android — the profile is one object across platforms,
            /// so this says where the purchase happened, not where the app runs.
            /// </summary>
            [DataMember(Name = "active_promotional_offer_id")]
            public readonly string ActivePromotionalOfferId;

            /// <summary>
            /// The Google Play offer the current period was bought with. Null when there was none,
            /// and for a subscription bought on iOS — see <see cref="ActivePromotionalOfferId"/>.
            /// </summary>
            [DataMember(Name = "offer_id")]
            public readonly string OfferId;

            /// <summary>
            /// The reason why the subscription was cancelled. Null when it was not.
            /// </summary>
            /// <remarks>
            /// The values the native SDKs list: voluntarily_cancelled, billing_error,
            /// price_increase, product_was_not_available, refund, upgraded, unknown. It stays a
            /// string rather than an enum because the contract leaves the set open — do not write
            /// a switch that assumes these are all of them.
            /// </remarks>
            [DataMember(Name = "cancellation_reason")]
            public readonly string CancellationReason;

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
