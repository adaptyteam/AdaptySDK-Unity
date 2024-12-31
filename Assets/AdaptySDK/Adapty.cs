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

    /// <summary>
    /// The main class for interacting with the Adapty SDK.
    /// </summary>
    public static partial class Adapty
    {
        /// <summary>
        /// The version of the Adapty SDK.
        /// </summary>
        public static readonly string SDKVersion = "3.3.0";


        /// <summary>
        /// Use this method to initialize the Adapty SDK.
        /// </summary>
        /// <param name="configurationBuilder">The builder to use for the configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Activate(AdaptyConfiguration.Builder configurationBuilder, Action<AdaptyError> completionHandler) =>
            Activate(configurationBuilder.Build(), completionHandler);

        /// <summary>
        /// Use this method to initialize the Adapty SDK.
        /// </summary>
        /// <param name="configuration">The configuration to use for the SDK.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Activate(AdaptyConfiguration configuration, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("configuration", configuration.ToJSONNode());

            Request.Send(
                "activate",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.Activate(..)", e);
                    }
                });
        }

        /// <summary>
        /// Adapty allows you remotely configure the products that will be displayed in your app.
        /// This way you don’t have to hardcode the products and can dynamically change offers or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywall(string placementId, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywall(placementId, null, null, null, completionHandler);

        /// <summary>
        /// Adapty allows you remotely configure the products that will be displayed in your app.
        /// This way you don’t have to hardcode the products and can dynamically change offers or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="loadTimeout">The timeout for the paywall loading.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywall(string placementId, AdaptyPaywallFetchPolicy fetchPolicy, TimeSpan? loadTimeout, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywall(placementId, null, fetchPolicy, loadTimeout, completionHandler);

        /// <summary>
        /// Adapty allows you remotely configure the products that will be displayed in your app.
        /// This way you don’t have to hardcode the products and can dynamically change offers or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywall(string placementId, string locale, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywall(placementId, locale, null, null, completionHandler);

        /// <summary>
        /// Adapty allows you remotely configure the products that will be displayed in your app.
        /// This way you don’t have to hardcode the products and can dynamically change offers or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="loadTimeout">The timeout for the paywall loading.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyPaywall,AdaptyError> completionHandler in Adapty.GetPaywall(..)", e);
                    }
                });
        }

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(string placementId, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
           GetPaywallForDefaultAudience(placementId, null, null, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(string placementId, AdaptyPaywallFetchPolicy fetchPolicy, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywallForDefaultAudience(placementId, null, fetchPolicy, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(string placementId, string locale, Action<AdaptyPaywall, AdaptyError> completionHandler) =>
            GetPaywallForDefaultAudience(placementId, locale, null, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyPaywall,AdaptyError> completionHandler in Adapty.GetPaywallForDefaultAudience(..)", e);
                    }
                });
        }

        /// <summary>
        /// Once you have a [AdaptyPaywall], fetch corresponding products array using this method.
        /// </summary>
        /// <param name="paywall">an [AdaptyPaywall] for which you want to get a products.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallProducts(AdaptyPaywall paywall, Action<IList<AdaptyPaywallProduct>, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

            Request.Send(
                "get_paywall_products",
                parameters,
                JSONNodeExtensions.GetAdaptyPaywallProductList,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<IList<AdaptyPaywallProduct>,AdaptyError> completionHandler in Adapty.GetPaywallProducts(..)", e);
                    }
                });
        }

        /// <summary>
        /// The main function for getting a user profile. Allows you to define the level of access, as well as other parameters.
        /// </summary>
        /// <remarks>
        /// The getProfile method provides the most up-to-date result as it always tries to query the API.
        /// If for some reason (e.g. no internet connection), the Adapty SDK fails to retrieve information from the server, the data from cache will be returned.
        /// It is also important to note that the Adapty SDK updates AdaptyProfile cache on a regular basis, in order to keep this information as up-to-date as possible.
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result. The result contains a [AdaptyProfile] object.</param>
        public static void GetProfile(Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            Request.Send(
                "get_profile",
                null,
                JSONNodeExtensions.GetAdaptyProfile,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyProfile,AdaptyError> completionHandler in Adapty.GetProfile(..)", e);
                    }
                });
        }

        /// <summary>
        /// Use this method for identifying user with it’s user id in your system.
        /// </summary>
        /// <remarks>
        /// If you don’t have a user id on SDK configuration, you can set it later at any time with `.identify()` method.
        /// The most common cases are after registration/authorization when the user switches from being an anonymous user to an authenticated user.
        /// </remarks>
        /// <param name="customerUserId">User identifier in your system.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Identify(string customerUserId, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("customer_user_id", customerUserId);

            Request.Send(
                "identify",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.Identify(..)", e);
                    }
                });
        }

        /// <summary>
        /// Checks if the Native SDK is activated.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void IsActivated(Action<bool, AdaptyError> completionHandler)
        {
            Request.Send(
                "is_activated",
                null,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<bool,AdaptyError> completionHandler in Adapty.IsActivated(..)", e);
                    }
                });
        }

        public static void GetLoglevel(Action<AdaptyLogLevel, AdaptyError> completionHandler)
        {
            Request.Send(
                "get_log_level",
                null,
                JSONNodeExtensions.GetAdaptyLogLevel,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyLogLevel,AdaptyError> completionHandler in Adapty.GetLoglevel(..)", e);
                    }
                });
        }

        /// <summary>
        /// Set to the most appropriate level of logging.
        /// </summary>
        /// <param name="level">AdaptyLogLevel value</param>
        /// <param name="completionHandler">Action whith the result</param>
        public static void SetLogLevel(AdaptyLogLevel level, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("value", level.ToJSONNode());

            Request.Send(
                "set_log_level",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetLogLevel(..)", e);
                    }
                });
        }

        /// <summary>
        /// You can logout the user anytime by calling this method.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Logout(Action<AdaptyError> completionHandler)
        {
            Request.Send(
                "logout",
                null,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.Logout(..)", e);
                    }
                });
        }

        /// <summary>
        /// Call this method to keep track of the user’s steps while onboarding
        /// </summary>
        /// <remarks>
        /// The onboarding stage is a very common situation in modern mobile apps.
        /// The quality of its implementation, content, and number of steps can have a rather significant influence on further user behavior, especially on his desire to become a subscriber or simply make some purchases.
        /// In order for you to be able to analyze user behavior at this critical stage without leaving Adapty, we have implemented the ability to send dedicated events every time a user visits yet another onboarding screen.
        /// </remarks>
        /// <param name="name">Name of your onboarding.</param>
        /// <param name="screenName">Readable name of a particular screen as part of onboarding.</param>
        /// <param name="screenOrder">An unsigned integer value representing the order of this screen in your onboarding sequence (it must me greater than 0).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void LogShowOnboarding(string name, string screenName, uint screenOrder, Action<AdaptyError> completionHandler) =>
            LogShowOnboarding(new AdaptyOnboardingScreenParameters(name, screenName, screenOrder), completionHandler);

        /// <summary>
        /// Call this method to keep track of the user’s steps while onboarding
        /// </summary>
        /// <remarks>
        /// The onboarding stage is a very common situation in modern mobile apps.
        /// The quality of its implementation, content, and number of steps can have a rather significant influence on further user behavior, especially on his desire to become a subscriber or simply make some purchases.
        /// In order for you to be able to analyze user behavior at this critical stage without leaving Adapty, we have implemented the ability to send dedicated events every time a user visits yet another onboarding screen.
        /// </remarks>
        /// <param name="onboardingScreenParameters">An [AdaptyOnboardingScreenParameters] object.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void LogShowOnboarding(AdaptyOnboardingScreenParameters onbordingScreenparameters, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("params", onbordingScreenparameters.ToJSONNode());

            Request.Send(
                "log_show_onboarding",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.LogShowOnboarding(..)", e);
                    }
                });
        }

        /// <summary>
        /// Call this method to notify Adapty SDK, that particular paywall was shown to user.
        /// </summary>
        /// <remarks>
        /// Adapty helps you to measure the performance of the paywalls.
        /// We automatically collect all the metrics related to purchases except for paywall views.
        /// This is because only you know when the paywall was shown to a customer. Whenever you show a paywall to your user, call .logShowPaywall(paywall) to log the event, and it will be accumulated in the paywall metrics.
        /// Read more on the <see href="https://adapty.io/docs/present-remote-config-paywalls#track-paywall-view-events">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An [AdaptyPaywall] object.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void LogShowPaywall(AdaptyPaywall paywall, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

            Request.Send(
                "log_show_paywall",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.LogShowPaywall(..)", e);
                    }
                });
        }

        /// <summary>
        /// To make the purchase, you have to call this method.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">an [AdaptyPaywallProduct] object retrieved from the paywall.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void MakePurchase(AdaptyPaywallProduct product, Action<AdaptyPurchaseResult, AdaptyError> completionHandler) =>
            MakePurchase(product, null, null, completionHandler);

        /// <summary>
        /// To make the purchase, you have to call this method.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">an [AdaptyPaywallProduct] object retrieved from the paywall.</param>
        /// <param name="subscriptionUpdate">an [AdaptySubscriptionUpdateParameters] object used to upgrade or downgrade a subscription (use for Android).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void MakePurchase(AdaptyPaywallProduct product, AdaptySubscriptionUpdateParameters subscriptionUpdate, Action<AdaptyPurchaseResult, AdaptyError> completionHandler) =>
            MakePurchase(product, subscriptionUpdate, null, completionHandler);

        /// <summary>
        /// To make the purchase, you have to call this method.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">an [AdaptyPaywallProduct] object retrieved from the paywall.</param>
        /// <param name="subscriptionUpdate">an [AdaptySubscriptionUpdateParameters] object used to upgrade or downgrade a subscription (use for Android).</param>
        /// <param name="isOfferPersonalized">Specifies whether the offer is personalized to the buyer (use for Android).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyPurchaseResult, AdaptyError> completionHandler in Adapty.MakePurchase(..)", e);
                    }
                });
        }

        /// <summary>
        /// Call this method to have StoreKit present a sheet enabling the user to redeem codes provided by your app.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentCodeRedemptionSheet(Action<AdaptyError> completionHandler)
        {
#if UNITY_IOS && !UNITY_EDITOR
            Request.Send(
                "present_code_redemption_sheet",
                null,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentCodeRedemptionSheet(..)", e);
                    }
                });
