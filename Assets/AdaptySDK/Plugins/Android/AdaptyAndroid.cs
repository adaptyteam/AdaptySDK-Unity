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
        private static bool ResponseHasError(JSONNode response)
        {
            if (response == null || response.IsNull) return false;
            var error = response["error"];
            return error != null && !error.IsNull;
        }

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
                if (ResponseHasError(response))
                {
                    completionHandler(new Adapty.Error(response["error"]));
                }
                else
                {
                    completionHandler(null);
                }

            }));
        }

        internal static void Logout(Action<Adapty.Error> completionHandler)
        { 
            AdaptyAndroidClass.CallStatic("logout", AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
                }
            }));
        }

        internal static void GetPaywalls(bool forceUpdate, Action<Adapty.GetPaywallsResponse, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("getPaywalls", forceUpdate, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(null, Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(new Adapty.GetPaywallsResponse(response["success"]), null);
                }
            }));
        }

        internal static void GetPurchaserInfo(bool forceUpdate, Action<Adapty.PurchaserInfo, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("getPurchaserInfo", forceUpdate, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(null, Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(Adapty.PurchaserInfoFromJSON(response["success"]), null);
                }
            }));
        }

        internal static void RestorePurchases(Action<Adapty.RestorePurchasesResponse, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("restorePurchases", AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(null, Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(new Adapty.RestorePurchasesResponse(response["success"]), null);
                }
            }));
        }

        internal static void MakePurchase(string productId, string variationId, Adapty.AndroidSubscriptionUpdate subscriptionUpdate, Action<Adapty.MakePurchaseResponse, Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("makePurchase", productId, variationId, (subscriptionUpdate == null) ? null: subscriptionUpdate.ToJSONString(), AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(null, Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(new Adapty.MakePurchaseResponse(response["success"]), null);
                }
            }));
        }

        internal static void LogShowPaywall(Adapty.Paywall paywall, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("logShowPaywall", paywall.VariationId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
                }
            }));
        }

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("setFallbackPaywalls", paywalls, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
                }
            }));
        }

        internal static void GetPromo(Action<Adapty.Promo, Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("getPromo",  AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(null, Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(Adapty.PromoFromJSON(response["success"]), null);
                }
            }));
        }

        internal static void UpdateProfile(Adapty.ProfileParameterBuilder param, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("updateProfile", param.ToJSONString(), AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
                }
            }));
        }

        internal static void UpdateAttribution(string attributionJson, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler)
        {
            AdaptyAndroidClass.CallStatic("updateAttribution", attributionJson, source.AttributionNetworkToString(), networkUserId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
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
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
                }
            }));
        }


        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler) {
            AdaptyAndroidClass.CallStatic("setTransactionVariationId", transactionId, variationId, AdaptyAndroidCallback.Action((string json) =>
            {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                }
                else
                {
                    completionHandler(null);
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