//
//  Adapty.Events.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using UnityEngine;
using AdaptySDK.Serialization;
using Newtonsoft.Json.Linq;
#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{
    public static partial class Adapty
    {
        private static IAdaptyEventListener m_Listener;
        private static IAdaptyFlowsEventsListener m_FlowsEventsListener;
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

        /// <summary>
        /// Entry point for every event the native side pushes.
        /// </summary>
        /// <remarks>
        /// Nothing is allowed to escape. The call arrives from native code - on iOS through a
        /// reverse-P/Invoke callback with no handler behind it - so an exception here does not
        /// surface as a C# error, it takes the process down on IL2CPP. A malformed payload or a
        /// throwing listener is logged and the event is dropped.
        /// </remarks>
        internal static void OnMessage(string id, string json)
        {
            if (string.IsNullOrEmpty(json))
                return;

            try
            {
                if (!(AdaptyJson.ParseDocument(json) is JObject parameters))
                {
                    return;
                }

                Dispatch(id, parameters);
            }
            catch (Exception e)
            {
                Debug.LogError(
                    string.Format("[Adapty] Event '{0}' failed: {1}", id ?? "(null)", e)
                );
            }
        }

        private static T Required<T>(JObject parameters, string key) =>
            JsonRequire.Token(parameters, key).ToObject<T>(AdaptyJson.CreateSerializerFor(typeof(T)));

        private static T Optional<T>(JObject parameters, string key)
        {
            var value = parameters[key];
            return value is null || value.Type == JTokenType.Null
                ? default(T)
                : value.ToObject<T>(AdaptyJson.CreateSerializerFor(typeof(T)));
        }

        private static void Dispatch(string id, JObject parameters)
        {
            switch (id)
            {
                case "did_load_latest_profile":
                    {
                        if (!RequireEventListener(id))
                            return;
                        var profile = Required<AdaptyProfile>(parameters, "profile");
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
                        var details = Required<AdaptyInstallationDetails>(parameters, "details");
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
                        var error = Required<AdaptyError>(parameters, "error");
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
                case "onboarding_on_analytics_action":
                case "onboarding_did_finish_loading":
                case "onboarding_on_close_action":
                case "onboarding_on_paywall_action":
                case "onboarding_on_custom_action":
                case "onboarding_on_state_updated_action":
                    OnLegacyOnboardingMessage(id, parameters);
                    return;
                case "flow_view_did_appear":
                    {
                        if (!RequireFlowsListener(id))
                            return;
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var action = Required<AdaptyUIUserAction>(parameters, "action");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var productId = Required<string>(parameters, "product_id");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var product = Required<AdaptyPaywallProduct>(parameters, "product");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var product = Required<AdaptyPaywallProduct>(parameters, "product");
                        var purchaseResult = Required<AdaptyPurchaseResult>(parameters, "purchased_result");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var product = Required<AdaptyPaywallProduct>(parameters, "product");
                        var error = Required<AdaptyError>(parameters, "error");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var profile = Required<AdaptyProfile>(parameters, "profile");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var error = Required<AdaptyError>(parameters, "error");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var error = Required<AdaptyError>(parameters, "error");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var error = Required<AdaptyError>(parameters, "error");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var product = Optional<AdaptyPaywallProduct>(parameters, "product");
                        var error = Optional<AdaptyError>(parameters, "error");
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var name = Required<string>(parameters, "name");
                        var @params = Required<Dictionary<string, object>>(parameters, "params");
                        try
                        {
                            m_FlowsEventsListener.FlowViewDidReceiveAnalyticEvent(
                                view,
                                name,
                                new ReadOnlyDictionary<string, object>(@params)
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var eventId = Required<string>(parameters, "event_id");
                        var permission = Required<string>(parameters, "permission");

                        var customArgs = Optional<Dictionary<string, string>>(
                            parameters,
                            "custom_args"
                        );

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
                                customArgs is null
                                    ? null
                                    : new ReadOnlyDictionary<string, string>(customArgs),
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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");

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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var eventId = Required<string>(parameters, "event_id");
                        var product = Required<AdaptyPaywallProduct>(parameters, "product");

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
                        var view = Required<AdaptyUIFlowView>(parameters, "view");
                        var eventId = Required<string>(parameters, "event_id");

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
