using System;
using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;

namespace AdaptyExample
{
    public class AdaptyListener
        : MonoBehaviour,
            IAdaptyEventListener,
            IAdaptyFlowsEventsListener,
            IAdaptyUISystemRequestsHandler,
            IAdaptyUIObserverModeResolver
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
            Adapty.SetFlowsEventsListener(this);
            Adapty.SetSystemRequestsHandler(this);
            Adapty.SetObserverModeResolver(this);

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

        public void GetFlowForDefaultAudience(
            string id,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyFlow> completionHandler
        )
        {
            this.LogMethodRequest("GetFlowForDefaultAudience");

            Adapty.GetFlowForDefaultAudience(
                id,
                fetchPolicy,
                (flow, error) =>
                {
                    this.LogMethodResult("GetFlowForDefaultAudience", error);
                    completionHandler.Invoke(flow);
                }
            );
        }

        public void GetFlow(
            string id,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyFlow> completionHandler
        )
        {
            this.LogMethodRequest("GetFlow");

            Adapty.GetFlow(
                id,
                fetchPolicy,
                new TimeSpan(0, 0, 4),
                (flow, error) =>
                {
                    this.LogMethodResult("GetFlow", error);
                    completionHandler.Invoke(flow);
                }
            );
        }

        public void GetPaywallProducts(
            AdaptyFlow flow,
            Action<IList<AdaptyPaywallProduct>> completionHandler
        )
        {
            this.LogMethodRequest("GetPaywallProducts");

            Adapty.GetPaywallProducts(
                flow,
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

        public void LogShowFlow(AdaptyFlow flow, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("LogShowFlow");

            Adapty.LogShowFlow(
                flow,
                (error) =>
                {
                    this.LogMethodResult("LogShowFlow", error);
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

        private void LogIncomingCall_AdaptyUIFlowView(
            string methodName,
            AdaptyUIFlowView view,
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

        // – IAdaptyEventListener

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

            this.Router.SetInstallationDetails(details);
        }

        public void OnInstallationDetailsFail(AdaptyError error)
        {
            Debug.Log(
                "#AdaptyListener# OnInstallationDetailsFail called, error = " + error.ToString()
            );
        }

        // AdaptyUI

        public void CreateFlowView(
            AdaptyFlow flow,
            bool preloadProducts,
            string locale,
            Action<AdaptyUIFlowView> completionHandler
        )
        {
            this.LogMethodRequest("CreateFlowView");

            var productPurchaseParams =
                new Dictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters>();

            foreach (var productId in flow.ProductIdentifiers)
            {
                productPurchaseParams[productId] = new AdaptyPurchaseParametersBuilder()
                    .SetIsOfferPersonalized(false)
                    // .SetSubscriptionUpdateParams(new AdaptySubscriptionUpdateParameters()
                    .Build();
            }

            // Create custom assets dictionary
            var customAssets = AdaptyCustomAssetsConfiguration.CreateCustomAssets();

            var parameters = new AdaptyUICreateFlowViewParameters()
                .SetLocale(string.IsNullOrEmpty(locale) ? null : locale)
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

            AdaptyUI.CreateFlowView(
                flow,
                parameters,
                (view, error) =>
                {
                    this.LogMethodResult("CreateFlowView", error);

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

        public void PresentFlowView(AdaptyUIFlowView view, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("PresentFlowView");

            AdaptyUI.PresentFlowView(
                view,
                (error) =>
                {
                    this.LogMethodResult("PresentFlowView", error);

                    if (completionHandler != null)
                    {
                        completionHandler.Invoke(error);
                    }
                }
            );
        }

        // - IAdaptyFlowsEventsListener

        public void FlowViewDidAppear(AdaptyUIFlowView view)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidAppear", view, null);
        }

        public void FlowViewDidDisappear(AdaptyUIFlowView view)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidDisappear", view, null);
        }

        public void FlowViewDidFinishWebPaymentNavigation(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        )
        {
            var meta = product != null ? product.VendorProductId : "(no product)";
            if (error != null)
            {
                meta += ", error = " + error.ToString();
            }
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidFinishWebPaymentNavigation", view, meta);
        }

        public void FlowViewDidPerformAction(AdaptyUIFlowView view, AdaptyUIUserAction action)
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidPerformAction",
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

        public void FlowViewDidSelectProduct(AdaptyUIFlowView view, string productId)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidSelectProduct", view, productId);
        }

        public void FlowViewDidStartPurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product)
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidStartPurchase",
                view,
                product.VendorProductId
            );
        }

        public void FlowViewDidFinishPurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyPurchaseResult purchasedResult
        )
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidFinishPurchase",
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
                                    "#AdaptyListener# FlowViewDidFinishPurchase: Success, profile is null!"
                                )
                            );
                            break;
                        }

                        Debug.Log(
                            string.Format(
                                "#AdaptyListener# FlowViewDidFinishPurchase: Success, profile = {0}",
                                profile.ToString()
                            )
                        );

                        var accessLevels = profile.AccessLevels;

                        if (accessLevels == null)
                        {
                            Debug.Log(
                                string.Format(
                                    "#AdaptyListener# FlowViewDidFinishPurchase: Success, accessLevels is null!"
                                )
                            );
                            break;
                        }

                        var premiumAccessLevel = accessLevels["premium"];

                        if (premiumAccessLevel == null)
                        {
                            Debug.Log(
                                string.Format(
                                    "#AdaptyListener# FlowViewDidFinishPurchase: Success, premium accessLevel is null!"
                                )
                            );
                            break;
                        }

                        Debug.Log(
                            string.Format(
                                "#AdaptyListener# FlowViewDidFinishPurchase: Success, accessLevel = {0}",
                                premiumAccessLevel.ToString()
                            )
                        );
                    }
                    catch (Exception e)
                    {
                        Debug.Log(
                            string.Format(
                                "#AdaptyListener# FlowViewDidFinishPurchase: Success, error = {0}",
                                e.ToString()
                            )
                        );
                    }

                    break;
                default:
                    break;
            }
        }

        public void FlowViewDidFailPurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        )
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidFailPurchase",
                view,
                string.Format("id: {0}, error: {1}", product.VendorProductId, error.ToString())
            );
        }

        public void FlowViewDidStartRestore(AdaptyUIFlowView view)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidStartRestore", view, null);
        }

        public void FlowViewDidFinishRestore(AdaptyUIFlowView view, AdaptyProfile profile)
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidFinishRestore",
                view,
                profile.ProfileId
            );

            var dialog = new AdaptyUIDialogConfiguration()
                .SetContent("Success!")
                .SetContent("Purchases were successfully restored.")
                .SetDefaultActionTitle("OK");

            AdaptyUI.ShowDialog(view, dialog, (action, error) => { });
        }

        public void FlowViewDidFailRestore(AdaptyUIFlowView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidFailRestore", view, error.ToString());
        }

        public void FlowViewDidReceiveError(AdaptyUIFlowView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidReceiveError", view, error.ToString());
        }

        public void FlowViewDidFailLoadingProducts(AdaptyUIFlowView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidFailLoadingProducts",
                view,
                error.ToString()
            );
        }

        public void FlowViewDidReceiveAnalyticEvent(
            AdaptyUIFlowView view,
            string name,
            IDictionary<string, object> @params
        )
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidReceiveAnalyticEvent", view, name);
        }

        // - IAdaptyUISystemRequestsHandler

        public void FlowViewDidAskPermission(
            AdaptyUIFlowView view,
            string permission,
            IDictionary<string, string> customArgs,
            Action<bool, string> respond
        )
        {
            var meta = permission;
            if (customArgs != null)
            {
                foreach (KeyValuePair<string, string> arg in customArgs)
                {
                    meta += string.Format(", {0} = {1}", arg.Key, arg.Value);
                }
            }
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidAskPermission", view, meta);

            // A real app requests the permission from the OS here and reports the actual outcome.
            respond(true, "Answered by the Unity sample app without asking the OS.");
        }

        public void FlowViewDidRequestAppReview(AdaptyUIFlowView view)
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidRequestAppReview", view, null);

            this.LogMethodRequest("RequestAppReview");

            AdaptyUI.RequestAppReview(
                (error) =>
                {
                    this.LogMethodResult("RequestAppReview", error);
                }
            );
        }

        // - IAdaptyUIObserverModeResolver

        public void FlowViewDidInitiatePurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            Action onStartPurchase,
            Action onFinishPurchase
        )
        {
            LogIncomingCall_AdaptyUIFlowView(
                "FlowViewDidInitiatePurchase",
                view,
                product.VendorProductId
            );

            // A real app runs its own billing flow between these two calls.
            onStartPurchase();
            onFinishPurchase();
        }

        public void FlowViewDidInitiateRestore(
            AdaptyUIFlowView view,
            Action onStartRestore,
            Action onFinishRestore
        )
        {
            LogIncomingCall_AdaptyUIFlowView("FlowViewDidInitiateRestore", view, null);

            onStartRestore();
            onFinishRestore();
        }
    }
}
