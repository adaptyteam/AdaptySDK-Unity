using System;
using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;

namespace AdaptyExample
{
    public class AdaptyListener
        : MonoBehaviour,
            AdaptyEventListener,
            AdaptyPaywallsEventsListener,
            AdaptyOnboardingsEventsListener
    {
        public event Action OnInitializeFinished;
        public AdaptyRouter Router;

        void Start()
        {
            Application.SetStackTraceLogType(LogType.Exception, StackTraceLogType.ScriptOnly);
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
            Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

            this.Router = this.GetComponent<AdaptyRouter>();

            this.InitializeAdapty();
            this.SetFallbacks();
        }

        private void InitializeAdapty()
        {
            Adapty.SetEventListener(this);
            Adapty.SetPaywallsEventsListener(this);
            Adapty.SetOnboardingsEventsListener(this);

            this.LogMethodRequest("SetLogLevel");

            Adapty.SetLogLevel(
                AdaptyLogLevel.Verbose,
                (error) =>
                {
                    this.LogMethodResult("SetLogLevel", error);
                }
            );

            var builder = new AdaptyConfiguration.Builder(
                "public_live_iNuUlSsN.83zcTTR8D5Y8FI9cGUI6"
            )
                .SetCustomerUserId(null)
                .SetObserverMode(false)
                .SetServerCluster(AdaptyServerCluster.Default)
                .SetIPAddressCollectionDisabled(false)
                .SetAppleIDFACollectionDisabled(false)
                .SetAppleClearDataOnBackup(true)
                .SetGoogleAdvertisingIdCollectionDisabled(false)
                .SetGoogleEnablePendingPrepaidPlans(true)
                .SetGoogleLocalAccessLevelAllowed(true)
                .SetActivateUI(true)
                .SetAdaptyUIMediaCache(
                    100 * 1024 * 1024, // 100MB
                    null,
                    100 * 1024 * 1024 // 100MB
                );

            this.LogMethodRequest("Activate");

            Adapty.Activate(
                builder.Build(),
                (error) =>
                {
                    this.LogMethodResult("Activate", error);
                    this.OnInitializeFinished?.Invoke();
                    this.GetProfile();
                }
            );
        }

        private void SetFallbacks()
        {
#if UNITY_IOS
            var assetId = "adapty_fallback_ios.json";
#elif UNITY_ANDROID
            var assetId = "adapty_fallback_android.json";
#else
            var assetId = "";
#endif

            this.LogMethodRequest("SetFallbacks");
            Adapty.SetFallback(
                assetId,
                (error) =>
                {
                    this.LogMethodResult("SetFallbacks", error);
                }
            );
        }

        public void GetProfile()
        {
            this.LogMethodRequest("GetProfile");

            Adapty.GetProfile(
                (profile, error) =>
                {
                    this.LogMethodResult("GetProfile", error);

                    if (profile != null)
                    {
                        this.Router.SetProfile(profile);
                    }
                }
            );
        }

        public void GetPaywallForDefaultAudience(
            string id,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyPaywall> completionHandler
        )
        {
            this.LogMethodRequest("GetPaywallForDefaultAudience");

            Adapty.GetPaywallForDefaultAudience(
                id,
                locale,
                fetchPolicy,
                (paywall, error) =>
                {
                    this.LogMethodResult("GetPaywallForDefaultAudience", error);
                    completionHandler.Invoke(paywall);
                }
            );
        }

        public void GetPaywall(
            string id,
            string locale,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyPaywall> completionHandler
        )
        {
            this.LogMethodRequest("GetPaywall");

            Adapty.GetPaywall(
                id,
                locale,
                fetchPolicy,
                new TimeSpan(0, 0, 4),
                (paywall, error) =>
                {
                    this.LogMethodResult("GetPaywall", error);
                    completionHandler.Invoke(paywall);
                }
            );
        }

        public void GetPaywallProducts(
            AdaptyPaywall paywall,
            Action<IList<AdaptyPaywallProduct>> completionHandler
        )
        {
            this.LogMethodRequest("GetPaywallProducts");

            Adapty.GetPaywallProducts(
                paywall,
                (products, error) =>
                {
                    this.LogMethodResult("GetPaywallProducts", error);
                    completionHandler.Invoke(products);
                }
            );
        }

        public void MakePurchase(
            AdaptyPaywallProduct product,
            Action<AdaptyError> completionHandler
        )
        {
            this.LogMethodRequest("MakePurchase");

            Adapty.MakePurchase(
                product,
                (result, error) =>
                {
                    this.LogMethodResult("MakePurchase", error);
                    completionHandler.Invoke(error);

                    switch (result.Type)
                    {
                        case AdaptyPurchaseResultType.Pending:
                            // handle pending
                            break;
                        case AdaptyPurchaseResultType.UserCancelled:
                            // handle cancelation
                            break;
                        case AdaptyPurchaseResultType.Success:
                            var profile = result.Profile;
                            this.Router.SetProfile(profile);
                            break;
                        default:
                            break;
                    }
                }
            );
        }

        public void RestorePurchases(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("RestorePurchases");

            Adapty.RestorePurchases(
                (profile, error) =>
                {
                    this.LogMethodResult("RestorePurchases", error);
                    completionHandler.Invoke(error);

                    if (profile != null)
                    {
                        this.Router.SetProfile(profile);
                    }
                }
            );
        }

        public void Identify(string customerUserId, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("Identify");

            Adapty.Identify(
                customerUserId,
                (error) =>
                {
                    this.LogMethodResult("Identify", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void UpdateProfile(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("UpdateProfile");

            var builder = new AdaptyProfileParameters.Builder()
                .SetFirstName("John")
                .SetLastName("Appleseed")
                .SetBirthday(new DateTime(1990, 5, 14))
                .SetGender(AdaptyProfileGender.Female)
                .SetEmail("example@adapty.io");

            builder = builder.SetAnalyticsDisabled(true);

            Debug.Log("#AdaptyListener# UpdateProfile Test [0]: no exception");
            try
            {
                builder = builder.SetCustomStringAttribute("string_key", "string_value");
                builder = builder.SetCustomStringAttribute("key_to_remove", "test");
                builder = builder.SetCustomDoubleAttribute("double_key", 123.0f);
                builder = builder.RemoveCustomAttribute("key_to_remove");
                Debug.Log("#AdaptyListener# UpdateProfile Test [0]: DONE");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: FAIL");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: value.length > 50");
                builder = builder.SetCustomStringAttribute(
                    "string_key",
                    "01234567890123456789012345678901234567890123456789_"
                );
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: DONE");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [2]: key.length > 30");
                builder = builder.SetCustomStringAttribute(
                    "012345678901234567890123456789_1",
                    "value"
                );
                Debug.Log("#AdaptyListener# UpdateProfile Test [2]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [2]: DONE");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [3]: key wrong symbols");
                builder = builder.SetCustomStringAttribute("key{}``", "value");
                Debug.Log("#AdaptyListener# UpdateProfile Test [3]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [3]: DONE");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [4]: attributes.count > 30");

                for (var i = 1; i <= 31; ++i)
                {
                    builder = builder.SetCustomStringAttribute(
                        string.Format("key_{0}", i),
                        string.Format("value_{0}", i)
                    );
                }

                Debug.Log("#AdaptyListener# UpdateProfile Test [4]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [4]: DONE");
            }

            Adapty.UpdateProfile(
                builder.Build(),
                (error) =>
                {
                    this.LogMethodResult("UpdateProfile", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void SetIntegrationIdentifier(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("SetIntegrationIdentifier");

            Adapty.SetIntegrationIdentifier(
                "test_integration",
                "test_id",
                (error) =>
                {
                    this.LogMethodResult("SetIntegrationIdentifier", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void ReportTransaction(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("ReportTransaction");

            Adapty.ReportTransaction(
                "transaction_id",
                "variation_id",
                (error) =>
                {
                    this.LogMethodResult("ReportTransaction", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void UpdateAttribution(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("UpdateAttribution");

            Adapty.UpdateAttribution(
                "{\"test_key\": \"test_value\"}",
                "custom",
                (error) =>
                {
                    this.LogMethodResult("UpdateAttribution", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void LogShowPaywall(AdaptyPaywall paywall, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("LogShowPaywall");

            Adapty.LogShowPaywall(
                paywall,
                (error) =>
                {
                    this.LogMethodResult("LogShowPaywall", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void UpdateAppStoreCollectingRefundDataConsent(
            Boolean value,
            Action<AdaptyError> completionHandler
        )
        {
            this.LogMethodRequest("UpdateAppStoreCollectingRefundDataConsent");

            Adapty.UpdateAppStoreCollectingRefundDataConsent(
                value,
                (error) =>
                {
                    this.LogMethodResult("UpdateAppStoreCollectingRefundDataConsent", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void UpdateAppStoreRefundPreference(int value, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("UpdateAppStoreRefundPreference");

            AdaptyRefundPreference preferenceValue = AdaptyRefundPreference.NoPreference;

            switch (value)
            {
                case 1:
                    preferenceValue = AdaptyRefundPreference.Decline;
                    break;
                case 2:
                    preferenceValue = AdaptyRefundPreference.Grant;
                    break;

                default:
                    preferenceValue = AdaptyRefundPreference.NoPreference;
                    break;
            }

            Adapty.UpdateAppStoreRefundPreference(
                preferenceValue,
                (error) =>
                {
                    this.LogMethodResult("UpdateAppStoreRefundPreference", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void PresentCodeRedemptionSheet()
        {
            this.LogMethodRequest("PresentCodeRedemptionSheet");

            Adapty.PresentCodeRedemptionSheet(
                (error) =>
                {
                    this.LogMethodResult("PresentCodeRedemptionSheet", error);
                }
            );
        }

        public void Logout(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("Logout");

            Adapty.Logout(
                (error) =>
                {
                    this.LogMethodResult("Logout", error);
                    completionHandler.Invoke(error);
                }
            );
        }

        public void GetInstallationDetails(
            Action<AdaptyInstallationStatus, AdaptyError> completionHandler
        )
        {
            this.LogMethodRequest("GetInstallationDetails");

            Adapty.GetCurrentInstallationStatus(
                (status, error) =>
                {
                    this.LogMethodResult("GetInstallationDetails", error);
                    completionHandler.Invoke(status, error);
                }
            );
        }

        // - Logging

        private void LogMethodRequest(string methodName)
        {
            Debug.Log(string.Format("#AdaptyListener# --> {0}", methodName));
        }

        private void LogMethodResult(string methodName, AdaptyError error)
        {
            if (error != null)
            {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} error {1}", methodName, error));

                this.Router.ShowAlertPanel(error.ToString());
            }
            else
            {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} success", methodName));
            }
        }

        private void LogIncomingCall_AdaptyUIPaywallView(
            string methodName,
            AdaptyUIPaywallView view,
            string meta
        )
        {
            Debug.Log(
                string.Format(
                    "#AdaptyListener# <-- {0}, viewId = {1}, meta = {2}",
                    methodName,
                    view.Id,
                    meta
                )
            );
        }

        private void LogIncomingCall_AdaptyUIOnboardingView(
            string methodName,
            AdaptyUIOnboardingView view,
            string meta
        )
        {
            Debug.Log(
                string.Format(
                    "#AdaptyListener# <-- {0}, viewId = {1}, meta = {2}",
                    methodName,
                    view.Id,
                    meta
                )
            );
        }

        // – AdaptyEventListener

        public void OnLoadLatestProfile(AdaptyProfile profile)
        {
            Debug.Log("#AdaptyListener# OnReceiveUpdatedProfile called");

            this.Router.SetProfile(profile);
        }

        public void OnInstallationDetailsSuccess(AdaptyInstallationDetails details)
        {
            Debug.Log(
                "#AdaptyListener# OnInstallationDetailsSuccess called, details = "
                    + details.ToString()
            );

            this.Router.SetInstallation(new AdaptyInstallationStatusDetermined(details));
        }

        public void OnInstallationDetailsFail(AdaptyError error)
        {
            Debug.Log(
                "#AdaptyListener# OnInstallationDetailsFail called, error = " + error.ToString()
            );
        }

        // AdaptyUI

        public void CreatePaywallView(
            AdaptyPaywall paywall,
            bool preloadProducts,
            Action<AdaptyUIPaywallView> completionHandler
        )
        {
            this.LogMethodRequest("CreatePaywallView");

            var productPurchaseParams =
                new Dictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters>();

            foreach (var productId in paywall.ProductIdentifiers)
            {
                productPurchaseParams[productId] = new AdaptyPurchaseParametersBuilder()
                    .SetIsOfferPersonalized(false)
                    // .SetSubscriptionUpdateParams(new AdaptySubscriptionUpdateParameters()
                    .Build();
            }

            // Create custom assets dictionary
            var customAssets = AdaptyCustomAssetsConfiguration.CreateCustomAssets();

            var parameters = new AdaptyUICreatePaywallViewParameters()
                .SetPreloadProducts(preloadProducts)
                .SetCustomTags(
                    new Dictionary<string, string>
                    {
                        { "CUSTOM_TAG_NAME", "Walter White" },
                        { "CUSTOM_TAG_PHONE", "+1 234 567890" },
                        { "CUSTOM_TAG_CITY", "Albuquerque" },
                        { "CUSTOM_TAG_EMAIL", "walter@white.com" },
                    }
                )
                .SetCustomTimers(
                    new Dictionary<string, DateTime>
                    {
                        { "CUSTOM_TIMER_24H", DateTime.Now.AddSeconds(86400) },
                        { "CUSTOM_TIMER_10H", DateTime.Now.AddSeconds(36000) },
                        { "CUSTOM_TIMER_1H", DateTime.Now.AddSeconds(3600) },
                        { "CUSTOM_TIMER_10M", DateTime.Now.AddSeconds(600) },
                        { "CUSTOM_TIMER_1M", DateTime.Now.AddSeconds(60) },
                        { "CUSTOM_TIMER_10S", DateTime.Now.AddSeconds(10) },
                        { "CUSTOM_TIMER_5S", DateTime.Now.AddSeconds(5) },
                    }
                )
                .SetCustomAssets(customAssets)
                .SetProductPurchaseParameters(productPurchaseParams)
                .SetLoadTimeout(new TimeSpan(0, 0, 3));

            AdaptyUI.CreatePaywallView(
                paywall,
                parameters,
                (view, error) =>
                {
                    this.LogMethodResult("CreatePaywallView", error);

                    if (error != null)
                    {
                        this.Router.ShowAlertPanel(error.ToString());
                    }
                    else
                    {
                        completionHandler.Invoke(view);
                    }
                }
            );
        }

        public void CreateOnboardingView(
            AdaptyOnboarding onboarding,
            AdaptyWebPresentation externalUrlsPresentation,
            Action<AdaptyUIOnboardingView> completionHandler
        )
        {
            this.LogMethodRequest("CreateOnboardingView");

            AdaptyUI.CreateOnboardingView(
                onboarding,
                externalUrlsPresentation,
                (view, error) =>
                {
                    this.LogMethodResult("CreateOnboardingView", error);

                    if (error != null)
                    {
                        this.Router.ShowAlertPanel(error.ToString());
                    }
                    else
                    {
                        completionHandler.Invoke(view);
                    }
                }
            );
        }

        public void PresentPaywallView(
            AdaptyUIPaywallView view,
            Action<AdaptyError> completionHandler
        )
        {
            this.LogMethodRequest("PresentPaywallView");

            AdaptyUI.PresentPaywallView(
                view,
                (error) =>
                {
                    this.LogMethodResult("PresentPaywallView", error);

                    if (completionHandler != null)
                    {
                        completionHandler.Invoke(error);
                    }
                }
            );
        }

        // - AdaptyUIEventListener

        public void PaywallViewDidAppear(AdaptyUIPaywallView view)
        {
            LogIncomingCall_AdaptyUIPaywallView("PaywallViewDidAppear", view, null);
        }

        public void PaywallViewDidDisappear(AdaptyUIPaywallView view)
        {
            LogIncomingCall_AdaptyUIPaywallView("PaywallViewDidDisappear", view, null);
        }

        public void PaywallViewDidFinishWebPaymentNavigation(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        )
        {
            var meta = product.VendorProductId;
            if (error != null)
            {
                meta += ", error = " + error.ToString();
            }
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidFinishWebPaymentNavigation",
                view,
                meta
            );
        }

        public void PaywallViewDidPerformAction(AdaptyUIPaywallView view, AdaptyUIUserAction action)
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidPerformAction",
                view,
                action.Type.ToString()
            );

            switch (action.Type)
            {
                case AdaptyUIUserActionType.Close:
                    view.Dismiss(null);
                    break;
                case AdaptyUIUserActionType.OpenUrl:
                    var urlString = action.Value;
                    var dialog = new AdaptyUIDialogConfiguration()
                        .SetTitle("Open URL?")
                        .SetContent(urlString)
                        .SetDefaultActionTitle("Cancel")
                        .SetSecondaryActionTitle("OK");

                    AdaptyUI.ShowDialog(
                        view,
                        dialog,
                        (action, error) =>
                        {
                            switch (action)
                            {
                                case AdaptyUIDialogActionType.Primary:
                                    break;
                                case AdaptyUIDialogActionType.Secondary:
                                    Application.OpenURL(urlString);
                                    break;
                            }
                        }
                    );

                    break;
                default:
                    break;
            }
        }

        public void PaywallViewDidSelectProduct(AdaptyUIPaywallView view, string productId)
        {
            LogIncomingCall_AdaptyUIPaywallView("PaywallViewDidSelectProduct", view, productId);
        }

        public void PaywallViewDidStartPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product
        )
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidStartPurchase",
                view,
                product.VendorProductId
            );
        }

        public void PaywallViewDidFinishPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyPurchaseResult purchasedResult
        )
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidFinishPurchase",
                view,
                product.VendorProductId
            );

            switch (purchasedResult.Type)
            {
                case AdaptyPurchaseResultType.UserCancelled:
                    // handle user canceled
                    break;
                case AdaptyPurchaseResultType.Pending:
                    // handle pending purchase
                    break;
                case AdaptyPurchaseResultType.Success:
                    try
                    {
                        view.Dismiss(null);

                        var profile = purchasedResult.Profile;

                        if (profile == null)
                        {
                            Debug.Log(
                                string.Format(
                                    "#AdaptyListener# PaywallViewDidFinishPurchase: Success, profile is null!"
                                )
                            );
                            break;
                        }

                        Debug.Log(
                            string.Format(
                                "#AdaptyListener# PaywallViewDidFinishPurchase: Success, profile = {0}",
                                profile.ToString()
                            )
                        );

                        var accessLevels = profile.AccessLevels;

                        if (accessLevels == null)
                        {
                            Debug.Log(
                                string.Format(
                                    "#AdaptyListener# PaywallViewDidFinishPurchase: Success, accessLevels is null!"
                                )
                            );
                            break;
                        }

                        var premiumAccessLevel = accessLevels["premium"];

                        if (premiumAccessLevel == null)
                        {
                            Debug.Log(
                                string.Format(
                                    "#AdaptyListener# PaywallViewDidFinishPurchase: Success, premium accessLevel is null!"
                                )
                            );
                            break;
                        }

                        Debug.Log(
                            string.Format(
                                "#AdaptyListener# PaywallViewDidFinishPurchase: Success, accessLevel = {0}",
                                premiumAccessLevel.ToString()
                            )
                        );
                    }
                    catch (Exception e)
                    {
                        Debug.Log(
                            string.Format(
                                "#AdaptyListener# PaywallViewDidFinishPurchase: Success, error = {0}",
                                e.ToString()
                            )
                        );
                    }

                    break;
                default:
                    break;
            }
        }

        public void PaywallViewDidFailPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        )
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidFailPurchase",
                view,
                string.Format("id: {0}, error: {1}", product.VendorProductId, error.ToString())
            );
        }

        public void PaywallViewDidStartRestore(AdaptyUIPaywallView view)
        {
            LogIncomingCall_AdaptyUIPaywallView("PaywallViewDidStartRestore", view, null);
        }

        public void PaywallViewDidFinishRestore(AdaptyUIPaywallView view, AdaptyProfile profile)
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidFinishRestore",
                view,
                profile.ProfileId
            );

            var dialog = new AdaptyUIDialogConfiguration()
                .SetContent("Success!")
                .SetContent("Purchases were successfully restored.")
                .SetDefaultActionTitle("OK");

            AdaptyUI.ShowDialog(view, dialog, (action, error) => { });
        }

        public void PaywallViewDidFailRestore(AdaptyUIPaywallView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIPaywallView("aywallViewDidFailRestore", view, error.ToString());
        }

        public void PaywallViewDidFailRendering(AdaptyUIPaywallView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidFailRendering",
                view,
                error.ToString()
            );
        }

        public void PaywallViewDidFailLoadingProducts(AdaptyUIPaywallView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIPaywallView(
                "PaywallViewDidFailLoadingProducts",
                view,
                error.ToString()
            );
        }

        // - AdaptyOnboardingsEventsListener

        public void OnboardingViewDidFailWithError(AdaptyUIOnboardingView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIOnboardingView(
                "OnboardingViewDidFailWithError",
                view,
                error.ToString()
            );
        }

        public void OnboardingViewDidFinishLoading(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta
        )
        {
            LogIncomingCall_AdaptyUIOnboardingView(
                "OnboardingViewDidFinishLoading",
                view,
                meta.ToString()
            );
        }

        public void OnboardingViewOnCloseAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        )
        {
            LogIncomingCall_AdaptyUIOnboardingView("OnboardingViewOnCloseAction", view, actionId);

            view.Dismiss(null);
        }

        public void OnboardingViewOnPaywallAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        )
        {
            LogIncomingCall_AdaptyUIOnboardingView("OnboardingViewOnPaywallAction", view, actionId);

            // TODO: present paywall with ID == actionId
        }

        public void OnboardingViewOnCustomAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        )
        {
            LogIncomingCall_AdaptyUIOnboardingView("OnboardingViewOnCustomAction", view, actionId);
        }

        public void OnboardingViewOnStateUpdatedAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string elementId,
            AdaptyOnboardingsStateUpdatedParams @params
        )
        {
            switch (@params)
            {
                case AdaptyOnboardingsSelectParams selectParams:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnStateUpdatedAction",
                        view,
                        "Element: " + elementId + " SelectParams: " + selectParams.ToString()
                    );

                    break;
                case AdaptyOnboardingsMultiSelectParams multiSelectParams:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnStateUpdatedAction",
                        view,
                        "Element: "
                            + elementId
                            + " MultiSelectParams: "
                            + multiSelectParams.ToString()
                    );
                    break;
                case AdaptyOnboardingsInputParams inputParams:

                    switch (inputParams.Input)
                    {
                        case AdaptyOnboardingsTextInput textInput:
                            LogIncomingCall_AdaptyUIOnboardingView(
                                "OnboardingViewOnStateUpdatedAction",
                                view,
                                "Element: " + elementId + " TextInput: " + textInput.Value
                            );
                            break;
                        case AdaptyOnboardingsEmailInput emailInput:
                            LogIncomingCall_AdaptyUIOnboardingView(
                                "OnboardingViewOnStateUpdatedAction",
                                view,
                                "Element: " + elementId + " EmailInput: " + emailInput.Value
                            );
                            break;
                        case AdaptyOnboardingsNumberInput numberInput:
                            LogIncomingCall_AdaptyUIOnboardingView(
                                "OnboardingViewOnStateUpdatedAction",
                                view,
                                "Element: " + elementId + " NumberInput: " + numberInput.Value
                            );
                            break;
                    }
                    break;

                case AdaptyOnboardingsDatePickerParams dateParams:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnStateUpdatedAction",
                        view,
                        "Element: " + elementId + " DatePickerParams: " + dateParams.ToString()
                    );
                    break;
            }
        }

        public void OnboardingViewOnAnalyticsEvent(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            AdaptyOnboardingsAnalyticsEvent analyticsEvent
        )
        {
            switch (analyticsEvent)
            {
                case AdaptyOnboardingsAnalyticsEventOnboardingStarted onboardingStarted:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "OnboardingStarted: " + onboardingStarted.ToString()
                    );
                    break;
                case AdaptyOnboardingsAnalyticsEventScreenPresented screenPresented:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "ScreenPresented: " + screenPresented.ToString()
                    );
                    break;

                case AdaptyOnboardingsAnalyticsEventScreenCompleted screenCompleted:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "ScreenCompleted: " + screenCompleted.ToString()
                    );
                    break;
                case AdaptyOnboardingsAnalyticsEventOnboardingCompleted onboardingCompleted:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "OnboardingCompleted: " + onboardingCompleted.ToString()
                    );
                    break;
                case AdaptyOnboardingsAnalyticsEventUserEmailCollected userEmailCollected:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "UserEmailCollected: " + userEmailCollected.ToString()
                    );
                    break;
                case AdaptyOnboardingsAnalyticsEventUnknown unknownEvent:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "UnknownEvent: " + unknownEvent.Name
                    );
                    break;
                default:
                    LogIncomingCall_AdaptyUIOnboardingView(
                        "OnboardingViewOnAnalyticsEvent",
                        view,
                        "UnknownEvent (default): " + analyticsEvent.GetType().Name
                    );
                    break;
            }
        }
    }
}
