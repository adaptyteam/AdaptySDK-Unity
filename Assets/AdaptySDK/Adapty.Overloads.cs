using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products-unity">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(
            string placementId,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywallForDefaultAudience(placementId, null, null, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products-unity">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywallForDefaultAudience(placementId, null, fetchPolicy, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the paywall from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products-unity">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(
            string placementId,
            string locale,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywallForDefaultAudience(placementId, locale, null, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the onboarding localization.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            string locale,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, locale, null, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="fetchPolicy">By default SDK will try to load data from server and will return cached data in case of failure. Otherwise use `.returnCacheDataElseLoad` to return cached data if it exists.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, null, fetchPolicy, completionHandler);

        /// <summary>
        /// This method enables you to retrieve the onboarding from the Default Audience without having to wait for the Adapty SDK to send all the user information required for segmentation to the server.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetOnboardingForDefaultAudience(
            string placementId,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, null, null, completionHandler);

        /// <summary>
        /// Adapty allows you remotely configure onboarding screens that will be displayed in your app.
        /// </summary>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetOnboarding(
            string placementId,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboarding(placementId, null, null, null, completionHandler);

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
        /// Updates attribution data for the profile to track user acquisition sources.
        /// </summary>
        /// <remarks>
        /// This method allows you to send attribution data from various sources (e.g., AppsFlyer, Adjust, Branch) to Adapty.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </remarks>
        /// <param name="attribution">A dictionary containing attribution (conversion) data from the attribution provider.</param>
        /// <param name="source">The source of attribution (e.g., "appsflyer", "adjust", "branch", "custom").</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void UpdateAttribution(
            Dictionary<string, dynamic> attribution,
            string source,
            Action<AdaptyError> completionHandler
        ) => UpdateAttribution(attribution.ToJSONObject().ToString(), source, completionHandler);
    }

    public static partial class AdaptyUI
    {
        /// <summary>
        /// Creates a paywall view from an AdaptyPaywall object.
        /// </summary>
        /// <remarks>
        /// Right after receiving an <see cref="AdaptyPaywall"/>, you can create the corresponding <see cref="AdaptyUIPaywallView"/> to present it afterwards.
        /// Read more at <see href="https://adapty.io/docs/unity-quickstart-paywalls">Adapty Documentation</see>
        /// </remarks>
        /// <param name="paywall">An <see cref="AdaptyPaywall"/> object for which you are trying to create a view.</param>
        /// <param name="completionHandler">The action that will be called with the result. The result contains an <see cref="AdaptyUIPaywallView"/> object.</param>
        public static void CreatePaywallView(
            AdaptyPaywall paywall,
            Action<AdaptyUIPaywallView, AdaptyError> completionHandler
        ) => CreatePaywallView(paywall, null, completionHandler);
    }
}
