//
//  AdaptyProductReference.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    /// Represents a reference to a product from a paywall with optional platform-specific identifiers.
    public partial class AdaptyProductReference
    {
        public readonly string VendorProductId;
        private readonly string _AdaptyProductId;

        // iOS Only
        public readonly string PromotionalOfferId; // nullable
        public readonly string WinBackOfferId; // nullable

        // Android Only
        public readonly string AndroidBasePlanId; // nullable
        public readonly string AndroidOfferId; // nullable

        public AdaptyProductReference(
            string vendorProductId,
            string adaptyProductId,
            string promotionalOfferId,
            string winBackOfferId,
            string androidBasePlanId,
            string androidOfferId
        )
        {
            VendorProductId = vendorProductId;
            _AdaptyProductId = adaptyProductId;
            PromotionalOfferId = promotionalOfferId;
            WinBackOfferId = winBackOfferId;
            AndroidBasePlanId = androidBasePlanId;
            AndroidOfferId = androidOfferId;
        }

        public AdaptyProductIdentifier ToAdaptyProductIdentifier()
        {
            return new AdaptyProductIdentifier(
                vendorProductId: VendorProductId,
                adaptyProductId: _AdaptyProductId,
                basePlanId: AndroidBasePlanId
            );
        }

        public override string ToString()
        {
            return "(vendorId: "
                + VendorProductId
                + ", _adaptyProductId: "
                + _AdaptyProductId
                + ", promotionalOfferId: "
                + PromotionalOfferId
                + ", winBackOfferId: "
                + WinBackOfferId
                + ", basePlanId: "
                + AndroidBasePlanId
                + ", offerId: "
                + AndroidOfferId
                + ")";
        }
    }
}
