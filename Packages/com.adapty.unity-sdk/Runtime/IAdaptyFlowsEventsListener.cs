using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    /// <summary>
    /// Interface for listening to flow view events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about flow view lifecycle, user actions, purchases, and errors.
    /// Use <see cref="Adapty.SetFlowsEventsListener(IAdaptyFlowsEventsListener)"/> to register your listener.
    /// Note that the SDK applies no default behavior to these events: a successful purchase or an error does not dismiss the view automatically — call <see cref="AdaptyUIFlowView.Dismiss(Action{AdaptyError})"/> yourself when appropriate.
    /// </remarks>
    public interface IAdaptyFlowsEventsListener
    {
        /// <summary>
        /// Called when the flow view appears on screen.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that appeared.</param>
        void FlowViewDidAppear(AdaptyUIFlowView view);

        /// <summary>
        /// Called when the flow view disappears from screen.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that disappeared.</param>
        void FlowViewDidDisappear(AdaptyUIFlowView view);

        /// <summary>
        /// Called when a user performs an action in the flow view (e.g., close, system back, opening a URL, custom actions).
        /// </summary>
        /// <remarks>
        /// The Android system back button is delivered here as a <c>system_back</c> action and does not dismiss the view automatically.
        /// To keep the default URL behavior for <c>open_url</c> actions, call <see cref="AdaptyUI.OpenUrl(string, AdaptyWebPresentation, Action{AdaptyError})"/>.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the action occurred.</param>
        /// <param name="action">The <see cref="AdaptyUIUserAction"/> object describing the action.</param>
        void FlowViewDidPerformAction(AdaptyUIFlowView view, AdaptyUIUserAction action);

        /// <summary>
        /// Called when a user selects a product in the flow view.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the selection occurred.</param>
        /// <param name="productId">The identifier of the selected product.</param>
        void FlowViewDidSelectProduct(AdaptyUIFlowView view, string productId);

        /// <summary>
        /// Called when a purchase is initiated for a product.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase was initiated.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> being purchased.</param>
        void FlowViewDidStartPurchase(AdaptyUIFlowView view, AdaptyPaywallProduct product);

        /// <summary>
        /// Called when a purchase is successfully completed.
        /// </summary>
        /// <remarks>
        /// The view is not dismissed automatically — call <see cref="AdaptyUIFlowView.Dismiss(Action{AdaptyError})"/> if desired.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase was completed.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> that was purchased.</param>
        /// <param name="purchasedResult">The <see cref="AdaptyPurchaseResult"/> object containing purchase details.</param>
        void FlowViewDidFinishPurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyPurchaseResult purchasedResult
        );

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase failed.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> that failed to purchase.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidFailPurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            AdaptyError error
        );

        /// <summary>
        /// Called when the restore purchases process is initiated.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore was initiated.</param>
        void FlowViewDidStartRestore(AdaptyUIFlowView view);

        /// <summary>
        /// Called when the restore purchases process completes successfully.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore was completed.</param>
        /// <param name="profile">The updated <see cref="AdaptyProfile"/> object containing restored purchases.</param>
        void FlowViewDidFinishRestore(AdaptyUIFlowView view, AdaptyProfile profile);

        /// <summary>
        /// Called when the restore purchases process fails.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore failed.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidFailRestore(AdaptyUIFlowView view, AdaptyError error);

        /// <summary>
        /// Called when the flow view receives an error (including rendering failures).
        /// </summary>
        /// <remarks>
        /// The view is not dismissed automatically — call <see cref="AdaptyUIFlowView.Dismiss(Action{AdaptyError})"/> if desired.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that received the error.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidReceiveError(AdaptyUIFlowView view, AdaptyError error);

        /// <summary>
        /// Called when the flow view fails to load products.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> that failed to load products.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void FlowViewDidFailLoadingProducts(AdaptyUIFlowView view, AdaptyError error);

        /// <summary>
        /// Called when web payment navigation finishes (for web-based purchases).
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the navigation occurred.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> associated with the web payment, or null.</param>
        /// <param name="error">The <see cref="AdaptyError"/> object, or null if no error occurred.</param>
        void FlowViewDidFinishWebPaymentNavigation(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product, // can be null
            AdaptyError error // can be null if no error occurred
        );

        /// <summary>
        /// Called when the flow view emits a customer-facing analytics event.
        /// </summary>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the event occurred.</param>
        /// <param name="name">The name of the analytics event.</param>
        /// <param name="parameters">The parameters of the analytics event.</param>
        void FlowViewDidReceiveAnalyticEvent(
            AdaptyUIFlowView view,
            string name,
            IReadOnlyDictionary<string, object> parameters
        );
    }
}
