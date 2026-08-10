//
//  AdaptyUIFlowView.cs
//  AdaptySDK
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public class AdaptyUIFlowView
    {
        private AdaptyUIFlowView() { }

        [DataMember(Name = "id", IsRequired = true)]
        public string Id;
        [DataMember(Name = "placement_id", IsRequired = true)]
        public string PlacementId;
        [DataMember(Name = "variation_id", IsRequired = true)]
        public string VariationId;

        /// <summary>
        /// The localization the view was actually built with.
        /// </summary>
        /// <remarks>
        /// This is the locale passed to <see cref="AdaptyUICreateFlowViewParameters.SetLocale(string)"/> when that
        /// localization exists, and the flow's default localization otherwise. It is null when the native SDK is
        /// older than iOS 4.0.2 / Android 4.0.1 and does not report it.
        /// </remarks>
        [DataMember(Name = "locale")]
        public string Locale;

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(PlacementId)}: {PlacementId}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(Locale)}: {Locale}";

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="iosPresentationStyle"></param> an [AdaptyUIIOSPresentationStyle] object, for which is representing the iOS presentation style.
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Present(
            AdaptyUIIOSPresentationStyle iosPresentationStyle,
            System.Action<AdaptyError> completionHandler
        ) => AdaptyUI.PresentFlowView(this, iosPresentationStyle, completionHandler);

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Present(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.PresentFlowView(
                this,
                AdaptyUIIOSPresentationStyle.FullScreen,
                completionHandler
            );

        /// <summary>
        /// Call this function if you wish to dismiss the view.
        /// </summary>
        /// <remarks>
        /// A dismissed view is released and cannot be presented again. Create a new view via <see cref="AdaptyUI.CreateFlowView(AdaptyFlow, AdaptyUICreateFlowViewParameters, System.Action{AdaptyUIFlowView, AdaptyError})"/> if you need to re-present it.
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Dismiss(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DismissFlowView(this, completionHandler);
    }
}
