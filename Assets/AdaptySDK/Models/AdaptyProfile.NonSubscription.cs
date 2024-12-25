//
//  AdaptyProfile.NonSubscription.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    public partial class AdaptyProfile
    {
        public partial class NonSubscription
        {
            /// The identifier of the purchase in Adapty.
            /**
            * You can use it to ensure that you've already processed this purchase (for example tracking one time products).
            */
            public readonly string PurchaseId;

            /// The store of the purchase.
            /**
            * The possible values are: app_store, play_store , adapty.
            */
            public readonly string Store;

            /// The identifier of the product in the App Store Connect.
            public readonly string VendorProductId;

            /// Transaction id from the App Store.
            public readonly string VendorTransactionId; // nullable

            /// The time when the product was purchased.
            public readonly DateTime PurchasedAt;

            /// Whether the product was purchased in the sandbox environment.
            public readonly bool IsSandbox;

            /// Whether the purchase was refunded.
            public readonly bool IsRefund;

            /// Deprecated, use 'isConsumable'
            public bool IsOneTime => IsConsumable;

            /// Whether the product should only be processed once.
            /**
            * If true, the purchase will be returned by Adapty API one time only.
            */
            public readonly bool IsConsumable;

            public override string ToString() => $"{nameof(PurchaseId)}: {PurchaseId}, " +
                       $"{nameof(VendorProductId)}: {VendorProductId}, " +
                       $"{nameof(Store)}: {Store}, " +
                       $"{nameof(PurchasedAt)}: {PurchasedAt}, " +
                       $"{nameof(IsConsumable)}: {IsConsumable}, " +
                       $"{nameof(IsSandbox)}: {IsSandbox}, " +
                       $"{nameof(VendorTransactionId)}: {VendorTransactionId}, " +
                       $"{nameof(IsRefund)}: {IsRefund}";
        }
    }
}