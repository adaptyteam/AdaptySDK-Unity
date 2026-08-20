using System;

namespace AdaptySDK
{
    /// <summary>
    /// Interface for resolving purchases and restores initiated by a flow while the SDK runs in Observer mode.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Adapty.SetObserverModeResolver(IAdaptyUIObserverModeResolver)"/> to register your resolver.
    /// Read more at <see href="https://adapty.io/docs/observer-vs-full-mode">Adapty Documentation</see>
    /// </remarks>
    public interface IAdaptyUIObserverModeResolver
    {
        /// <summary>
        /// Called when a user initiates a purchase in a flow view while the SDK runs in Observer mode.
        /// </summary>
        /// <remarks>
        /// Perform the purchase with your own billing implementation. Invoke <paramref name="onStartPurchase"/> when your purchase flow starts and <paramref name="onFinishPurchase"/> when it finishes (successfully or not). Both are safe to invoke from any thread - the SDK sends the report from the Unity main thread.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the purchase was initiated.</param>
        /// <param name="product">The <see cref="AdaptyPaywallProduct"/> being purchased.</param>
        /// <param name="onStartPurchase">Invoke when your purchase flow starts.</param>
        /// <param name="onFinishPurchase">Invoke when your purchase flow finishes.</param>
        void FlowViewDidInitiatePurchase(
            AdaptyUIFlowView view,
            AdaptyPaywallProduct product,
            Action onStartPurchase,
            Action onFinishPurchase
        );

        /// <summary>
        /// Called when a user initiates a restore in a flow view while the SDK runs in Observer mode.
        /// </summary>
        /// <remarks>
        /// Perform the restore with your own billing implementation. Invoke <paramref name="onStartRestore"/> when your restore flow starts and <paramref name="onFinishRestore"/> when it finishes (successfully or not). Both are safe to invoke from any thread - the SDK sends the report from the Unity main thread.
        /// </remarks>
        /// <param name="view">The <see cref="AdaptyUIFlowView"/> where the restore was initiated.</param>
        /// <param name="onStartRestore">Invoke when your restore flow starts.</param>
        /// <param name="onFinishRestore">Invoke when your restore flow finishes.</param>
        void FlowViewDidInitiateRestore(
            AdaptyUIFlowView view,
            Action onStartRestore,
            Action onFinishRestore
        );
    }
}
