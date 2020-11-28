using System.Collections.Generic;
using UnityEngine;

namespace AdaptySDK {
	public class Adapty {
		public static readonly string pluginVersion = "1.0.0";

		static DataState stringToDataState(string state) {
			switch (state) {
				case "cached":
					return DataState.Cached;
				case "synced":
					return DataState.Synced;
				default:
					return DataState.Unknown;
			}
		}

		static PeriodUnit stringToPeriodUnit(string unit) {
			switch (unit) {
				case "day":
					return PeriodUnit.Day;
				case "week":
					return PeriodUnit.Week;
				case "month":
					return PeriodUnit.Month;
				case "year":
					return PeriodUnit.Year;
				default:
					return PeriodUnit.Unknown;
			}
		}

		static string periodUnitToString(PeriodUnit unit){
			switch (unit) {
				case PeriodUnit.Day:
					return "day";
				case PeriodUnit.Week:
					return "week";
				case PeriodUnit.Month:
					return "month";
				case PeriodUnit.Year:
					return "year";
				default:
					return "unknown";
			}
		}

		static PaymentMode stringToPaymentMode(string mode) {
			switch (mode) {
				case "free_trial":
					return PaymentMode.FreeTrial;
				case "pay_as_you_go":
					return PaymentMode.PayAsYouGo;
				case "pay_up_front":
					return PaymentMode.PayUpFront;
				default:
					return PaymentMode.Unknown;
			}
		}

		static System.DateTime stringToDateTime(string date) {
			return System.DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
		}

		static AdaptyError dictionaryToAdaptyError(Dictionary<string, object> dictionary) {
			AdaptyError obj = new AdaptyError();
			if (dictionary.ContainsKey("message")) {
				obj.message = (string)dictionary["message"];
			}
			if (dictionary.ContainsKey("code")) {
				obj.code = (long)dictionary["code"];
			}
			return obj;
		}

		static PaywallModel dictionaryToPaywallModel(Dictionary<string, object> dictionary) {
			PaywallModel obj = new PaywallModel();
			if (dictionary.ContainsKey("developerId")) {
				obj.developerId = (string)dictionary["developerId"];
			}
			if (dictionary.ContainsKey("variationId")) {
				obj.variationId = (string)dictionary["variationId"];
			}
			if (dictionary.ContainsKey("revision")) {
				obj.revision = (long)dictionary["revision"];
			}
			if (dictionary.ContainsKey("isPromo")) {
				obj.isPromo = (bool)dictionary["isPromo"];
			}
			if (dictionary.ContainsKey("products")) {
				List<object> products = (List<object>)dictionary["products"];
				List<ProductModel> array = new List<ProductModel>();
				foreach (object product in products) {
					array.Add(dictionaryToProductModel((Dictionary<string, object>)product));
				}
				obj.products = array.ToArray();
			}
			if (dictionary.ContainsKey("visualPaywall")) {
				obj.visualPaywall = (string)dictionary["visualPaywall"];
			}
			if (dictionary.ContainsKey("customPayload")) {
				obj.customPayload = (Dictionary<string, object>)dictionary["customPayload"];
			}
			return obj;
		}

		static ProductSubscriptionPeriodModel dictionaryToProductSubscriptionPeriodModel(Dictionary<string, object> dictionary) {
			ProductSubscriptionPeriodModel obj = new ProductSubscriptionPeriodModel();
			if (dictionary.ContainsKey("unit")) {
				obj.unit = stringToPeriodUnit((string)dictionary["unit"]);
			}
			if (dictionary.ContainsKey("numberOfUnits")) {
				obj.numberOfUnits = (long)dictionary["numberOfUnits"];
			}
			return obj;
		}

