//
//  AdaptyUIOnboardingView.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK
{
    public partial class AdaptyUIOnboardingView
    {
        public string Id;
        public string PlacementId;
        public string PaywallVariationId;

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(PlacementId)}: {PlacementId}, "
            + $"{nameof(PaywallVariationId)}: {PaywallVariationId}";

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="iosPresentationStyle">an [AdaptyUIIOSPresentationStyle] object, for which is representing the iOS presentation style.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Present(
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            System.Action<AdaptyError> completionHandler
        ) => AdaptyUI.PresentOnboardingView(this, iosPresentationStyle, completionHandler);

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Present(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.PresentOnboardingView(
                this,
                AdaptyUIIOSPresentationStyle.FullScreen,
                completionHandler
            );

        /// <summary>
        /// Call this function if you wish to dismiss the view.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Dismiss(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DismissOnboardingView(this, completionHandler);
    }
}
