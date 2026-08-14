using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    [Preserve]
    public sealed partial class AdaptyProfile
    {
        /// <summary>
        /// One non-subscription purchase of a profile — consumable or lifetime.
        /// </summary>
        [DataContract]
        public sealed class NonSubscription
        {
            private NonSubscription() { }

            /// <summary>
            /// The identifier of the purchase in Adapty.
            /// </summary>
            /// <remarks>
            /// You can use it to ensure that you've already processed this purchase (for example tracking one time products).
            /// </remarks>
            [DataMember(Name = "purchase_id", IsRequired = true)]
            public readonly string PurchaseId;

            /// <summary>
            /// The store of the purchase.
            /// </summary>
            /// <remarks>
            /// The possible values are: app_store, play_store, adapty.
            /// </remarks>
            [DataMember(Name = "store", IsRequired = true)]
            public readonly string Store;

            /// <summary>
            /// The identifier of the product in the store it was bought from.
            /// </summary>
            [DataMember(Name = "vendor_product_id", IsRequired = true)]
            public readonly string VendorProductId;

            /// <summary>
            /// The transaction id the store reported. Null when it does not report one.
            /// </summary>
            [DataMember(Name = "vendor_transaction_id")]
            public readonly string VendorTransactionId;

            /// <summary>
            /// The time when the product was purchased.
            /// </summary>
            [DataMember(Name = "purchased_at", IsRequired = true)]
            public readonly DateTime PurchasedAt;

            /// <summary>
            /// Whether the product was purchased in the sandbox environment.
            /// </summary>
            [DataMember(Name = "is_sandbox", IsRequired = true)]
            public readonly bool IsSandbox;

            /// <summary>
            /// Whether the purchase was refunded.
            /// </summary>
            [DataMember(Name = "is_refund", IsRequired = true)]
            public readonly bool IsRefund;

            /// <summary>
            /// Whether the product should only be processed once.
            /// </summary>
            /// <remarks>
            /// If true, the purchase will be returned by Adapty API one time only.
            /// </remarks>
            [DataMember(Name = "is_consumable", IsRequired = true)]
            public readonly bool IsConsumable;

            /// <summary>
            /// A description for logs and the debugger. The format is not part of the contract —
            /// read the members rather than parsing it.
            /// </summary>
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
