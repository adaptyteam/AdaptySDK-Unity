//
//  AdaptyUIFlowView.cs
//  AdaptySDK
//

namespace AdaptySDK
{
    public partial class AdaptyUIFlowView
    {
        public string Id;
        public string PlacementId;
        public string VariationId;

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(PlacementId)}: {PlacementId}, "
            + $"{nameof(VariationId)}: {VariationId}";

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
