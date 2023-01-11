using System;
using System.Collections.Generic;
using AdaptySDK.iOS;
using AdaptySDK.Android;
using AdaptySDK.Noop;
using AdaptySDK.SimpleJSON;

#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public static readonly string sdkVersion = "2.2.0";

        public static void SetLogLevel(LogLevel level)
            => _Adapty.SetLogLevel(level);

        public static void Identify(string customerUserId, Action<Error> completionHandler)
            => _Adapty.Identify(customerUserId, completionHandler);

        public static void Logout(Action<Error> completionHandler)
            => _Adapty.Logout(completionHandler);

        public static void GetPaywall(string id, Action<Paywall, Error> completionHandler)
            => _Adapty.GetPaywall(id, completionHandler);

        public static void GetPaywallProducts(Paywall paywall, IOSProductsFetchPolicy fetchPolicy, Action<IList<PaywallProduct>, Error> completionHandler)
            => _Adapty.GetPaywallProducts(paywall, fetchPolicy, completionHandler);
        public static void GetPaywallProducts(Paywall paywall, Action<IList<PaywallProduct>, Error> completionHandler)
            => GetPaywallProducts(paywall, IOSProductsFetchPolicy.Default, completionHandler);

        public static void GetProfile(Action<Profile, Error> completionHandler)
            => _Adapty.GetProfile(completionHandler);

        public static void RestorePurchases(Action<Profile, Error> completionHandler)
            => _Adapty.RestorePurchases(completionHandler);

        public static void MakePurchase(PaywallProduct product, AndroidSubscriptionUpdateParameters subscriptionUpdate, Action<Profile, Error> completionHandler)
        {
#if UNITY_IOS
            AdaptyIOS.MakePurchase(product, completionHandler);
#elif UNITY_ANDROID
                AdaptyAndroid.MakePurchase(product, subscriptionUpdate, completionHandler);
#else
                AdaptyNoop.MakePurchase(product, completionHandler);
#endif
        }
        public static void MakePurchase(PaywallProduct product, Action<Profile, Error> completionHandler)
            => MakePurchase(product, null, completionHandler);

        public static void LogShowPaywall(Paywall paywall, Action<Error> completionHandler)
            => _Adapty.LogShowPaywall(paywall, completionHandler);

        public static void LogShowOnboarding(string name, string screenName, uint screenOrder, Action<Error> completionHandler)
            => _Adapty.LogShowOnboarding(new OnboardingScreenParameters(name, screenName, screenOrder), completionHandler);

        public static void SetFallbackPaywalls(string paywalls, Action<Error> completionHandler)
            => _Adapty.SetFallbackPaywalls(paywalls, completionHandler);

        public static void UpdateProfile(ProfileParameters param, Action<Error> completionHandler)
            => _Adapty.UpdateProfile(param, completionHandler);

        public static void UpdateAttribution(string jsonstring, AttributionSource source, string networkUserId, Action<Error> completionHandler)
            => _Adapty.UpdateAttribution(jsonstring, source, networkUserId, completionHandler);
        public static void UpdateAttribution(string jsonstring, AttributionSource source, Action<Error> completionHandler)
            => UpdateAttribution(jsonstring, source, null, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, string networkUserId, Action<Error> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, networkUserId, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, Action<Error> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, null, completionHandler);



        public static void SetVariationForTransaction(string variationId, string transactionId, Action<Error> completionHandler)
            => _Adapty.SetVariationForTransaction(variationId, transactionId, completionHandler);


#if UNITY_IOS
        public static void PresentCodeRedemptionSheet()
            => AdaptyIOS.PresentCodeRedemptionSheet();
#endif
    }
}