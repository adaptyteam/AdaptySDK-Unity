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
        private readonly string _AdaptyProductId;
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
