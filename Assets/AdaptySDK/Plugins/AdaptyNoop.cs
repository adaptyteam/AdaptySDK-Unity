using System;
using System.Collections.Generic;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        //internal static void Activate(string key, bool observeMode, string customerUserId) { }

        internal static Adapty.LogLevel GetLogLevel() { return Adapty.LogLevel.None; }

        internal static void SetLogLevel(Adapty.LogLevel value) { }

        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void Logout(Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void GetPaywalls(bool forceUpdate, Action<Adapty.GetPaywallsResponse, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void GetPurchaserInfo(bool forceUpdate, Action<Adapty.PurchaserInfo, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void RestorePurchases(Action<Adapty.RestorePurchasesResponse, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void MakePurchase(string productId, string variationId, Action<Adapty.MakePurchaseResponse, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void LogShowPaywall(Adapty.Paywall paywall, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void GetPromo(Action<Adapty.Promo, Adapty.Error> completionHandler) { completionHandler(null, null); }

        internal static void UpdateProfile(Adapty.ProfileParameterBuilder param, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void UpdateAttribution(string jsonstring, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void UpdateAttribution(Dictionary<string, dynamic> attribution, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler) { completionHandler(null); }

        internal static void SetExternalAnalyticsEnabled(bool enabled, Action<Adapty.Error> completionHandler) { completionHandler(null);  }

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler) { completionHandler(null); }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}