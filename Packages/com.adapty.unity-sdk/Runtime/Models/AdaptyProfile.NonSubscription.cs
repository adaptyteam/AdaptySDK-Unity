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
            /// <summary>
            /// The identifier of the purchase in Adapty.
            /// </summary>
            /// <remarks>
            /// You can use it to ensure that you've already processed this purchase (for example tracking one time products).
            /// </remarks>
            public readonly string PurchaseId;

            /// <summary>
            /// The store of the purchase.
            /// </summary>
            /// <remarks>
            /// The possible values are: app_store, play_store, adapty.
            /// </remarks>
            public readonly string Store;

            /// <summary>
            /// The identifier of the product in the App Store Connect.
            /// </summary>
            public readonly string VendorProductId;

            /// <summary>
            /// Transaction id from the App Store.
            /// </summary>
            public readonly string VendorTransactionId; // nullable

            /// <summary>
            /// The time when the product was purchased.
            /// </summary>
            public readonly DateTime PurchasedAt;

            /// <summary>
            /// Whether the product was purchased in the sandbox environment.
            /// </summary>
            public readonly bool IsSandbox;

            /// <summary>
            /// Whether the purchase was refunded.
            /// </summary>
            public readonly bool IsRefund;

            /// <summary>
            /// Deprecated, use 'IsConsumable'.
            /// </summary>
            public bool IsOneTime => IsConsumable;

            /// <summary>
            /// Whether the product should only be processed once.
            /// </summary>
            /// <remarks>
            /// If true, the purchase will be returned by Adapty API one time only.
            /// </remarks>
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