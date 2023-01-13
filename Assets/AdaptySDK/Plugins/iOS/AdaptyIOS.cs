using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AdaptySDK.SimpleJSON;

namespace AdaptySDK.iOS
{
#if UNITY_IOS
    internal static class AdaptyIOS
    {

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setLogLevel")]
        private static extern void _SetLogLevel(string value);

        internal static void SetLogLevel(Adapty.LogLevel value) => _SetLogLevel(value.ToJSON());

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_identify")]
        private static extern void _Identify(string customerUserId, IntPtr callback);

        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler)
            => _Identify(customerUserId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.Identify(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logout")]
        private static extern void _Logout(IntPtr callback);

        internal static void Logout(Action<Adapty.Error> completionHandler)
            => _Logout(AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.Logout(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPaywall")]
        private static extern void _GetPaywall(string id, IntPtr callback);

        internal static void GetPaywall(string id, Action<Adapty.Paywall, Adapty.Error> completionHandler)
            => _GetPaywall(id, AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var response = JSONNode.Parse(json);
            var error = response.GetErrorIfPresent("error");
            var result = error != null ? null : response.GetPaywall("success");
            try
            {
                completionHandler(result, error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Paywall,Adapty.Error> completionHandler in AdaptyIOS.GetPaywall(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPaywallProducts")]
        private static extern void _GetPaywallProducts(string paywall, string fetchPolicy, IntPtr callback);

        internal static void GetPaywallProducts(Adapty.Paywall paywall, Adapty.IOSProductsFetchPolicy fetchPolicy, Action<IList<Adapty.PaywallProduct>, Adapty.Error> completionHandler)
            => _GetPaywallProducts(paywall.ToJSONNode().ToString(), fetchPolicy.ToJSON(), AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var response = JSONNode.Parse(json);
            var error = response.GetErrorIfPresent("error");
            var result = error != null ? null : response.GetPaywallProductList("success");
            try
            {
                completionHandler(result, error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<IList<Adapty.PaywallProduct>,Adapty.Error> completionHandler in AdaptyIOS.GetPaywallProducts(..)", e);
            }
        }));


        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getProfile")]
        private static extern void _GetProfile(IntPtr callback);

        internal static void GetProfile(Action<Adapty.Profile, Adapty.Error> completionHandler)
            => _GetProfile(AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var response = JSONNode.Parse(json);
            var error = response.GetErrorIfPresent("error");
            var result = error != null ? null : response.GetProfile("success");
            try
            {
                completionHandler(result, error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in AdaptyIOS.GetProfile(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_restorePurchases")]
        private static extern void _RestorePurchases(IntPtr callback);

        internal static void RestorePurchases(Action<Adapty.Profile, Adapty.Error> completionHandler)
            => _RestorePurchases(AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var response = JSONNode.Parse(json);
            var error = response.GetErrorIfPresent("error");
            var result = error != null ? null : response.GetProfile("success");
            try
            {
                completionHandler(result, error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in AdaptyIOS.RestorePurchases(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_makePurchase")]
        private static extern void _MakePurchase(string product, IntPtr callback);

        internal static void MakePurchase(Adapty.PaywallProduct product, Action<Adapty.Profile, Adapty.Error> completionHandler)
            => _MakePurchase(product.ToJSONNode().ToString(), AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var response = JSONNode.Parse(json);
            var error = response.GetErrorIfPresent("error");
            var result = error != null ? null : response.GetProfile("success");
            try
            {
                completionHandler(result, error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in AdaptyIOS.MakePurchase(..)", e);
            }
        }));



        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logShowPaywall")]
        private static extern void _LogShowPaywall(string paywall, IntPtr callback);

        internal static void LogShowPaywall(Adapty.Paywall paywall, Action<Adapty.Error> completionHandler)
            => _LogShowPaywall(paywall.ToJSONNode().ToString(), AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.LogShowPaywall(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logShowOnboarding")]
        private static extern void _LogShowOnboarding(string onboardingScreenParameters, IntPtr callback);

        internal static void LogShowOnboarding(Adapty.OnboardingScreenParameters onboardingScreenParameters, Action<Adapty.Error> completionHandler)
            => _LogShowOnboarding(onboardingScreenParameters.ToJSONNode().ToString(), AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.LogShowOnboarding(..)", e);
            }
        }));


        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setFallbackPaywalls")]
        private static extern void _SetFallbackPaywalls(string paywalls, IntPtr callback);

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler)
            => _SetFallbackPaywalls(paywalls, AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.SetFallbackPaywalls(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_updateProfile")]
        private static extern void _UpdateProfile(string param, IntPtr callback);

        internal static void UpdateProfile(Adapty.ProfileParameters param, Action<Adapty.Error> completionHandler)
            => _UpdateProfile(param.ToJSONNode().ToString(), AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.UpdateProfile(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_updateAttribution")]
        private static extern void _UpdateAttribution(string attributions, string source, string networkUserId, IntPtr callback);

        internal static void UpdateAttribution(string jsonstring, Adapty.AttributionSource source, string networkUserId, Action<Adapty.Error> completionHandler)
            => _UpdateAttribution(jsonstring, source.ToJSON(), networkUserId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.UpdateAttribution(..)", e);
            }
        }));


        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setVariationForTransaction")]
        private static extern void _SetVariationForTransaction(string variationId, string transactionId, IntPtr callback);

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler)
            => _SetVariationForTransaction(variationId, transactionId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.SetVariationForTransaction(..)", e);
            }
        }));

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_presentCodeRedemptionSheet")]
        internal static extern void PresentCodeRedemptionSheet();

    }
#endif
}