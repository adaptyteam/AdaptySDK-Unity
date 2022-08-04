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

        //internal static void Activate(string key, bool observeMode, string customerUserId)
        //{
        //    AdaptyAndroidClass.CallStatic("activate",  key, customerUserId, observeMode); 
        //}

        internal static Adapty.LogLevel GetLogLevel() { return Adapty.LogLevel.None; }

        internal static  void SetLogLevel(Adapty.LogLevel value)
        {
            AdaptyAndroidClass.CallStatic("setLogLevel", value.LogLevelToString());
        }


        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("identify", customerUserId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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

        internal static void Logout(Action<Adapty.Error> completionHandler)
        { 
            AdaptyAndroidClass.CallStatic("logout", AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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

        internal static void GetPaywalls(bool forceUpdate, Action<Adapty.GetPaywallsResponse, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("getPaywalls", forceUpdate, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
                var result = error != null ? null : new Adapty.GetPaywallsResponse(response["success"]);
                try
                {
                    completionHandler(result, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.GetPaywallsResponse,Adapty.Error> completionHandler", e);
                }
            }));
        }

        internal static void GetPurchaserInfo(bool forceUpdate, Action<Adapty.PurchaserInfo, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("getPurchaserInfo", forceUpdate, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
                var result = error != null ? null : Adapty.PurchaserInfoFromJSON(response["success"]);
                try
                {
                    completionHandler(result, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.PurchaserInfo,Adapty.Error> completionHandler", e);
                }
            }));
        }

        internal static void RestorePurchases(Action<Adapty.RestorePurchasesResponse, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("restorePurchases", AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
                var result = error != null ? null : new Adapty.RestorePurchasesResponse(response["success"]);
                try
                {
                    completionHandler(result, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.RestorePurchasesResponse,Adapty.Error> completionHandler", e);
                }
            }));
        }

        internal static void MakePurchase(string productId, string variationId, Adapty.AndroidSubscriptionUpdate subscriptionUpdate, Action<Adapty.MakePurchaseResponse, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("makePurchase", productId, variationId, (subscriptionUpdate == null) ? null: subscriptionUpdate.ToJSONString(), AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
                var result = error != null ? null : new Adapty.MakePurchaseResponse(response["success"]);
                try
                {
                    completionHandler(result, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.MakePurchaseResponse,Adapty.Error> completionHandler", e);
                }
            }));
        }

        internal static void LogShowPaywall(Adapty.Paywall paywall, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("logShowPaywall", paywall.VariationId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("setFallbackPaywalls", paywalls, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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

        internal static void GetPromo(Action<Adapty.Promo, Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("getPromo",  AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
                var result = error != null ? null : Adapty.PromoFromJSON(response["success"]);
                try
                {
                    completionHandler(result, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler", e);
                }
            }));
        }

        internal static void UpdateProfile(Adapty.ProfileParameterBuilder param, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("updateProfile", param.ToJSONString(), AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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

        internal static void UpdateAttribution(string attributionJson, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("updateAttribution", attributionJson, source.AttributionNetworkToString(), networkUserId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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

        internal static void UpdateAttribution(Dictionary<string, dynamic> attribution, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler) {
            UpdateAttribution(DictionaryExtensions.ToJSON(attribution).ToString(), source, networkUserId, completionHandler);
        }

        internal static void SetExternalAnalyticsEnabled(bool enabled, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("setExternalAnalyticsEnabled", enabled, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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


        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("setTransactionVariationId", transactionId, variationId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                var error = Adapty.ExtructErrorFromResponse(response);
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


        internal static void NewPushToken(string pushToken)
        {
            AdaptyAndroidClass.CallStatic("newPushToken", pushToken);
        }

        internal static void PushReceived(string pushMessageJson)
        {
            AdaptyAndroidClass.CallStatic("pushReceived", pushMessageJson);
        }

    }
#endif
}