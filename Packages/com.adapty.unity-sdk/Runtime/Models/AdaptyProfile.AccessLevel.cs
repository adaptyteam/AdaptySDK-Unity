using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    public sealed partial class AdaptyProfile
    {
        /// <summary>
        /// One access level of a profile: whether it is active, what granted it, and until when.
        /// </summary>
        [DataContract]
        public sealed class AccessLevel
        {
            private AccessLevel() { }

            /// <summary>
            /// Unique identifier of the access level configured by you in Adapty Dashboard.
            /// </summary>
            [DataMember(Name = "id", IsRequired = true)]
            public readonly string Id;

            /// <summary>
            /// Whether the access level is active.
            /// </summary>
            /// <remarks>
            /// Generally, you have to check just this property to determine if the user has access to premium features. 
            /// </remarks>
            [DataMember(Name = "is_active", IsRequired = true)]
            public readonly bool IsActive;

            /// <summary>
            /// The identifier of the product in the App Store Connect that unlocked this access level.
            /// </summary>
            [DataMember(Name = "vendor_product_id", IsRequired = true)]
            public readonly string VendorProductId;

            /// <summary>
            /// The store of the purchase that unlocked this access level.
            /// </summary>
            /// <remarks>
            /// The possible values are: app_store, play_store, adapty.
            /// </remarks>
            [DataMember(Name = "store", IsRequired = true)]
            public readonly string Store;

            /// <summary>
            /// The time when the access level was activated.
            /// </summary>
            [DataMember(Name = "activated_at", IsRequired = true)]
            public readonly DateTime ActivatedAt;

            /// <summary>
            /// The time when the access level was renewed.
            /// </summary>
            [DataMember(Name = "renewed_at")]
            public readonly DateTime? RenewedAt; // nullable

            /// <summary>
            /// The time when the access level will expire (could be in the past and could be null for lifetime access).
            /// </summary>
            [DataMember(Name = "expires_at")]
            public readonly DateTime? ExpiresAt; // nullable

            /// <summary>
            /// Whether the access level is active for a lifetime (no expiration date).
            /// </summary>
            /// <remarks>
            /// If set to true you shouldn't check expires_at , or you could just check isActive. 
            /// </remarks>
            [DataMember(Name = "is_lifetime", IsRequired = true)]
            public readonly bool IsLifetime;

            /// <summary>
            /// The type of active introductory offer.
            /// </summary>
            /// <remarks>
            /// Possible values are: free_trial, pay_as_you_go, pay_up_front. 
            /// If the value is not null, it means that the offer was applied during the current subscription period. 
            /// </remarks>
            [DataMember(Name = "active_introductory_offer_type")]
            public readonly string ActiveIntroductoryOfferType; // nullable

            /// <summary>
            /// The type of active promotional offer.
            /// </summary>
            /// <remarks>
            /// Possible values are: free_trial, pay_as_you_go, pay_up_front.
            /// If the value is not null, it means that the offer was applied during the current subscription period.
            /// </remarks>
            [DataMember(Name = "active_promotional_offer_type")]
            public readonly string ActivePromotionalOfferType; // nullable

            /// <summary>
            /// An identifier of active promotional offer.
            /// </summary>
            [DataMember(Name = "active_promotional_offer_id")]
            public readonly string ActivePromotionalOfferId; // nullable

            /// <summary>
            /// The offer the purchase behind this access level used, when it used one.
            /// </summary>
            [DataMember(Name = "offer_id")]
            public readonly string OfferId; // nullable

            /// <summary>
            /// Whether the auto-renewable subscription is set to renew.
            /// </summary>
            [DataMember(Name = "will_renew", IsRequired = true)]
            public readonly bool WillRenew;

            /// <summary>
            /// Whether the auto-renewable subscription is in the grace period.
            /// </summary>
            [DataMember(Name = "is_in_grace_period", IsRequired = true)]
            public readonly bool IsInGracePeriod;

            /// <summary>
            /// The time when the auto-renewable subscription was cancelled.
            /// </summary>
            /// <remarks>
            /// Subscription can still be active, it just means that auto-renewal turned off.
            /// Will be set to null if the user reactivates the subscription. 
            /// </remarks>
            [DataMember(Name = "unsubscribed_at")]
            public readonly DateTime? UnsubscribedAt; // nullable

            /// <summary>
            /// The time when billing issue was detected (Apple was not able to charge the card).
            /// </summary>
            /// <remarks>
            /// Subscription can still be active. Will be set to null if the charge will be made. 
            /// </remarks>
            [DataMember(Name = "billing_issue_detected_at")]
            public readonly DateTime? BillingIssueDetectedAt; // nullable


            /// <summary>
            /// The time when the access level has started (could be in the future).
            /// </summary>
            [DataMember(Name = "starts_at")]
            public readonly DateTime? StartsAt; // nullable

            /// <summary>
            /// The reason why the subscription was cancelled.
            /// </summary>
            /// <remarks>
            /// Possible values are: voluntarily_cancelled, billing_error, refund, price_increase, product_was_not_available, unknown.
            /// </remarks>
            [DataMember(Name = "cancellation_reason")]
            public readonly string CancellationReason; // nullable

            /// <summary>
            /// Whether the purchase was refunded.
            /// </summary>
            [DataMember(Name = "is_refund", IsRequired = true)]
            public readonly bool IsRefund;

            /// <summary>
            /// A description for logs and the debugger. The format is not part of the contract —
            /// read the members rather than parsing it.
            /// </summary>
            public override string ToString() => $"{nameof(Id)}: {Id}, " +
                       $"{nameof(IsActive)}: {IsActive}, " +
                       $"{nameof(VendorProductId)}: {VendorProductId}, " +
                       $"{nameof(Store)}: {Store}, " +
                       $"{nameof(ActivatedAt)}: {ActivatedAt}, " +
                       $"{nameof(RenewedAt)}: {RenewedAt}, " +
                       $"{nameof(ExpiresAt)}: {ExpiresAt}, " +
                       $"{nameof(IsLifetime)}: {IsLifetime}, " +
                       $"{nameof(ActiveIntroductoryOfferType)}: {ActiveIntroductoryOfferType}, " +
                       $"{nameof(ActivePromotionalOfferType)}: {ActivePromotionalOfferType}, " +
                       $"{nameof(ActivePromotionalOfferId)}: {ActivePromotionalOfferId}, " +
                       $"{nameof(OfferId)}: {OfferId}, " +
                       $"{nameof(WillRenew)}: {WillRenew}, " +
                       $"{nameof(IsInGracePeriod)}: {IsInGracePeriod}, " +
                       $"{nameof(UnsubscribedAt)}: {UnsubscribedAt}, " +
                       $"{nameof(BillingIssueDetectedAt)}: {BillingIssueDetectedAt}, " +
                       $"{nameof(StartsAt)}: {StartsAt}, " +
                       $"{nameof(CancellationReason)}: {CancellationReason}, " +
                       $"{nameof(IsRefund)}: {IsRefund}";
        }
    }
}