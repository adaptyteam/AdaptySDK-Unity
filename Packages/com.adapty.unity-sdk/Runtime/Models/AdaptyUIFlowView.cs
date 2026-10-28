using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// A flow view built by <see cref="AdaptyUI.CreateFlowView(AdaptySDK.AdaptyFlow, AdaptySDK.AdaptyUICreateFlowViewParameters, System.Action{AdaptySDK.AdaptyUIFlowView, AdaptySDK.AdaptyError})"/>. Dismissing it releases it
    /// unless it is dismissed with <c>destroy</c> false, which keeps it for another
    /// <see cref="Present(System.Action{AdaptyError})"/> until <see cref="Destroy"/>.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyUIFlowView
    {
        private AdaptyUIFlowView() { }

        /// <summary>
        /// The identifier of this view, which the events carry back.
        /// </summary>
        [DataMember(Name = "id", IsRequired = true)]
        public string Id;
        /// <summary>
        /// The placement the flow behind this view was fetched for.
        /// </summary>
        [DataMember(Name = "placement_id", IsRequired = true)]
        public string PlacementId;
        /// <summary>
        /// The variation the flow resolved to. Purchases made here are attributed to it.
        /// </summary>
        [DataMember(Name = "variation_id", IsRequired = true)]
        public string VariationId;

        /// <summary>
        /// The localization the view was actually built with.
        /// </summary>
        /// <remarks>
        /// May differ from the localization requested through
        /// <see cref="AdaptyUICreateFlowViewParameters.SetLocale(string)"/> when that one does not exist in the
        /// flow, and may be null when the native side reports none.
        /// </remarks>
        [DataMember(Name = "locale")]
        public string Locale;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(PlacementId)}: {PlacementId}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(Locale)}: {Locale}";

        /// <summary>
        /// Call this function if you wish to present the view.
        /// </summary>
        /// <param name="iosPresentationStyle">An <see cref="AdaptyUIIOSPresentationStyle"/> object representing the iOS presentation style (iOS only).</param>
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
        /// Dismisses the view and releases it.
        /// </summary>
        /// <remarks>
        /// A released view cannot be presented again. Create a new view via <see cref="AdaptyUI.CreateFlowView(AdaptyFlow, AdaptyUICreateFlowViewParameters, System.Action{AdaptyUIFlowView, AdaptyError})"/> if you need to re-present it,
        /// or keep this one with <see cref="Dismiss(bool, System.Action{AdaptyError})"/>.
        /// </remarks>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Dismiss(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DismissFlowView(this, true, completionHandler);

        /// <summary>
        /// Dismisses the view, and releases it unless asked to keep it.
        /// </summary>
        /// <remarks>
        /// A kept view can be presented again and resumes where it was. It stays in memory until
        /// <see cref="Destroy"/>.
        /// </remarks>
        /// <param name="destroy">Whether to release the view once it is dismissed.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Dismiss(bool destroy, System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DismissFlowView(this, destroy, completionHandler);

        /// <summary>
        /// Releases a view kept by <see cref="Dismiss(bool, System.Action{AdaptyError})"/>, dismissing
        /// it first if it is on screen.
        /// </summary>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public void Destroy(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DestroyFlowView(this, completionHandler);
    }
}
