//
//  ErrorCode.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum ErrorCode
        {
            // system storekit codes
            Unknown = 0,
            ClientInvalid = 1, // client is not allowed to issue the request, etc.
            PaymentCancelled = 2, // user cancelled the request, etc.
            PaymentInvalid = 3, // purchase identifier was invalid, etc.
            PaymentNotAllowed = 4, // this device is not allowed to make the payment
            StoreProductNotAvailable = 5, // Product is not available in the current storefront
            CloudServicePermissionDenied = 6, // user has not allowed access to cloud service information
            CloudServiceNetworkConnectionFailed = 7, // the device could not connect to the nework
            CloudServiceRevoked = 8, // user has revoked permission to use this cloud service
            PrivacyAcknowledgementRequired = 9, // The user needs to acknowledge Apple's privacy policy
            UnauthorizedRequestData = 10, // The app is attempting to use SKPayment's requestData property, but does not have the appropriate entitlement
            InvalidOfferIdentifier = 11, // The specified subscription offer identifier is not valid
            InvalidSignature = 12, // The cryptographic signature provided is not valid
            MissingOfferParams = 13, // One or more parameters from SKPaymentDiscount is missing
            InvalidOfferPrice = 14,

            //custom android codes
            AdaptyNotInitialized = 20,
            ProductNotFound = 22,
            InvalidJson = 23,
            CurrentSubscriptionToUpdateNotFoundInHistory = 24,
            PendingPurchase = 25,
            BillingServiceTimeout = 97,
            FeatureNotSupported = 98,
            BillingServiceDisconnected = 99,
            BillingServiceUnavailable = 102,
            BillingUnavailable = 103,
            DeveloperError = 105,
            BillingError = 106,
            ItemAlreadyOwned = 107,
            ItemNotOwned = 108,

            // custom storekit codes
            NoProductIDsFound = 1000, // No In-App Purchase product identifiers were found
            ProductRequestFailed = 1002, // Unable to fetch available In-App Purchase products at the moment
            CantMakePayments = 1003, // In-App Purchases are not allowed on this device
            NoPurchasesToRestore = 1004, // No purchases to restore
            CantReadReceipt = 1005, // Can't find a valid receipt
            ProductPurchaseFailed = 1006, // Product purchase failed
            RefreshReceiptFailed = 1010,
            ReceiveRestoredTransactionsFailed = 1011,

            // custom network codes
            NotActivated = 2002, // You need to be authenticated first
            BadRequest = 2003, // Bad request
            ServerError = 2004, // Response code is 429 or 500s
            NetworkFailed = 2005, // Network request failed
            DecodingFailed = 2006, // We could not decode the response
            EncodingFailed = 2009, // Parameters encoding failed
            AnalyticsDisabled = 3000, // Request url is nil

            /// Wrong parameter was passed.
            WrongParam = 3001,

            /// It is not possible to call `.activate` method more than once.
            ActivateOnceError = 3005,

            /// The user profile was changed during the operation.
            ProfileWasChanged = 3006,
            PersistingDataError = 3100,
            OperationInterrupted = 9000,

            /// Plugin errors
            WrongCallParameter = 10001
        }
    }
}
