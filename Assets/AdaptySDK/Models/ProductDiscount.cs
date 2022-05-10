using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
	public static partial class Adapty
	{
		public class ProductDiscount
		{
			/// The discount price of the product in the user's local currency.
			public readonly double Price;
			/// An identifier of the discount offer for the product.
			///
			/// [Nullable]
			public readonly string Identifier;
			/// A [Adapty.Period] object that defines the period for the product discount.
			public readonly Period SubscriptionPeriod;
			/// An integer that indicates the number of periods the product discount is available.
			public readonly long NumberOfPeriods;
			/// The payment mode for this product discount.
			public readonly PaymentMode PaymentMode;
			/// The formatted price of the discount for the user's localization.
			///
			/// [Nullable]
			public readonly string LocalizedPrice;
			/// The formatted subscription period of the discount for the user's localization.
			///
			/// [Nullable]
			public readonly string LocalizedSubscriptionPeriod;
			/// The formatted number of periods of the discount for the user's localization.
			///
			/// [Nullable]
			public readonly string LocalizedNumberOfPeriods;

			internal ProductDiscount(JSONNode response)
			{
				Price = response["price"];
				Identifier = response["identifier"];
				SubscriptionPeriod = PeriodFromJSON(response["subscription_period"]);
				NumberOfPeriods = response["number_of_periods"]; 
				PaymentMode = PaymentModeFromJSON(response["payment_mode"]);
				LocalizedPrice = response["localized_price"];
				LocalizedSubscriptionPeriod = response["localized_subscription_period"];
				LocalizedNumberOfPeriods = response["localized_number_of_periods"];
			}

			public override string ToString()
			{
				return $"{nameof(Price)}: {Price}, " +
					   $"{nameof(Identifier)}: {Identifier}, " +
					   $"{nameof(SubscriptionPeriod)}: {SubscriptionPeriod}, " +
					   $"{nameof(NumberOfPeriods)}: {NumberOfPeriods}, " +
					   $"{nameof(PaymentMode)}: {PaymentMode}, " +
					   $"{nameof(LocalizedPrice)}: {LocalizedPrice}, " +
					   $"{nameof(LocalizedSubscriptionPeriod)}: {LocalizedSubscriptionPeriod}, " +
					   $"{nameof(LocalizedNumberOfPeriods)}: {LocalizedNumberOfPeriods}";
			}
		}

		public static ProductDiscount ProductDiscountFromJSON(JSONNode response)
		{
			if (response == null || response.IsNull || !response.IsObject) return null;
			try
			{ 
				return new ProductDiscount(response);
			}
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding ProductDiscount: {e} source: {response}");
                return null;
            }
		}
	}
}