		static ProductDiscountModel dictionaryToProductDiscountModel(Dictionary<string, object> dictionary) {
			ProductDiscountModel obj = new ProductDiscountModel();
			if (dictionary.ContainsKey("price")) {
				obj.price = decimal.Parse((string)dictionary["price"]);
			}
			if (dictionary.ContainsKey("identifier")) {
				obj.identifier = (string)dictionary["identifier"];
			}
			if (dictionary.ContainsKey("subscriptionPeriod")) {
				obj.subscriptionPeriod = dictionaryToProductSubscriptionPeriodModel((Dictionary<string, object>)dictionary["subscriptionPeriod"]);
			}
			if (dictionary.ContainsKey("numberOfPeriods")) {
				obj.numberOfPeriods = (long)dictionary["numberOfPeriods"];
			}
			if (dictionary.ContainsKey("paymentMode")) {
				obj.paymentMode = stringToPaymentMode((string)dictionary["paymentMode"]);
			}
			if (dictionary.ContainsKey("localizedPrice")) {
				obj.localizedPrice = (string)dictionary["localizedPrice"];
			}
			if (dictionary.ContainsKey("localizedSubscriptionPeriod")) {
				obj.localizedSubscriptionPeriod = (string)dictionary["localizedSubscriptionPeriod"];
			}
			if (dictionary.ContainsKey("localizedNumberOfPeriods")) {
				obj.localizedNumberOfPeriods = (string)dictionary["localizedNumberOfPeriods"];
			}
			return obj;
		}

		static AccessLevelInfoModel dictionaryToAccessLevelInfoModel(Dictionary<string, object> dictionary) {
			AccessLevelInfoModel obj = new AccessLevelInfoModel();
			if (dictionary.ContainsKey("id")) {
				obj.id = (string)dictionary["id"];
			}
			if (dictionary.ContainsKey("isActive")) {
				obj.isActive = (bool)dictionary["isActive"];
			}
			if (dictionary.ContainsKey("vendorProductId")) {
				obj.vendorProductId = (string)dictionary["vendorProductId"];
			}
			if (dictionary.ContainsKey("store")) {
				obj.store = (string)dictionary["store"];
			}
			if (dictionary.ContainsKey("activatedAt")) {
				obj.activatedAt = stringToDateTime((string)dictionary["activatedAt"]);
			}
			if (dictionary.ContainsKey("renewedAt")) {
				obj.renewedAt = stringToDateTime((string)dictionary["renewedAt"]);
			}
			if (dictionary.ContainsKey("expiresAt")) {
				obj.expiresAt = stringToDateTime((string)dictionary["expiresAt"]);
			}
			if (dictionary.ContainsKey("isLifetime")) {
				obj.isLifetime = (bool)dictionary["isLifetime"];
			}
			if (dictionary.ContainsKey("activeIntroductoryOfferType")) {
				obj.activeIntroductoryOfferType = (string)dictionary["activeIntroductoryOfferType"];
			}
			if (dictionary.ContainsKey("activePromotionalOfferType")) {
				obj.activePromotionalOfferType = (string)dictionary["activePromotionalOfferType"];
			}
			if (dictionary.ContainsKey("willRenew")) {
				obj.willRenew = (bool)dictionary["willRenew"];
			}
			if (dictionary.ContainsKey("isInGracePeriod")) {
				obj.isInGracePeriod = (bool)dictionary["isInGracePeriod"];
			}
			if (dictionary.ContainsKey("unsubscribedAt")) {
				obj.unsubscribedAt = stringToDateTime((string)dictionary["unsubscribedAt"]);
			}
			if (dictionary.ContainsKey("billingIssueDetectedAt")) {
				obj.billingIssueDetectedAt = stringToDateTime((string)dictionary["billingIssueDetectedAt"]);
			}
			if (dictionary.ContainsKey("vendorTransactionId")) {
				obj.vendorTransactionId = (string)dictionary["vendorTransactionId"];
			}
			if (dictionary.ContainsKey("vendorOriginalTransactionId")) {
				obj.vendorOriginalTransactionId = (string)dictionary["vendorOriginalTransactionId"];
			}
			if (dictionary.ContainsKey("startsAt")) {
				obj.startsAt = stringToDateTime((string)dictionary["startsAt"]);
			}
			if (dictionary.ContainsKey("cancellationReason")) {
				obj.cancellationReason = (string)dictionary["cancellationReason"];
			}
			if (dictionary.ContainsKey("isRefund")) {
				obj.isRefund = (bool)dictionary["isRefund"];
			}
			return obj;
		}

