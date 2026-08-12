using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public sealed partial class AdaptyFlowPaywall
    {
        [DataContract]
        internal class ProductReference
        {
            private ProductReference() { }

            [DataMember(Name = "flow_product_id")]
            internal readonly string FlowProductId;
            [DataMember(Name = "vendor_product_id", IsRequired = true)]
            internal readonly string VendorProductId;
            [DataMember(Name = "adapty_product_id", IsRequired = true)]
            internal readonly string AdaptyProductId;
            [DataMember(Name = "access_level_id", IsRequired = true)]
            internal readonly string AccessLevelId;
            [DataMember(Name = "product_type", IsRequired = true)]
            internal readonly string ProductType;
            #if UNITY_IOS
            [DataMember(Name = "promotional_offer_id")]
#endif
            internal readonly string PromotionalOfferId;
            #if UNITY_IOS
            [DataMember(Name = "win_back_offer_id")]
#endif
            internal readonly string WinBackOfferId;
            #if UNITY_ANDROID
            [DataMember(Name = "base_plan_id")]
#endif
            internal readonly string AndroidBasePlanId;
            #if UNITY_ANDROID
            [DataMember(Name = "offer_id")]
#endif
            internal readonly string AndroidOfferId;

            internal AdaptyProductIdentifier ToAdaptyProductIdentifier()
            {
                return new AdaptyProductIdentifier(
                    vendorProductId: VendorProductId,
                    adaptyProductId: AdaptyProductId,
                    basePlanId: AndroidBasePlanId
                );
            }

            /// <summary>
            /// A description for logs and the debugger. The format is not part of the contract —
            /// read the members rather than parsing it.
            /// </summary>
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