#else
            try
            {
                completionHandler?.Invoke(null);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentCodeRedemptionSheet(..)", e);
            }
#endif
        }

        /// <summary>
        /// In Observer mode, Adapty SDK doesn’t know, where the purchase was made from.
        /// If you display products using our <see href="https://docs.adapty.io/v2.0/docs/paywall">Paywalls</see> or <  see href="https://docs.adapty.io/v2.0/docs/ab-test">A/B Tests</see>, you can manually assign variation to the purchase.
        /// After doing this, you’ll be able to see metrics in Adapty Dashboard.
        /// </summary>
        /// <param name="transactionId">A string identifier of your purchased transaction <see href="https://developer.apple.com/documentation/storekit/skpaymenttransaction">SKPaymentTransaction</see> for iOS or string identifier (`purchase.getOrderId()`) of the purchase, where the purchase is an instance of the billing library Purchase class for Android.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ReportTransaction(string transactionId, Action<AdaptyError> completionHandler) =>
            ReportTransaction(transactionId, null, completionHandler);

        /// <summary>
        /// In Observer mode, Adapty SDK doesn’t know, where the purchase was made from.
        /// If you display products using our <see href="https://docs.adapty.io/v2.0/docs/paywall">Paywalls</see> or <  see href="https://docs.adapty.io/v2.0/docs/ab-test">A/B Tests</see>, you can manually assign variation to the purchase.
        /// After doing this, you’ll be able to see metrics in Adapty Dashboard.
        /// </summary>
        /// <param name="transactionId">A string identifier of your purchased transaction <see href="https://developer.apple.com/documentation/storekit/skpaymenttransaction">SKPaymentTransaction</see> for iOS or string identifier (`purchase.getOrderId()`) of the purchase, where the purchase is an instance of the billing library Purchase class for Android.</param>
        /// <param name="variationId">A string identifier of variation. You can get it using variationId property of AdaptyPaywall.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ReportTransaction(string transactionId, string variationId, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("transaction_id", transactionId);
            if (variationId != null) parameters.Add("variation_id", variationId);

            Request.Send(
                "report_transaction",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.ReportTransaction(..)", e);
                    }
                });
        }

        /// <summary>
        /// To restore purchases, you have to call this method.
        /// </summary>
        /// <remarks>
        /// A result containing the AdaptyProfile object. This model contains info about access levels, subscriptions, and non-subscription purchases.
        /// Generally, you have to check only access level status to determine whether the user has premium access to the app.
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void RestorePurchases(Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            Request.Send(
                "restore_purchases",
                null,
                JSONNodeExtensions.GetAdaptyProfile,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyProfile, AdaptyError> completionHandler in Adapty.RestorePurchases(..)", e);
                    }
                });
        }

        /// <summary>
        /// Returns the version of the Native SDK.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetNativeSDKVersion(Action<string, AdaptyError> completionHandler)
        {
            Request.Send(
                "get_sdk_version",
                null,
                JSONNodeExtensions.GetString,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<string, AdaptyError> completionHandler in Adapty.GetNativeSDKVersion(..)", e);
                    }
                });
        }

        /// <summary>
        /// To set fallback paywalls, use this method. You should pass exactly the same payload you’re getting from Adapty backend. You can copy it from Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// Adapty allows you to provide fallback paywalls that will be used when a user opens the app for the first time and there’s no internet connection or in the rare case when Adapty backend is down and there’s no cache on the device.
        /// Read more on the <see href="https://adapty.io/docs/unity-use-fallback-paywalls">Adapty Documentation</see>
        /// </remarks>
        /// <param name="fileName">The name of the fallback paywalls file.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void SetFallbackPaywalls(string fileName, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();