		static SubscriptionInfoModel dictionaryToSubscriptionInfoModel(Dictionary<string, object> dictionary) {
			SubscriptionInfoModel obj = new SubscriptionInfoModel();
			if (dictionary.ContainsKey("isActive")) {
				obj.isActive = (bool)dictionary["isActive"];
			}
			if (dictionary.ContainsKey("vendorProductId")) {
				obj.vendorProductId = (string)dictionary["vendorProductId"];
			}
			if (dictionary.ContainsKey("store")) {
				obj.store = (string)dictionary["store"];
			}
			if (dictionary.ContainsKey("activatedAt")) {
				obj.activatedAt = stringToDateTime((string)dictionary["activatedAt"]);
			}
			if (dictionary.ContainsKey("renewedAt")) {
				obj.renewedAt = stringToDateTime((string)dictionary["renewedAt"]);
			}
			if (dictionary.ContainsKey("expiresAt")) {
				obj.expiresAt = stringToDateTime((string)dictionary["expiresAt"]);
			}
			if (dictionary.ContainsKey("startsAt")) {
				obj.startsAt = stringToDateTime((string)dictionary["startsAt"]);
			}
			if (dictionary.ContainsKey("isLifetime")) {
				obj.isLifetime = (bool)dictionary["isLifetime"];
			}
			if (dictionary.ContainsKey("activeIntroductoryOfferType")) {
				obj.activeIntroductoryOfferType = (string)dictionary["activeIntroductoryOfferType"];
			}
			if (dictionary.ContainsKey("activePromotionalOfferType")) {
				obj.activePromotionalOfferType = (string)dictionary["activePromotionalOfferType"];
			}
			if (dictionary.ContainsKey("willRenew")) {
				obj.willRenew = (bool)dictionary["willRenew"];
			}
			if (dictionary.ContainsKey("isInGracePeriod")) {
				obj.isInGracePeriod = (bool)dictionary["isInGracePeriod"];
			}
			if (dictionary.ContainsKey("unsubscribedAt")) {
				obj.unsubscribedAt = stringToDateTime((string)dictionary["unsubscribedAt"]);
			}
			if (dictionary.ContainsKey("billingIssueDetectedAt")) {
				obj.billingIssueDetectedAt = stringToDateTime((string)dictionary["billingIssueDetectedAt"]);
			}
			if (dictionary.ContainsKey("isSandbox")) {
				obj.isSandbox = (bool)dictionary["isSandbox"];
			}
			if (dictionary.ContainsKey("vendorTransactionId")) {
				obj.vendorTransactionId = (string)dictionary["vendorTransactionId"];
			}
			if (dictionary.ContainsKey("vendorOriginalTransactionId")) {
				obj.vendorOriginalTransactionId = (string)dictionary["vendorOriginalTransactionId"];
			}
			if (dictionary.ContainsKey("cancellationReason")) {
				obj.cancellationReason = (string)dictionary["cancellationReason"];
			}
			if (dictionary.ContainsKey("isRefund")) {
				obj.isRefund = (bool)dictionary["isRefund"];
			}
			return obj;
		}

		static NonSubscriptionInfoModel dictionaryToNonSubscriptionInfoModel(Dictionary<string, object> dictionary) {
			NonSubscriptionInfoModel obj = new NonSubscriptionInfoModel();
			if (dictionary.ContainsKey("purchaseId")) {
				obj.purchaseId = (string)dictionary["purchaseId"];
			}
			if (dictionary.ContainsKey("vendorProductId")) {
				obj.vendorProductId = (string)dictionary["vendorProductId"];
			}
			if (dictionary.ContainsKey("store")) {
				obj.store = (string)dictionary["store"];
			}
			if (dictionary.ContainsKey("purchasedAt")) {
				obj.purchasedAt = stringToDateTime((string)dictionary["purchasedAt"]);
			}
			if (dictionary.ContainsKey("isOneTime")) {
				obj.isOneTime = (bool)dictionary["isOneTime"];
			}
			if (dictionary.ContainsKey("isSandbox")) {
				obj.isSandbox = (bool)dictionary["isSandbox"];
			}
			if (dictionary.ContainsKey("vendorTransactionId")) {
				obj.vendorTransactionId = (string)dictionary["vendorTransactionId"];
			}
			if (dictionary.ContainsKey("vendorOriginalTransactionId")) {
				obj.vendorOriginalTransactionId = (string)dictionary["vendorOriginalTransactionId"];
			}
			if (dictionary.ContainsKey("isRefund")) {
				obj.isRefund = (bool)dictionary["isRefund"];
			}
			return obj;
		}

