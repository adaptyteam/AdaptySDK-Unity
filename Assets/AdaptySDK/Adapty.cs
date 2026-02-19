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
        public static readonly string SDKVersion = "3.15.1";

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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.Activate(..)",
                            e
                        );
                    }
                }
            );
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
        public static void GetPaywall(
            string placementId,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywall(placementId, null, null, null, completionHandler);

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
        public static void GetPaywall(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            TimeSpan? loadTimeout,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywall(placementId, null, fetchPolicy, loadTimeout, completionHandler);

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
        public static void GetPaywall(
            string placementId,
            string locale,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywall(placementId, locale, null, null, completionHandler);

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
        public static void GetPaywall(
            string placementId,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            TimeSpan? loadTimeout,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("placement_id", placementId);

            if (locale != null)
            {
                parameters.Add("locale", locale);
            }

            if (fetchPolicy != null)
            {
                parameters.Add("fetch_policy", fetchPolicy.ToJSONNode());
            }

            if (loadTimeout.HasValue)
            {
                parameters.Add("load_timeout", loadTimeout.Value.TotalSeconds);
            }

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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyPaywall,AdaptyError> completionHandler in Adapty.GetPaywall(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Adapty allows you remotely configure onboarding screens that will be displayed in your app.
        /// This way you don't have to hardcode the onboarding content and can dynamically change it or run A/B tests without app releases.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/onboardings">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the onboarding <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="loadTimeout">The timeout for the onboarding loading.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetOnboarding(
            string placementId,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            TimeSpan? loadTimeout,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();

            parameters.Add("placement_id", placementId);

            if (locale != null)
            {
                parameters.Add("locale", locale);
            }

            if (fetchPolicy != null)
            {
                parameters.Add("fetch_policy", fetchPolicy.ToJSONNode());
            }

            if (loadTimeout.HasValue)
            {
                parameters.Add("load_timeout", loadTimeout.Value.TotalSeconds);
            }

            Request.Send(
                "get_onboarding",
                parameters,
                JSONNodeExtensions.GetOnboarding,
                (value, error) =>
                {
                    completionHandler?.Invoke(value, error);
                }
            );
        }

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products#speed-up-paywall-fetching-with-default-audience-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(
            string placementId,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("placement_id", placementId);
            if (locale != null)
            {
                parameters.Add("locale", locale);
            }

            if (fetchPolicy != null)
            {
                parameters.Add("fetch_policy", fetchPolicy.ToJSONNode());
            }

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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyPaywall,AdaptyError> completionHandler in Adapty.GetPaywallForDefaultAudience(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/onboardings">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the onboarding <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("placement_id", placementId);

            if (locale != null)
            {
                parameters.Add("locale", locale);
            }

            if (fetchPolicy != null)
            {
                parameters.Add("fetch_policy", fetchPolicy.ToJSONNode());
            }

            Request.Send(
                "get_onboarding_for_default_audience",
                parameters,
                JSONNodeExtensions.GetOnboarding,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyOnboarding,AdaptyError> completionHandler in Adapty.GetOnboardingForDefaultAudience(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Fetches the products array for a given paywall.
        /// </summary>
        /// <remarks>
        /// Once you have an <see cref="AdaptyPaywall"/>, use this method to fetch the corresponding products with full pricing and subscription information.
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyPaywall"/> for which you want to get the products.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains a list of <see cref="AdaptyPaywallProduct"/> objects.</param>
        public static void GetPaywallProducts(
            AdaptyPaywall paywall,
            Action<IList<AdaptyPaywallProduct>, AdaptyError> completionHandler
        )
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
                        throw new Exception(
                            "Failed to invoke Action<IList<AdaptyPaywallProduct>,AdaptyError> completionHandler in Adapty.GetPaywallProducts(..)",
                            e
                        );
                    }
                }
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyProfile,AdaptyError> completionHandler in Adapty.GetProfile(..)",
                            e
                        );
                    }
                }
            );
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
            var parameters = new JSONObject();

            parameters.Add("customer_user_id", customerUserId);

            var customerIdentity = new AdaptyCustomerIdentity(
                iosAppAccountToken,
                androidObfuscatedAccountId
            );

            if (!customerIdentity.IsEmpty)
            {
                parameters.Add("parameters", customerIdentity.ToJSONNode());
            }

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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.Identify(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Checks if the native Adapty SDK is activated and ready to use.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result. The result contains a boolean value indicating whether the SDK is activated.</param>
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
                        throw new Exception(
                            "Failed to invoke Action<bool,AdaptyError> completionHandler in Adapty.IsActivated(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Returns the current log level of the Adapty SDK.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the current <see cref="AdaptyLogLevel"/> value.</param>
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyLogLevel,AdaptyError> completionHandler in Adapty.GetLoglevel(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetLogLevel(..)",
                            e
                        );
                    }
                }
            );
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
            Request.Send(
                "get_current_installation_status",
                null,
                JSONNodeExtensions.GetInstallationStatus,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyInstallationDetails,AdaptyError> completionHandler in Adapty.GetCurrentInstallationStatus(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.Logout(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Creates a web URL for the paywall that can be used to display the paywall in a web view or browser.
        /// </summary>
        /// <remarks>
        /// This is useful for platforms that don't support native paywall views or for web-based implementations.
        /// Read more at <see href="https://adapty.io/docs/web-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyPaywall"/> object for which to create the web URL.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the web URL string.</param>
        public static void CreateWebPaywallUrl(
            AdaptyPaywall paywall,
            Action<string, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

            Request.Send(
                "create_web_paywall_url",
                parameters,
                JSONNodeExtensions.GetString,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.CreateWebPaywallUrl(..)",
                            e
                        );
                    }
                }
            );
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
            var parameters = new JSONObject();
            parameters.Add("product", product.ToJSONNode());

            Request.Send(
                "create_web_paywall_url",
                parameters,
                JSONNodeExtensions.GetString,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.CreateWebPaywallUrl(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Opens the paywall in a web view or browser.
        /// </summary>
        /// <remarks>
        /// This method opens the web paywall URL in the default browser or web view.
        /// Read more at <see href="https://adapty.io/docs/web-paywall">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyPaywall"/> object to open.</param>
        /// <param name="openIn">Controls whether to open in external browser or in-app browser. Default is <see cref="AdaptyWebPresentation.ExternalBrowser"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void OpenWebPaywall(
            AdaptyPaywall paywall,
            AdaptyWebPresentation openIn,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());
            parameters.Add("open_in", openIn.ToJSONNode());

            Request.Send(
                "open_web_paywall",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.OpenWebPaywall(..)",
                            e
                        );
                    }
                }
            );
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
            var parameters = new JSONObject();
            parameters.Add("product", product.ToJSONNode());
            parameters.Add("open_in", openIn.ToJSONNode());

            Request.Send(
                "open_web_paywall",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.OpenWebPaywall(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Call this method to notify Adapty SDK, that particular paywall was shown to user.
        /// </summary>
        /// <remarks>
        /// Adapty helps you to measure the performance of the paywalls.
        /// We automatically collect all the metrics related to purchases except for paywall views.
        /// This is because only you know when the paywall was shown to a customer. Whenever you show a paywall to your user, call .logShowPaywall(paywall) to log the event, and it will be accumulated in the paywall metrics.
        /// Read more on the <see href="https://adapty.io/docs/present-remote-config-paywalls-unity#track-paywall-view-events">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An [AdaptyPaywall] object.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void LogShowPaywall(
            AdaptyPaywall paywall,
            Action<AdaptyError> completionHandler
        )
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.LogShowPaywall(..)",
                            e
                        );
                    }
                }
            );
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
#if UNITY_IOS && !UNITY_EDITOR
            var parameters = new JSONObject();
            parameters.Add("consent", consent);

            Request.Send(
                "update_collecting_refund_data_consent",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateAppStoreCollectingRefundDataConsent(..)",
                            e
                        );
                    }
                }
            );
