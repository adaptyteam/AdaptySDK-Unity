//
//  Adapty.Events.Obsolete.cs
//  AdaptySDK
//

using System;
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
        [Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
        private static IAdaptyOnboardingsEventsListener m_OnboardingsEventsListener;

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

        [Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
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

        /// <summary>
        /// Dispatches the events of the legacy onboarding API.
        /// </summary>
        /// <remarks>
        /// Split out of <see cref="Dispatch"/> so that the deprecation warnings it raises stay on
        /// this one method instead of on every case of the main switch.
        /// </remarks>
        [Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
        private static void OnLegacyOnboardingMessage(string id, JObject parameters)
        {
            switch (id)
            {
                case "onboarding_did_fail_with_error":
                    {
                        if (!RequireOnboardingsListener(id))
                            return;
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var error = Required<AdaptyError>(parameters, "error");
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
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var meta = Required<AdaptyUIOnboardingMeta>(parameters, "meta");
                        var ev = Required<AdaptyOnboardingsAnalyticsEvent>(parameters, "event");
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
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var meta = Required<AdaptyUIOnboardingMeta>(parameters, "meta");
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
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var meta = Required<AdaptyUIOnboardingMeta>(parameters, "meta");
                        var actionId = Required<string>(parameters, "action_id");
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
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var meta = Required<AdaptyUIOnboardingMeta>(parameters, "meta");
                        var actionId = Required<string>(parameters, "action_id");
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
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var meta = Required<AdaptyUIOnboardingMeta>(parameters, "meta");
                        var actionId = Required<string>(parameters, "action_id");
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
                        var view = Required<AdaptyUIOnboardingView>(parameters, "view");
                        var meta = Required<AdaptyUIOnboardingMeta>(parameters, "meta");
                        var elementId = JsonRequire.String(
                            JsonRequire.Object(parameters, "action"),
                            "element_id"
                        );
                        var @params = Required<AdaptyOnboardingsStateUpdatedParams>(parameters, "action");
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
            }
        }
    }
}
