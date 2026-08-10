using System;

namespace AdaptySDK
{
    /// <summary>
    /// Interface for listening to onboarding view events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about onboarding view lifecycle, user actions, and analytics events.
    /// Use <see cref="Adapty.SetOnboardingsEventsListener(IAdaptyOnboardingsEventsListener)"/> to register your listener.
    /// Part of the legacy onboarding API, which is deprecated in favor of flows — see <see cref="IAdaptyFlowsEventsListener"/>.
    /// </remarks>
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
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
}
