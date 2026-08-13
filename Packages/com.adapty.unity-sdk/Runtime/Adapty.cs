using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif
using AdaptySDK.Serialization;
using Newtonsoft.Json.Linq;

namespace AdaptySDK
{
    /// <summary>
    /// The main class for interacting with the Adapty SDK.
    /// </summary>
    public static partial class Adapty
    {
        /// <summary>
        /// The version of the Adapty SDK.
        /// </summary>
        public static readonly string SDKVersion = "4.0.0-beta.2";

        /// <summary>
        /// Use this method to initialize the Adapty SDK.
        /// </summary>
        /// <param name="configurationBuilder">The builder to use for the configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Activate(
            AdaptyConfiguration.Builder configurationBuilder,
            Action<AdaptyError> completionHandler
        ) => Activate(configurationBuilder.Build(), completionHandler);

        /// <summary>
        /// Use this method to initialize the Adapty SDK.
        /// </summary>
        /// <param name="configuration">The configuration to use for the SDK.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Activate(
            AdaptyConfiguration configuration,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["configuration"] = AdaptyJson.ToNode(configuration);

            AdaptyRequest.SendVoid("activate", parameters, completionHandler);
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
        public static void GetFlow(
            string placementId,
            Action<AdaptyFlow, AdaptyError> completionHandler
        ) => GetFlow(placementId, null, null, completionHandler);

        /// <summary>
        /// Adapty allows you remotely configure the products that will be displayed in your app.
        /// This way you don’t have to hardcode the products and can dynamically change offers or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="loadTimeout">The timeout for the flow loading.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetFlow(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            TimeSpan? loadTimeout,
            Action<AdaptyFlow, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["placement_id"] = placementId;

            if (fetchPolicy != null)
            {
                parameters["fetch_policy"] = AdaptyJson.ToNode(fetchPolicy);
            }

            if (loadTimeout.HasValue)
            {
                parameters["load_timeout"] = loadTimeout.Value.TotalSeconds;
            }

