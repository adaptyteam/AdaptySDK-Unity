using System;
using System.Collections.Generic;
using UnityEngine;
namespace AdaptySDK.Android
{
#if UNITY_ANDROID
    using AdaptyAndroidCallback = AdaptyAndroidCallbackAction;

    internal static class AdaptyAndroid
    {
        private static AndroidJavaClass AdaptyAndroidClass = new AndroidJavaClass("com.adapty.unity.AdaptyAndroidWrapper");

        internal static void SetLogLevel(string value)
            => AdaptyAndroidClass.CallStatic("setLogLevel", value);

        internal static void Identify(string customerUserId, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("identify", customerUserId, AdaptyAndroidCallback.Action(completionHandler));

        internal static void Logout(Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("logout", AdaptyAndroidCallback.Action(completionHandler));

        internal static void GetPaywall(string placementId, string locale, string fetchPolicy, Int64? timeoutInMillisecond, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("getPaywall", placementId, locale, fetchPolicy, timeoutInMillisecond != null ? new AndroidJavaObject("java.lang.Long", timeoutInMillisecond) : null, AdaptyAndroidCallback.Action(completionHandler));

        internal static void GetPaywallProducts(string paywall, Action<string> completionHandler)
           => AdaptyAndroidClass.CallStatic("getPaywallProducts", paywall, AdaptyAndroidCallback.Action(completionHandler));

        internal static void GetProductsIntroductoryOfferEligibility(string products, Action<string> completionHandler) { completionHandler(null); }

        internal static void GetProfile(Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("getProfile", AdaptyAndroidCallback.Action(completionHandler));

        internal static void RestorePurchases(Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("restorePurchases", AdaptyAndroidCallback.Action(completionHandler));

        internal static void MakePurchase(string product, string androidSubscriptionUpdate, bool? isOfferPersonalized, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("makePurchase", product, androidSubscriptionUpdate, isOfferPersonalized ?? false, AdaptyAndroidCallback.Action(completionHandler));

        internal static void LogShowPaywall(string paywall, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("logShowPaywall", paywall, AdaptyAndroidCallback.Action(completionHandler));

        internal static void LogShowOnboarding(string onboardingScreenParameters, Action<string> completionHandler)
             => AdaptyAndroidClass.CallStatic("logShowOnboarding", onboardingScreenParameters, AdaptyAndroidCallback.Action(completionHandler));

        internal static void SetFallbackPaywalls(string paywalls, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("setFallbackPaywalls", paywalls, AdaptyAndroidCallback.Action(completionHandler));

        internal static void UpdateProfile(string param, Action<string> completionHandler)
         => AdaptyAndroidClass.CallStatic("updateProfile", param, AdaptyAndroidCallback.Action(completionHandler));

        internal static void UpdateAttribution(string jsonstring, string source, string networkUserId, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("updateAttribution", jsonstring, source, networkUserId, AdaptyAndroidCallback.Action(completionHandler));

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<string> completionHandler)
            => AdaptyAndroidClass.CallStatic("setVariationId", transactionId, variationId, AdaptyAndroidCallback.Action(completionHandler));

        internal static void PresentCodeRedemptionSheet() { }
    }
#endif
}