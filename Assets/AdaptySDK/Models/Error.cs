using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum ErrorCode
        {
            None = -1,

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
            PaywallNotFound = 21,
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
            NoProductsFound = 1001, // No In-App Purchases were found
            ProductRequestFailed = 1002, // Unable to fetch available In-App Purchase products at the moment
            CantMakePayments = 1003, // In-App Purchases are not allowed on this device
            NoPurchasesToRestore = 1004, // No purchases to restore
            CantReadReceipt = 1005, // Can't find a valid receipt
            ProductPurchaseFailed = 1006, // Product purchase failed
            MissingOfferSigningParams = 1007, // Missing offer signing required params
            FallbackPaywallsNotRequired = 1008, // Fallback paywalls are not required

            // custom network codes
            EmptyResponse = 2000, //Response is empty
            EmptyData = 2001, // Request data is empty
            AuthenticationError = 2002, // You need to be authenticated first
            BadRequest = 2003, // Bad request
            ServerError = 2004, // Response code is 429 or 500s
            Failed = 2005, // Network request failed
            UnableToDecode = 2006, // We could not decode the response
            MissingParam = 2007, // Missing some of the required params
            InvalidProperty = 2008, // Received invalid property data
            EncodingFailed = 2009, // Parameters encoding failed
            MissingURL = 2010, // Request url is nil

            // plugin codes
            NotFoundProduct = 10001,
            NotFoundPaywall = 10002,
            NotDeferredPurchase = 10003
        }

        public class Error
        {
           

            public readonly long Code;
            public readonly string Domain;
            public readonly string Message;
            public readonly ErrorCode AdaptyCode;

            internal Error(JSONNode response)
            {
                Message = response["message"];
                Domain = response["domain"];
                Code = response["adapty_code"];
                if (Code == 0)
                {
                    Code = response["code"];
                    AdaptyCode = ErrorCode.Unknown;
                } else
                {
                    AdaptyCode = (ErrorCode)Code;
                }
            }

            public override string ToString()
            {
                return $"{nameof(Domain)}: {Domain}, " +
                       $"{nameof(Code)}: {Code}, " +
                       $"{nameof(AdaptyCode)}: {AdaptyCode}, " +
                       $"{nameof(Message)}: {Message}";
            }
        }


        internal static bool ResponseHasError(JSONNode response)
        {
            if (response == null || response.IsNull) return false;
            var error = response["error"];
            return error != null && !error.IsNull;
        }



        public static Error ErrorFromJSON(JSONNode response)
        {
            if (response == null || response.IsNull || !response.IsObject) return null;
            try
            {
                return new Error(response);
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception on decoding Error: {e} source: {response}");
                return null;
            }
        }

    }
}

