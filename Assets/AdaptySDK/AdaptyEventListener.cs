using System;
using UnityEngine;
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

    /// <summary>
    /// Interface for listening to Adapty SDK events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about profile updates and installation details.
    /// Use <see cref="Adapty.SetEventListener(AdaptyEventListener)"/> to register your listener.
    /// </remarks>
    public interface AdaptyEventListener
    {
        /// <summary>
        /// Called when the latest profile is loaded.
        /// </summary>
        /// <param name="profile">The updated <see cref="AdaptyProfile"/> object.</param>
        void OnLoadLatestProfile(AdaptyProfile profile);

        /// <summary>
        /// Called when installation details are successfully retrieved.
        /// </summary>
        /// <param name="details">The <see cref="AdaptyInstallationDetails"/> object containing installation information.</param>
        void OnInstallationDetailsSuccess(AdaptyInstallationDetails details);

        /// <summary>
        /// Called when installation details retrieval fails.
        /// </summary>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void OnInstallationDetailsFail(AdaptyError error);
    }

    /// <summary>
    /// Interface for listening to paywall view events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about paywall view lifecycle, user actions, purchases, and errors.
    /// Use <see cref="Adapty.SetPaywallsEventsListener(AdaptyPaywallsEventsListener)"/> to register your listener.
    /// </remarks>
    public interface AdaptyPaywallsEventsListener
    {
        /// <summary>
        /// Called when the paywall view appears on screen.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> that appeared.</param>
        void PaywallViewDidAppear(AdaptyUIPaywallView view);

        /// <summary>
        /// Called when the paywall view disappears from screen.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> that disappeared.</param>
        void PaywallViewDidDisappear(AdaptyUIPaywallView view);

        /// <summary>
        /// Called when a user performs an action in the paywall view (e.g., button tap, swipe).
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the action occurred.</param>
        /// <param name="action">The <see cref="AdaptyUIUserAction"/> object describing the action.</param>
        void PaywallViewDidPerformAction(AdaptyUIPaywallView view, AdaptyUIUserAction action);

        /// <summary>
        /// Called when a user selects a product in the paywall view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the selection occurred.</param>
        /// <param name="productId">The identifier of the selected product.</param>
        void PaywallViewDidSelectProduct(AdaptyUIPaywallView view, string productId);

        /// <summary>
        /// Called when a purchase is initiated for a product.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the purchase was initiated.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> being purchased.</param>
        void PaywallViewDidStartPurchase(AdaptyUIPaywallView view, AdaptyPaywallProduct product);

        /// <summary>
        /// Called when a purchase is successfully completed.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the purchase was completed.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> that was purchased.</param>
        /// <param name="purchasedResult">The <see cref="AdaptyPurchaseResult"/> object containing purchase details.</param>
        void PaywallViewDidFinishPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyPurchaseResult purchasedResult
        );

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the purchase failed.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> that failed to purchase.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void PaywallViewDidFailPurchase(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        );

        /// <summary>
        /// Called when the restore purchases process is initiated.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the restore was initiated.</param>
        void PaywallViewDidStartRestore(AdaptyUIPaywallView view);

        /// <summary>
        /// Called when the restore purchases process completes successfully.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the restore was completed.</param>
        /// <param name="profile">The updated <see cref="AdaptyProfile"/> object containing restored purchases.</param>
        void PaywallViewDidFinishRestore(AdaptyUIPaywallView view, AdaptyProfile profile);

        /// <summary>
        /// Called when the restore purchases process fails.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the restore failed.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void PaywallViewDidFailRestore(AdaptyUIPaywallView view, AdaptyError error);

        /// <summary>
        /// Called when the paywall view fails to render.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> that failed to render.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void PaywallViewDidFailRendering(AdaptyUIPaywallView view, AdaptyError error);

        /// <summary>
        /// Called when the paywall view fails to load products.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> that failed to load products.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void PaywallViewDidFailLoadingProducts(AdaptyUIPaywallView view, AdaptyError error);

        /// <summary>
        /// Called when web payment navigation finishes (for web-based purchases).
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIPaywallView"/> where the navigation occurred.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> associated with the web payment.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object, or null if no error occurred.</param>
        void PaywallViewDidFinishWebPaymentNavigation(
            AdaptyUIPaywallView view,
            AdaptyPaywallProduct product,
            AdaptyError error // can be null if no error occurred
        );
    }

    /// <summary>
    /// Interface for listening to onboarding view events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about onboarding view lifecycle, user actions, and analytics events.
    /// Use <see cref="Adapty.SetOnboardingsEventsListener(AdaptyOnboardingsEventsListener)"/> to register your listener.
    /// </remarks>
    public interface AdaptyOnboardingsEventsListener
    {
        /// <summary>
        /// Called when the onboarding view fails with an error.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> that failed.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void OnboardingViewDidFailWithError(AdaptyUIOnboardingView view, AdaptyError error);

        /// <summary>
        /// Called when the onboarding view finishes loading.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> that finished loading.</param>
        /// <param name="meta">The <see cref="AdaptyUIOnboardingMeta"/> object containing onboarding metadata.</param>
        void OnboardingViewDidFinishLoading(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta
        );

        /// <summary>
        /// Called when a close action is triggered in the onboarding view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> where the action occurred.</param>
        /// <param name="meta">The <see cref="AdaptyUIOnboardingMeta"/> object containing onboarding metadata.</param>
        /// <param name="actionId">The identifier of the close action.</param>
        void OnboardingViewOnCloseAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        );

        /// <summary>
        /// Called when a paywall action is triggered in the onboarding view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> where the action occurred.</param>
        /// <param name="meta">The <see cref="AdaptyUIOnboardingMeta"/> object containing onboarding metadata.</param>
        /// <param name="actionId">The identifier of the paywall action.</param>
        void OnboardingViewOnPaywallAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        );

        /// <summary>
        /// Called when a custom action is triggered in the onboarding view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> where the action occurred.</param>
        /// <param name="meta">The <see cref="AdaptyUIOnboardingMeta"/> object containing onboarding metadata.</param>
        /// <param name="actionId">The identifier of the custom action.</param>
        void OnboardingViewOnCustomAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string actionId
        );

        /// <summary>
        /// Called when the state of an element in the onboarding view is updated.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> where the update occurred.</param>
        /// <param name="meta">The <see cref="AdaptyUIOnboardingMeta"/> object containing onboarding metadata.</param>
        /// <param name="elementId">The identifier of the element whose state was updated.</param>
        /// <param name="params">The <see cref="AdaptyOnboardingsStateUpdatedParams"/> object containing the updated state parameters.</param>
        void OnboardingViewOnStateUpdatedAction(
            AdaptyUIOnboardingView view,
            AdaptyUIOnboardingMeta meta,
            string elementId,
            AdaptyOnboardingsStateUpdatedParams @params
        );

        /// <summary>
        /// Called when an analytics event is triggered in the onboarding view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIOnboardingView"/> where the event occurred.</param>
        /// <param name="meta">The <see cref="AdaptyUIOnboardingMeta"/> object containing onboarding metadata.</param>
        /// <param name="analyticsEvent">The <see cref="AdaptyOnboardingsAnalyticsEvent"/> object containing analytics event data.</param>
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

        /// <summary>
        /// Sets the event listener for Adapty SDK events.
        /// </summary>
        /// <param name="listener">The <see cref="AdaptyEventListener"/> implementation to receive events.</param>
        public static void SetEventListener(AdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
        }

        /// <summary>
        /// Sets the event listener for paywall view events.
        /// </summary>
        /// <param name="listener">The <see cref="AdaptyPaywallsEventsListener"/> implementation to receive events.</param>
        public static void SetPaywallsEventsListener(AdaptyPaywallsEventsListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_PaywallsEventsListener = listener;
        }

        /// <summary>
        /// Sets the event listener for onboarding view events.
        /// </summary>
        /// <param name="listener">The <see cref="AdaptyOnboardingsEventsListener"/> implementation to receive events.</param>
        public static void SetOnboardingsEventsListener(AdaptyOnboardingsEventsListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_OnboardingsEventsListener = listener;
        }

        private static bool RequireEventListener(string eventId)
        {
            if (m_Listener == null)
            {
                Debug.LogWarning(
                    string.Format(
                        "[Adapty] Event listener is not set, ignoring event '{0}'. Call Adapty.SetEventListener() to receive events.",
                        eventId
                    )
                );
                return false;
            }
            return true;
        }

        private static bool RequirePaywallsListener(string eventId)
        {
            if (m_PaywallsEventsListener == null)
            {
                Debug.LogWarning(
                    string.Format(
                        "[Adapty] Paywalls events listener is not set, ignoring event '{0}'. Call Adapty.SetPaywallsEventsListener() to receive paywall events.",
                        eventId
                    )
                );
                return false;
            }
            return true;
        }

        private static bool RequireOnboardingsListener(string eventId)
        {
            if (m_OnboardingsEventsListener == null)
            {
                Debug.LogWarning(
                    string.Format(
                        "[Adapty] Onboardings events listener is not set, ignoring event '{0}'. Call Adapty.SetOnboardingsEventsListener() to receive onboarding events.",
                        eventId
                    )
                );
                return false;
            }
            return true;
        }

        internal static void OnMessage(string id, string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            JSONNode response;
            try
            {
                response = JSONNode.Parse(json);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    string.Format(
                        "[Adapty] Failed to parse event JSON for event '{0}': {1}",
                        id ?? "(null)",
                        e.Message
                    )
                );
                return;
            }

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
                    if (!RequireEventListener(id))
                        return;
                    var profile = parameters.GetAdaptyProfile("profile");
                    try
                    {
                        m_Listener.OnLoadLatestProfile(profile);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyEventListener.OnLoadLatestProfile(..)",
                            e
                        );
                    }
                    return;
                }
                case "on_installation_details_success":
                {
                    if (!RequireEventListener(id))
                        return;
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
                    if (!RequireEventListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequireOnboardingsListener(id))
                        return;
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
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidAppear(view);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidAppear(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_disappear":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidDisappear(view);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidDisappear(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_perform_action":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var action = parameters.GetAdaptyUIUserAction("action");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidPerformAction(view, action);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidPerformAction(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_select_product":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var productId = parameters.GetString("product_id");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidSelectProduct(view, productId);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidSelectProduct(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_start_purchase":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var product = parameters.GetAdaptyPaywallProduct("product");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidStartPurchase(view, product);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidStartPurchase(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_finish_purchase":
                {
                    if (!RequirePaywallsListener(id))
                        return;
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
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFinishPurchase(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_purchase":
                {
                    if (!RequirePaywallsListener(id))
                        return;
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
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFailPurchase(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_start_restore":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidStartRestore(view);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidStartRestore(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_finish_restore":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var profile = parameters.GetAdaptyProfile("profile");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFinishRestore(view, profile);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFinishRestore(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_restore":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailRestore(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFailRestore(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_rendering":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailRendering(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFailRendering(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_fail_loading_products":
                {
                    if (!RequirePaywallsListener(id))
                        return;
                    var view = parameters.GetAdaptyUIPaywallView("view");
                    var error = parameters.GetAdaptyError("error");
                    try
                    {
                        m_PaywallsEventsListener.PaywallViewDidFailLoadingProducts(view, error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFailLoadingProducts(..)",
                            e
                        );
                    }
                    return;
                }
                case "paywall_view_did_finish_web_payment_navigation":
                {
                    if (!RequirePaywallsListener(id))
                        return;
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
                            "Failed to invoke AdaptyPaywallsEventsListener.PaywallViewDidFinishWebPaymentNavigation(..)",
                            e
                        );
                    }
                    return;
                }
                default:
                    Debug.LogWarning(
                        string.Format("[Adapty] Unknown event id '{0}', ignoring.", id ?? "(null)")
                    );
                    return;
            }
        }
    }
}