		static ProductModel dictionaryToProductModel(Dictionary<string, object> dictionary) {
			ProductModel obj = new ProductModel();
			if (dictionary.ContainsKey("vendorProductId")) {
				obj.vendorProductId = (string)dictionary["vendorProductId"];
			}
			if (dictionary.ContainsKey("introductoryOfferEligibility")) {
				obj.introductoryOfferEligibility = (bool)dictionary["introductoryOfferEligibility"];
			}
			if (dictionary.ContainsKey("promotionalOfferEligibility")) {
				obj.promotionalOfferEligibility = (bool)dictionary["promotionalOfferEligibility"];
			}
			if (dictionary.ContainsKey("promotionalOfferId")) {
				obj.promotionalOfferId = (string)dictionary["promotionalOfferId"];
			}
			if (dictionary.ContainsKey("localizedDescription")) {
				obj.localizedDescription = (string)dictionary["localizedDescription"];
			}
			if (dictionary.ContainsKey("localizedTitle")) {
				obj.localizedTitle = (string)dictionary["localizedTitle"];
			}
			if (dictionary.ContainsKey("price")) {
				obj.price = decimal.Parse((string)dictionary["price"]);
			}
			if (dictionary.ContainsKey("currencyCode")) {
				obj.currencyCode = (string)dictionary["currencyCode"];
			}
			if (dictionary.ContainsKey("currencySymbol")) {
				obj.currencySymbol = (string)dictionary["currencySymbol"];
			}
			if (dictionary.ContainsKey("regionCode")) {
				obj.regionCode = (string)dictionary["regionCode"];
			}
			if (dictionary.ContainsKey("subscriptionPeriod")) {
				obj.subscriptionPeriod = dictionaryToProductSubscriptionPeriodModel((Dictionary<string, object>)dictionary["subscriptionPeriod"]);
			}
			if (dictionary.ContainsKey("introductoryDiscount")) {
				obj.introductoryDiscount = dictionaryToProductDiscountModel((Dictionary<string, object>)dictionary["introductoryDiscount"]);
			}
			if (dictionary.ContainsKey("subscriptionGroupIdentifier")) {
				obj.subscriptionGroupIdentifier = (string)dictionary["subscriptionGroupIdentifier"];
			}
			if (dictionary.ContainsKey("discounts")) {
				List<object> discounts = (List<object>)dictionary["discounts"];
				List<ProductDiscountModel> array = new List<ProductDiscountModel>();
				foreach (object discount in discounts) {
					array.Add(dictionaryToProductDiscountModel((Dictionary<string, object>)discount));
				}
				obj.discounts = array.ToArray();
			}
			if (dictionary.ContainsKey("localizedPrice")) {
				obj.localizedPrice = (string)dictionary["localizedPrice"];
			}
			if (dictionary.ContainsKey("localizedSubscriptionPeriod")) {
				obj.localizedSubscriptionPeriod = (string)dictionary["localizedSubscriptionPeriod"];
			}
			if (dictionary.ContainsKey("skuId")) {
				obj.skuId = (string)dictionary["skuId"];
			}
			return obj;
		}

