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

        internal static void Logout(Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("logout", AdaptyAndroidCallback.Action(completionHandler));

        internal static void GetPaywall(string id, Action<Adapty.Paywall, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("getPaywall", id, AdaptyAndroidCallback.Action(completionHandler));

        internal static void GetPaywallProducts(string paywall, string fetchPolicy, Action<IList<Adapty.PaywallProduct>, Adapty.Error> completionHandler)
           => AdaptyAndroidClass.CallStatic("getPaywallProducts", paywall, AdaptyAndroidCallback.Action(completionHandler));

        internal static void GetProfile(Action<Adapty.Profile, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("getProfile", AdaptyAndroidCallback.Action(completionHandler));

        internal static void RestorePurchases(Action<Adapty.Profile, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("restorePurchases", AdaptyAndroidCallback.Action(completionHandler));

        internal static void MakePurchase(string product, string androidSubscriptionUpdate, Action<Adapty.Profile, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("makePurchase", product, androidSubscriptionUpdate, AdaptyAndroidCallback.Action(completionHandler));

        internal static void LogShowPaywall(string paywall, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("logShowPaywall", paywall, AdaptyAndroidCallback.Action(completionHandler));

        internal static void LogShowOnboarding(string onboardingScreenParameters, Action<Adapty.Error> completionHandler)
             => AdaptyAndroidClass.CallStatic("logShowOnboarding", onboardingScreenParameters, AdaptyAndroidCallback.Action(completionHandler));

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("setFallbackPaywalls", paywalls, AdaptyAndroidCallback.Action(completionHandler));

        internal static void UpdateProfile(string param, Action<Adapty.Error> completionHandler)
         => AdaptyAndroidClass.CallStatic("updateProfile", param, AdaptyAndroidCallback.Action(completionHandler));

        internal static void UpdateAttribution(string jsonstring, string source, string networkUserId, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("updateAttribution", jsonstring, source, networkUserId, AdaptyAndroidCallback.Action(completionHandler));

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("setVariationId", transactionId, variationId, AdaptyAndroidCallback.Action(completionHandler));

        internal static void PresentCodeRedemptionSheet() { }
    }
#endif
}