            AdaptyRequest.Send("get_flow", parameters, completionHandler);
        }

        /// <summary>
        /// This method enables you to retrieve the flow from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products#speed-up-paywall-fetching-with-default-audience-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetFlowForDefaultAudience(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyFlow, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["placement_id"] = placementId;

            if (fetchPolicy != null)
            {
                parameters["fetch_policy"] = AdaptyJson.ToNode(fetchPolicy);
            }

            AdaptyRequest.Send("get_flow_for_default_audience", parameters, completionHandler);
        }

        /// <summary>
        /// Fetches the products array for a given flow.
        /// </summary>
        /// <remarks>
        /// Once you have an <see cref="AdaptyFlow"/>, use this method to fetch the corresponding products with full pricing and subscription information.
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="flow">An <see cref="AdaptyFlow"/> for which you want to get the products.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains a list of <see cref="AdaptyPaywallProduct"/> objects.</param>
        public static void GetPaywallProducts(
            AdaptyFlow flow,
            Action<IReadOnlyList<AdaptyPaywallProduct>, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["flow"] = AdaptyJson.ToNode(flow);

            AdaptyRequest.Send<List<AdaptyPaywallProduct>>(
                "get_paywall_products",
                parameters,
                (value, error) =>
                    completionHandler?.Invoke(
                        value is null ? null : new ReadOnlyCollection<AdaptyPaywallProduct>(value),
                        error
                    )
            );
        }

        /// <summary>
        /// Retrieves the current user profile with access levels, subscriptions, and other information.
        /// </summary>
        /// <remarks>
        /// The GetProfile method provides the most up-to-date result as it always tries to query the API.
        /// If for some reason (e.g., no internet connection), the Adapty SDK fails to retrieve information from the server, the data from cache will be returned.
        /// It is also important to note that the Adapty SDK updates the AdaptyProfile cache on a regular basis to keep this information as up-to-date as possible.
        /// Read more at <see href="https://adapty.io/docs/unity-check-subscription-status">Adapty Documentation</see>
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyProfile"/> object.</param>
        public static void GetProfile(Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            AdaptyRequest.Send("get_profile", null, completionHandler);
        }

        /// <summary>
        /// Identifies the user with their user ID in your system.
        /// </summary>
        /// <remarks>
        /// If you don't have a user ID in the SDK configuration, you can set it later at any time with this method.
        /// The most common cases are after registration/authorization when the user switches from being an anonymous user to an authenticated user.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-identify">Adapty Documentation</see>
        /// </remarks>
        /// <param name="customerUserId">The user identifier in your system.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Identify(string customerUserId, Action<AdaptyError> completionHandler)
        {
            Identify(customerUserId, Guid.Empty, null, completionHandler);
        }

        /// <summary>
        /// Identifies the user with their user ID and platform-specific account identifiers.
        /// </summary>
        /// <remarks>
        /// If you don't have a user ID in the SDK configuration, you can set it later at any time with this method.
        /// The most common cases are after registration/authorization when the user switches from being an anonymous user to an authenticated user.
        /// This overload allows you to provide platform-specific account identifiers for better purchase tracking.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-identify">Adapty Documentation</see>
        /// </remarks>
        /// <param name="customerUserId">The user identifier in your system.</param>
        /// <param name="iosAppAccountToken">The UUID that you generate to associate a customer's In-App Purchase with its resulting App Store transaction (iOS only). Read more at <see href="https://developer.apple.com/documentation/appstoreserverapi/appaccounttoken">Apple Documentation</see>.</param>
        /// <param name="androidObfuscatedAccountId">The obfuscated account identifier (Android only). Read more at <see href="https://developer.android.com/google/play/billing/developer-payload#attribute">Android Documentation</see>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Identify(
            string customerUserId,
            Guid iosAppAccountToken,
            string androidObfuscatedAccountId,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();

            parameters["customer_user_id"] = customerUserId;

            var customerIdentity = new AdaptyCustomerIdentity(
                iosAppAccountToken,
                androidObfuscatedAccountId
            );

            if (!customerIdentity.IsEmpty)
            {
                parameters["parameters"] = AdaptyJson.ToNode(customerIdentity);
            }

            AdaptyRequest.SendVoid("identify", parameters, completionHandler);
        }

        /// <summary>
        /// Checks if the native Adapty SDK is activated and ready to use.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result. The result contains a boolean value indicating whether the SDK is activated.</param>
        public static void IsActivated(Action<bool, AdaptyError> completionHandler)
        {
            AdaptyRequest.Send("is_activated", null, completionHandler);
        }

        /// <summary>
        /// Returns the current log level of the Adapty SDK.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the current <see cref="AdaptyLogLevel"/> value.</param>
        public static void GetLoglevel(Action<AdaptyLogLevel, AdaptyError> completionHandler)
        {
            AdaptyRequest.Send("get_log_level", null, completionHandler);
        }

        /// <summary>
        /// Sets the log level for the Adapty SDK.
        /// </summary>
        /// <remarks>
        /// Use this method to control the verbosity of SDK logging. Available levels include Error, Warn, Info, and Verbose.
        /// </remarks>
        /// <param name="level">The <see cref="AdaptyLogLevel"/> value to set.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void SetLogLevel(AdaptyLogLevel level, Action<AdaptyError> completionHandler)
        {
            var parameters = new JObject();
            parameters["value"] = AdaptyJson.ToNode(level);

            AdaptyRequest.SendVoid("set_log_level", parameters, completionHandler);
        }

        /// <summary>
        /// Returns the current installation status of the app.
        /// </summary>
        /// <remarks>
        /// This method provides information about whether the app installation status has been determined.
        /// Read more at <see href="https://adapty.io/docs/user-acquisition">Adapty Documentation</see>
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyInstallationStatus"/> object.</param>
        public static void GetCurrentInstallationStatus(
            Action<AdaptyInstallationStatus, AdaptyError> completionHandler
        )
        {
            AdaptyRequest.Send("get_current_installation_status", null, completionHandler);
        }

        /// <summary>
        /// Logs out the current user and clears the local profile data.
        /// </summary>
        /// <remarks>
        /// After calling this method, the SDK will create a new anonymous profile for the next user session.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-identify#log-users-out">Adapty Documentation</see>
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Logout(Action<AdaptyError> completionHandler)
        {
            AdaptyRequest.SendVoid("logout", null, completionHandler);
        }

        /// <summary>
        /// Creates a web URL for the paywall that can be used to display the paywall in a web view or browser.
        /// </summary>
        /// <remarks>
        /// This is useful for platforms that don't support native paywall views or for web-based implementations.
        /// Read more at <see href="https://adapty.io/docs/web-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyFlowPaywall"/> object for which to create the web URL.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the web URL string.</param>
        public static void CreateWebPaywallUrl(
            AdaptyFlowPaywall paywall,
            Action<string, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["paywall"] = AdaptyJson.ToNode(paywall);

            AdaptyRequest.Send("create_web_paywall_url", parameters, completionHandler);
        }

        /// <summary>
        /// Creates a web URL for a specific product that can be used to display the purchase page in a web view or browser.
        /// </summary>
        /// <remarks>
        /// This is useful for platforms that don't support native purchase flows or for web-based implementations.
        /// Read more at <see href="https://adapty.io/docs/web-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">An <see cref="AdaptyPaywallProduct"/> object for which to create the web URL.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the web URL string.</param>
        public static void CreateWebPaywallUrl(
            AdaptyPaywallProduct product,
            Action<string, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["product"] = AdaptyJson.ToNode(new AdaptyPaywallProductRequest(product));

            AdaptyRequest.Send("create_web_paywall_url", parameters, completionHandler);
        }

        /// <summary>
        /// Opens the paywall in a web view or browser.
        /// </summary>
        /// <remarks>
        /// This method opens the web paywall URL in the default browser or web view.
        /// Read more at <see href="https://adapty.io/docs/web-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyFlowPaywall"/> object to open.</param>
        /// <param name="openIn">Controls whether to open in external browser or in-app browser. Default is <see cref="AdaptyWebPresentation.ExternalBrowser"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void OpenWebPaywall(
            AdaptyFlowPaywall paywall,
            AdaptyWebPresentation openIn,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["paywall"] = AdaptyJson.ToNode(paywall);
            parameters["open_in"] = AdaptyJson.ToNode(openIn);

            AdaptyRequest.SendVoid("open_web_paywall", parameters, completionHandler);
        }

        /// <summary>
        /// Opens the product purchase page in a web view or browser.
        /// </summary>
        /// <remarks>
        /// This method opens the web purchase URL for the specific product in the default browser or web view.
        /// Read more at <see href="https://adapty.io/docs/web-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">An <see cref="AdaptyPaywallProduct"/> object to open.</param>
        /// <param name="openIn">Controls whether to open in external browser or in-app browser. Default is <see cref="AdaptyWebPresentation.ExternalBrowser"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void OpenWebPaywall(
            AdaptyPaywallProduct product,
            AdaptyWebPresentation openIn,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["product"] = AdaptyJson.ToNode(new AdaptyPaywallProductRequest(product));
            parameters["open_in"] = AdaptyJson.ToNode(openIn);

            AdaptyRequest.SendVoid("open_web_paywall", parameters, completionHandler);
        }

        /// <summary>
        /// Call this method to notify Adapty SDK, that particular flow was shown to user.
        /// </summary>
        /// <remarks>
        /// Adapty helps you to measure the performance of the flows.
        /// We automatically collect all the metrics related to purchases except for flow views.
        /// This is because only you know when the flow was shown to a customer. Whenever you show a flow to your user, call .LogShowFlow(flow) to log the event, and it will be accumulated in the flow metrics.
        /// Read more on the <see href="https://adapty.io/docs/present-remote-config-paywalls-unity#track-paywall-view-events">Adapty Documentation</see>
        /// </remarks>
        /// <param name="flow">An [AdaptyFlow] object.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void LogShowFlow(AdaptyFlow flow, Action<AdaptyError> completionHandler)
        {
            var parameters = new JObject();
            parameters["flow"] = AdaptyJson.ToNode(flow);

            AdaptyRequest.SendVoid("log_show_flow", parameters, completionHandler);
        }

        /// <summary>
        /// Updates the current user's refund data collection consent for App Store purchases.
        /// </summary>
        /// <remarks>
        /// This method is iOS-only and allows you to manage user consent for refund data collection.
        /// Read more on the <see href="https://adapty.io/docs/refund-saver#obtain-user-consent">Adapty Documentation</see>
        /// </remarks>
        /// <param name="consent">A boolean value indicating whether the user gave consent for refund data collection.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateAppStoreCollectingRefundDataConsent(
            Boolean consent,
            Action<AdaptyError> completionHandler
        )
        {
#if UNITY_IOS || UNITY_EDITOR
            var parameters = new JObject();
            parameters["consent"] = consent;

            AdaptyRequest.SendVoid("update_collecting_refund_data_consent", parameters, completionHandler);
#else
            AdaptyCallbacks.InvokeSafe(
                () => completionHandler?.Invoke(null),
                $"Failed to invoke completionHandler in {nameof(UpdateAppStoreCollectingRefundDataConsent)}(..)"
            );
#endif
        }

        /// <summary>
        /// Sets the refund preference individually for the current user.
        /// </summary>
        /// <remarks>
        /// This method is iOS-only and allows you to set how refunds should be handled for a specific user.
        /// Read more on the <see href="https://adapty.io/docs/refund-saver#set-refund-behavior-for-a-specific-user-in-the-dashboard">Adapty Documentation</see>
        /// </remarks>
        /// <param name="refundPreference">The <see cref="AdaptyRefundPreference"/> value to set.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateAppStoreRefundPreference(
            AdaptyRefundPreference refundPreference,
            Action<AdaptyError> completionHandler
        )
        {
#if UNITY_IOS || UNITY_EDITOR
            var parameters = new JObject();
            parameters["refund_preference"] = AdaptyJson.ToNode(refundPreference);

            AdaptyRequest.SendVoid("update_refund_preference", parameters, completionHandler);
#else
            AdaptyCallbacks.InvokeSafe(
                () => completionHandler?.Invoke(null),
                $"Failed to invoke completionHandler in {nameof(UpdateAppStoreRefundPreference)}(..)"
            );
#endif
        }

        /// <summary>
        /// Makes a purchase for the specified product.
        /// </summary>
        /// <remarks>
        /// This method initiates the purchase flow for a product. The result contains information about the purchase status.
        /// Read more on the <see href="https://adapty.io/docs/unity-making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">An <see cref="AdaptyPaywallProduct"/> object retrieved from the paywall.</param>
        /// <param name="purchaseParameters">Android only. An optional <see cref="AdaptyPurchaseParameters"/> object containing purchase configuration (e.g., subscription update parameters, offer personalization, etc.). Ignored on iOS.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyPurchaseResult"/> object.</param>
        public static void MakePurchase(
            AdaptyPaywallProduct product,
            AdaptyPurchaseParameters purchaseParameters,
            Action<AdaptyPurchaseResult, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["product"] = AdaptyJson.ToNode(new AdaptyPaywallProductRequest(product));
            if (purchaseParameters != null)
            {
                parameters["parameters"] = AdaptyJson.ToNode(purchaseParameters);
            }

            AdaptyRequest.Send("make_purchase", parameters, completionHandler);
        }

        /// <summary>
        /// Presents the App Store code redemption sheet, enabling the user to redeem promotional codes.
        /// </summary>
        /// <remarks>
        /// This method is iOS-only and presents the native App Store code redemption interface.
        /// Read more at <see href="https://developer.apple.com/documentation/storekit/appstore/presentoffercoderedeemsheet(in:)">Apple Documentation</see>
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentCodeRedemptionSheet(Action<AdaptyError> completionHandler)
        {
#if UNITY_IOS || UNITY_EDITOR
            AdaptyRequest.SendVoid("present_code_redemption_sheet", null, completionHandler);
#else
            AdaptyCallbacks.InvokeSafe(
                () => completionHandler?.Invoke(null),
                $"Failed to invoke completionHandler in {nameof(PresentCodeRedemptionSheet)}(..)"
            );
#endif
        }

        /// <summary>
        /// Reports a transaction to Adapty in Observer mode.
        /// </summary>
        /// <remarks>
        /// In Observer mode, Adapty SDK doesn't know where the purchase was made from.
        /// If you display products using Adapty Paywalls or A/B Tests, you can manually assign a variation to the purchase.
        /// After doing this, you'll be able to see metrics in the Adapty Dashboard.
        /// Read more at <see href="https://adapty.io/docs/observer-vs-full-mode">Adapty Documentation</see>
        /// </remarks>
        /// <param name="transactionId">A string identifier of your purchased transaction. For iOS, use the transaction identifier from <see href="https://developer.apple.com/documentation/storekit/skpaymenttransaction">SKPaymentTransaction</see>. For Android, use the order ID from the purchase object (`purchase.getOrderId()`).</param>
        /// <param name="variationId">An optional string identifier of the variation. You can get it using the <see cref="AdaptyFlow.VariationId"/> property of <see cref="AdaptyFlow"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ReportTransaction(
            string transactionId,
            string variationId,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["transaction_id"] = transactionId;
            if (variationId != null)
            {
                parameters["variation_id"] = variationId;
            }

            AdaptyRequest.SendVoid("report_transaction", parameters, completionHandler);
        }

        /// <summary>
        /// Restores previous purchases made by the user.
        /// </summary>
        /// <remarks>
        /// This method restores all previous purchases and returns an <see cref="AdaptyProfile"/> object containing information about access levels, subscriptions, and non-subscription purchases.
        /// Generally, you only need to check the access level status to determine whether the user has premium access to the app.
        /// Read more at <see href="https://adapty.io/docs/unity-restore-purchase">Adapty Documentation</see>
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyProfile"/> object.</param>
        public static void RestorePurchases(Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            AdaptyRequest.Send("restore_purchases", null, completionHandler);
        }

        /// <summary>
        /// Returns the version of the native Adapty SDK (iOS or Android).
        /// </summary>
        /// <remarks>
        /// This method returns the version string of the underlying native SDK, which may differ from the Unity wrapper version.
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the native SDK version string.</param>
        public static void GetNativeSDKVersion(Action<string, AdaptyError> completionHandler)
        {
            AdaptyRequest.Send("get_sdk_version", null, completionHandler);
        }

        /// <summary>
        /// Sets fallback paywalls that will be used when there's no internet connection or when the Adapty backend is unavailable.
        /// </summary>
        /// <remarks>
        /// Adapty allows you to provide fallback paywalls that will be used when a user opens the app for the first time and there's no internet connection, or in the rare case when the Adapty backend is down and there's no cache on the device.
        /// You should pass exactly the same payload you're getting from the Adapty backend. You can copy it from the Adapty Dashboard.
        /// The fallback paywalls file should be placed in the <c>StreamingAssets</c> folder in your Unity project.
        /// Read more on the <see href="https://adapty.io/docs/unity-use-fallback-paywalls">Adapty Documentation</see>
        /// </remarks>
        /// <param name="fileName">The name of the fallback paywalls file. The file should be placed in the <c>StreamingAssets</c> folder in your Unity project.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void SetFallback(string fileName, Action<AdaptyError> completionHandler)
        {
            var parameters = new JObject();

#if UNITY_IOS && !UNITY_EDITOR
            parameters["path"] = UnityEngine.Application.dataPath + "/Raw/" + fileName;
#elif UNITY_ANDROID && !UNITY_EDITOR
            parameters["path"] = "jar:file://" + UnityEngine.Application.dataPath + "!/assets/" + fileName;
#endif

            AdaptyRequest.SendVoid("set_fallback", parameters, completionHandler);
        }

        /// <summary>
        /// Sets integration identifiers for the profile to integrate with third-party analytics and attribution services.
        /// </summary>
        /// <remarks>
        /// Integration identifiers allow you to link Adapty profiles with external services like analytics platforms or attribution providers.
        /// </remarks>
        /// <param name="key">The identifier key of the integration (e.g., "amplitude_user_id", "mixpanel_distinct_id").</param>
        /// <param name="value">The value of the integration identifier.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void SetIntegrationIdentifier(
            string key,
            string value,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            var identifier = new JObject { [key] = value };
            parameters["key_values"] = identifier;

            AdaptyRequest.SendVoid("set_integration_identifiers", parameters, completionHandler);
        }

        /// <summary>
        /// Updates attribution data for the profile to track user acquisition sources.
        /// </summary>
        /// <remarks>
        /// This method allows you to send attribution data from various sources (e.g., AppsFlyer, Adjust, Branch) to Adapty.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </remarks>
        /// <param name="jsonString">A serialized JSON string containing attribution (conversion) data from the attribution provider.</param>
        /// <param name="source">The source of attribution (e.g., "appsflyer", "adjust", "branch", "custom").</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateAttribution(
            string jsonString,
            string source,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["attribution"] = jsonString;
            parameters["source"] = source;

            AdaptyRequest.SendVoid("update_attribution_data", parameters, completionHandler);
        }

        /// <summary>
        /// Updates the user profile with optional attributes such as email, phone number, name, etc.
        /// </summary>
        /// <remarks>
        /// You can use these attributes to create user <see href="https://adapty.io/docs/segments">segments</see> or view them in the CRM.
        /// Use <see cref="AdaptyProfileParameters.Builder"/> to build the parameters object.
        /// Read more at <see href="https://adapty.io/docs/unity-setting-user-attributes">Adapty Documentation</see>
        /// </remarks>
        /// <param name="param">An <see cref="AdaptyProfileParameters"/> object containing the attributes to update. Use <see cref="AdaptyProfileParameters.Builder"/> to build this object.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateProfile(
            AdaptyProfileParameters param,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["params"] = AdaptyJson.ToNode(param);

            AdaptyRequest.SendVoid("update_profile", parameters, completionHandler);
        }
    }

    /// <summary>
    /// Building, presenting and dismissing the views that render a flow.
    /// </summary>
    public static partial class AdaptyUI
    {
        /// <summary>
        /// Creates a flow view from an AdaptyFlow object.
        /// </summary>
        /// <remarks>
        /// Right after receiving an <see cref="AdaptyFlow"/>, you can create the corresponding <see cref="AdaptyUIFlowView"/> to present it afterwards.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
        /// </remarks>
        /// <param name="flow">An <see cref="AdaptyFlow"/> object for which you are trying to create a view.</param>
        /// <param name="optionalParameters">An optional <see cref="AdaptyUICreateFlowViewParameters"/> object that contains optional parameters like load timeout, custom tags, custom timers, product purchase parameters, and custom assets.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyUIFlowView"/> object.</param>
        public static void CreateFlowView(
            AdaptyFlow flow,
            AdaptyUICreateFlowViewParameters optionalParameters,
            Action<AdaptyUIFlowView, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["flow"] = AdaptyJson.ToNode(flow);

            if (optionalParameters != null)
            {
                // The optional parameters are contract members of the same request object, not a
                // nested one, so the serialized form is merged in rather than added under a key.
                foreach (var entry in (JObject)AdaptyJson.ToNode(optionalParameters))
                {
                    parameters[entry.Key] = entry.Value;
                }
            }

            AdaptyRequest.Send("adapty_ui_create_flow_view", parameters, completionHandler);
        }

        /// <summary>
        /// Dismisses the flow view.
        /// </summary>
        /// <remarks>
        /// Call this method when you want to dismiss the flow view from the screen.
        /// A dismissed view is released and cannot be presented again — create a new view via <see cref="CreateFlowView(AdaptyFlow, AdaptyUICreateFlowViewParameters, Action{AdaptyUIFlowView, AdaptyError})"/> if you need to re-present it.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIFlowView"/> object representing the view to dismiss.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void DismissFlowView(
            AdaptyUIFlowView view,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["id"] = view.Id;
            parameters["destroy"] = true;

            AdaptyRequest.SendVoid("adapty_ui_dismiss_flow_view", parameters, completionHandler);
        }

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIFlowView] object, for which is representing the view.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentFlowView(
            AdaptyUIFlowView view,
            Action<AdaptyError> completionHandler
        )
        {
            PresentFlowView(view, AdaptyUIIOSPresentationStyle.FullScreen, completionHandler);
        }

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIFlowView] object, for which is representing the view.</param>
        /// <param name="iosPresentationStyle">an [AdaptyUIIOSPresentationStyle] object, for which is representing the iOS presentation style.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentFlowView(
            AdaptyUIFlowView view,
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["id"] = view.Id;
            parameters["ios_presentation_style"] = AdaptyJson.ToNode(iosPresentationStyle);

            AdaptyRequest.SendVoid("adapty_ui_present_flow_view", parameters, completionHandler);
        }

        /// <summary>
        /// Presents a dialog on the flow view.
        /// </summary>
        /// <remarks>
        /// This method shows a dialog with custom configuration on the flow view. The dialog can be used for various purposes like showing terms, privacy policy, or custom messages.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIFlowView"/> object representing the view on which to show the dialog.</param>
        /// <param name="configuration">An <see cref="AdaptyUIDialogConfiguration"/> object that contains the dialog configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the <see cref="AdaptyUIDialogActionType"/> indicating which action was taken.</param>
        public static void ShowDialog(
            AdaptyUIFlowView view,
            AdaptyUIDialogConfiguration configuration,
            Action<AdaptyUIDialogActionType, AdaptyError> completionHandler
        )
        {
            ShowDialog(view.Id, configuration, completionHandler);
        }

        private static void ShowDialog(
            string viewId,
            AdaptyUIDialogConfiguration configuration,
            Action<AdaptyUIDialogActionType, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["id"] = viewId;
            parameters["configuration"] = AdaptyJson.ToNode(configuration);

            AdaptyRequest.Send("adapty_ui_show_dialog", parameters, completionHandler);
        }

        /// <summary>
        /// Opens the URL natively, honoring the presentation option.
        /// </summary>
        /// <remarks>
        /// This is the same handling the SDK applies by default to <c>open_url</c> user actions. Use it when you override <see cref="IAdaptyFlowsEventsListener.FlowViewDidPerformAction(AdaptyUIFlowView, AdaptyUIUserAction)"/> and want to keep the default URL behavior.
        /// </remarks>
        /// <param name="url">The URL to open.</param>
        /// <param name="openIn">Controls whether to open in external browser or in-app browser. Default is <see cref="AdaptyWebPresentation.ExternalBrowser"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void OpenUrl(
            string url,
            AdaptyWebPresentation openIn,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["url"] = url;
            parameters["open_in"] = AdaptyJson.ToNode(openIn);

            AdaptyRequest.SendVoid("adapty_ui_open_url", parameters, completionHandler);
        }

        /// <summary>
        /// Requests a native store review prompt (App Store / Google Play in-app review).
        /// </summary>
        /// <remarks>
        /// This is the same handling the SDK applies by default to the flow app review request. Use it when you override <see cref="IAdaptyUISystemRequestsHandler.FlowViewDidRequestAppReview(AdaptyUIFlowView)"/> and want to keep the default behavior.
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void RequestAppReview(Action<AdaptyError> completionHandler)
        {
            AdaptyRequest.SendVoid("adapty_ui_request_app_review", null, completionHandler);
        }

        internal static void FlowViewAnswerPermission(string eventId, bool granted, string detail)
        {
            var parameters = new JObject();
            parameters["event_id"] = eventId;
            parameters["status"] = granted ? "granted" : "denied";
            if (detail != null)
            {
                parameters["detail"] = detail;
            }

            AdaptyRequest.SendVoid(
                "flow_view_did_answer_permission",
                parameters,
                (error) => LogRoundTripError("flow_view_did_answer_permission", error)
            );
        }

        internal static void SendObserverEvent(string method, string eventId)
        {
            var parameters = new JObject();
            parameters["event_id"] = eventId;

            AdaptyRequest.SendVoid(method, parameters, (error) => LogRoundTripError(method, error));
        }

        private static void LogRoundTripError(string method, AdaptyError error)
        {
            if (error == null)
            {
                return;
            }

            UnityEngine.Debug.LogError(
                string.Format(
                    "[Adapty] '{0}' failed: {1}. The flow may stay blocked waiting for this answer.",
                    method,
                    error
                )
            );
        }
    }

    internal static class AdaptyRequest
    {
        /// <summary>
        /// Sends one request to the native side and hands the typed reply to
        /// <paramref name="completionHandler"/>.
        /// </summary>
        /// <param name="method">The method name the bridge dispatches on.</param>
        /// <param name="request">
        /// The parameters, either a model or a <see cref="JObject"/> built at the call site. Null
        /// sends the method alone.
        /// </param>
        /// <param name="completionHandler">
        /// Called with the decoded reply, or with the error the reply carried.
        /// </param>
        /// <param name="caller">
        /// The public method the request was made from, filled in by the compiler. It names the
        /// call in the diagnostic when the app's handler throws.
        /// </param>
        internal static void Send<T>(
            string method,
            object request,
            Action<T, AdaptyError> completionHandler,
            [CallerMemberName] string caller = null
        )
        {
            SendRaw<T>(
                method,
                request,
                (value, error) =>
                    InvokeCompletion(() => completionHandler?.Invoke(value, error), caller)
            );
        }

        /// <summary>
        /// Sends one request whose reply carries no value of its own, and reports only the error.
        /// </summary>
        /// <param name="method">The method name the bridge dispatches on.</param>
        /// <param name="request">
        /// The parameters, either a model or a <see cref="JObject"/> built at the call site. Null
        /// sends the method alone.
        /// </param>
        /// <param name="completionHandler">Called with the error the reply carried, or null.</param>
        /// <param name="caller">
        /// The public method the request was made from, filled in by the compiler. It names the
        /// call in the diagnostic when the app's handler throws.
        /// </param>
        internal static void SendVoid(
            string method,
            object request,
            Action<AdaptyError> completionHandler,
            [CallerMemberName] string caller = null
        ) =>
            Send<bool>(
                method,
                request,
                (value, error) => completionHandler?.Invoke(error),
                caller
            );

        /// <summary>
        /// Names the request that is calling back, and hands the call to the one callback policy.
        /// </summary>
        /// <remarks>
        /// The wrapping itself belongs to <see cref="AdaptyCallbacks.InvokeSafe"/> - what lives here is
        /// only the wording, in one place, so that the 40 requests cannot drift apart.
        /// </remarks>
        private static void InvokeCompletion(Action invocation, string caller) =>
            AdaptyCallbacks.InvokeSafe(invocation, $"Failed to invoke completionHandler in {caller}(..)");

        private static void SendRaw<T>(
            string method,
            object request,
            Action<T, AdaptyError> completionHandler
        )
        {
            string payload;
            try
            {
                payload = AdaptyJson.SerializeRequest(method, request);
            }
            catch (Exception ex)
            {
                completionHandler(
                    default(T),
                    new AdaptyError(
                        AdaptyErrorCode.EncodingFailed,
                        $"Failed encoding request: {method}",
                        $"AdaptyUnityError.EncodingFailed({ex})"
                    )
                );
                return;
            }

            _Adapty.Invoke(
                method,
                payload,
                (json) =>
                {
                    var result = AdaptyResponse.Parse<T>(json);
                    completionHandler(result.Value, result.Error);
                }
            );
        }
    }
}
