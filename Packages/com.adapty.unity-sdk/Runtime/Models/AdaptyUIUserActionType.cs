using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK {
    /// <summary>
    /// What the user did in a flow view, as reported by <c>FlowViewDidPerformAction</c>.
    /// </summary>
    [Preserve]
    public enum AdaptyUIUserActionType {
        /// <summary>
        /// The close control of the flow was tapped. The view is not dismissed for you — call <see cref="AdaptyUI.DismissFlowView"/> if that is what you want.
        /// </summary>
        [EnumMember(Value = "close")]
        Close = 0,
        /// <summary>
        /// Android only. The system back button was pressed. Handled the same way as a close: the view stays until you dismiss it.
        /// </summary>
        [EnumMember(Value = "system_back")]
        SystemBack = 1,
        /// <summary>
        /// A link in the flow was tapped; the URL is on the action.
        /// </summary>
        [EnumMember(Value = "open_url")]
        OpenUrl = 2,
        /// <summary>
        /// An action the flow defines itself was triggered; its identifier is on the action.
        /// </summary>
        [EnumMember(Value = "custom")]
        Custom = 3,
    }
}