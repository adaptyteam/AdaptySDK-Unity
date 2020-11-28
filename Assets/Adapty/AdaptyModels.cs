using System.Collections.Generic;
using System;

namespace AdaptySDK {
	public enum LogLevel {
		None,
		Error,
		Verbose
	}

	public enum DataState {
		Cached,
		Synced,
		Unknown
	}

	public enum PurchaseType {
		Inapp,
		Subscription,
		Unknown
	}

	public enum AttributionNetwork {
		Appsflyer,
		Adjust,
		Branch,
		Custom,
		Unknown
	}

	public enum PeriodUnit {
		Day,
		Week,
		Month,
		Year,
		Unknown
	}

	public enum PaymentMode {
		FreeTrial,
		PayAsYouGo,
		PayUpFront,
		Unknown
	}

	public enum Gender {
		Female,
		Male,
		Other,
		Unknown
	}

	public class AdaptyError {
		public string message;
		public long code;
	}

	public class ProductSubscriptionPeriodModel {
		public PeriodUnit unit;
		public long numberOfUnits;
	}

	public class ProductDiscountModel {
		public decimal price;
		public string identifier;
		public ProductSubscriptionPeriodModel subscriptionPeriod;
		public long numberOfPeriods;
		public PaymentMode paymentMode;
		public string localizedPrice;
		public string localizedSubscriptionPeriod;
		public string localizedNumberOfPeriods;
	}

	public class ProductModel {
		public string vendorProductId;
		public bool introductoryOfferEligibility = false;
		public bool promotionalOfferEligibility = false;
		public string promotionalOfferId;
		public string localizedDescription = "";
		public string localizedTitle = "";
		public decimal price = 0;
		public string currencyCode;
		public string currencySymbol;
		public string regionCode;
		public ProductSubscriptionPeriodModel subscriptionPeriod;
		public ProductDiscountModel introductoryDiscount;
		public string subscriptionGroupIdentifier;
		public ProductDiscountModel[] discounts;
		public string localizedPrice;
		public string localizedSubscriptionPeriod;
		public string skuId;
	}

	public class PaywallModel {
		public string developerId;
		public string variationId;
		public long revision = 0;
		public bool isPromo = false;
		public ProductModel[] products;
		public string visualPaywall = "";
		public Dictionary<string, object> customPayload;
	}

	public class PromoModel {
		public string promoType;
		public string variationId;
		public DateTime expiresAt;
		public PaywallModel paywall;
	}

	public class AccessLevelInfoModel {
		public string id;
		public bool isActive;
		public string vendorProductId;
		public string store;
		public DateTime activatedAt;
		public DateTime renewedAt;
		public DateTime expiresAt;
		public bool isLifetime;
		public string activeIntroductoryOfferType;
		public string activePromotionalOfferType;
		public bool willRenew;
		public bool isInGracePeriod;
		public DateTime unsubscribedAt;
		public DateTime billingIssueDetectedAt;
		public string vendorTransactionId;
		public string vendorOriginalTransactionId;
		public DateTime startsAt;
		public string cancellationReason;
		public bool isRefund;
	}

	public class SubscriptionInfoModel {
		public bool isActive;
		public string vendorProductId;
		public string store;
		public DateTime activatedAt;
		public DateTime renewedAt;
		public DateTime expiresAt;
		public DateTime startsAt;
		public bool isLifetime;
		public string activeIntroductoryOfferType;
		public string activePromotionalOfferType;
		public bool willRenew;
		public bool isInGracePeriod;
		public DateTime unsubscribedAt;
		public DateTime billingIssueDetectedAt;
		public bool isSandbox;
		public string vendorTransactionId;
		public string vendorOriginalTransactionId;
		public string cancellationReason;
		public bool isRefund;
	}

	public class NonSubscriptionInfoModel {
		public string purchaseId;
		public string vendorProductId;
		public string store;
		public DateTime purchasedAt;
		public bool isOneTime;
		public bool isSandbox;
		public string vendorTransactionId;
		public string vendorOriginalTransactionId;
		public bool isRefund;
	}

	public class PurchaserInfoModel {
		public string customerUserId;
		public Dictionary<string, AccessLevelInfoModel> accessLevels;
		public Dictionary<string, SubscriptionInfoModel> subscriptions;
		public Dictionary<string, NonSubscriptionInfoModel> nonSubscriptions;
	}

	public class ProfileParameterBuilder {
		private Dictionary<string, object> values = new Dictionary<string, object>();
		public Dictionary<string, object> getDictionary() {
			return values;
		}
		public ProfileParameterBuilder withEmail(string email) {
			values.Add("email", email);
			return this;
		}
		public ProfileParameterBuilder withPhoneNumber(string phoneNumber) {
			values.Add("phoneNumber", phoneNumber);
			return this;
		}
		public ProfileParameterBuilder withFacebookUserId(string facebookUserId) {
			values.Add("facebookUserId", facebookUserId);
			return this;
		}
		public ProfileParameterBuilder withAmplitudeUserId(string amplitudeUserId) {
			values.Add("amplitudeUserId", amplitudeUserId);
			return this;
		}
		public ProfileParameterBuilder withAmplitudeDeviceId(string amplitudeDeviceId) {
			values.Add("amplitudeDeviceId", amplitudeDeviceId);
			return this;
		}
		public ProfileParameterBuilder withMixpanelUserId(string mixpanelUserId) {
			values.Add("mixpanelUserId", mixpanelUserId);
			return this;
		}
		public ProfileParameterBuilder withAppmetricaProfileId(string appmetricaProfileId) {
			values.Add("appmetricaProfileId", appmetricaProfileId);
			return this;
		}
		public ProfileParameterBuilder withAppmetricaDeviceId(string appmetricaDeviceId) {
			values.Add("appmetricaDeviceId", appmetricaDeviceId);
			return this;
		}
		public ProfileParameterBuilder withFirstName(string firstName) {
			values.Add("firstName", firstName);
			return this;
		}
		public ProfileParameterBuilder withLastName(string lastName) {
			values.Add("lastName", lastName);
			return this;
		}
		public ProfileParameterBuilder withGender(Gender gender) {
			values.Add("gender", gender.ToString("g").ToLower());
			return this;
		}
		// YYYY-MM-DD
		public ProfileParameterBuilder withBirthday(string birthday) {
			values.Add("birthday", birthday);
			return this;
		}
		public ProfileParameterBuilder withAppTrackingTransparencyStatus(uint appTrackingTransparencyStatus) {
			values.Add("appTrackingTransparencyStatus", appTrackingTransparencyStatus);
			return this;
		}
		public ProfileParameterBuilder withCustomAttributes(Dictionary<string, object> customAttributes) {
			values.Add("customAttributes", customAttributes);
			return this;
		}
	}

	class Callback {
		public string objectName;
		public string method;
		public string message;
	}
}