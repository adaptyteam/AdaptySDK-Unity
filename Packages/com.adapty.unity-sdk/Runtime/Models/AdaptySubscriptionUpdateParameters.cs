using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// Android only. Turns a purchase into an upgrade or downgrade of a subscription the user
    /// already has, rather than a new one. Pass it through
    /// <see cref="AdaptyPurchaseParameters.SubscriptionUpdateParams"/>; iOS handles the change
    /// itself through the subscription group and ignores this.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscriptionUpdateParameters
    {
        /// <summary>
        /// The Google Play product id of the subscription being replaced. Required.
        /// </summary>
        [DataMember(Name = "old_sub_vendor_product_id", IsRequired = true)]
        public string OldSubVendorProductId;

        /// <summary>
        /// When the change takes effect and how the remaining time is credited. Required.
        /// </summary>
        [DataMember(Name = "replacement_mode", IsRequired = true)]
        public AdaptySubscriptionUpdateReplacementMode ReplacementMode;

        /// <param name="oldSubVendorProductId">
        /// The Google Play product id of the subscription being replaced.
        /// </param>
        /// <summary>
        /// Describes the subscription this purchase replaces.
        /// </summary>
        /// <param name="replacementMode">When the change takes effect.</param>
        /// <exception cref="ArgumentNullException"><paramref name="oldSubVendorProductId"/> is null.</exception>
        public AdaptySubscriptionUpdateParameters(
            string oldSubVendorProductId,
            AdaptySubscriptionUpdateReplacementMode replacementMode
        )
        {
            OldSubVendorProductId =
                oldSubVendorProductId
                ?? throw new ArgumentNullException(nameof(oldSubVendorProductId));
            ReplacementMode = replacementMode;
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(OldSubVendorProductId)}: {OldSubVendorProductId}, "
            + $"{nameof(ReplacementMode)}: {ReplacementMode}";
    }
}
