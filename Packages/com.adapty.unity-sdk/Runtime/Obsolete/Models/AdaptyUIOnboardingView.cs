using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyUIOnboardingView
    {
        private AdaptyUIOnboardingView() { }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id;
        [DataMember(Name = "placement_id", IsRequired = true)]
        public string PlacementId;
        [DataMember(Name = "variation_id", IsRequired = true)]
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
        [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
        public void Present(
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            System.Action<AdaptyError> completionHandler
        ) => AdaptyUI.PresentOnboardingView(this, iosPresentationStyle, completionHandler);

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
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
        [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
        public void Dismiss(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DismissOnboardingView(this, completionHandler);
    }
}