#if UNITY_IOS && !UNITY_EDITOR
            parameters.Add("path", UnityEngine.Application.dataPath + "/Raw/" + fileName);
#elif UNITY_ANDROID && !UNITY_EDITOR
            parameters.Add("path", "jar:file://" + UnityEngine.Application.dataPath + "!/assets/" + fileName);
#endif

            Request.Send(
                "set_fallback_paywalls",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetFallbackPaywalls(..)", e);
                    }
                });
        }

        /// <summary>
        /// You can set integration identifiers for the profile, using method.
        /// </summary>
        /// <param name="key">a identifier of the integration.</param>
        /// <param name="value">a value of the integration identifier.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetIntegrationIdentifier(..)", e);
                    }
                });
        }

        /// <summary>
        /// You can set attribution data for the profile, using method.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </summary>
        /// <param name="attribution">a map containing attribution (conversion) data.</param>
        /// <param name="source">a source of attribution.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, string source, Action<AdaptyError> completionHandler) =>
            UpdateAttribution(attribution.ToJSONObject().ToString(), source, completionHandler);

        /// <summary>
        /// You can set attribution data for the profile, using method.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </summary>
        /// <param name="jsonString">a serialized JSON string containing attribution (conversion) data.</param>
        /// <param name="source">a source of attribution.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateAttribution(string jsonString, string source, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("attribution", jsonString);
            parameters.Add("source", source);

            Request.Send(
                "update_attribution_data",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateAttribution(..)", e);
                    }
                });
        }

        /// <summary>
        /// You can set optional attributes such as email, phone number, etc, to the user of your app.
        /// You can then use attributes to create user <see href="https://adapty.io/docs/segments">segments</see> or just view them in CRM.
        /// </summary>
        /// <param name="param">use [AdaptyProfileParametersBuilder] to build this object.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateProfile(AdaptyProfileParameters param, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("params", param.ToJSONNode());

            Request.Send(
                "update_profile",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateProfile(..)", e);
                    }
                });
        }
    }

    public static partial class AdaptyUI
    {

        /// <summary>
        /// Right after receiving ``AdaptyPaywall``, you can create the corresponding ``AdaptyUIView`` to present it afterwards.
        /// </summary>
        /// <param name="paywall">an [AdaptyPaywall] object, for which you are trying to get a controller.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void CreateView(AdaptyPaywall paywall, Action<AdaptyUIView, AdaptyError> completionHandler) =>
            CreateView(paywall, null, completionHandler);

        /// <summary>
        /// Right after receiving ``AdaptyPaywall``, you can create the corresponding ``AdaptyUIView`` to present it afterwards.
        /// </summary>
        /// <param name="paywall">an [AdaptyPaywall] object, for which you are trying to get a controller.</param>
        /// <param name="optionalParameters">an [AdaptyUICreateViewParameters] object that contains optional parameters.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void CreateView(AdaptyPaywall paywall, AdaptyUICreateViewParameters optionalParameters, Action<AdaptyUIView, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());
            if (optionalParameters != null)
            {
                if (optionalParameters.LoadTimeout.HasValue) parameters.Add("load_timeout", optionalParameters.LoadTimeout.Value.TotalSeconds);

                if (optionalParameters.PreloadProducts.HasValue) parameters.Add("preload_products", optionalParameters.PreloadProducts.Value);

                if (optionalParameters.CustomTags != null)
                {
                    var node = new JSONObject();
                    foreach (KeyValuePair<string, string> entry in optionalParameters.CustomTags)
                    {
                        node.Add(entry.Key, entry.Value);
                    }
                    parameters.Add("custom_tags", node);
                }
                if (optionalParameters.CustomTimers != null)
                {
                    var node = new JSONObject();
                    foreach (KeyValuePair<string, DateTime> entry in optionalParameters.CustomTimers)
                    {
                        node.Add(entry.Key, entry.Value.ToJSONNode());
                    }
                    parameters.Add("custom_timers", node);
                }
                if (optionalParameters.AndroidPersonalizedOffers != null)
                {
                    var node = new JSONObject();
                    foreach (KeyValuePair<string, bool> entry in optionalParameters.AndroidPersonalizedOffers)
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
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyUIView, AdaptyError> completionHandler in Adapty.CreateView(..)", e);
                    }
                });
        }

        /// <summary>
        /// Call this function if you wish to dismiss the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIView] object, for which is representing the view.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param> 
        public static void DismissView(AdaptyUIView view, Action<AdaptyError> completionHandler) =>
            DismissView(view, false, completionHandler);

        private static void DismissView(AdaptyUIView view, bool destroy, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("destroy", destroy);

            Request.Send(
                "adapty_ui_dismiss_view",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.DismissView(..)", e);
                    }
                });
        }


        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIView] object, for which is representing the view.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentView(AdaptyUIView view, Action<AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);

            Request.Send(
                "adapty_ui_present_view",
                parameters,
                JSONNodeExtensions.GetBoolean,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentView(..)", e);
                    }
                });
        }

        /// <summary>
        /// Call this function if you wish to present the dialog.
        /// </summary>
        /// <param name="view">an [AdaptyUIView] object, for which is representing the view.</param>
        /// <param name="configuration">an [AdaptyUIDialogConfiguration] object that contains the dialog configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ShowDialog(AdaptyUIView view, AdaptyUIDialogConfiguration configuration, Action<AdaptyUIDialogActionType, AdaptyError> completionHandler)
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("configuration", configuration.ToJSONNode());

            Request.Send(
                "adapty_ui_show_dialog",
                parameters,
                JSONNodeExtensions.GetAdaptyUIDialogActionType,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<AdaptyUIDialogActionType, AdaptyError> completionHandler in Adapty.ShowDialog(..)", e);
                    }
                });
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