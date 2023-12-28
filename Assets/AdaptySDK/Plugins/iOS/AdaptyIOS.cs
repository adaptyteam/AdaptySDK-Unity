using System;
using System.Runtime.InteropServices;
using AdaptySDK.SimpleJSON;

namespace AdaptySDK.iOS
{
#if UNITY_IOS
    internal static class AdaptyIOS
    {
        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setLogLevel")]
        private static extern void _SetLogLevel(string value);

        internal static void SetLogLevel(string value) => _SetLogLevel(value);

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_identify")]
        private static extern void _Identify(string customerUserId, IntPtr callback);

        internal static void Identify(string customerUserId, Action<string> completionHandler)
            => _Identify(customerUserId, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logout")]
        private static extern void _Logout(IntPtr callback);

        internal static void Logout(Action<string> completionHandler)
            => _Logout(AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPaywall")]
        private static extern void _GetPaywall(string placementId, string locale, string fetchPolicy, Int64 timeoutInMilisecond, IntPtr callback);

        internal static void GetPaywall(string placementId, string locale, string fetchPolicy, Int64? timeoutInMillisecond, Action<string> completionHandler)
            => _GetPaywall(placementId, locale, fetchPolicy, timeoutInMillisecond ?? -1, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPaywallProducts")]
        private static extern void _GetPaywallProducts(string paywall, IntPtr callback);

        internal static void GetPaywallProducts(string paywall, Action<string> completionHandler)
            => _GetPaywallProducts(paywall, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getProductsIntroductoryOfferEligibility")]
        private static extern void _GetProductsIntroductoryOfferEligibility(string products, IntPtr callback);

        internal static void GetProductsIntroductoryOfferEligibility(string products, Action<string> completionHandler)
            => _GetProductsIntroductoryOfferEligibility(products, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getProfile")]
        private static extern void _GetProfile(IntPtr callback);

        internal static void GetProfile(Action<string> completionHandler)
            => _GetProfile(AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_restorePurchases")]
        private static extern void _RestorePurchases(IntPtr callback);

        internal static void RestorePurchases(Action<string> completionHandler)
            => _RestorePurchases(AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_makePurchase")]
        private static extern void _MakePurchase(string product, IntPtr callback);

        internal static void MakePurchase(string product, string androidSubscriptionUpdate, bool? isOfferPersonalized, Action<string> completionHandler)
            => _MakePurchase(product, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logShowPaywall")]
        private static extern void _LogShowPaywall(string paywall, IntPtr callback);

        internal static void LogShowPaywall(string paywall, Action<string> completionHandler)
            => _LogShowPaywall(paywall, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logShowOnboarding")]
        private static extern void _LogShowOnboarding(string onboardingScreenParameters, IntPtr callback);

        internal static void LogShowOnboarding(string onboardingScreenParameters, Action<string> completionHandler)
            => _LogShowOnboarding(onboardingScreenParameters, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setFallbackPaywalls")]
        private static extern void _SetFallbackPaywalls(string paywalls, IntPtr callback);

        internal static void SetFallbackPaywalls(string paywalls, Action<string> completionHandler)
            => _SetFallbackPaywalls(paywalls, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_updateProfile")]
        private static extern void _UpdateProfile(string param, IntPtr callback);

        internal static void UpdateProfile(string param, Action<string> completionHandler)
            => _UpdateProfile(param, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_updateAttribution")]
        private static extern void _UpdateAttribution(string attributions, string source, string networkUserId, IntPtr callback);

        internal static void UpdateAttribution(string jsonstring, string source, string networkUserId, Action<string> completionHandler)
            => _UpdateAttribution(jsonstring, source, networkUserId, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setVariationForTransaction")]
        private static extern void _SetVariationForTransaction(string parameters, IntPtr callback);

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<string> completionHandler)
        {
            var node = new JSONObject();
            node.Add("variation_id", variationId);
            node.Add("transaction_id", transactionId);
            _SetVariationForTransaction(node.ToString(), AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));
        }

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_presentCodeRedemptionSheet")]
        internal static extern void PresentCodeRedemptionSheet();
    }
#endif
}