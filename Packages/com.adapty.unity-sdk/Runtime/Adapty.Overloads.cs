using System;
using System.Collections.Generic;
using AdaptySDK.Serialization;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        /// <summary>
        /// This method enables you to retrieve the flow from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products-unity">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetFlowForDefaultAudience(
            string placementId,
            Action<AdaptyFlow, AdaptyError> completionHandler
        ) => GetFlowForDefaultAudience(placementId, null, completionHandler);

        /// <summary>
        /// Makes a purchase for the specified product.
        /// </summary>
        /// <remarks>
        /// This method initiates the purchase flow for a product. The result contains information about the purchase status.
        /// Read more on the <see href="https://adapty.io/docs/unity-making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">An <see cref="AdaptyPaywallProduct"/> object retrieved from the paywall.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyPurchaseResult"/> object.</param>
        public static void MakePurchase(
            AdaptyPaywallProduct product,
            Action<AdaptyPurchaseResult, AdaptyError> completionHandler
        ) => MakePurchase(product, null, completionHandler);

        /// <summary>
        /// Reports a transaction to Adapty in Observer mode.
        /// </summary>
        /// <remarks>
        /// In Observer mode, Adapty SDK doesn't know where the purchase was made from.
        /// If you display products using Adapty Paywalls or A/B Tests, you can manually assign a variation to the purchase.
        /// After doing this, you'll be able to see metrics in the Adapty Dashboard.
        /// Read more at <see href="https://adapty.io/docs/observer-vs-full-mode">Adapty Documentation</see>
        /// </remarks>
        /// <param name="transactionId">A string identifier of your purchased transaction. For iOS, use the transaction identifier from <see href="https://developer.apple.com/documentation/storekit/skpaymenttransaction">SKPaymentTransaction</see>. For Android, use the order ID from the purchase object (`purchase.getOrderId()`).</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ReportTransaction(
            string transactionId,
            Action<AdaptyError> completionHandler
        ) => ReportTransaction(transactionId, null, completionHandler);

        /// <summary>
        /// Updates external attribution data for the profile to track user acquisition sources.
        /// </summary>
        /// <remarks>
        /// This method allows you to send attribution data from external providers (e.g., AppsFlyer, Adjust, Branch) to Adapty.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </remarks>
        /// <param name="attribution">A dictionary containing attribution (conversion) data from the attribution provider.</param>
        /// <param name="provider">The external attribution provider (e.g., "appsflyer", "adjust", "branch", "custom").</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateExternalAttribution(
            IReadOnlyDictionary<string, object> attribution,
            string provider,
            Action<AdaptyError> completionHandler
        )
        {
            // The only overload that has to encode an argument before it can build the request,
            // and therefore the only one that can fail outside the transport's own guard - a
            // reference loop or a throwing getter in the provider's graph. Reported the way the
            // transport would have reported it rather than thrown at the caller.
            string json;
            try
            {
                json = AdaptyJson.Serialize(attribution);
            }
            catch (Exception exception)
            {
                AdaptyRequest.FailEncoding(
                    "update_external_attribution_data",
                    exception,
                    completionHandler
                );
                return;
            }

            UpdateExternalAttribution(json, provider, completionHandler);
        }

        /// <summary>
        /// Opens the paywall in a web view or browser.
        /// </summary>
        /// <param name="paywall">An <see cref="AdaptyFlowPaywall"/> object to open.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void OpenWebPaywall(
            AdaptyFlowPaywall paywall,
            Action<AdaptyError> completionHandler
        ) => OpenWebPaywall(paywall, AdaptyWebPresentation.ExternalBrowser, completionHandler);

        /// <summary>
        /// Opens the product purchase page in a web view or browser.
        /// </summary>
        /// <param name="product">An <see cref="AdaptyPaywallProduct"/> object to open.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void OpenWebPaywall(
            AdaptyPaywallProduct product,
            Action<AdaptyError> completionHandler
        ) => OpenWebPaywall(product, AdaptyWebPresentation.ExternalBrowser, completionHandler);
    }

    public static partial class AdaptyUI
    {
        /// <summary>
        /// Creates a flow view from an AdaptyFlow object.
        /// </summary>
        /// <remarks>
        /// Right after receiving an <see cref="AdaptyFlow"/>, you can create the corresponding <see cref="AdaptyUIFlowView"/> to present it afterwards.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
        /// </remarks>
        /// <param name="flow">An <see cref="AdaptyFlow"/> object for which you are trying to create a view.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyUIFlowView"/> object.</param>
        public static void CreateFlowView(
            AdaptyFlow flow,
            Action<AdaptyUIFlowView, AdaptyError> completionHandler
        ) => CreateFlowView(flow, null, completionHandler);

    }
}
