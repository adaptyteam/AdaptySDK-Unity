//
//  AdaptyProductIdentifier.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    /// A lightweight identifier used when addressing a specific product across platforms.
    public partial class AdaptyProductIdentifier
    {
        public readonly string VendorProductId;
        internal readonly string _AdaptyProductId;
        public readonly string BasePlanId; // Android Only, nullable

        public AdaptyProductIdentifier(
            string vendorProductId,
            string adaptyProductId,
            string basePlanId
        )
        {
            VendorProductId = vendorProductId;
            _AdaptyProductId = adaptyProductId;
            BasePlanId = basePlanId;
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
