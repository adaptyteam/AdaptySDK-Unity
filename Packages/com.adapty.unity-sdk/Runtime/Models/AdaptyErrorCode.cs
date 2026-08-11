using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The numeric code carried by <see cref="AdaptyError.Code"/>.
    /// </summary>
    /// <remarks>
    /// The value is the native SDK's own, so a code always arrives whether or not this enum names
    /// it. Most codes are produced by one platform only, and each member says which; where both
    /// produce a number, both meanings are given, because they are not always the same one.
    /// Verified against AdaptySDK-iOS 4.0.2 and AdaptySDK-Android 4.0.1, the pinned dependencies.
    /// </remarks>
    [Preserve]
    public enum AdaptyErrorCode
    {
        /// <summary>
        /// A failure the native SDK could not classify.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// iOS only. The client is not allowed to make the request.
        /// </summary>
        ClientInvalid = 1,

        /// <summary>
        /// iOS only. The user cancelled the request. Not a failure to report to them.
        /// </summary>
        PaymentCancelled = 2,

        /// <summary>
        /// iOS only. The purchase identifier was invalid.
        /// </summary>
        PaymentInvalid = 3,

        /// <summary>
        /// iOS only. This device is not allowed to make the payment — parental controls, for
        /// example.
        /// </summary>
        PaymentNotAllowed = 4,

        /// <summary>
        /// The product is not available in the current storefront. On Android this is Google
        /// Play's <c>ITEM_UNAVAILABLE</c>, which the native SDK maps to this code by name rather
        /// than by the offset the other billing codes use.
        /// </summary>
        StoreProductNotAvailable = 5,

        /// <summary>
        /// iOS only. The user has not allowed access to cloud service information.
        /// </summary>
        CloudServicePermissionDenied = 6,

        /// <summary>
        /// iOS only. The device could not connect to the network.
        /// </summary>
        CloudServiceNetworkConnectionFailed = 7,

        /// <summary>
        /// iOS only. The user has revoked permission to use this cloud service.
        /// </summary>
        CloudServiceRevoked = 8,

        /// <summary>
        /// iOS only. The user needs to acknowledge Apple's privacy policy.
        /// </summary>
        PrivacyAcknowledgementRequired = 9,

        /// <summary>
        /// iOS only. The app is using <c>SKPayment.requestData</c> without the entitlement for it.
        /// </summary>
        UnauthorizedRequestData = 10,

        /// <summary>
        /// iOS only. The subscription offer identifier is not valid.
        /// </summary>
        InvalidOfferIdentifier = 11,

        /// <summary>
        /// iOS only. The cryptographic signature of a promotional offer is not valid.
        /// </summary>
        InvalidSignature = 12,

        /// <summary>
        /// iOS only. One or more parameters of <c>SKPaymentDiscount</c> is missing.
        /// </summary>
        MissingOfferParams = 13,

        /// <summary>
        /// iOS only. The price of the offer is not valid.
        /// </summary>
        InvalidOfferPrice = 14,

        /// <summary>
        /// Android only. The SDK was called before <see cref="Adapty.Activate(AdaptySDK.AdaptyConfiguration, System.Action{AdaptySDK.AdaptyError})"/>.
        /// </summary>
        AdaptyNotInitialized = 20,

        /// <summary>
        /// Android only. The product was not found in Google Play for this application.
        /// </summary>
        ProductNotFound = 22,

        /// <summary>
        /// Neither pinned native SDK produces this code. It is kept so a number that reached an
        /// older app still has a name.
        /// </summary>
        InvalidJson = 23,

        /// <summary>
        /// Android only. The subscription being replaced was not found in the purchase history.
        /// </summary>
        CurrentSubscriptionToUpdateNotFoundInHistory = 24,

        /// <summary>
        /// Neither pinned native SDK produces this code. It is kept so a number that reached an
        /// older app still has a name; a pending purchase now arrives as
        /// <see cref="AdaptyPurchaseResultType.Pending"/>.
        /// </summary>
        PendingPurchase = 25,

        /// <summary>
        /// Android only. Google Play's <c>SERVICE_TIMEOUT</c>: the billing service did not answer
        /// in time. Worth retrying.
        /// </summary>
        BillingServiceTimeout = 97,

        /// <summary>
        /// Android only. Google Play's <c>FEATURE_NOT_SUPPORTED</c>: the Play Store version on the
        /// device does not support what was asked for.
        /// </summary>
        FeatureNotSupported = 98,

        /// <summary>
        /// Android only. Google Play's <c>SERVICE_DISCONNECTED</c>: the connection to the billing
        /// service was lost. Worth retrying.
        /// </summary>
        BillingServiceDisconnected = 99,

        /// <summary>
        /// Android only. Google Play's <c>SERVICE_UNAVAILABLE</c>: the billing service is not
        /// reachable, usually a network problem. Worth retrying.
        /// </summary>
        BillingServiceUnavailable = 102,

        /// <summary>
        /// Android only. Google Play's <c>BILLING_UNAVAILABLE</c>: billing is unavailable for this
        /// user or this API version — an unsupported Play Store, or a user who cannot transact.
        /// </summary>
        BillingUnavailable = 103,

        /// <summary>
        /// Android only. Google Play's <c>DEVELOPER_ERROR</c>: the request was malformed. A
        /// configuration problem in the app or the Play Console, not something the user can act on.
        /// </summary>
        DeveloperError = 105,

        /// <summary>
        /// Android only. Google Play's <c>ERROR</c>, and the fallback for any billing response the
        /// native SDK does not name.
        /// </summary>
        BillingError = 106,

        /// <summary>
        /// Android only. Google Play's <c>ITEM_ALREADY_OWNED</c>: the user already owns this
        /// product. Restore rather than buy.
        /// </summary>
        ItemAlreadyOwned = 107,

        /// <summary>
        /// Android only. Google Play's <c>ITEM_NOT_OWNED</c>: the product being consumed or
        /// replaced is not owned by the user.
        /// </summary>
        ItemNotOwned = 108,

        /// <summary>
        /// Android only. Google Play's <c>NETWORK_ERROR</c>: the request to the billing service
        /// failed on the network. Worth retrying.
        /// </summary>
        BillingNetworkError = 112,

        /// <summary>
        /// No products were found for the placement. Usually a Dashboard or store configuration
        /// that has not propagated yet.
        /// </summary>
        NoProductIDsFound = 1000,

        /// <summary>
        /// iOS only. The App Store could not be asked for the products.
        /// </summary>
        ProductRequestFailed = 1002,

        /// <summary>
        /// iOS only. In-app purchases are not allowed on this device.
        /// </summary>
        CantMakePayments = 1003,

        /// <summary>
        /// Android only. <see cref="Adapty.RestorePurchases(System.Action{AdaptySDK.AdaptyProfile, AdaptySDK.AdaptyError})"/> found nothing to restore. iOS does
        /// not produce this code.
        /// </summary>
        NoPurchasesToRestore = 1004,

        /// <summary>
        /// iOS only. No valid App Store receipt was found on the device.
        /// </summary>
        CantReadReceipt = 1005,

        /// <summary>
        /// iOS only. The purchase failed in StoreKit.
        /// </summary>
        ProductPurchaseFailed = 1006,

        /// <summary>
        /// iOS only. Refreshing the App Store receipt failed.
        /// </summary>
        RefreshReceiptFailed = 1010,

        /// <summary>
        /// iOS only. The subscription status could not be fetched from the App Store.
        /// </summary>
        FetchSubscriptionStatusFailed = 1020,

        /// <summary>
        /// iOS only. Reported by <see cref="Adapty.ReportTransaction(System.String, System.Action{AdaptySDK.AdaptyError})"/> when the purchase is
        /// waiting for confirmation — Ask to Buy, or a pending payment method. The profile updates
        /// when the store resolves it, so wait for it rather than retrying.
        /// </summary>
        PaymentPendingError = 1050,

        /// <summary>
        /// The two platforms mean different things by this number. On iOS the SDK was called
        /// before <see cref="Adapty.Activate(AdaptySDK.AdaptyConfiguration, System.Action{AdaptySDK.AdaptyError})"/>; on Android the Adapty backend answered 401 or 403,
        /// which points at the API key.
        /// </summary>
        NotActivated = 2002,

        /// <summary>
        /// The Adapty backend answered with a 4xx other than 401 and 403.
        /// </summary>
        BadRequest = 2003,

        /// <summary>
        /// The Adapty backend answered 429, 499 or a 5xx. Worth retrying.
        /// </summary>
        ServerError = 2004,

        /// <summary>
        /// The request to the Adapty backend failed on the network.
        /// </summary>
        NetworkFailed = 2005,

        /// <summary>
        /// A response could not be decoded. If it arrives from a call this SDK makes, the versions
        /// of the Unity and native SDKs may not match.
        /// </summary>
        DecodingFailed = 2006,

        /// <summary>
        /// iOS only. The parameters of a request could not be encoded.
        /// </summary>
        EncodingFailed = 2009,

        /// <summary>
        /// The call needs analytics, which the profile has switched off.
        /// </summary>
        AnalyticsDisabled = 3000,

        /// <summary>
        /// A parameter of the call was not valid.
        /// </summary>
        WrongParam = 3001,

        /// <summary>
        /// iOS only. <see cref="Adapty.Activate(AdaptySDK.AdaptyConfiguration, System.Action{AdaptySDK.AdaptyError})"/> was called more than once.
        /// </summary>
        ActivateOnceError = 3005,

        /// <summary>
        /// The profile changed while the operation was running — an <see cref="Adapty.Identify(System.String, System.Action{AdaptySDK.AdaptyError})"/>
        /// or <see cref="Adapty.Logout(System.Action{AdaptySDK.AdaptyError})"/> in between. Repeat the operation on the new profile.
        /// </summary>
        ProfileWasChanged = 3006,

        /// <summary>
        /// iOS only. The data handed to the SDK is of a shape it does not support.
        /// </summary>
        UnsupportedData = 3007,

        /// <summary>
        /// <see cref="Adapty.Logout(System.Action{AdaptySDK.AdaptyError})"/> was called for a profile that was never identified.
        /// </summary>
        UnidentifiedUserLogout = 3020,

        /// <summary>
        /// iOS only. The fetch did not finish within the timeout the call was given.
        /// </summary>
        FetchTimeoutError = 3101,

        /// <summary>
        /// Android only. Reported through <c>FlowViewDidReceiveError</c> when an asset in the flow
        /// is not of the type the layout expects.
        /// </summary>
        WrongAssetType = 4104,

        /// <summary>
        /// Android only. Reported through <c>FlowViewDidReceiveError</c> when the flow's web view
        /// raised a JavaScript exception.
        /// </summary>
        JsException = 4105,

        /// <summary>
        /// Android only. Reported through <c>FlowViewDidReceiveError</c> when the flow asked to
        /// navigate somewhere the renderer has no navigator for.
        /// </summary>
        NavigatorNotFound = 4106,

        /// <summary>
        /// Android only. Reported through <c>FlowViewDidReceiveError</c> when an action in the flow
        /// carries a URL that cannot be opened.
        /// </summary>
        InvalidActionUrl = 4107,

        /// <summary>
        /// iOS only. The operation was interrupted before it could finish.
        /// </summary>
        OperationInterrupted = 9000,
    }
}
