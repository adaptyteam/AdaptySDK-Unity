using System;
using System.Collections.Generic;
using AdaptySDK.iOS;
using AdaptySDK.Android;
using AdaptySDK.Noop;

#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif

namespace AdaptySDK {
    public static partial class Adapty {
        public static readonly string pluginVersion = "1.1.0";

        public static LogLevel GetLogLevel() => _Adapty.GetLogLevel();
        public static void SetLogLevel(LogLevel level) => _Adapty.SetLogLevel(level);

        //public static void Activate(string key, bool observeMode, string customerUserId) => _Adapty.Activate(key, observeMode, customerUserId);
        //public static void Activate(string key, bool observeMode) => Activate(key, observeMode, null);
        //public static void Activate(string key, string customerUserId) => Activate(key, false, customerUserId);
        //public static void Activate(string key) => Activate(key, false, null);

        public static void Identify(string customerUserId, Action<Error> completionHandler) => _Adapty.Identify(customerUserId, completionHandler);
        public static void Identify(string customerUserId) => Identify(customerUserId, null);

        public static void Logout(Action<Error> completionHandler) => _Adapty.Logout(completionHandler);
        public static void Logout() => Logout(null);

        public static void GetPaywalls(bool forceUpdate, Action<GetPaywallsResponse, Error> completionHandler) => _Adapty.GetPaywalls(forceUpdate, completionHandler);
        public static void GetPaywalls(Action<GetPaywallsResponse, Error> completionHandler) => GetPaywalls(false, completionHandler);

        public static void GetPurchaserInfo(bool forceUpdate, Action<PurchaserInfo, Error> completionHandler) => _Adapty.GetPurchaserInfo(forceUpdate, completionHandler);
        public static void GetPurchaserInfo(Action<PurchaserInfo, Error> completionHandler) => GetPurchaserInfo(false, completionHandler);

        public static void RestorePurchases(Action<RestorePurchasesResponse, Error> completionHandler) => _Adapty.RestorePurchases(completionHandler);

        public static void MakePurchase(string productId, string variationId, string offerId, AndroidSubscriptionUpdate subscriptionUpdate, Action<MakePurchaseResponse, Error> completionHandler)
        {
            #if UNITY_IOS
                AdaptyIOS.MakePurchase(productId, variationId, offerId, completionHandler);
            #elif UNITY_ANDROID
                AdaptyAndroid.MakePurchase(productId, variationId, subscriptionUpdate, completionHandler);
            #else
                AdaptyNoop.MakePurchase(productId, variationId, completionHandler);
            #endif
        }
        public static void MakePurchase(string productId, string variationId, string offerId, Action<MakePurchaseResponse, Error> completionHandler) => MakePurchase(productId, variationId, offerId, null, completionHandler);
        public static void MakePurchase(string productId, string variationId, AndroidSubscriptionUpdate subscriptionUpdate, Action<MakePurchaseResponse, Error> completionHandler) => MakePurchase(productId, variationId, null, subscriptionUpdate, completionHandler);
        public static void MakePurchase(string productId, string variationId, Action<MakePurchaseResponse, Error> completionHandler) => MakePurchase(productId, variationId, null, null, completionHandler);

        public static void LogShowPaywall(Paywall paywall, Action<Error> completionHandler) => _Adapty.LogShowPaywall(paywall, completionHandler);
        public static void LogShowPaywall(Paywall paywall) => LogShowPaywall(paywall, null);

        public static void SetFallbackPaywalls(string paywalls, Action<Error> completionHandler) => _Adapty.SetFallbackPaywalls(paywalls, completionHandler);
        public static void SetFallbackPaywalls(string paywalls) => SetFallbackPaywalls(paywalls, null);

        public static void GetPromo(Action<Promo, Error> completionHandler) => _Adapty.GetPromo(completionHandler);

        public static void UpdateProfile(ProfileParameterBuilder param, Action<Error> completionHandler) => _Adapty.UpdateProfile(param, completionHandler);
        public static void UpdateProfile(ProfileParameterBuilder param) => UpdateProfile(param, null);

        public static void UpdateAttribution(string jsonstring, AttributionNetwork source, string networkUserId, Action<Error> completionHandler) => _Adapty.UpdateAttribution(jsonstring, source, networkUserId, completionHandler);
        public static void UpdateAttribution(string jsonstring, AttributionNetwork source, Action<Error> completionHandler) => UpdateAttribution(jsonstring, source, null, completionHandler);
        public static void UpdateAttribution(string jsonstring, AttributionNetwork source, string networkUserId) => UpdateAttribution(jsonstring, source, networkUserId, null);
        public static void UpdateAttribution(string jsonstring, AttributionNetwork source) => UpdateAttribution(jsonstring, source, null, null);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionNetwork source, string networkUserId, Action<Error> completionHandler) => _Adapty.UpdateAttribution(attribution, source, networkUserId, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionNetwork source, Action<Error> completionHandler) => UpdateAttribution(attribution, source, null, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionNetwork source, string networkUserId) => UpdateAttribution(attribution, source, networkUserId, null);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionNetwork source) => UpdateAttribution(attribution, source, null, null);

        public static void SetExternalAnalyticsEnabled(bool enabled, Action<Error> completionHandler) => _Adapty.SetExternalAnalyticsEnabled(enabled, completionHandler);
        public static void SetExternalAnalyticsEnabled(bool enabled) => SetExternalAnalyticsEnabled(enabled, null);

        public static void SetVariationForTransaction(string variationId, string transactionId, Action<Error> completionHandler) => _Adapty.SetVariationForTransaction(variationId, transactionId, completionHandler);
        public static void SetVariationForTransaction(string variationId, string transactionId) => SetVariationForTransaction(variationId, transactionId, null);


#if UNITY_IOS
        public static void SetIdfaCollectionDisabled(bool disabled) => AdaptyIOS.SetIdfaCollectionDisabled(disabled);

        public static void MakeDeferredPurchase(string productId, Action<MakePurchaseResponse, Error> completionHandler) => AdaptyIOS.MakeDeferredPurchase(productId, completionHandler);

        public static string GetApnsToken() => AdaptyIOS.GetApnsToken();
        public static void SetApnsToken(string apnsToken) => AdaptyIOS.SetApnsToken(apnsToken);

        public static void HandlePushNotification(Dictionary<string, dynamic> userInfo, Action<Error> completionHandler) => AdaptyIOS.HandlePushNotification(userInfo, completionHandler);
        public static void HandlePushNotification(Dictionary<string, dynamic> userInfo) => HandlePushNotification(userInfo, null);
        public static void HandlePushNotification(string userInfo, Action<Error> completionHandler) => AdaptyIOS.HandlePushNotification(userInfo, completionHandler);
        public static void HandlePushNotification(string userInfo) => HandlePushNotification(userInfo, null);

        public static void PresentCodeRedemptionSheet() => AdaptyIOS.PresentCodeRedemptionSheet();
#elif UNITY_ANDROID
        public static void NewPushToken(string pushToken) => AdaptyAndroid.NewPushToken(pushToken);

        public static void PushReceived(string pushMessageJson) => AdaptyAndroid.PushReceived(pushMessageJson);
#endif
    }
}