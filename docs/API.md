# API

<img src="https://github.com/adaptyteam/AdaptySDK-Android/raw/master/adapty.png" width="512">

AdaptySDK Unity API

Adapty uses `AdaptySDK` namespace.

## Methods

- [setLogLevel](#setLogLevel)
- [activate](#activate)
- [identify](#identify)
- [logout](#logout)
- [getPaywalls](#getPaywalls)
- [makePurchase](#makePurchase)
- [restorePurchases](#restorePurchases)
- [syncPurchases](#syncPurchases)
- [validateAppleReceipt](#validateAppleReceipt)
- [validateGooglePurchase](#validateGooglePurchase)
- [getPurchaserInfo](#getPurchaserInfo)
- [setOnPurchaserInfoUpdatedListener](#setOnPurchaserInfoUpdatedListener)
- [updateAttribution](#updateAttribution)
- [updateProfile](#updateProfile)
- [getPromo](#getPromo)
- [setOnPromoReceivedListener](#setOnPromoReceivedListener)
- [executeCallback](#executeCallback)

## Callbacks

- [OnIdentify](#OnIdentify)
- [OnLogout](#OnLogout)
- [OnGetPaywalls](#OnGetPaywalls)
- [OnMakePurchase](#OnMakePurchase)
- [OnRestorePurchases](#OnRestorePurchases)
- [OnSyncPurchases](#OnSyncPurchases)
- [OnValidateAppleReceipt](#OnValidateAppleReceipt)
- [OnValidateGooglePurchase](#OnValidateGooglePurchase)
- [OnGetPurchaserInfo](#OnGetPurchaserInfo)
- [OnPurchaserInfoUpdated](#OnPurchaserInfoUpdated)
- [OnUpdateAttribution](#OnUpdateAttribution)
- [OnUpdateProfile](#OnUpdateProfile)
- [OnGetPromo](#OnGetPromo)
- [OnPromoReceived](#OnPromoReceived)
- [OnMakeDeferredPurchase](#OnMakeDeferredPurchase)

## Classes and Enums

- [LogLevel](#LogLevel)
- [DataState](#DataState)
- [PurchaseType](#PurchaseType)
- [AttributionNetwork](#AttributionNetwork)
- [PeriodUnit](#PeriodUnit)
- [PaymentMode](#PaymentMode)
- [Gender](#Gender)
- [AdaptyError](#AdaptyError)
- [ProductSubscriptionPeriodModel](#ProductSubscriptionPeriodModel)
- [ProductDiscountModel](#ProductDiscountModel)
- [ProductModel](#ProductModel)
- [PaywallModel](#PaywallModel)
- [PromoModel](#PromoModel)
- [AccessLevelInfoModel](#AccessLevelInfoModel)
- [SubscriptionInfoModel](#SubscriptionInfoModel)
- [NonSubscriptionInfoModel](#NonSubscriptionInfoModel)
- [PurchaserInfoModel](#PurchaserInfoModel)
- [ProfileParameterBuilder](#ProfileParameterBuilder)

---

## Methods Details

---

#### <a id="setLogLevel"> **`void setLogLevel(LogLevel logLevel)`**

- `logLevel`, LogLevel, logging level.

Adapty logs errors and other important information to help you understand what is going on. There are three levels available:

- `LogLevel.None`
- `LogLevel.Error`
- `LogLevel.Verbose`

*Example:*

```c#
Adapty.setLogLevel(LogLevel.Verbose);
```

---

#### <a id="activate"> **`void activate(string key, bool observeMode)`**

- `key`, string, dev key.
- `observeMode`, bool, enable observe mode.

Initialize the Adapty SDK with the devKey and appID.

*Example:*

```c#
Adapty.activate("key", false);
```

---

#### <a id="identify"> **`void identify(string customerUserId, MonoBehaviour gameObject)`**

- `customerUserId`, string, customer user id.
- `gameObject`, MonoBehaviour, game object with a script that implements [OnIdentify](#OnIdentify) callback.

Adapty creates an internal profile id for every user. But if you have your authentification system you should set your own Customer User Id.

*Example:*

```c#
Adapty.identify("my_customer_id", this);
```

---

#### <a id="logout"> **`void logout(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnLogout](#OnLogout) callback.

Logout the user.

*Example:*

```c#
Adapty.logout(this);
```

---

#### <a id="getPaywalls"> **`void getPaywalls(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnGetPaywalls](#OnGetPaywalls) callback.

Fetch products.

*Example:*

```c#
Adapty.getPaywalls(this);
```

---

#### <a id="makePurchase"> **`void makePurchase(ProductModel product, string offerId, MonoBehaviour gameObject)`**

- `product`, ProductModel, object retrieved from the paywall.
- `offerId`, string (optional), an identifier of promotional offer from App Store Connect.
- `gameObject`, MonoBehaviour, game object with a script that implements [OnMakePurchase](#OnMakePurchase) callback.

Initiate purchasing of a product.

*Example:*

```c#
// Retrieve products using Adapty.getPaywalls(), when the user selects a product for purchase, pass it to the Adapty.makePurchase() method.
ProductModel product = selectedProduct;

Adapty.makePurchase(product, null, this);
```

When the purchase is completed, the [OnMakePurchase](#OnMakePurchase) callback is invoked.
```c#
public void OnMakePurchase(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object> validationResult, ProductModel product, AdaptyError error) {
	// Check if there was an error.
	if (error != null) {
		Debug.Log("Error message: " + error.message + ", code: " + error.code);
	} else {
		// No error - the purchase is successful.
		// User information, who made the purchase, if previously identified.
		if (purchaserInfo != null) {
			Debug.Log("Purchaser customerUserId: " + purchaserInfo.customerUserId);
		}
		// Confirmation receipt from Apple or purchase token from Google.
		Debug.Log("Receipt/PurchaseToken: " + receipt);
		// The purchased product.
		Debug.Log("Product vendorProductId: " + product.vendorProductId);
	}
}
```

---

#### <a id="restorePurchases"> **`void restorePurchases(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnRestorePurchases](#OnRestorePurchases) callback.

Restore purchases.

*Example:*

```c#
Adapty.restorePurchases(this);
```

---

#### <a id="syncPurchases"> **`void syncPurchases(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnSyncPurchases](#OnSyncPurchases) callback.

Android only. Syncs already made purchases with Adapty.

*Example:*

```c#
Adapty.syncPurchases(this);
```

---

#### <a id="validateAppleReceipt"> **`void validateAppleReceipt(string receipt, MonoBehaviour gameObject)`**

- `receipt`, string, Apple's receipt.
- `gameObject`, MonoBehaviour, game object with a script that implements [OnValidateAppleReceipt](#OnValidateAppleReceipt) callback.

iOS only. Validate Apple purchase.

*Example:*

```c#
Adapty.validateAppleReceipt(receipt, this);
```

---

#### <a id="validateGooglePurchase"> **`void validateGooglePurchase(PurchaseType purchaseType, string productId, string purchaseToken, string purchaseOrderId, ProductModel product, MonoBehaviour gameObject)`**

- `purchaseType`, PurchaseType, purchase type.
- `productId`, string, product identifier.
- `purchaseToken`, string, Google's purchase token.
- `purchaseOrderId`, string (optional), purchase order id.
- `product`, ProductModel (optional), purchased product.
- `gameObject`, MonoBehaviour, game object with a script that implements [OnValidateGooglePurchase](#OnValidateGooglePurchase) callback.

Android only. Validate Google purchase.

*Example:*

```c#
Adapty.validateGooglePurchase(PurchaseType.Inapp, productId, purchaseToken, null, null, this);
```

---

#### <a id="getPurchaserInfo"> **`void getPurchaserInfo(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnGetPurchaserInfo](#OnGetPurchaserInfo) callback.

Fetch purchaser info.

*Example:*

```c#
Adapty.getPurchaserInfo(this);
```

---

#### <a id="setOnPurchaserInfoUpdatedListener"> **`void setOnPurchaserInfoUpdatedListener(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnPurchaserInfoUpdated](#OnPurchaserInfoUpdated) callback.

Set a listener that will receive updates when purchaser info changes.

*Example:*

```c#
Adapty.setOnPurchaserInfoUpdatedListener(this);
```

---

#### <a id="updateAttribution"> **`void updateAttribution(Dictionary<string, string> attribution, AttributionNetwork source, string networkUserId, MonoBehaviour gameObject)`**

- `attribution`, Dictionary<string, string>, attribution (conversion) data.
- `source`, AttributionNetwork, the source of attribution.
- `networkUserId`, string (optional), profile's identifier from the attribution service.
- `gameObject`, MonoBehaviour, game object with a script that implements [OnUpdateAttribution](#OnUpdateAttribution) callback.

Set attribution data for the profile

*Example:*

```c#
var attribution = new Dictionary<string, string>();
attribution.Add("attribution_param", "attribution_value");
Adapty.updateAttribution(attribution, AttributionNetwork.Appsflyer, null, this);
```

---

#### <a id="updateProfile"> **`void updateProfile(ProfileParameterBuilder profileParams, MonoBehaviour gameObject)`**

- `profileParams`, ProfileParameterBuilder, user profile data.
- `gameObject`, MonoBehaviour, game object with a script that implements [OnUpdateProfile](#OnUpdateProfile) callback.

You can set optional attributes such as email, phone number, etc, to the user of your app.

*Example:*

```c#
ProfileParameterBuilder profile = new ProfileParameterBuilder()
	.withBirthday("1970-01-01")
	.withEmail("some_email@gmail.com")
	.withFirstName("John")
	.withLastName("Smith")
	.withGender(Gender.Male)
	.withPhoneNumber("+11234567890");
Adapty.updateProfile(profile, this);
```

---

#### <a id="getPromo"> **`void getPromo(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnGetPromo](#OnGetPromo) callback.

Fetch a promo.

*Example:*

```c#
Adapty.getPromo(this);
```

---

#### <a id="setOnPromoReceivedListener"> **`void setOnPromoReceivedListener(MonoBehaviour gameObject)`**

- `gameObject`, MonoBehaviour, game object with a script that implements [OnPromoReceived](#OnPromoReceived) callback.

Set a listener that will receive new promos.

*Example:*

```c#
Adapty.setOnPromoReceivedListener(this);
```

---

#### <a id="executeCallback"> **`void executeCallback()`**

This method should be called inside an `Update()` method of the AdaptyObject prefab. It's used for communication between iOS SDK, Android SDK and Unity SDK.

*Example:*

```c#
Adapty.executeCallback();
```

---

## Callbacks Details

---

#### <a id="OnIdentify"> **`void OnIdentify(AdaptyError error)`**

- `error`, AdaptyError.

---

#### <a id="OnLogout"> **`void OnLogout(AdaptyError error)`**

- `error`, AdaptyError.

---

#### <a id="OnGetPaywalls"> **`void OnGetPaywalls(PaywallModel[] paywalls, ProductModel[] products, DataState state, AdaptyError error)`**

- `paywalls`, PaywallModel[].
- `products`, ProductModel[].
- `state`, DataState.
- `error`, AdaptyError.

---

#### <a id="OnMakePurchase"> **`void OnMakePurchase(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object> validationResult, ProductModel product, AdaptyError error)`**

- `purchaserInfo`, PurchaserInfoModel.
- `receipt`, string.
- `validationResult`, Dictionary<string, object>.
- `product`, ProductModel.
- `error`, AdaptyError.

---

#### <a id="OnRestorePurchases"> **`void OnRestorePurchases(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object>[] validationResults, AdaptyError error)`**

- `purchaserInfo`, PurchaserInfoModel.
- `receipt`, string.
- `validationResults`, Dictionary<string, object>[].
- `error`, AdaptyError.

---

#### <a id="OnSyncPurchases"> **`void OnSyncPurchases(AdaptyError error)`**

- `error`, AdaptyError.

---

#### <a id="OnValidateAppleReceipt"> **`void OnValidateAppleReceipt(PurchaserInfoModel purchaserInfo, Dictionary<string, object> validationResult, AdaptyError error)`**

- `purchaserInfo`, PurchaserInfoModel.
- `validationResult`, Dictionary<string, object>.
- `error`, AdaptyError.

---

#### <a id="OnValidateGooglePurchase"> **`void OnValidateGooglePurchase(PurchaserInfoModel purchaserInfo, Dictionary<string, object> validationResult, AdaptyError error)`**

- `purchaserInfo`, PurchaserInfoModel.
- `validationResult`, Dictionary<string, object>.
- `error`, AdaptyError.

---

#### <a id="OnGetPurchaserInfo"> **`void OnGetPurchaserInfo(PurchaserInfoModel purchaserInfo, DataState state, AdaptyError error)`**

- `purchaserInfo`, PurchaserInfoModel.
- `state`, DataState.
- `error`, AdaptyError.

---

#### <a id="OnPurchaserInfoUpdated"> **`void OnPurchaserInfoUpdated(PurchaserInfoModel purchaserInfo)`**

- `purchaserInfo`, PurchaserInfoModel.

---

#### <a id="OnUpdateAttribution"> **`void OnUpdateAttribution(AdaptyError error)`**

- `error`, AdaptyError.

---

#### <a id="OnUpdateProfile"> **`void OnUpdateProfile(AdaptyError error)`**

- `error`, AdaptyError.

---

#### <a id="OnGetPromo"> **`void OnGetPromo(PromoModel promo, AdaptyError error)`**

- `promo`, PromoModel.
- `error`, AdaptyError.

---

#### <a id="OnPromoReceived"> **`void OnPromoReceived(PromoModel promo)`**

- `promo`, PromoModel.

---

#### <a id="OnMakeDeferredPurchase"> **`void OnMakeDeferredPurchase(PurchaserInfoModel purchaserInfo, string receipt, Dictionary<string, object> validationResult, ProductModel product, AdaptyError error)`**

- `purchaserInfo`, PurchaserInfoModel.
- `receipt`, string.
- `validationResult`, Dictionary<string, object>.
- `product`, ProductModel.
- `error`, AdaptyError.

---

## Classes and Enums Details

---

#### <a id="LogLevel"> **`enum LogLevel`**
- `None`
- `Error`
- `Verbose`

---

#### <a id="DataState"> **`enum DataState`**
- `Cached`
- `Synced`

---

#### <a id="PurchaseType"> **`enum PurchaseType`**
- `Inapp`
- `Subscription`

---

#### <a id="AttributionNetwork"> **`enum AttributionNetwork`**
- `Appsflyer`
- `Adjust`
- `Branch`
- `Custom`

---

#### <a id="PeriodUnit"> **`enum PeriodUnit`**
- `Day`
- `Week`
- `Month`
- `Year`

---

#### <a id="PaymentMode"> **`enum PaymentMode`**
- `FreeTrial`
- `PayAsYouGo`
- `PayUpFront`

---

#### <a id="Gender"> **`enum Gender`**
- `Female`
- `Male`
- `Other`

---

#### <a id="AdaptyError"> **`class AdaptyError`**
- `message`, string.
- `code`, long.

---

#### <a id="ProductSubscriptionPeriodModel"> **`class ProductSubscriptionPeriodModel`**
- `unit`, PeriodUnit.
- `numberOfUnits`, long.

---

#### <a id="ProductDiscountModel"> **`class ProductDiscountModel`**
- `price`, decimal.
- `identifier`, string.
- `subscriptionPeriod`, ProductSubscriptionPeriodModel.
- `numberOfPeriods`, long.
- `paymentMode`, PaymentMode.
- `localizedPrice`, string.
- `localizedSubscriptionPeriod`, string.
- `localizedNumberOfPeriods`, string.

---

#### <a id="ProductModel"> **`class ProductModel`**
- `vendorProductId`, string.
- `introductoryOfferEligibility`, bool.
- `promotionalOfferEligibility`, bool.
- `promotionalOfferId`, string.
- `localizedDescription`, string.
- `localizedTitle`, string.
- `price`, decimal.
- `currencyCode`, string.
- `currencySymbol`, string.
- `regionCode`, string.
- `subscriptionPeriod`, ProductSubscriptionPeriodModel.
- `introductoryDiscount`, ProductDiscountModel.
- `subscriptionGroupIdentifier`, string.
- `discounts`, ProductDiscountModel[].
- `localizedPrice`, string.
- `localizedSubscriptionPeriod`, string.
- `skuId`, string.

---

#### <a id="PaywallModel"> **`class PaywallModel`**
- `developerId`, string.
- `variationId`, string.
- `revision`, long.
- `isPromo`, bool.
- `products`, ProductModel[].
- `visualPaywall`, string.
- `customPayload`, Dictionary<string, object>.

---

#### <a id="PromoModel"> **`class PromoModel`**
- `promoType`, string.
- `variationId`, string.
- `expiresAt`, DateTime.
- `paywall`, PaywallModel.

---

#### <a id="AccessLevelInfoModel"> **`class AccessLevelInfoModel`**
- `id`, string.
- `isActive`, bool.
- `vendorProductId`, string.
- `store`, string.
- `activatedAt`, DateTime.
- `renewedAt`, DateTime.
- `expiresAt`, DateTime.
- `isLifetime`, bool.
- `activeIntroductoryOfferType`, string.
- `activePromotionalOfferType`, string.
- `willRenew`, bool.
- `isInGracePeriod`, bool.
- `unsubscribedAt`, DateTime.
- `billingIssueDetectedAt`, DateTime.
- `vendorTransactionId`, string.
- `vendorOriginalTransactionId`, string.
- `startsAt`, DateTime.
- `cancellationReason`, string.
- `isRefund`, bool.

---

#### <a id="SubscriptionInfoModel"> **`class SubscriptionInfoModel`**
- `isActive`, bool.
- `vendorProductId`, string.
- `store`, string.
- `activatedAt`, DateTime.
- `renewedAt`, DateTime.
- `expiresAt`, DateTime.
- `startsAt`, DateTime.
- `isLifetime`, bool.
- `activeIntroductoryOfferType`, string.
- `activePromotionalOfferType`, string.
- `willRenew`, bool.
- `isInGracePeriod`, bool.
- `unsubscribedAt`, DateTime.
- `billingIssueDetectedAt`, DateTime.
- `isSandbox`, bool.
- `vendorTransactionId`, string.
- `vendorOriginalTransactionId`, string.
- `cancellationReason`, string.
- `isRefund`, bool.

---

#### <a id="NonSubscriptionInfoModel"> **`class NonSubscriptionInfoModel`**
- `purchaseId`, string.
- `vendorProductId`, string.
- `store`, string.
- `purchasedAt`, DateTime.
- `isOneTime`, bool.
- `isSandbox`, bool.
- `vendorTransactionId`, string.
- `vendorOriginalTransactionId`, string.
- `isRefund`, bool.

---

#### <a id="PurchaserInfoModel"> **`class PurchaserInfoModel`**
- `customerUserId`, string.
- `accessLevels`, Dictionary<string, AccessLevelInfoModel>.
- `subscriptions`, Dictionary<string, SubscriptionInfoModel>.
- `nonSubscriptions`, Dictionary<string, NonSubscriptionInfoModel>.

---

#### <a id="ProfileParameterBuilder"> **`class ProfileParameterBuilder`**
- `withEmail(string email)`
- `withPhoneNumber(string phoneNumber)`
- `withFacebookUserId(string facebookUserId)`
- `withAmplitudeUserId(string amplitudeUserId)`
- `withAmplitudeDeviceId(string amplitudeDeviceId)`
- `withMixpanelUserId(string mixpanelUserId)`
- `withAppmetricaProfileId(string appmetricaProfileId)`
- `withAppmetricaDeviceId(string appmetricaDeviceId)`
- `withFirstName(string firstName)`
- `withLastName(string lastName)`
- `withGender(Gender gender)`
- `withBirthday(string birthday)`
- `withAppTrackingTransparencyStatus(uint appTrackingTransparencyStatus)`
- `withCustomAttributes(Dictionary<string, object> customAttributes)`

---
