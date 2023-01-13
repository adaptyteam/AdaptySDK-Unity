using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;
using UnityEngine;
namespace AdaptySDK.Android
{
#if UNITY_ANDROID
    using AdaptyAndroidCallback = AdaptyAndroidCallbackAction;

    internal static class AdaptyAndroid
    {
        private static AndroidJavaClass AdaptyAndroidClass = new AndroidJavaClass("com.adapty.unity.AdaptyAndroidWrapper");

        internal static void SetLogLevel(string value)
        {
            AdaptyAndroidClass.CallStatic("setLogLevel", value);
        }

        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("identify", customerUserId, AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));

        internal static void Logout(Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("logout", AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));

        internal static void GetPaywall(string id, Action<Adapty.Paywall, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("getPaywall", id, AdaptyAndroidCallback.Action((string json) =>
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
                throw new Exception("Failed to invoke Action<Adapty.Paywall,Adapty.Error> completionHandler", e);
            }
        }));

        internal static void GetPaywallProducts(string paywall, string fetchPolicy, Action<IList<Adapty.PaywallProduct>, Adapty.Error> completionHandler)
           => AdaptyAndroidClass.CallStatic("getPaywallProducts", paywall, AdaptyAndroidCallback.Action((string json) =>
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
                throw new Exception("Failed to invoke Action<IList<Adapty.PaywallProduct>,Adapty.Error> completionHandler", e);
            }
        }));

        internal static void GetProfile(Action<Adapty.Profile, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("getProfile", AdaptyAndroidCallback.Action((string json) =>
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
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler", e);
            }
        }));

        internal static void RestorePurchases(Action<Adapty.Profile, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("restorePurchases", AdaptyAndroidCallback.Action((string json) =>
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
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler", e);
            }
        }));

        internal static void MakePurchase(string product, string subscriptionUpdate, Action<Adapty.Profile, Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("makePurchase", product, subscriptionUpdate, AdaptyAndroidCallback.Action((string json) =>
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
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler", e);
            }
        }));

        internal static void LogShowPaywall(string paywall, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("logShowPaywall", paywall, AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));

        internal static void LogShowOnboarding(string onboardingScreenParameters, Action<Adapty.Error> completionHandler)
             => AdaptyAndroidClass.CallStatic("logShowOnboarding", onboardingScreenParameters, AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("setFallbackPaywalls", paywalls, AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));


        internal static void UpdateProfile(string param, Action<Adapty.Error> completionHandler)
         => AdaptyAndroidClass.CallStatic("updateProfile", param, AdaptyAndroidCallback.Action((string json) =>
         {
             if (completionHandler == null) return;
             var error = JSONNode.Parse(json).GetErrorIfPresent("error");
             try
             {
                 completionHandler(error);
             }
             catch (Exception e)
             {
                 throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
             }
         }));

        internal static void UpdateAttribution(string jsonstring, string source, string networkUserId, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("updateAttribution", jsonstring, source, networkUserId, AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));



        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler)
            => AdaptyAndroidClass.CallStatic("setVariationId", transactionId, variationId, AdaptyAndroidCallback.Action((string json) =>
        {
            if (completionHandler == null) return;
            var error = JSONNode.Parse(json).GetErrorIfPresent("error");
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
            }
        }));

    }
#endif
}