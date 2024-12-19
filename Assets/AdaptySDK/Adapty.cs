using System;
using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public static readonly string SDKVersion = "3.3.0";

        public static void Activate(AdaptyConfiguration.Builder configurationBuilder, Action<AdaptyError> completionHandler) =>
            Activate(configurationBuilder.Build(), completionHandler);

        public static void Activate(AdaptyConfiguration configuration, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("configuration", configuration.ToJSONNode());

            Request.Send(
                "activate",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void GetPaywall(string placementId, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywall(placementId, null, null, null, completionHandler);

        public static void GetPaywall(string placementId, AdaptyPaywallFetchPolicy fetchPolicy, TimeSpan? loadTimeout, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywall(placementId, null, fetchPolicy, loadTimeout, completionHandler);

        public static void GetPaywall(string placementId, string locale, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywall(placementId, locale, null, null, completionHandler);

        public static void GetPaywall(string placementId, string locale, AdaptyPaywallFetchPolicy fetchPolicy, TimeSpan? loadTimeout, Action<AdaptyPaywall, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("placement_id", placementId);
            if (locale != null) parameters.Add("locale", locale);
            if (fetchPolicy != null) parameters.Add("fetch_policy", fetchPolicy.ToJSONNode());
            if (loadTimeout.HasValue) parameters.Add("load_timeout", loadTimeout.Value.TotalSeconds);

            Request.Send(
                "get_paywall",
                parameters,
                JSONNodeExtensions.GetPaywall,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void GetPaywallForDefaultAudience(string placementId, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
           GetPaywallForDefaultAudience(placementId, null, null, completionHandler);

        public static void GetPaywallForDefaultAudience(string placementId, AdaptyPaywallFetchPolicy fetchPolicy, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywallForDefaultAudience(placementId, null, fetchPolicy, completionHandler);

        public static void GetPaywallForDefaultAudience(string placementId, string locale, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywallForDefaultAudience(placementId, locale, null, completionHandler);

        public static void GetPaywallForDefaultAudience(string placementId, string locale, AdaptyPaywallFetchPolicy fetchPolicy, Action<AdaptyPaywall, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("placement_id", placementId);
            if (locale != null) parameters.Add("locale", locale);
            if (fetchPolicy != null) parameters.Add("fetch_policy", fetchPolicy.ToJSONNode());

            Request.Send(
                "get_paywall_for_default_audience",
                parameters,
                JSONNodeExtensions.GetPaywall,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void GetPaywallProducts(AdaptyPaywall paywall, Action<IList<AdaptyPaywallProduct>, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

            Request.Send(
                "get_paywall_products",
                parameters,
                JSONNodeExtensions.GetAdaptyPaywallProductList,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void GetProfile(Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            Request.Send(
                "get_profile",
                null,
                JSONNodeExtensions.GetAdaptyProfile,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void Identify(string customerUserId, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("customer_user_id", customerUserId);

            Request.Send(
                "identify",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void IsActivated(Action<bool, AdaptyError> completionHandler)
        {
            Request.Send(
                "is_activated",
                null,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void GetLoglevel(Action<AdaptyLogLevel, AdaptyError> completionHandler)
        {
            Request.Send(
                "get_log_level",
                null,
                JSONNodeExtensions.GetAdaptyLogLevel,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void SetLogLevel(AdaptyLogLevel level, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("value", level.ToJSONNode());

            Request.Send(
                "set_log_level",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void Logout(Action<AdaptyError> completionHandler)
        {
            Request.Send(
                "logout",
                null,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void LogShowOnboarding(string name, string screenName, uint screenOrder, Action<AdaptyError> completionHandler) =>
            LogShowOnboarding(new AdaptyOnboardingScreenParameters(name, screenName, screenOrder), completionHandler);

        public static void LogShowOnboarding(AdaptyOnboardingScreenParameters onbordingScreenparameters, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("params", onbordingScreenparameters.ToJSONNode());

            Request.Send(
                "log_show_onboarding",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void LogShowPaywall(AdaptyPaywall paywall, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

            Request.Send(
                "log_show_paywall",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }


        public static void MakePurchase(AdaptyPaywallProduct product, Action<AdaptyPurchaseResult, AdaptyError> completionHandler) =>
            MakePurchase(product, null, null, completionHandler);

        public static void MakePurchase(AdaptyPaywallProduct product, AdaptySubscriptionUpdateParameters subscriptionUpdate, Action<AdaptyPurchaseResult, AdaptyError> completionHandler) =>
            MakePurchase(product, subscriptionUpdate, null, completionHandler);

        public static void MakePurchase(AdaptyPaywallProduct product, AdaptySubscriptionUpdateParameters subscriptionUpdate, bool? isOfferPersonalized, Action<AdaptyPurchaseResult, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("product", product.ToJSONNode());
            if (subscriptionUpdate != null) parameters.Add("subscription_update_params", subscriptionUpdate.ToJSONNode());
            if (isOfferPersonalized.HasValue) parameters.Add("is_offer_personalized", isOfferPersonalized.Value);

            Request.Send(
                "make_purchase",
                parameters,
                JSONNodeExtensions.GetAdaptyPurchaseResult,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void PresentCodeRedemptionSheet(Action<AdaptyError> completionHandler)
        {
            Request.Send(
                "present_code_redemption_sheet",
                null,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void ReportTransaction(string transactionId, Action<AdaptyError> completionHandler) =>
            ReportTransaction(transactionId, null, completionHandler);
        public static void ReportTransaction(string transactionId, string variationId, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("transaction_id", transactionId);
            if (variationId != null) parameters.Add("variation_id", variationId);

            Request.Send(
                "report_transaction",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void RestorePurchases(Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            Request.Send(
                "restore_purchases",
                null,
                JSONNodeExtensions.GetAdaptyProfile,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void GetNativeSDKVersion(Action<string, AdaptyError> completionHandler)
        {
            Request.Send(
                "get_sdk_version",
                null,
                JSONNodeExtensions.GetString,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void SetFallbackPaywalls(string fileName, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("path", UnityEngine.Application.dataPath + "/Raw/" + fileName);

            Request.Send(
                "set_fallback_paywalls",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void SetIntegrationIdentifier(string key, string value, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            var identifier = new JSONObject();
            identifier.Add(key, value);
            parameters.Add("key_values", identifier);

            Request.Send(
                "set_integration_identifiers",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }


        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, string source, Action<AdaptyError> completionHandler) =>
            UpdateAttribution(attribution.ToJSONObject().ToString(), source, completionHandler);

        public static void UpdateAttribution(string jsonString, string source, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("attribution", jsonString);
            parameters.Add("source", source);

            Request.Send(
                "update_attribution_data",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void UpdateProfile(AdaptyProfileParameters param, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("params", param.ToJSONNode());

            Request.Send(
                "update_profile",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }
    }


    public static partial class AdaptyUI
    {
        
        public static void Activate(AdaptyUIConfiguration configuration, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("configuration", configuration.ToJSONNode());

            Request.Send(
                "adapty_ui_activate",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }


        public static void CreateView(AdaptyPaywall paywall, Action<AdaptyUIView, AdaptyError> completionHandler) =>
            CreateView(paywall, null, completionHandler);
        public static void CreateView(AdaptyPaywall paywall, AdaptyUICreateViewOptional optional, Action<AdaptyUIView, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());
            if (optional != null)
            {
                if (optional.LoadTimeout.HasValue) parameters.Add("load_timeout", optional.LoadTimeout.Value.TotalSeconds);

                if (optional.PreloadProducts.HasValue) parameters.Add("preload_products", optional.PreloadProducts.Value);

                if (optional.CustomTags != null)
                {
                    var node = new JSONObject();
                    foreach (KeyValuePair<string, string> entry in optional.CustomTags)
                    {
                        node.Add(entry.Key, entry.Value);
                    }
                    parameters.Add("custom_tags", node);
                }
                if (optional.CustomTimers != null)
                {
                    var node = new JSONObject();
                    foreach (KeyValuePair<string, DateTime> entry in optional.CustomTimers)
                    {
                        node.Add(entry.Key, entry.Value.ToJSONNode());
                    }
                    parameters.Add("custom_timers", node);
                }
                if (optional.AndroidPersonalizedOffers != null)
                {
                    var node = new JSONObject();
                    foreach (KeyValuePair<string, bool> entry in optional.AndroidPersonalizedOffers)
                    {
                        node.Add(entry.Key, entry.Value);
                    }
                    parameters.Add("android_personalized_offers", node);
                }
            }

            Request.Send(
                "adapty_ui_create_view",
                parameters,
                JSONNodeExtensions.GetAdaptyUIView,
                (value, error) => completionHandler?.Invoke(value, error)
            );
        }

        public static void DismissView(AdaptyUIView view, Action<AdaptyError> completionHandler) =>
            DismissView(view, false, completionHandler);

        public static void DismissView(AdaptyUIView view, bool destroy, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("destroy", destroy);

            Request.Send(
                "adapty_ui_dismiss_view",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

        public static void PresentView(AdaptyUIView view, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);

            Request.Send(
                "adapty_ui_present_view",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }

         public static void ShowDialog(AdaptyUIView view, AdaptyUIDialogConfiguration configuration, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("configuration", configuration.ToJSONNode());

            Request.Send(
                "adapty_ui_show_dialog",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) => completionHandler?.Invoke(error)
            );
        }
    }

    internal static class Request
    {
        internal static void Send<T>(string method, JSONObject request, Func<JSONNode, T> mapResponseValue, Action<T, AdaptyError> completionHandler)
        {
            string stringJson;
            try
            {
                if (request == null)
                {
                    request = new JSONObject();
                }
                request.Add("method", method);
                stringJson = request.ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, $"Failed encoding request: {method}", $"AdaptyUnityError.EncodingFailed({ex})");
                completionHandler(default(T), error);
                return;
            }

            _Adapty.Invoke(method, stringJson, (json) =>
            {
                var result = json.GetAdaptyResult(mapResponseValue);
                completionHandler(result.Value, result.Error);
            });
        }
    }
}