		static PurchaserInfoModel dictionaryToPurchaserInfoModel(Dictionary<string, object> dictionary) {
			PurchaserInfoModel obj = new PurchaserInfoModel();
			if (dictionary.ContainsKey("customerUserId")) {
				obj.customerUserId = (string)dictionary["customerUserId"];
			}
			if (dictionary.ContainsKey("accessLevels")) {
				Dictionary<string, object> accessLevels = (Dictionary<string, object>)dictionary["accessLevels"];
				Dictionary<string, AccessLevelInfoModel> converted_dictionary = new Dictionary<string, AccessLevelInfoModel>();
				foreach (KeyValuePair<string, object> item in accessLevels) {
					converted_dictionary.Add(item.Key, dictionaryToAccessLevelInfoModel((Dictionary<string, object>)item.Value));
				}
				obj.accessLevels = converted_dictionary;
			}
			if (dictionary.ContainsKey("subscriptions")) {
				Dictionary<string, object> subscriptions = (Dictionary<string, object>)dictionary["subscriptions"];
				Dictionary<string, SubscriptionInfoModel> converted_dictionary = new Dictionary<string, SubscriptionInfoModel>();
				foreach (KeyValuePair<string, object> item in subscriptions) {
					converted_dictionary.Add(item.Key, dictionaryToSubscriptionInfoModel((Dictionary<string, object>)item.Value));
				}
				obj.subscriptions = converted_dictionary;
			}
			if (dictionary.ContainsKey("nonSubscriptions")) {
				Dictionary<string, object> nonSubscriptions = (Dictionary<string, object>)dictionary["nonSubscriptions"];
				Dictionary<string, NonSubscriptionInfoModel> converted_dictionary = new Dictionary<string, NonSubscriptionInfoModel>();
				foreach (KeyValuePair<string,object> item in nonSubscriptions) {
					converted_dictionary.Add(item.Key, dictionaryToNonSubscriptionInfoModel((Dictionary<string, object>)item.Value));
				}
				obj.nonSubscriptions = converted_dictionary;
			}
			return obj;
		}

		static PromoModel dictionaryToPromoModel(Dictionary<string, object> dictionary) {
			PromoModel obj = new PromoModel();
			if (dictionary.ContainsKey("promoType")) {
				obj.promoType = (string)dictionary["promoType"];
			}
			if (dictionary.ContainsKey("variationId")) {
				obj.variationId = (string)dictionary["variationId"];
			}
			if (dictionary.ContainsKey("expiresAt")) {
				obj.expiresAt = stringToDateTime((string)dictionary["expiresAt"]);
			}
			if (dictionary.ContainsKey("paywall")) {
				obj.paywall = dictionaryToPaywallModel((Dictionary<string, object>)dictionary["paywall"]);
			}
			return obj;
		}

		static Dictionary<string, object> subscriptionPeriodToDictionary(ProductSubscriptionPeriodModel subscriptionPeriod) {
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("unit", periodUnitToString(subscriptionPeriod.unit));
			dictionary.Add("numberOfUnits", subscriptionPeriod.numberOfUnits);
			return dictionary;
		}

		static Dictionary<string, object> discountToDictionary(ProductDiscountModel discount) {
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("price", discount.price.ToString());
			dictionary.Add("numberOfPeriods", discount.numberOfPeriods);
			if (discount.localizedPrice != null) {
				dictionary.Add("localizedPrice", discount.localizedPrice);
			}
			if (discount.subscriptionPeriod != null) {
				dictionary.Add("subscriptionPeriod", subscriptionPeriodToDictionary(discount.subscriptionPeriod));
			}
			return dictionary;
		}

