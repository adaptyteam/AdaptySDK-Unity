using System;
#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public interface AdaptyEventListener
    {
        void OnLoadLatestProfile(AdaptyProfile profile);

        void OnInstallationDetailsSuccess(AdaptyInstallationDetails details);

        void OnInstallationDetailsFail(AdaptyError error);
    }

    public interface AdaptyPaywallsEventsListener
    {
        void PaywallViewDidAppear(AdaptyUIPaywallView view);

        void PaywallViewDidDisappear(AdaptyUIPaywallView view);

        void PaywallViewDidPerformAction(AdaptyUIPaywallView view, AdaptyUIUserAction action);

        void PaywallViewDidSelectProduct(AdaptyUIPaywallView view, string productId);

        void PaywallViewDidStartPurchase(AdaptyUIPaywallView view, AdaptyPaywallProduct product);

        void PaywallViewDidFinishPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyPurchaseResult purchasedResult
        );

        void PaywallViewDidFailPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        );

        void PaywallViewDidStartRestore(AdaptyUIPaywallView view);

        void PaywallViewDidFinishRestore(AdaptyUIPaywallView view, AdaptyProfile profile);

        void PaywallViewDidFailRestore(AdaptyUIPaywallView view, AdaptyError error);

        void PaywallViewDidFailRendering(AdaptyUIPaywallView view, AdaptyError error);

        void PaywallViewDidFailLoadingProducts(AdaptyUIPaywallView view, AdaptyError error);

        void PaywallViewDidFinishWebPaymentNavigation(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyError? error
        );
    }

    public interface AdaptyOnboardingsEventsListener
    {
        void OnboardingViewDidFailWithError(AdaptyUIOnboardingView view, AdaptyError error);

        void OnboardingViewDidFinishLoading(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta
        );

        void OnboardingViewOnCloseAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        );

        void OnboardingViewOnPaywallAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        );

        void OnboardingViewOnCustomAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        );

        void OnboardingViewOnStateUpdatedAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string elementId,
            AdaptyOnboardingsStateUpdatedParams @params
        );

        void OnboardingViewOnAnalyticsEvent(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            AdaptyOnboardingsAnalyticsEvent analyticsEvent
        );
    }

    public static partial class Adapty
    {
        private static AdaptyEventListener m_Listener;
        private static AdaptyPaywallsEventsListener m_PaywallsEventsListener;
        private static AdaptyOnboardingsEventsListener m_OnboardingsEventsListener;

        public static void SetEventListener(AdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
        }

        public static void SetPaywallsEventsListener(AdaptyPaywallsEventsListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_PaywallsEventsListener = listener;
        }

        public static void SetOnboardingsEventsListener(AdaptyOnboardingsEventsListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_OnboardingsEventsListener = listener;
        }

        internal static void OnMessage(string id, string json)
        {
            if (string.IsNullOrEmpty(json) || m_Listener == null)
            {
                return;
            }

            var response = JSONNode.Parse(json);
            if (response == null || response.IsNull)
            {
                return;
            }

            if (!response.IsObject)
            {
                return;
            }

            var parameters = response.AsObject;
            switch (id)
            {
                case "did_load_latest_profile":
                {
                    var profile = parameters.GetAdaptyProfile("profile");
                    try
                    {
                        m_Listener.OnLoadLatestProfile(profile);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.OnLoadLatestProfile((..)",
                            e
                        );
                    }
                    return;
                }
                case "on_installation_details_success":
                {
                    var details = parameters.GetAdaptyInstallationDetails("details");
                    try
                    {
                        m_Listener.OnInstallationDetailsSuccess(details);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.OnInstallationDetailsSuccess(..)",
                            e
                        );
                    }
                    return;
                }
                case "on_installation_details_fail":
                {
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_Listener.OnInstallationDetailsFail(error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.OnInstallationDetailsFail(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_did_fail_with_error":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewDidFailWithError(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewDidFailWithError(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_on_analytics_action":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var meta = parameters.GetAdaptyUIOnboardingMeta("meta");
                    var ev = parameters.GetOnboardingsAnalyticsEvent("event");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewOnAnalyticsEvent(view, meta, ev);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewOnAnalyticsEvent(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_did_finish_loading":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var meta = parameters.GetAdaptyUIOnboardingMeta("meta");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewDidFinishLoading(view, meta);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewDidFinishLoading(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_on_close_action":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var meta = parameters.GetAdaptyUIOnboardingMeta("meta");
                    var actionId = parameters.GetString("action_id");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewOnCloseAction(
                            view,
                            meta,
                            actionId
                        );
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewOnCloseAction(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_on_paywall_action":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var meta = parameters.GetAdaptyUIOnboardingMeta("meta");
                    var actionId = parameters.GetString("action_id");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewOnPaywallAction(
                            view,
                            meta,
                            actionId
                        );
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewOnPaywallAction(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_on_custom_action":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var meta = parameters.GetAdaptyUIOnboardingMeta("meta");
                    var actionId = parameters.GetString("action_id");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewOnCustomAction(
                            view,
                            meta,
                            actionId
                        );
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewOnCustomAction(..)",
                            e
                        );
                    }
                    return;
                }
                case "onboarding_on_state_updated_action":
                {
                    var view = parameters.GetAdaptyUIOnboardingView("view");
                    var meta = parameters.GetAdaptyUIOnboardingMeta("meta");
                    var elementId = JSONNodeExtensions
                        .GetObject(parameters, "action")
                        .GetString("element_id");
                    var @params = parameters.GetOnboardingsStateUpdatedParams("action");
                    try
                    {
                        m_OnboardingsEventsListener.OnboardingViewOnStateUpdatedAction(
                            view,
                            meta,
                            elementId,
                            @params
                        );
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyOnboardingsEventsListener.OnboardingViewOnStateUpdatedAction(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_appear":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidAppear(view);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidAppear((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_disappear":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidDisappear(view);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidDisappear((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_perform_action":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var action = parameters.GetAdaptyUIUserAction("action");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidPerformAction(view, action);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidPerformAction((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_select_product":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var productId = parameters.GetString("product_id");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidSelectProduct(view, productId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidSelectProduct((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_start_purchase":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var product = parameters.GetAdaptyPaywallProduct("product");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidStartPurchase(view, product);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidStartPurchase((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_finish_purchase":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var product = parameters.GetAdaptyPaywallProduct("product");
                    var purchaseResult = parameters.GetAdaptyPurchaseResult("purchased_result");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFinishPurchase(
                            view,
                            product,
                            purchaseResult
                        );
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFinishPurchase((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_purchase":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var product = parameters.GetAdaptyPaywallProduct("product");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailPurchase(view, product, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFailPurchase((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_start_restore":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidStartRestore(view);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidStartRestore((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_finish_restore":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var profile = parameters.GetAdaptyProfile("profile");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFinishRestore(view, profile);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFinishRestore((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_restore":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailRestore(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFailRestore((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_rendering":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailRendering(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFailRendering((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_loading_products":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailLoadingProducts(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFailLoadingProducts((..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_finish_web_payment_navigation":
                {
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var product = parameters.GetAdaptyPaywallProduct("product");
                    var error = parameters.GetAdaptyErrorIfPresent("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFinishWebPaymentNavigation(
                            view,
                            product,
                            error
                        );
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.PaywallViewDidFinishWebPaymentNavigation((..)",
                            e
                        );
                    }
                    return;
                }
                default:
                    return;
            }
        }
    }
}
