using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
	public static partial class Adapty
	{
		public class NonSubscriptionInfo
		{
			/// The identifier of the purchase in Adapty.
			/// You can use it to ensure that you've already processed this purchase (for example tracking one time products).
			public readonly string PurchaseId;

			/// The identifier of the product in the App Store Connect.
			public readonly string VendorProductId;

			/// The store of the purchase.
			/// The possible values are: app_store, play_store , adapty.
			public readonly string Store;

			/// The time when the product was purchased.
			///
			/// [Nullable]
			public readonly DateTime? PurchasedAt; // nullable

			/// Whether the product should only be processed once.
			/// If true, the purchase will be returned by Adapty API one time only.
			public readonly bool IsOneTime;

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

			/// Whether the purchase was refunded.
			public readonly bool IsRefund;

			internal NonSubscriptionInfo(JSONNode response)
			{
				PurchaseId = response["purchase_id"];
				VendorProductId = response["vendor_product_id"];
				Store = response["store"];
				PurchasedAt = NullableDateTimeFromJSON(response["purchased_at"]);
				IsOneTime = response["is_one_time"];
				IsSandbox = response["is_sandbox"];
				VendorTransactionId = response["vendor_transaction_id"];
				VendorOriginalTransactionId = response["vendor_original_transaction_id"];
				IsRefund = response["is_refund"];
			}

			public override string ToString()
			{
				return $"{nameof(PurchaseId)}: {PurchaseId}, " +
					   $"{nameof(VendorProductId)}: {VendorProductId}, " +
					   $"{nameof(Store)}: {Store}, " +
					   $"{nameof(PurchasedAt)}: {PurchasedAt}, " +
					   $"{nameof(IsOneTime)}: {IsOneTime}, " +
					   $"{nameof(IsSandbox)}: {IsSandbox}, " +
					   $"{nameof(VendorTransactionId)}: {VendorTransactionId}, " +
					   $"{nameof(VendorOriginalTransactionId)}: {VendorOriginalTransactionId}, " +
					   $"{nameof(IsRefund)}: {IsRefund}";
			}
		}

		public static NonSubscriptionInfo NonSubscriptionInfoFromJSON(JSONNode response)
		{
			if (response == null || response.IsNull || !response.IsObject) return null;
			try { 
				return new NonSubscriptionInfo(response);
			}
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding NonSubscriptionInfo: {e} source: {response}");
                return null;
            }
		}
	}
}