#else
            try
            {
                completionHandler?.Invoke(null);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateAppStoreCollectingRefundDataConsent(..)",
                    e
                );
            }
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
#if UNITY_IOS && !UNITY_EDITOR
            var parameters = new JSONObject();
            parameters.Add("refund_preference", refundPreference.ToJSONNode());

            Request.Send(
                "update_refund_preference",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateAppStoreRefundPreference(..)",
                            e
                        );
                    }
                }
            );
#else
            try
            {
                completionHandler?.Invoke(null);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateAppStoreRefundPreference(..)",
                    e
                );
            }
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
        /// <param name="purchaseParameters">An optional <see cref="AdaptyPurchaseParameters"/> object containing purchase configuration (e.g., subscription update parameters for Android, offer personalization, etc.).</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyPurchaseResult"/> object.</param>
        public static void MakePurchase(
            AdaptyPaywallProduct product,
            AdaptyPurchaseParameters purchaseParameters,
            Action<AdaptyPurchaseResult, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("product", product.ToJSONNode());
            if (purchaseParameters != null)
            {
                parameters.Add("parameters", purchaseParameters.ToJSONNode());
            }

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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyPurchaseResult, AdaptyError> completionHandler in Adapty.MakePurchase(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentCodeRedemptionSheet(..)",
                            e
                        );
                    }
                }
            );
