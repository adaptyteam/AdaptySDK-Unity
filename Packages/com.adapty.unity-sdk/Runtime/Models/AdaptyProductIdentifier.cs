//
//  AdaptyProductIdentifier.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// A lightweight identifier used when addressing a specific product across platforms.
    [DataContract]
    [Preserve]
    public partial class AdaptyProductIdentifier
    {
        private AdaptyProductIdentifier() { }

        [DataMember(Name = "vendor_product_id", IsRequired = true)]
        public readonly string VendorProductId;
        [DataMember(Name = "adapty_product_id", IsRequired = true)]
        internal readonly string _AdaptyProductId;
        /// <remarks>
        /// Empty is the same as none: the contract leaves the key out rather than sending it empty,
        /// so the constructor normalizes it and <c>NullValueHandling</c> drops it.
        /// </remarks>
        [DataMember(Name = "base_plan_id")]
        public readonly string BasePlanId; // Android Only, nullable

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

        /// <remarks>
        /// Value equality, so an identifier can be used as a dictionary key — for example in
        /// <see cref="AdaptyUICreateFlowViewParameters.SetProductPurchaseParameters(System.Collections.Generic.Dictionary{AdaptyProductIdentifier, AdaptyPurchaseParameters})"/>,
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

        public override int GetHashCode()
        {
            var hash = 17;
            hash = (hash * 31) + (VendorProductId?.GetHashCode() ?? 0);
            hash = (hash * 31) + (_AdaptyProductId?.GetHashCode() ?? 0);
            hash = (hash * 31) + (BasePlanId?.GetHashCode() ?? 0);
            return hash;
        }

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
