using System;
using System.Collections.Generic;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        internal static void SetLogLevel(string value) { }

        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void Logout(Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void GetPaywall(string id, Action<Adapty.Paywall, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void GetPaywallProducts(string paywall, string fetchPolicy, Action<IList<Adapty.PaywallProduct>, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void GetProfile(Action<Adapty.Profile, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void RestorePurchases(Action<Adapty.Profile, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void MakePurchase(string product, Action<Adapty.Profile, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void LogShowPaywall(string paywall, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void LogShowOnboarding(string onboardingScreenParameters, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void UpdateProfile(string param, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void UpdateAttribution(string jsonstring, string source, string networkUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler) { completionHandler(null); }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}