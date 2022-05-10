using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
	public static partial class Adapty
	{
		public class SubscriptionInfo
		{
			/// Whether the subscription is active.
			public readonly bool IsActive;

			/// The identifier of the product in the App Store Connect.
			public readonly string VendorProductId;

			/// The store of the purchase. The possible values are: app_store, play_store , adapty.
			public readonly string Store;

			/// The time when the subscription was activated.
			///
			/// [Nullable]
			public readonly DateTime? ActivatedAt; // nullable

			/// The time when the subscription was renewed.
			///
			/// [Nullable]
			public readonly DateTime? RenewedAt; // nullable

			/// The time when the subscription will expire (could be in the past and could be null for lifetime access).
			///
			/// [Nullable]
			public readonly DateTime? ExpiresAt; // nullable

			/// The time when the subscription has started (could be in the future).
			///
			/// [Nullable]
			public readonly DateTime? StartsAt; // nullable

			/// Whether the subscription is active for a lifetime (no expiration date).
			/// If set to true you shouldn't check expires_at , or you could just check isActive.
			public readonly bool IsLifetime;

			/// The type of active introductory offer.
			/// Possible values are: free_trial, pay_as_you_go, pay_up_front.
			/// If the value is not null, it means that the offer was applied during the current subscription period.
			///
			/// [Nullable]
			public readonly string ActiveIntroductoryOfferType; // nullable

			/// The type of active promotional offer.
			/// Possible values are: free_trial, pay_as_you_go, pay_up_front.
			/// If the value is not null, it means that the offer was applied during the current subscription period.
			///
			/// [Nullable]
			public readonly string ActivePromotionalOfferType; // nullable

			/// Whether the auto-renewable subscription is set to renew.
			public readonly bool WillRenew;

			/// Whether the auto-renewable subscription is in the grace period.
			public readonly bool IsInGracePeriod;

			/// The time when the auto-renewable subscription was cancelled.
			/// Subscription can still be active, it just means that auto-renewal turned off.
			/// Will be set to null if the user reactivates the subscription.
			///
			/// [Nullable]
			public readonly DateTime? UnsubscribedAt; // nullable

			/// The time when billing issue was detected (Apple was not able to charge the card).
			/// Subscription can still be active. Will be set to null if the charge will be made.
			///
			/// [Nullable]
			public readonly DateTime? BillingIssueDetectedAt; // nullable

			/// Whether the product was purchased in the sandbox environment.
			public readonly bool IsSandbox;

			/// Transaction id from the App Store.
			///
			/// [Nullable]
			public readonly string VendorTransactionId; // nullable

			/// Original transaction id from the App Store.
			/// For auto-renewable subscription, this will be the id of the first transaction in the subscription.
			///
			/// [Nullable]
			public readonly string VendorOriginalTransactionId; // nullable

			/// The reason why the subscription was cancelled.
			/// Possible values are: voluntarily_cancelled, billing_error, refund, price_increase, product_was_not_available, unknown.
			///
			/// [Nullable]
			public readonly string CancellationReason; // nullable

			/// Whether the purchase was refunded.
			public readonly bool IsRefund;

			internal SubscriptionInfo(JSONNode response)
			{
				IsActive = response["is_active"];
				VendorProductId = response["vendor_product_id"];
				Store = response["store"];
				ActivatedAt = NullableDateTimeFromJSON(response["activated_at"]);
				RenewedAt = NullableDateTimeFromJSON(response["renewed_at"]);
				ExpiresAt = NullableDateTimeFromJSON(response["expires_at"]);
				StartsAt = NullableDateTimeFromJSON(response["starts_at"]);
				IsLifetime = response["is_lifetime"];
				ActiveIntroductoryOfferType = response["active_introductory_offer_type"];
				ActivePromotionalOfferType = response["active_promotional_offer_type"];
				WillRenew = response["will_renew"];
				IsInGracePeriod = response["is_in_grace_period"];
				UnsubscribedAt = NullableDateTimeFromJSON(response["unsubscribed_at"]);
				BillingIssueDetectedAt = NullableDateTimeFromJSON(response["billing_issue_detected_at"]);
				IsSandbox = response["is_sandbox"];
				VendorTransactionId = response["vendor_transaction_id"];
				VendorOriginalTransactionId = response["vendor_original_transaction_id"];
				CancellationReason = response["cancellation_reason"];
				IsRefund = response["is_refund"];
			}

			public override string ToString()
			{
				return $"{nameof(IsActive)}: {IsActive}, " +
					   $"{nameof(VendorProductId)}: {VendorProductId}, " +
					   $"{nameof(Store)}: {Store}, " +
					   $"{nameof(ActivatedAt)}: {ActivatedAt}, " +
					   $"{nameof(RenewedAt)}: {RenewedAt}, " +
					   $"{nameof(ExpiresAt)}: {ExpiresAt}, " +
					   $"{nameof(StartsAt)}: {StartsAt}, " +
					   $"{nameof(IsLifetime)}: {IsLifetime}, " +
					   $"{nameof(ActiveIntroductoryOfferType)}: {ActiveIntroductoryOfferType}, " +
					   $"{nameof(ActivePromotionalOfferType)}: {ActivePromotionalOfferType}, " +
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

		public static SubscriptionInfo SubscriptionInfoFromJSON(JSONNode response)
		{
			if (response == null || response.IsNull || !response.IsObject) return null;
			try { 
				return new SubscriptionInfo(response);
			}
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding SubscriptionInfo: {e} source: {response}");
                return null;
            }
		}
	}
}