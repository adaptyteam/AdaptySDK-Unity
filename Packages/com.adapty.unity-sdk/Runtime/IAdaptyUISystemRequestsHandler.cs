using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    /// <summary>
    /// Interface for handling system requests initiated by a flow: OS permission prompts and store review requests.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Adapty.SetSystemRequestsHandler(IAdaptyUISystemRequestsHandler)"/> to register your handler.
    /// If no handler is registered, permission requests are ignored (no answer is sent), and app review requests fall back to <see cref="AdaptyUI.RequestAppReview(Action{AdaptyError})"/>.
    /// </remarks>
    public interface IAdaptyUISystemRequestsHandler
    {
        /// <summary>
        /// Called when a flow asks for an OS permission.
        /// </summary>
        /// <remarks>
        /// Request the permission from the OS yourself, then invoke <paramref name="respond"/> exactly once with the outcome.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that asked for the permission.</param>
        /// <param name="permission">The permission identifier (e.g., "push", "camera", "tracking"). Unknown values pass through unchanged.</param>
        /// <param name="customArgs">Optional custom arguments configured in the Adapty Dashboard, or null.</param>
        /// <param name="respond">Invoke with the outcome: granted flag and an optional detail string (may be null).</param>
        void FlowViewDidAskPermission(
            AdaptyUIFlowView view,
            string permission,
            IReadOnlyDictionary<string, string> customArgs,
            Action<bool, string> respond
        );

        /// <summary>
        /// Called when a flow requests a native store review prompt.
        /// </summary>
        /// <remarks>
        /// To keep the default behavior, call <see cref="AdaptyUI.RequestAppReview(Action{AdaptyError})"/>.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that requested the review.</param>
        void FlowViewDidRequestAppReview(AdaptyUIFlowView view);
    }
}
