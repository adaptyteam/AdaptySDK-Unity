using System;
using System.Collections.Generic;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        internal static void SetLogLevel(string value) { }

        internal static void Identify(string customerUserId, Action<string> completionHandler) { completionHandler(null); }

        internal static void Logout(Action<string> completionHandler) { completionHandler(null); }

        internal static void GetPaywall(string id, Action<string> completionHandler) { completionHandler(null); }

        internal static void GetPaywallProducts(string paywall, string fetchPolicy, Action<string> completionHandler) { completionHandler(null); }

        internal static void GetProfile(Action<string> completionHandler) { completionHandler(null); }

        internal static void RestorePurchases(Action<string> completionHandler) { completionHandler(null); }

        internal static void MakePurchase(string product, string androidSubscriptionUpdate, Action<string> completionHandler) { completionHandler(null); }

        internal static void LogShowPaywall(string paywall, Action<string> completionHandler) { completionHandler(null); }

        internal static void LogShowOnboarding(string onboardingScreenParameters, Action<string> completionHandler) { completionHandler(null); }

        internal static void SetFallbackPaywalls(string paywalls, Action<string> completionHandler) { completionHandler(null); }

        internal static void UpdateProfile(string param, Action<string> completionHandler) { completionHandler(null); }

        internal static void UpdateAttribution(string jsonstring, string source, string networkUserId, Action<string> completionHandler) { completionHandler(null); }

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<string> completionHandler) { completionHandler(null); }

        internal static void PresentCodeRedemptionSheet() { }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}