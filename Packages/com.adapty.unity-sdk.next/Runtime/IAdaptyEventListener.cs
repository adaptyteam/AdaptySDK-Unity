using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using AdaptySDK.SimpleJSON;
#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{

    /// <summary>
    /// Interface for listening to Adapty SDK events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about profile updates and installation details.
    /// Use <see cref="Adapty.SetEventListener(IAdaptyEventListener)"/> to register your listener.
    /// </remarks>
    public interface IAdaptyEventListener
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
    /// Interface for listening to flow view events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about flow view lifecycle, user actions, purchases, and errors.
    /// Use <see cref="Adapty.SetFlowsEventsListener(IAdaptyFlowsEventsListener)"/> to register your listener.
    /// Note that the SDK applies no default behavior to these events: a successful purchase or an error does not dismiss the view automatically — call <see cref="AdaptyUIFlowView.Dismiss(Action{AdaptyError})"/> yourself when appropriate.
    /// </remarks>
    public interface IAdaptyFlowsEventsListener
    {
        /// <summary>
        /// Called when the flow view appears on screen.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that appeared.</param>
        void FlowViewDidAppear(AdaptyUIFlowView view);

        /// <summary>
        /// Called when the flow view disappears from screen.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that disappeared.</param>
        void FlowViewDidDisappear(AdaptyUIFlowView view);

        /// <summary>
        /// Called when a user performs an action in the flow view (e.g., close, system back, opening a URL, custom actions).
        /// </summary>
        /// <remarks>
        /// The Android system back button is delivered here as a <c>system_back</c> action and does not dismiss the view automatically.
        /// To keep the default URL behavior for <c>open_url</c> actions, call <see cref="AdaptyUI.OpenUrl(string, AdaptyWebPresentation, Action{AdaptyError})"/>.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the action occurred.</param>
        /// <param name="action">The <see cref="AdaptyUIUserAction"/> object describing the action.</param>
        void FlowViewDidPerformAction(AdaptyUIFlowView view, AdaptyUIUserAction action);

        /// <summary>
        /// Called when a user selects a product in the flow view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the selection occurred.</param>
        /// <param name="productId">The identifier of the selected product.</param>
        void FlowViewDidSelectProduct(AdaptyUIFlowView view, string productId);

        /// <summary>
        /// Called when a purchase is initiated for a product.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase was initiated.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> being purchased.</param>
        void FlowViewDidStartPurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product);

        /// <summary>
        /// Called when a purchase is successfully completed.
        /// </summary>
        /// <remarks>
        /// The view is not dismissed automatically — call <see cref="AdaptyUIFlowView.Dismiss(Action{AdaptyError})"/> if desired.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase was completed.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> that was purchased.</param>
        /// <param name="purchasedResult">The <see cref="AdaptyPurchaseResult"/> object containing purchase details.</param>
        void FlowViewDidFinishPurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyPurchaseResult purchasedResult
        );

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase failed.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> that failed to purchase.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidFailPurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        );

        /// <summary>
        /// Called when the restore purchases process is initiated.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore was initiated.</param>
        void FlowViewDidStartRestore(AdaptyUIFlowView view);

        /// <summary>
        /// Called when the restore purchases process completes successfully.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore was completed.</param>
        /// <param name="profile">The updated <see cref="AdaptyProfile"/> object containing restored purchases.</param>
        void FlowViewDidFinishRestore(AdaptyUIFlowView view, AdaptyProfile profile);

        /// <summary>
        /// Called when the restore purchases process fails.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore failed.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidFailRestore(AdaptyUIFlowView view, AdaptyError error);

        /// <summary>
        /// Called when the flow view receives an error (including rendering failures).
        /// </summary>
        /// <remarks>
        /// The view is not dismissed automatically — call <see cref="AdaptyUIFlowView.Dismiss(Action{AdaptyError})"/> if desired.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that received the error.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidReceiveError(AdaptyUIFlowView view, AdaptyError error);

        /// <summary>
        /// Called when the flow view fails to load products.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that failed to load products.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidFailLoadingProducts(AdaptyUIFlowView view, AdaptyError error);

        /// <summary>
        /// Called when web payment navigation finishes (for web-based purchases).
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the navigation occurred.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> associated with the web payment, or null.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object, or null if no error occurred.</param>
        void FlowViewDidFinishWebPaymentNavigation(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product, // can be null
            AdaptyError error // can be null if no error occurred
        );

        /// <summary>
        /// Called when the flow view emits a customer-facing analytics event.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the event occurred.</param>
        /// <param name="name">The name of the analytics event.</param>
        /// <param name="params">The parameters of the analytics event.</param>
        void FlowViewDidReceiveAnalyticEvent(
            AdaptyUIFlowView view,
            string name,
            IDictionary<string, object> @params
        );
    }

    /// <summary>
    /// Interface for handling system requests initiated by a flow: OS permission prompts and store review requests.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Adapty.SetSystemRequestsHandler(IAdaptyUISystemRequestsHandler)"/> to register your handler.
    /// If no handler is registered, permission requests are ignored (no answer is sent), and app review requests fall back to <see cref="AdaptyUI.RequestAppReview(Action{AdaptyError})"/>.
    /// </remarks>
    public interface IAdaptyUISystemRequestsHandler
    {
        /// <summary>
        /// Called when a flow asks for an OS permission.
        /// </summary>
        /// <remarks>
        /// Request the permission from the OS yourself, then invoke <paramref name="respond"/> exactly once with the outcome.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that asked for the permission.</param>
        /// <param name="permission">The permission identifier (e.g., "push", "camera", "tracking"). Unknown values pass through unchanged.</param>
        /// <param name="customArgs">Optional custom arguments configured in the Adapty Dashboard, or null.</param>
        /// <param name="respond">Invoke with the outcome: granted flag and an optional detail string (may be null).</param>
        void FlowViewDidAskPermission(
            AdaptyUIFlowView view,
            string permission,
            IDictionary<string, string> customArgs,
            Action<bool, string> respond
        );

        /// <summary>
        /// Called when a flow requests a native store review prompt.
        /// </summary>
        /// <remarks>
        /// To keep the default behavior, call <see cref="AdaptyUI.RequestAppReview(Action{AdaptyError})"/>.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that requested the review.</param>
        void FlowViewDidRequestAppReview(AdaptyUIFlowView view);
    }

    /// <summary>
    /// Interface for resolving purchases and restores initiated by a flow while the SDK runs in Observer mode.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Adapty.SetObserverModeResolver(IAdaptyUIObserverModeResolver)"/> to register your resolver.
    /// Read more at <see href="https://adapty.io/docs/observer-vs-full-mode">Adapty Documentation</see>
    /// </remarks>
    public interface IAdaptyUIObserverModeResolver
    {
        /// <summary>
        /// Called when a user initiates a purchase in a flow view while the SDK runs in Observer mode.
        /// </summary>
        /// <remarks>
        /// Perform the purchase with your own billing implementation. Invoke <paramref name="onStartPurchase"/> when your purchase flow starts and <paramref name="onFinishPurchase"/> when it finishes (successfully or not).
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase was initiated.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> being purchased.</param>
        /// <param name="onStartPurchase">Invoke when your purchase flow starts.</param>
        /// <param name="onFinishPurchase">Invoke when your purchase flow finishes.</param>
        void FlowViewDidInitiatePurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            Action onStartPurchase,
            Action onFinishPurchase
        );

        /// <summary>
        /// Called when a user initiates a restore in a flow view while the SDK runs in Observer mode.
        /// </summary>
        /// <remarks>
        /// Perform the restore with your own billing implementation. Invoke <paramref name="onStartRestore"/> when your restore flow starts and <paramref name="onFinishRestore"/> when it finishes (successfully or not).
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore was initiated.</param>
        /// <param name="onStartRestore">Invoke when your restore flow starts.</param>
        /// <param name="onFinishRestore">Invoke when your restore flow finishes.</param>
        void FlowViewDidInitiateRestore(
            AdaptyUIFlowView view,
            Action onStartRestore,
            Action onFinishRestore
        );
    }

    /// <summary>
    /// Interface for listening to onboarding view events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about onboarding view lifecycle, user actions, and analytics events.
    /// Use <see cref="Adapty.SetOnboardingsEventsListener(IAdaptyOnboardingsEventsListener)"/> to register your listener.
    /// </remarks>
    public interface IAdaptyOnboardingsEventsListener
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
        private static IAdaptyEventListener m_Listener;
        private static IAdaptyFlowsEventsListener m_FlowsEventsListener;
        private static IAdaptyOnboardingsEventsListener m_OnboardingsEventsListener;
        private static IAdaptyUISystemRequestsHandler m_SystemRequestsHandler;
        private static IAdaptyUIObserverModeResolver m_ObserverModeResolver;

        /// <summary>
        /// Sets the event listener for Adapty SDK events.
        /// </summary>
        /// <param name="listener">The <see cref="IAdaptyEventListener"/> implementation to receive events, or null to detach the previous one.</param>
        public static void SetEventListener(IAdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
        }

        /// <summary>
        /// Sets the event listener for flow view events.
        /// </summary>
        /// <param name="listener">The <see cref="IAdaptyFlowsEventsListener"/> implementation to receive events, or null to detach the previous one.</param>
        public static void SetFlowsEventsListener(IAdaptyFlowsEventsListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_FlowsEventsListener = listener;
        }

        /// <summary>
        /// Sets the handler for system requests initiated by a flow (OS permission prompts and store review requests).
        /// </summary>
        /// <param name="handler">The <see cref="IAdaptyUISystemRequestsHandler"/> implementation to receive requests, or null to detach the previous one.</param>
        public static void SetSystemRequestsHandler(IAdaptyUISystemRequestsHandler handler)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_SystemRequestsHandler = handler;
        }

        /// <summary>
        /// Sets the resolver for purchases and restores initiated by a flow while the SDK runs in Observer mode.
        /// </summary>
        /// <param name="resolver">The <see cref="IAdaptyUIObserverModeResolver"/> implementation to resolve purchases and restores, or null to detach the previous one.</param>
        public static void SetObserverModeResolver(IAdaptyUIObserverModeResolver resolver)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_ObserverModeResolver = resolver;
        }

        /// <summary>
        /// Sets the event listener for onboarding view events.
        /// </summary>
        /// <param name="listener">The <see cref="IAdaptyOnboardingsEventsListener"/> implementation to receive events, or null to detach the previous one.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use SetFlowsEventsListener instead."
        )]
        public static void SetOnboardingsEventsListener(IAdaptyOnboardingsEventsListener listener)
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

        private static bool RequireFlowsListener(string eventId)
        {
            if (m_FlowsEventsListener == null)
            {
                Debug.LogWarning(
                    string.Format(
                        "[Adapty] Flows events listener is not set, ignoring event '{0}'. Call Adapty.SetFlowsEventsListener() to receive flow events.",
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
                                "Failed to invoke IAdaptyEventListener.OnLoadLatestProfile(..)",
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
                                "Failed to invoke IAdaptyEventListener.OnInstallationDetailsSuccess(..)",
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
                                "Failed to invoke IAdaptyEventListener.OnInstallationDetailsFail(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewDidFailWithError(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewOnAnalyticsEvent(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewDidFinishLoading(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewOnCloseAction(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewOnPaywallAction(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewOnCustomAction(..)",
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
                                "Failed to invoke IAdaptyOnboardingsEventsListener.OnboardingViewOnStateUpdatedAction(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_appear":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidAppear(view);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidAppear(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_disappear":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidDisappear(view);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidDisappear(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_perform_action":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var action = parameters.GetAdaptyUIUserAction("action");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidPerformAction(view, action);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidPerformAction(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_select_product":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var productId = parameters.GetString("product_id");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidSelectProduct(view, productId);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidSelectProduct(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_start_purchase":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var product = parameters.GetAdaptyPaywallProduct("product");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidStartPurchase(view, product);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidStartPurchase(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_finish_purchase":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var product = parameters.GetAdaptyPaywallProduct("product");
                        var purchaseResult = parameters.GetAdaptyPurchaseResult("purchased_result");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidFinishPurchase(
                                view,
                                product,
                                purchaseResult
                            );
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidFinishPurchase(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_fail_purchase":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var product = parameters.GetAdaptyPaywallProduct("product");
                        var error = parameters.GetAdaptyError("error");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidFailPurchase(view, product, error);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidFailPurchase(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_start_restore":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidStartRestore(view);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidStartRestore(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_finish_restore":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var profile = parameters.GetAdaptyProfile("profile");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidFinishRestore(view, profile);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidFinishRestore(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_fail_restore":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var error = parameters.GetAdaptyError("error");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidFailRestore(view, error);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidFailRestore(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_receive_error":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var error = parameters.GetAdaptyError("error");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidReceiveError(view, error);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidReceiveError(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_fail_loading_products":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var error = parameters.GetAdaptyError("error");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidFailLoadingProducts(view, error);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidFailLoadingProducts(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_finish_web_payment_navigation":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var product = parameters.GetAdaptyPaywallProductIfPresent("product");
                        var error = parameters.GetAdaptyErrorIfPresent("error");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidFinishWebPaymentNavigation(
                                view,
                                product,
                                error
                            );
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidFinishWebPaymentNavigation(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_receive_analytic_event":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var name = parameters.GetString("name");
                        var @params = parameters.GetDictionary("params");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidReceiveAnalyticEvent(
                                view,
                                name,
                                @params
                            );
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyFlowsEventsListener.FlowViewDidReceiveAnalyticEvent(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_ask_permission":
                    {
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var eventId = parameters.GetString("event_id");
                        var permission = parameters.GetString("permission");

                        Dictionary<string, string> customArgs = null;
                        var customArgsObject = JSONNodeExtensions.GetObjectIfPresent(
                            parameters,
                            "custom_args"
                        );
                        if (customArgsObject != null)
                        {
                            customArgs = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, JSONNode> pair in customArgsObject)
                            {
                                customArgs[pair.Key] = pair.Value.Value;
                            }
                        }

                        if (m_SystemRequestsHandler == null)
                        {
                            // Send no answer: the native HostRequestRegistry keeps the request pending
                            // until the view tears down, then resolves it as denied there. Fabricating an
                            // answer here would duplicate that fallback across two layers — this matches
                            // both the native no-handler behavior and the Flutter SDK.
                            Debug.LogWarning(
                                string.Format(
                                    "[Adapty] System requests handler is not set, ignoring permission request '{0}'. Call Adapty.SetSystemRequestsHandler() to handle permission requests.",
                                    permission
                                )
                            );
                            return;
                        }

                        // respond(..) is typically invoked from an OS permission callback, off the main thread.
                        var answered = 0;
                        Action<bool, string> respond = (granted, detail) =>
                        {
                            if (Interlocked.Exchange(ref answered, 1) == 1)
                            {
                                Debug.LogWarning(
                                    "[Adapty] Permission request has already been answered, ignoring subsequent respond(..) call."
                                );
                                return;
                            }
                            AdaptyUI.FlowViewAnswerPermission(eventId, granted, detail);
                        };

                        try
                        {
                            m_SystemRequestsHandler.FlowViewDidAskPermission(
                                view,
                                permission,
                                customArgs,
                                respond
                            );
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyUISystemRequestsHandler.FlowViewDidAskPermission(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_did_request_app_review":
                    {
                        var view = parameters.GetAdaptyUIFlowView("view");

                        if (m_SystemRequestsHandler == null)
                        {
                            AdaptyUI.RequestAppReview(null);
                            return;
                        }

                        try
                        {
                            m_SystemRequestsHandler.FlowViewDidRequestAppReview(view);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyUISystemRequestsHandler.FlowViewDidRequestAppReview(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_observer_did_initiate_purchase":
                    {
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var eventId = parameters.GetString("event_id");
                        var product = parameters.GetAdaptyPaywallProduct("product");

                        if (m_ObserverModeResolver == null)
                        {
                            Debug.LogWarning(
                                "[Adapty] Observer mode resolver is not set, ignoring initiated purchase. Call Adapty.SetObserverModeResolver() to handle purchases in Observer mode."
                            );
                            return;
                        }

                        Action onStartPurchase = () =>
                            AdaptyUI.SendObserverEvent("observer_purchase_did_start", eventId);
                        Action onFinishPurchase = () =>
                            AdaptyUI.SendObserverEvent("observer_purchase_did_finish", eventId);

                        try
                        {
                            m_ObserverModeResolver.FlowViewDidInitiatePurchase(
                                view,
                                product,
                                onStartPurchase,
                                onFinishPurchase
                            );
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyUIObserverModeResolver.FlowViewDidInitiatePurchase(..)",
                                e
                            );
                        }
                        return;
                    }
                case "flow_view_observer_did_initiate_restore":
                    {
                        var view = parameters.GetAdaptyUIFlowView("view");
                        var eventId = parameters.GetString("event_id");

                        if (m_ObserverModeResolver == null)
                        {
                            Debug.LogWarning(
                                "[Adapty] Observer mode resolver is not set, ignoring initiated restore. Call Adapty.SetObserverModeResolver() to handle restores in Observer mode."
                            );
                            return;
                        }

                        Action onStartRestore = () =>
                            AdaptyUI.SendObserverEvent("observer_restore_did_start", eventId);
                        Action onFinishRestore = () =>
                            AdaptyUI.SendObserverEvent("observer_restore_did_finish", eventId);

                        try
                        {
                            m_ObserverModeResolver.FlowViewDidInitiateRestore(
                                view,
                                onStartRestore,
                                onFinishRestore
                            );
                        }
                        catch (Exception e)
                        {
                            throw new Exception(
                                "Failed to invoke IAdaptyUIObserverModeResolver.FlowViewDidInitiateRestore(..)",
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
