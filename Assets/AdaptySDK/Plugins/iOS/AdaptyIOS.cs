using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AdaptySDK.SimpleJSON;

namespace AdaptySDK.iOS
{
#if UNITY_IOS
    internal static class AdaptyIOS
    {

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setIdfaCollectionDisabled")]
        internal static extern void SetIdfaCollectionDisabled(bool disabled);

        //[DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_activate")]
        //internal static extern void Activate(string key, bool observeMode, string customerUserId);

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getLogLevel")]
        private static extern string _GetLogLevel();

        internal static Adapty.LogLevel GetLogLevel()
        {
            return Adapty.LogLevelFromString(_GetLogLevel());
        }

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setLogLevel")]
        private static extern void _SetLogLevel(string value);

        internal static void SetLogLevel(Adapty.LogLevel value)
        {
            _SetLogLevel(value.LogLevelToString());
        }

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_identify")]
        private static extern void _Identify(string customerUserId, IntPtr callback);

        internal static void Identify(string customerUserId, Action<Adapty.Error> completionHandler)
        {
            _Identify(customerUserId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() : JSON.Parse(json);
                if (Adapty.ResponseHasError(response))
                {
                    completionHandler(Adapty.ErrorFromJSON(response["error"]));
                } else
                {
                    completionHandler(null);
                }
            }));
        }

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logout")]
        private static extern void _Logout(IntPtr callback);

        internal static void Logout(Action<Adapty.Error> completionHandler)
        {
            _Logout(AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPaywalls")]
        private static extern void _GetPaywalls(bool forceUpdate, IntPtr callback);

        internal static void GetPaywalls(bool forceUpdate, Action<Adapty.GetPaywallsResponse,Adapty.Error> completionHandler)
        {
            _GetPaywalls(forceUpdate, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPurchaserInfo")]
        private static extern void _GetPurchaserInfo(bool forceUpdate, IntPtr callback);

        internal static void GetPurchaserInfo(bool forceUpdate, Action<Adapty.PurchaserInfo,Adapty.Error> completionHandler)
        {
            _GetPurchaserInfo(forceUpdate, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_restorePurchases")]
        private static extern void _RestorePurchases(IntPtr callback);

        internal static void RestorePurchases(Action<Adapty.RestorePurchasesResponse, Adapty.Error> completionHandler)
        {
            _RestorePurchases(AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_makePurchase")]
        private static extern void _MakePurchase(string productId, string variationId, string offerId, IntPtr callback);

        internal static void MakePurchase(string productId, string variationId, string offerId, Action<Adapty.MakePurchaseResponse, Adapty.Error> completionHandler)
        {
            _MakePurchase(productId, variationId, offerId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_makeDeferredPurchase")]
        private static extern void _MakeDeferredPurchase(string productId, IntPtr callback);

        internal static void MakeDeferredPurchase(string productId, Action<Adapty.MakePurchaseResponse,Adapty.Error> completionHandler)
        {
            _MakeDeferredPurchase(productId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_logShowPaywall")]
        private static extern void _LogShowPaywall(string variationId, IntPtr callback);

        internal static void LogShowPaywall(Adapty.Paywall paywall, Action<Adapty.Error> completionHandler)
        {
            _LogShowPaywall(paywall.VariationId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setFallbackPaywalls")]
        private static extern void _SetFallbackPaywalls(string paywalls, IntPtr callback);

        internal static void SetFallbackPaywalls(string paywalls, Action<Adapty.Error> completionHandler)
        {
            _SetFallbackPaywalls(paywalls, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getPromo")]
        private static extern void _GetPromo(IntPtr callback);

        internal static void GetPromo(Action<Adapty.Promo, Adapty.Error> completionHandler)
        {
            _GetPromo(AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_updateProfile")]
        private static extern void _UpdateProfile(string param, IntPtr callback);

        internal static void UpdateProfile(Adapty.ProfileParameterBuilder param, Action<Adapty.Error> completionHandler)
        {
            _UpdateProfile(param.ToJSONString(), AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_updateAttribution")]
        private static extern void _UpdateAttribution(string attributions, string source, string networkUserId, IntPtr callback);

        internal static void UpdateAttribution(string jsonstring, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler)
        {
            _UpdateAttribution(jsonstring, source.AttributionNetworkToString(), networkUserId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        internal static void UpdateAttribution(Dictionary<string, dynamic> attribution, Adapty.AttributionNetwork source, string networkUserId, Action<Adapty.Error> completionHandler)
        {
            UpdateAttribution(DictionaryExtensions.ToJSON(attribution).ToString(),source,networkUserId,completionHandler);
        }

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setExternalAnalyticsEnabled")]
        private static extern void _SetExternalAnalyticsEnabled(bool enabled, IntPtr callback);

        internal static void SetExternalAnalyticsEnabled(bool enabled, Action<Adapty.Error> completionHandler)
        {
            _SetExternalAnalyticsEnabled(enabled, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setVariationForTransaction")]
        private static extern void _SetVariationForTransaction(string variationId, string transactionId, IntPtr callback);

        internal static void SetVariationForTransaction(string variationId, string transactionId, Action<Adapty.Error> completionHandler)
        {
            _SetVariationForTransaction(variationId, transactionId, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
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

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_getApnsToken")]
        internal static extern string GetApnsToken();

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_setApnsToken")]
        internal static extern void SetApnsToken(string apnsToken);

        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_handlePushNotification")]
        private static extern void _HandlePushNotification(string userInfo, IntPtr callback);

        internal static void HandlePushNotification(string userInfo, Action<Adapty.Error> completionHandler)
        {
            _HandlePushNotification(userInfo, AdaptyIOSCallbackAction.ActionToIntPtr((string json) => {
                if (completionHandler == null) return;
                var response = (string.IsNullOrEmpty(json)) ? new JSONObject() :  JSON.Parse(json);
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

        internal static void HandlePushNotification(Dictionary<string, dynamic> userInfo, Action<Adapty.Error> completionHandler)
        {
            HandlePushNotification(DictionaryExtensions.ToJSON(userInfo).ToString(), completionHandler);
        }

            [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_presentCodeRedemptionSheet")]
        internal static extern void PresentCodeRedemptionSheet();

    }
#endif
}