		static Dictionary<string, object> productToDictionary(ProductModel product) {
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (product.vendorProductId != null) {
				dictionary.Add("vendorProductId", product.vendorProductId);
			}
			if (product.localizedTitle != null) {
				dictionary.Add("localizedTitle", product.localizedTitle);
			}
			if (product.localizedDescription != null) {
				dictionary.Add("localizedDescription", product.localizedDescription);
			}
			if (product.localizedPrice != null) {
				dictionary.Add("localizedPrice", product.localizedPrice);
			}
			dictionary.Add("price", product.price.ToString());
			if (product.currencyCode != null) {
				dictionary.Add("currencyCode", product.currencyCode);
			}
			if (product.currencySymbol != null) {
				dictionary.Add("currencySymbol", product.currencySymbol);
			}
			if (product.regionCode != null) {
				dictionary.Add("regionCode", product.regionCode);
			}
			if (product.subscriptionPeriod != null) {
				dictionary.Add("subscriptionPeriod", subscriptionPeriodToDictionary(product.subscriptionPeriod));
			}
			if (product.subscriptionGroupIdentifier != null) {
				dictionary.Add("subscriptionGroupIdentifier", product.subscriptionGroupIdentifier);
			}
			dictionary.Add("introductoryOfferEligibility", product.introductoryOfferEligibility);
			dictionary.Add("promotionalOfferEligibility", product.promotionalOfferEligibility);
			if (product.promotionalOfferId != null) {
				dictionary.Add("promotionalOfferId", product.promotionalOfferId);
			}
			if (product.introductoryDiscount != null) {
				dictionary.Add("introductoryDiscount", discountToDictionary(product.introductoryDiscount));
			}
			if (product.discounts != null) {
				List<Dictionary<string, object>> discounts = new List<Dictionary<string, object>>();
				foreach (ProductDiscountModel discount in product.discounts) {
					discounts.Add(discountToDictionary(discount));
				}
				dictionary.Add("discounts", discounts);
			}
			if (product.skuId != null) {
				dictionary.Add("skuId", product.skuId);
			}
			return dictionary;
		}

