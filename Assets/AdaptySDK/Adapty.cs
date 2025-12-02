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
        public static readonly string SDKVersion = "3.14.0";

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
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
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
        /// Once you have a [AdaptyPaywall], fetch corresponding products array using this method.
        /// </summary>
        /// <param name="paywall">an [AdaptyPaywall] for which you want to get a products.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyProfile,AdaptyError> completionHandler in Adapty.GetProfile(..)",
                            e
                        );
                    }
                }
            );
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
            Identify(customerUserId, null, null, completionHandler);
        }

        /// <summary>
        /// Use this method for identifying user with it's user id in your system.
        /// </summary>
        /// <remarks>
        /// If you don't have a user id on SDK configuration, you can set it later at any time with `.identify()` method.
        /// The most common cases are after registration/authorization when the user switches from being an anonymous user to an authenticated user.
        /// </remarks>
        /// <param name="customerUserId">User identifier in your system.</param>
        /// <param name="iosAppAccountToken">The UUID that you generate to associate a customer’s In-App Purchase with its resulting App Store transaction. (use for iOS), [read more](https://developer.apple.com/documentation/appstoreserverapi/appaccounttoken).</param>
        /// <param name="androidObfuscatedAccountId">The obfuscated account identifier (use for Android), [read more](https://developer.android.com/google/play/billing/developer-payload#attribute).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void Identify(
            string customerUserId,
            Guid? iosAppAccountToken,
            string? androidObfuscatedAccountId,
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
                        throw new Exception(
                            "Failed to invoke Action<bool,AdaptyError> completionHandler in Adapty.IsActivated(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyLogLevel,AdaptyError> completionHandler in Adapty.GetLoglevel(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.SetLogLevel(..)",
                            e
                        );
                    }
                }
            );
        }

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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyError> completionHandler in Adapty.Logout(..)",
                            e
                        );
                    }
                }
            );
        }

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

        public static void OpenWebPaywall(
            AdaptyPaywall paywall,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("paywall", paywall.ToJSONNode());

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

        public static void OpenWebPaywall(
            AdaptyPaywallProduct product,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("product", product.ToJSONNode());

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
        /// Read more on the <see href="https://adapty.io/docs/present-remote-config-paywalls#track-paywall-view-events">Adapty Documentation</see>
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
        /// Call this method to update the current user's refund data consent.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/refund-saver#obtain-user-consent">Adapty Documentation</see>
        /// </remarks>
        /// <param name="consent">a Boolean value wehter user gave the consent or not.</param>
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
        /// Call this method to set the refund preference individually for current user.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/refund-saver#set-refund-behavior-for-a-specific-user-in-the-dashboard">Adapty Documentation</see>
        /// </remarks>
        /// <param name="refundPreference"> the [AdaptyRefundPreference] value.</param>
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
        /// To make the purchase, you have to call this method.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">an [AdaptyPaywallProduct] object retrieved from the paywall.</param>
        /// <param name="subscriptionUpdate">an [AdaptySubscriptionUpdateParameters] object used to upgrade or downgrade a subscription (use for Android).</param>
        /// <param name="isOfferPersonalized">Specifies whether the offer is personalized to the buyer (use for Android).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
        /// In Observer mode, Adapty SDK doesn’t know, where the purchase was made from.
        /// If you display products using our <see href="https://docs.adapty.io/v2.0/docs/paywall">Paywalls</see> or <  see href="https://docs.adapty.io/v2.0/docs/ab-test">A/B Tests</see>, you can manually assign variation to the purchase.
        /// After doing this, you’ll be able to see metrics in Adapty Dashboard.
        /// </summary>
        /// <param name="transactionId">A string identifier of your purchased transaction <see href="https://developer.apple.com/documentation/storekit/skpaymenttransaction">SKPaymentTransaction</see> for iOS or string identifier (`purchase.getOrderId()`) of the purchase, where the purchase is an instance of the billing library Purchase class for Android.</param>
        /// <param name="variationId">A string identifier of variation. You can get it using variationId property of AdaptyPaywall.</param>
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
                        throw new Exception(
                            "Failed to invoke Action<AdaptyProfile, AdaptyError> completionHandler in Adapty.RestorePurchases(..)",
                            e
                        );
                    }
                }
            );
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
                        throw new Exception(
                            "Failed to invoke Action<string, AdaptyError> completionHandler in Adapty.GetNativeSDKVersion(..)",
                            e
                        );
                    }
                }
            );
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
        /// You can set integration identifiers for the profile, using method.
        /// </summary>
        /// <param name="key">a identifier of the integration.</param>
        /// <param name="value">a value of the integration identifier.</param>
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
        /// You can set attribution data for the profile, using method.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </summary>
        /// <param name="jsonString">a serialized JSON string containing attribution (conversion) data.</param>
        /// <param name="source">a source of attribution.</param>
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
        /// You can set optional attributes such as email, phone number, etc, to the user of your app.
        /// You can then use attributes to create user <see href="https://adapty.io/docs/segments">segments</see> or just view them in CRM.
        /// </summary>
        /// <param name="param">use [AdaptyProfileParametersBuilder] to build this object.</param>
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
        /// Right after receiving ``AdaptyPaywall``, you can create the corresponding ``AdaptyUIPaywallView`` to present it afterwards.
        /// </summary>
        /// <param name="paywall">an [AdaptyPaywall] object, for which you are trying to get a controller.</param>
        /// <param name="optionalParameters">an [AdaptyUICreatePaywallViewParameters] object that contains optional parameters.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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

        public static void CreateOnboardingView(
            AdaptyOnboarding onboarding,
            Action<AdaptyUIOnboardingView, AdaptyError> completionHandler
        )
        {
            var parameters = new JSONObject();
            parameters.Add("onboarding", onboarding.ToJSONNode());

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
        /// Call this function if you wish to dismiss the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIPaywallView] object, for which is representing the view.</param>
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
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="completionHandler"></param>
        public static void PresentOnboardingView(
            AdaptyUIOnboardingView view,
            Action<AdaptyError> completionHandler
        )
        {
            PresentOnboardingView(view, AdaptyUIIOSPresentationStyle.FullScreen, completionHandler);
        }

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIPaywallView] object, for which is representing the view.</param>
        /// <param name="iosPresentationStyle">an [AdaptyUIIOSPresentationStyle] object, for which is representing the iOS presentation style.</param>
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
        /// Call this function if you wish to dismiss the view.
        /// </summary>
        /// <param name="view">an [AdaptyUIOnboardingView] object, for which is representing the view.</param>
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
        /// Call this function if you wish to present the dialog.
        /// </summary>
        /// <param name="view">an [AdaptyUIPaywallView] or [AdaptyUIOnboardingView] object, for which is representing the view.</param>
        /// <param name="configuration">an [AdaptyUIDialogConfiguration] object that contains the dialog configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ShowDialog(
            AdaptyUIPaywallView view,
            AdaptyUIDialogConfiguration configuration,
            Action<AdaptyUIDialogActionType, AdaptyError> completionHandler
        )
        {
            ShowDialog(view.Id, configuration, completionHandler);
        }

        /// <summary>
        /// Call this function if you wish to present the dialog.
        /// </summary>
        /// <param name="view">an [AdaptyUIOnboardingView] object, for which is representing the view.</param>
        /// <param name="configuration">an [AdaptyUIDialogConfiguration] object that contains the dialog configuration.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
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
