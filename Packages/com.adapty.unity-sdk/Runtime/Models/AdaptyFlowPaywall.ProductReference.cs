//
//  AdaptyFlowPaywall.ProductReference.cs
//  AdaptySDK
//

namespace AdaptySDK
{
    public partial class AdaptyFlowPaywall
    {
        public partial class ProductReference
        {
            internal readonly string FlowProductId; //nullable
            internal readonly string VendorProductId;
            internal readonly string AdaptyProductId;
            internal readonly string AccessLevelId;
            internal readonly string ProductType;
            internal readonly string PromotionalOfferId; //nullable
            internal readonly string WinBackOfferId; //nullable
            internal readonly string AndroidBasePlanId; //nullable
            internal readonly string AndroidOfferId; //nullable

            public AdaptyProductIdentifier ToAdaptyProductIdentifier()
            {
                return new AdaptyProductIdentifier(
                    vendorProductId: VendorProductId,
                    adaptyProductId: AdaptyProductId,
                    basePlanId: AndroidBasePlanId
                );
            }

            public override string ToString() =>
                $"{nameof(FlowProductId)}: {FlowProductId}, "
                + $"{nameof(VendorProductId)}: {VendorProductId}, "
                + $"{nameof(AdaptyProductId)}: {AdaptyProductId}, "
                + $"{nameof(AccessLevelId)}: {AccessLevelId}, "
                + $"{nameof(ProductType)}: {ProductType}, "
                + $"{nameof(PromotionalOfferId)}: {PromotionalOfferId}, "
                + $"{nameof(WinBackOfferId)}: {WinBackOfferId}, "
                + $"{nameof(AndroidBasePlanId)}: {AndroidBasePlanId}, "
                + $"{nameof(AndroidOfferId)}: {AndroidOfferId}";
        }
    }
}
