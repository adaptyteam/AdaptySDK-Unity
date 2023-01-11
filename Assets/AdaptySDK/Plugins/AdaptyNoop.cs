using System;
using System.Collections.Generic;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        internal static void SetLogLevel(Adapty.LogLevel value) { }

        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void Logout(Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void GetPaywall(string id, Action<Adapty.Paywall, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void GetPaywallProducts(Adapty.Paywall paywall, Adapty.IOSProductsFetchPolicy fetchPolicy, Action<IList<Adapty.PaywallProduct>, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void GetProfile(Action<Adapty.Profile, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void RestorePurchases(Action<Adapty.Profile, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void MakePurchase(Adapty.PaywallProduct product, Action<Adapty.Profile, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void LogShowPaywall(Adapty.Paywall paywall, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void LogShowOnboarding(Adapty.OnboardingScreenParameters onboardingScreenParameters, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void UpdateProfile(Adapty.ProfileParameters param, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void UpdateAttribution(string jsonstring, Adapty.AttributionSource source, string networkUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler) { completionHandler(null); }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}