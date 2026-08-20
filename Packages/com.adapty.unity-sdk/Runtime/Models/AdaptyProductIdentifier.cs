using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// A lightweight identifier used when addressing a specific product across platforms.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyProductIdentifier
    {
        private AdaptyProductIdentifier() { }

        /// <summary>
        /// The product id in App Store Connect or the Google Play Console.
        /// </summary>
        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        public readonly string VendorProductId;
        /// <summary>
        /// The product's id in Adapty, which is also the key a request addresses it by.
        /// </summary>
        [DataMember(Name = "adapty_product_id", IsRequired = true)]
        internal readonly string _AdaptyProductId;
        /// <summary>
        /// Android only. The Google Play base plan. Null on iOS.
        /// </summary>
        /// <remarks>
        /// Empty is the same as none: the contract leaves the key out rather than sending it empty,
        /// so the constructor normalizes it and <c>NullValueHandling</c> drops it.
        /// </remarks>
        [DataMember(Name = "base_plan_id")]
        public readonly string BasePlanId;

        /// <summary>
        /// Builds an identifier for a product you name yourself, rather than one taken from a flow.
        /// </summary>
        /// <param name="vendorProductId">The product id in App Store Connect or the Google Play Console.</param>
        /// <param name="adaptyProductId">
        /// The product's id in Adapty, as <see cref="AdaptyPaywallProduct.AdaptyProductId"/> carries it.
        /// </param>
        /// <param name="basePlanId">Android only. The Google Play base plan, or null for none.</param>
        public AdaptyProductIdentifier(
            string vendorProductId,
            string adaptyProductId,
            string basePlanId
        )
        {
            VendorProductId = vendorProductId;
            _AdaptyProductId = adaptyProductId;
            BasePlanId = string.IsNullOrEmpty(basePlanId) ? null : basePlanId;
        }

        /// <summary>
        /// Two identifiers are equal when all three of their values are.
        /// </summary>
        /// <remarks>
        /// Value equality, so an identifier can be used as a dictionary key — for example in
        /// <see cref="AdaptyUICreateFlowViewParameters.SetProductPurchaseParameters(System.Collections.Generic.IReadOnlyDictionary{AdaptyProductIdentifier, AdaptyPurchaseParameters})"/>,
        /// where the caller builds the keys from a flow rather than reusing the SDK's instances.
        /// </remarks>
        public override bool Equals(object obj)
        {
            var other = obj as AdaptyProductIdentifier;
            if (other == null)
            {
                return false;
            }

            return VendorProductId == other.VendorProductId
                && _AdaptyProductId == other._AdaptyProductId
                && BasePlanId == other.BasePlanId;
        }

        /// <summary>
        /// Hashes the three values <see cref="Equals"/> compares, so an identifier works as a dictionary
        /// key.
        /// </summary>
        public override int GetHashCode()
        {
            var hash = 17;
            hash = (hash * 31) + (VendorProductId?.GetHashCode() ?? 0);
            hash = (hash * 31) + (_AdaptyProductId?.GetHashCode() ?? 0);
            hash = (hash * 31) + (BasePlanId?.GetHashCode() ?? 0);
            return hash;
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString()
        {
            return nameof(VendorProductId)
                + ": "
                + VendorProductId
                + ", "
                + nameof(_AdaptyProductId)
                + ": "
                + _AdaptyProductId
                + ", "
                + nameof(BasePlanId)
                + ": "
                + BasePlanId;
        }
    }
}