#else
            try
            {
                completionHandler?.Invoke(null);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentCodeRedemptionSheet(..)",
                    e
                );
            }
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
        /// <param name="variationId">An optional string identifier of the variation. You can get it using the <see cref="AdaptyPaywall.VariationId"/> property of <see cref="AdaptyPaywall"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ReportTransaction(
            string transactionId,
            string variationId,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("transaction_id", transactionId);
            if (variationId != null)
            {
                parameters.Add("variation_id", variationId);
            }

            Request.Send(
                "report_transaction",
                parameters,
                JSONNodeExtensions.GetAdaptyProfile,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.ReportTransaction(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyProfile, AdaptyError> completionHandler in Adapty.RestorePurchases(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<string, AdaptyError> completionHandler in Adapty.GetNativeSDKVersion(..)",
                            e
                        );
                    }
                }
            );
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
            var parameters = new JSONObject();

#if UNITY_IOS && !UNITY_EDITOR
            parameters.Add("path", UnityEngine.Application.dataPath + "/Raw/" + fileName);
#elif UNITY_ANDROID && !UNITY_EDITOR
            parameters.Add(
                "path",
                "jar:file://" + UnityEngine.Application.dataPath + "!/assets/" + fileName
            );
#endif

            Request.Send(
                "set_fallback",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetFallback(..)",
                            e
                        );
                    }
                }
            );
        }

        [Obsolete("Use SetFallback instead")]
        public static void SetFallbackPaywalls(
            string fileName,
            Action<AdaptyError> completionHandler
        )
        {
            SetFallback(fileName, completionHandler);
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetIntegrationIdentifier(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateAttribution(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.UpdateProfile(..)",
                            e
                        );
                    }
                }
            );
        }
    }

    public static partial class AdaptyUI
    {
        /// <summary>
        /// Creates a paywall view from an AdaptyPaywall object.
        /// </summary>
        /// <remarks>
        /// Right after receiving an <see cref="AdaptyPaywall"/>, you can create the corresponding <see cref="AdaptyUIPaywallView"/> to present it afterwards.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyPaywall"/> object for which you are trying to create a view.</param>
        /// <param name="optionalParameters">An optional <see cref="AdaptyUICreatePaywallViewParameters"/> object that contains optional parameters like load timeout, custom tags, custom timers, product purchase parameters, and custom assets.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyUIPaywallView"/> object.</param>
        public static void CreatePaywallView(
            AdaptyPaywall paywall,
            AdaptyUICreatePaywallViewParameters optionalParameters,
            Action<AdaptyUIPaywallView, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

            if (optionalParameters != null)
            {
                if (optionalParameters.LoadTimeout.HasValue)
                {
                    parameters.Add(
                        "load_timeout",
                        optionalParameters.LoadTimeout.Value.TotalSeconds
                    );
                }

                if (optionalParameters.PreloadProducts.HasValue)
                {
                    parameters.Add("preload_products", optionalParameters.PreloadProducts.Value);
                }

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
                    foreach (
                        KeyValuePair<string, DateTime> entry in optionalParameters.CustomTimers
                    )
                    {
                        node.Add(entry.Key, entry.Value.ToJSONNode());
                    }
                    parameters.Add("custom_timers", node);
                }
                if (optionalParameters.ProductPurchaseParameters != null)
                {
                    var parametersNode = new JSONObject();

                    foreach (
                        KeyValuePair<
                            AdaptyProductIdentifier,
                            AdaptyPurchaseParameters
                        > entry in optionalParameters.ProductPurchaseParameters
                    )
                    {
                        parametersNode.Add(entry.Key._AdaptyProductId, entry.Value.ToJSONNode());
                    }

                    parameters.Add("product_purchase_parameters", parametersNode);
                }

                if (optionalParameters.CustomAssets != null)
                {
                    parameters.Add("custom_assets", optionalParameters.CustomAssets.ToJSONNode());
                }
            }

            Request.Send(
                "adapty_ui_create_paywall_view",
                parameters,
                JSONNodeExtensions.GetAdaptyUIPaywallView,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyUIPaywallView, AdaptyError> completionHandler in Adapty.CreatePaywallView(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Creates an onboarding view from an AdaptyOnboarding object.
        /// </summary>
        /// <remarks>
        /// Right after receiving an <see cref="AdaptyOnboarding"/>, you can create the corresponding <see cref="AdaptyUIOnboardingView"/> to present it afterwards.
        /// Read more at <see href="https://adapty.io/docs/onboardings">Adapty Documentation</see>
        /// </remarks>
        /// <param name="onboarding">An <see cref="AdaptyOnboarding"/> object for which you are trying to create a view.</param>
        /// <param name="externalUrlsPresentation">Controls how external URLs are presented in the onboarding (in-app browser vs external browser). Default is <see cref="AdaptyWebPresentation.ExternalBrowser"/>.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyUIOnboardingView"/> object.</param>
        public static void CreateOnboardingView(
            AdaptyOnboarding onboarding,
            AdaptyWebPresentation externalUrlsPresentation,
            Action<AdaptyUIOnboardingView, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("onboarding", onboarding.ToJSONNode());
            parameters.Add("external_urls_presentation", externalUrlsPresentation.ToJSONNode());

            Request.Send(
                "adapty_ui_create_onboarding_view",
                parameters,
                JSONNodeExtensions.GetAdaptyUIOnboardingView,
                (value, error) =>
                {
                    try
                    {
                        completionHandler?.Invoke(value, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke Action<AdaptyUIOnboardingView, AdaptyError> completionHandler in Adapty.CreateOnboardingView(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Dismisses the paywall view.
        /// </summary>
        /// <remarks>
        /// Call this method when you want to dismiss the paywall view from the screen.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIPaywallView"/> object representing the view to dismiss.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void DismissPaywallView(
            AdaptyUIPaywallView view,
            Action<AdaptyError> completionHandler
        ) => DismissPaywallView(view, false, completionHandler);

        private static void DismissPaywallView(
            AdaptyUIPaywallView view,
            bool destroy,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("destroy", destroy);

            Request.Send(
                "adapty_ui_dismiss_paywall_view",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.DismissPaywallView(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIPaywallView] object, for which is representing the view.</param>
        /// <param name="iosPresentationStyle">an [AdaptyUIIOSPresentationStyle] object, for which is representing the iOS presentation style.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentPaywallView(
            AdaptyUIPaywallView view,
            Action<AdaptyError> completionHandler
        )
        {
            PresentPaywallView(view, AdaptyUIIOSPresentationStyle.FullScreen, completionHandler);
        }

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIPaywallView] object, for which is representing the view.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentPaywallView(
            AdaptyUIPaywallView view,
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("ios_presentation_style", iosPresentationStyle.ToJSONNode());

            Request.Send(
                "adapty_ui_present_paywall_view",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentPaywallView(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Presents the onboarding view to the user.
        /// </summary>
        /// <remarks>
        /// This method presents the onboarding view using the default full-screen presentation style.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIOnboardingView"/> object representing the view to present.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentOnboardingView(
            AdaptyUIOnboardingView view,
            Action<AdaptyError> completionHandler
        )
        {
            PresentOnboardingView(view, AdaptyUIIOSPresentationStyle.FullScreen, completionHandler);
        }

        /// <summary>
        /// Presents the onboarding view to the user with a specified presentation style.
        /// </summary>
        /// <remarks>
        /// This method presents the onboarding view using the specified iOS presentation style (iOS only).
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIOnboardingView"/> object representing the view to present.</param>
        /// <param name="iosPresentationStyle">An <see cref="AdaptyUIIOSPresentationStyle"/> object representing the iOS presentation style (iOS only).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void PresentOnboardingView(
            AdaptyUIOnboardingView view,
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("ios_presentation_style", iosPresentationStyle.ToJSONNode());

            Request.Send(
                "adapty_ui_present_onboarding_view",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.PresentOnboardingView(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Dismisses the onboarding view.
        /// </summary>
        /// <remarks>
        /// Call this method when you want to dismiss the onboarding view from the screen.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIOnboardingView"/> object representing the view to dismiss.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void DismissOnboardingView(
            AdaptyUIOnboardingView view,
            Action<AdaptyError> completionHandler
        ) => DismissOnboardingView(view, false, completionHandler);

        private static void DismissOnboardingView(
            AdaptyUIOnboardingView view,
            bool destroy,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("id", view.Id);
            parameters.Add("destroy", destroy);

            Request.Send(
                "adapty_ui_dismiss_onboarding_view",
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.DismissOnboardingView(..)",
                            e
                        );
                    }
                }
            );
        }

        /// <summary>
        /// Presents a dialog on the paywall view.
        /// </summary>
        /// <remarks>
        /// This method shows a dialog with custom configuration on the paywall view. The dialog can be used for various purposes like showing terms, privacy policy, or custom messages.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIPaywallView"/> object representing the view on which to show the dialog.</param>
        /// <param name="configuration">An <see cref="AdaptyUIDialogConfiguration"/> object that contains the dialog configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the <see cref="AdaptyUIDialogActionType"/> indicating which action was taken.</param>
        public static void ShowDialog(
            AdaptyUIPaywallView view,
            AdaptyUIDialogConfiguration configuration,
            Action<AdaptyUIDialogActionType, AdaptyError> completionHandler
        )
        {
            ShowDialog(view.Id, configuration, completionHandler);
        }

        /// <summary>
        /// Presents a dialog on the onboarding view.
        /// </summary>
        /// <remarks>
        /// This method shows a dialog with custom configuration on the onboarding view. The dialog can be used for various purposes like showing terms, privacy policy, or custom messages.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIOnboardingView"/> object representing the view on which to show the dialog.</param>
        /// <param name="configuration">An <see cref="AdaptyUIDialogConfiguration"/> object that contains the dialog configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains the <see cref="AdaptyUIDialogActionType"/> indicating which action was taken.</param>
        public static void ShowDialog(
            AdaptyUIOnboardingView view,
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
            var parameters = new JSONObject();
            parameters.Add("id", viewId);
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyUIDialogActionType, AdaptyError> completionHandler in Adapty.ShowDialog(..)",
                            e
                        );
                    }
                }
            );
        }
    }

    internal static class Request
    {
        internal static void Send<T>(
            string method,
            JSONObject request,
            Func<JSONNode, T> mapResponseValue,
            Action<T, AdaptyError> completionHandler
        )
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
                var error = new AdaptyError(
                    AdaptyErrorCode.EncodingFailed,
                    $"Failed encoding request: {method}",
                    $"AdaptyUnityError.EncodingFailed({ex})"
                );
                completionHandler(default(T), error);
                return;
            }

            _Adapty.Invoke(
                method,
                stringJson,
                (json) =>
                {
                    var result = json.GetAdaptyResult(mapResponseValue);
                    completionHandler(result.Value, result.Error);
                }
            );
        }
    }
}