		static void addAdaptyErrorParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("error")) {
				parameters.Add(dictionaryToAdaptyError((Dictionary<string, object>)dictionary["error"]));
			} else {
				parameters.Add(null);
			}
		}

		static void addPaywallsParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("paywalls")) {
				List<object> paywalls = (List<object>)dictionary["paywalls"];
				List<PaywallModel> array = new List<PaywallModel>();
				foreach (object paywall in paywalls) {
					array.Add(dictionaryToPaywallModel((Dictionary<string, object>)paywall));
				}
				parameters.Add(array.ToArray());
			} else {
				parameters.Add(null);
			}
		}

		static void addProductsParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("products")) {
				List<object> products = (List<object>)dictionary["products"];
				List<ProductModel> array = new List<ProductModel>();
				foreach (object product in products) {
					array.Add(dictionaryToProductModel((Dictionary<string, object>)product));
				}
				parameters.Add(array.ToArray());
			} else {
				parameters.Add(null);
			}
		}

		static void addProductParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("product")) {
				parameters.Add(dictionaryToProductModel((Dictionary<string, object>)dictionary["product"]));
			} else {
				parameters.Add(null);
			}
		}

		static void addPurchaserInfoParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("purchaserInfo")) {
				parameters.Add(dictionaryToPurchaserInfoModel((Dictionary<string, object>)dictionary["purchaserInfo"]));
			} else {
				parameters.Add(null);
			}
		}

		static void addDataStateParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("state")) {
				parameters.Add(stringToDataState((string)dictionary["state"]));
			} else {
				parameters.Add(null);
			}
		}

		static void addReceiptOrPurchaseTokenParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("receipt")) {
				parameters.Add((string)dictionary["receipt"]);
			} else if (dictionary.ContainsKey("purchaseToken")) {
				parameters.Add((string)dictionary["purchaseToken"]);
			} else {
				parameters.Add(null);
			}
		}

		static void addValidationResultParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("validationResult")) {
				parameters.Add((Dictionary<string, object>)dictionary["validationResult"]);
			} else {
				parameters.Add(null);
			}
		}

		static void addValidationResultsParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("validationResults")) {
				List<object> validationResults = (List<object>)dictionary["validationResults"];
				List<Dictionary<string, object>> array = new List<Dictionary<string, object>>();
				foreach (object validationResult in validationResults) {
					array.Add((Dictionary<string, object>)validationResult);
				}
				parameters.Add(validationResults.ToArray());
			} else {
				parameters.Add(null);
			}
		}

		static void addPromoParameter(List<object> parameters, Dictionary<string, object> dictionary) {
			if (dictionary.ContainsKey("promo")) {
				parameters.Add(dictionaryToPromoModel((Dictionary<string, object>)dictionary["promo"]));
			} else {
				parameters.Add(null);
			}
		}

		static object[] messageToParameters(string method, string message) {
			List<object> parameters = new List<object>();
			Dictionary<string, object> dictionary = Json.Deserialize(message) as Dictionary<string,object>;
			switch (method) {
				case "OnIdentify":
				case "OnLogout":
				case "OnSyncPurchases":
				case "OnUpdateAttribution":
				case "OnUpdateProfile":
					// AdaptyError error
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnGetPaywalls":
					// PaywallModel[] paywalls, ProductModel[] products, DataState state, AdaptyError error
					addPaywallsParameter(parameters, dictionary);
					addProductsParameter(parameters, dictionary);
					addDataStateParameter(parameters, dictionary);
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnMakePurchase":
				case "OnMakeDeferredPurchase":
					// PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object> validationResult, ProductModel product, AdaptyError error
					addPurchaserInfoParameter(parameters, dictionary);
					addReceiptOrPurchaseTokenParameter(parameters, dictionary);
					addValidationResultParameter(parameters, dictionary);
					addProductParameter(parameters, dictionary);
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnRestorePurchases":
					// PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object>[] validationResults, AdaptyError error
					addPurchaserInfoParameter(parameters, dictionary);
					addReceiptOrPurchaseTokenParameter(parameters, dictionary);
					addValidationResultsParameter(parameters, dictionary);
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnValidateAppleReceipt":
				case "OnValidateGooglePurchase":
					// PurchaserInfoModel purchaserInfo, Dictionary<string, object> validationResult, AdaptyError error
					addPurchaserInfoParameter(parameters, dictionary);
					addValidationResultParameter(parameters, dictionary);
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnGetPurchaserInfo":
					// PurchaserInfoModel purchaserInfo, DataState state, AdaptyError error
					addPurchaserInfoParameter(parameters, dictionary);
					addDataStateParameter(parameters, dictionary);
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnPurchaserInfoUpdated":
					// PurchaserInfoModel purchaserInfo
					addPurchaserInfoParameter(parameters, dictionary);
					break;
				case "OnGetPromo":
					// PromoModel promo, AdaptyError error
					addPromoParameter(parameters, dictionary);
					addAdaptyErrorParameter(parameters, dictionary);
					break;
				case "OnPromoReceived":
					// PromoModel promo
					addPromoParameter(parameters, dictionary);
					break;

			}
			return parameters.ToArray();
		}

		public static void executeCallback() {
			Callback callback = null;
			#if UNITY_IOS
				callback = AdaptyiOS.getCallback();
			#elif UNITY_ANDROID
				callback = AdaptyAndroid.getCallback();
			#endif
			if (callback != null && callback.objectName != null && callback.method != null && callback.message != null) {
				GameObject gameObject = UnityEngine.GameObject.Find(callback.objectName);
				if (gameObject != null) {
					MonoBehaviour script = gameObject.GetComponent<MonoBehaviour>();
					if (script != null) {
						System.Reflection.MethodInfo method = script.GetType().GetMethod(callback.method);
						if (method != null) {
							method.Invoke(script, messageToParameters(callback.method, callback.message));
						}
					}
				}
			}
		}

		public static void setLogLevel(LogLevel logLevel) {
			#if !UNITY_EDITOR
				int log_level_int = -1;
				switch (logLevel) {
					case LogLevel.None:
						log_level_int = 0;
						break;
					case LogLevel.Error:
						log_level_int = 1;
						break;
					case LogLevel.Verbose:
						log_level_int = 2;
						break;
				}
				#if UNITY_IOS
					AdaptyiOS.setLogLevel(log_level_int);
				#elif UNITY_ANDROID
					AdaptyAndroid.setLogLevel(log_level_int);
				#endif
			#endif
		}
	
		public static void activate(string key, bool observeMode) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.activate(key, observeMode);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.activate(key);
			#endif
		}

		public static void identify(string customerUserId, MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.identify(customerUserId, gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.identify(customerUserId, gameObject ? gameObject.name : null);
			#endif
		}

		public static void logout(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.logout(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.logout(gameObject ? gameObject.name : null);
			#endif
		}

		public static void getPaywalls(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.getPaywalls(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.getPaywalls(gameObject ? gameObject.name : null);
			#endif
		}

		public static void makePurchase(ProductModel product, string offerId, MonoBehaviour gameObject) {
			#if !UNITY_EDITOR
				string productJson = Json.Serialize(productToDictionary(product));
				#if UNITY_IOS
					AdaptyiOS.makePurchase(productJson, offerId, gameObject ? gameObject.name : null);
				#elif UNITY_ANDROID
					AdaptyAndroid.makePurchase(productJson, gameObject ? gameObject.name : null);
				#endif
			#endif
		}

		public static void restorePurchases(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.restorePurchases(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.restorePurchases(gameObject ? gameObject.name : null);
			#endif
		}

		public static void syncPurchases(MonoBehaviour gameObject) {
			#if UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.syncPurchases(gameObject ? gameObject.name : null);
			#endif
		}

		public static void validateAppleReceipt(string receipt, MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.validateReceipt(receipt, gameObject ? gameObject.name : null);
			#endif
		}
		
		public static void validateGooglePurchase(PurchaseType purchaseType, string productId, string purchaseToken, string purchaseOrderId, ProductModel product, MonoBehaviour gameObject) {
			#if UNITY_ANDROID && !UNITY_EDITOR
				int purchaseTypeInt = 0;
				if (purchaseType == PurchaseType.Subscription) {
					purchaseTypeInt = 1;
				}
				string productJson = Json.Serialize(productToDictionary(product));
				AdaptyAndroid.validatePurchase(purchaseTypeInt, productId, purchaseToken, purchaseOrderId, productJson, gameObject ? gameObject.name : null);
			#endif
		}

		public static void getPurchaserInfo(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.getPurchaserInfo(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.getPurchaserInfo(gameObject ? gameObject.name : null);
			#endif
		}

		public static void setOnPurchaserInfoUpdatedListener(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.setOnPurchaserInfoUpdatedListener(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.setOnPurchaserInfoUpdatedListener(gameObject ? gameObject.name : null);
			#endif
		}

		public static void updateAttribution(Dictionary<string, string> attribution, AttributionNetwork source, string networkUserId, MonoBehaviour gameObject) {
			#if !UNITY_EDITOR
				int sourceInt = -1;
				switch (source) {
					case AttributionNetwork.Appsflyer:
						sourceInt = 0;
						break;
					case AttributionNetwork.Adjust:
						sourceInt = 1;
						break;
					case AttributionNetwork.Branch:
						sourceInt = 2;
						break;
					case AttributionNetwork.Custom:
						sourceInt = 3;
						break;
				}
				string attributionJson = Json.Serialize(attribution);
				#if UNITY_IOS
					AdaptyiOS.updateAttribution(attributionJson, sourceInt, networkUserId, gameObject ? gameObject.name : null);
				#elif UNITY_ANDROID
					AdaptyAndroid.updateAttribution(attributionJson, sourceInt, networkUserId, gameObject ? gameObject.name : null);
				#endif
			#endif
		}

		public static void updateProfile(ProfileParameterBuilder profileParams, MonoBehaviour gameObject) {
			#if !UNITY_EDITOR
				string paramsJson = Json.Serialize(profileParams.getDictionary());
				#if UNITY_IOS
					AdaptyiOS.updateProfile(paramsJson, gameObject ? gameObject.name : null);
				#elif UNITY_ANDROID
					AdaptyAndroid.updateProfile(paramsJson, gameObject ? gameObject.name : null);
				#endif
			#endif
		}

		public static void getPromo(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.getPromo(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.getPromo(gameObject ? gameObject.name : null);
			#endif
		}

		public static void setOnPromoReceivedListener(MonoBehaviour gameObject) {
			#if UNITY_IOS && !UNITY_EDITOR
				AdaptyiOS.setOnPromoReceivedListener(gameObject ? gameObject.name : null);
			#elif UNITY_ANDROID && !UNITY_EDITOR
				AdaptyAndroid.setOnPromoReceivedListener(gameObject ? gameObject.name : null);
			#endif
		}
	}
}