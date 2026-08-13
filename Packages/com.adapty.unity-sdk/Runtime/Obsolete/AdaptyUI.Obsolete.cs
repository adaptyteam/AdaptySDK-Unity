using System;
using System.Collections.Generic;
using AdaptySDK.Serialization;
using Newtonsoft.Json.Linq;

namespace AdaptySDK
{
    public static partial class AdaptyUI
    {
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
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use CreateFlowView instead."
        )]
        public static void CreateOnboardingView(
            AdaptyOnboarding onboarding,
            AdaptyWebPresentation externalUrlsPresentation,
            Action<AdaptyUIOnboardingView, AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["onboarding"] = AdaptyJson.ToNode(onboarding);
            parameters["external_urls_presentation"] = AdaptyJson.ToNode(externalUrlsPresentation);

            AdaptyRequest.Send("adapty_ui_create_onboarding_view", parameters, completionHandler);
        }

        /// <summary>
        /// Presents the onboarding view to the user.
        /// </summary>
        /// <remarks>
        /// This method presents the onboarding view using the default full-screen presentation style.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIOnboardingView"/> object representing the view to present.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use PresentFlowView instead."
        )]
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
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use PresentFlowView instead."
        )]
        public static void PresentOnboardingView(
            AdaptyUIOnboardingView view,
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["id"] = view.Id;
            parameters["ios_presentation_style"] = AdaptyJson.ToNode(iosPresentationStyle);

            AdaptyRequest.SendVoid("adapty_ui_present_onboarding_view", parameters, completionHandler);
        }

        /// <summary>
        /// Dismisses the onboarding view.
        /// </summary>
        /// <remarks>
        /// Call this method when you want to dismiss the onboarding view from the screen.
        /// </remarks>
        /// <param name="view">An <see cref="AdaptyUIOnboardingView"/> object representing the view to dismiss.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use DismissFlowView instead."
        )]
        public static void DismissOnboardingView(
            AdaptyUIOnboardingView view,
            Action<AdaptyError> completionHandler
        ) => DismissOnboardingView(view, false, completionHandler);

        [Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
        private static void DismissOnboardingView(
            AdaptyUIOnboardingView view,
            bool destroy,
            Action<AdaptyError> completionHandler
        )
        {
            var parameters = new JObject();
            parameters["id"] = view.Id;
            parameters["destroy"] = destroy;

            AdaptyRequest.SendVoid("adapty_ui_dismiss_onboarding_view", parameters, completionHandler);
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
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use the AdaptyUIFlowView overload instead."
        )]
        public static void ShowDialog(
            AdaptyUIOnboardingView view,
            AdaptyUIDialogConfiguration configuration,
            Action<AdaptyUIDialogActionType, AdaptyError> completionHandler
        )
        {
            ShowDialog(view.Id, configuration, completionHandler);
        }

        /// <summary>
        /// Creates an onboarding view from an AdaptyOnboarding object.
        /// </summary>
        /// <param name="onboarding">An <see cref="AdaptyOnboarding"/> object for which you are trying to create a view.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyUIOnboardingView"/> object.</param>
        [Obsolete(
            "The legacy onboarding API is deprecated in favor of Flows. Use CreateFlowView instead."
        )]
        public static void CreateOnboardingView(
            AdaptyOnboarding onboarding,
            Action<AdaptyUIOnboardingView, AdaptyError> completionHandler
        ) =>
            CreateOnboardingView(
                onboarding,
                AdaptyWebPresentation.ExternalBrowser,
                completionHandler
            );
    }
}
