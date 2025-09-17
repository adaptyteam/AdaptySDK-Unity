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
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
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
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
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
        /// Read more at <see href="https://adapty.io/docs/fetch-paywalls-and-products">Adapty Documentation</see>
        /// </remarks>
        /// <param name="placementId">The identifier of the desired placement. This is the value you specified when you created the placement in the Adapty Dashboard.</param>
        /// <param name="locale">The identifier of the paywall <a href="https://adapty.io/docs/add-remote-config-locale">localization</a>.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void GetPaywallForDefaultAudience(
            string placementId,
            string locale,
            Action<AdaptyPaywall, AdaptyError> completionHandler
        ) => GetPaywallForDefaultAudience(placementId, locale, null, completionHandler);

        public static void GetOnboardingForDefaultAudience(
            string placementId,
            string locale,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, locale, null, completionHandler);

        public static void GetOnboardingForDefaultAudience(
            string placementId,
            AdaptyPlacementFetchPolicy fetchPolicy,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, null, fetchPolicy, completionHandler);

        public static void GetOnboardingForDefaultAudience(
            string placementId,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboardingForDefaultAudience(placementId, null, null, completionHandler);

        public static void GetOnboarding(
            string placementId,
            Action<AdaptyOnboarding, AdaptyError> completionHandler
        ) => GetOnboarding(placementId, null, null, null, completionHandler);

        /// <summary>
        /// To make the purchase, you have to call this method.
        /// </summary>
        /// <remarks>
        /// Read more on the <see href="https://adapty.io/docs/making-purchases">Adapty Documentation</see>
        /// </remarks>
        /// <param name="product">an [AdaptyPaywallProduct] object retrieved from the paywall.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void MakePurchase(
            AdaptyPaywallProduct product,
            Action<AdaptyPurchaseResult, AdaptyError> completionHandler
        ) => MakePurchase(product, null, completionHandler);

        /// <summary>
        /// In Observer mode, Adapty SDK doesn’t know, where the purchase was made from.
        /// If you display products using our <see href="https://docs.adapty.io/v2.0/docs/paywall">Paywalls</see> or <  see href="https://docs.adapty.io/v2.0/docs/ab-test">A/B Tests</see>, you can manually assign variation to the purchase.
        /// After doing this, you’ll be able to see metrics in Adapty Dashboard.
        /// </summary>
        /// <param name="transactionId">A string identifier of your purchased transaction <see href="https://developer.apple.com/documentation/storekit/skpaymenttransaction">SKPaymentTransaction</see> for iOS or string identifier (`purchase.getOrderId()`) of the purchase, where the purchase is an instance of the billing library Purchase class for Android.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void ReportTransaction(
            string transactionId,
            Action<AdaptyProfile, AdaptyError> completionHandler
        ) => ReportTransaction(transactionId, null, completionHandler);

        /// <summary>
        /// You can set attribution data for the profile, using method.
        /// Read more on the <see href="https://adapty.io/docs/attribution-integration">Adapty Documentation</see>
        /// </summary>
        /// <param name="attribution">a map containing attribution (conversion) data.</param>
        /// <param name="source">a source of attribution.</param>
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
        /// Right after receiving ``AdaptyPaywall``, you can create the corresponding ``AdaptyUIPaywallView`` to present it afterwards.
        /// </summary>
        /// <param name="paywall">an [AdaptyPaywall] object, for which you are trying to get a controller.</param>
        /// <param name="completionHandler">The action that will be called with the result.</param>
        public static void CreatePaywallView(
            AdaptyPaywall paywall,
            Action<AdaptyUIPaywallView, AdaptyError> completionHandler
        ) => CreatePaywallView(paywall, null, completionHandler);
    }
}
