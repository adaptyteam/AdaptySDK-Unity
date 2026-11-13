using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine.Scripting;

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
            // Handed back to the native side as received: C# never reads the offers, and a typed
            // copy would drop any key a later native adds.
#if UNITY_IOS
            [DataMember(Name = "pricing_terms")]
#endif
            internal readonly JArray PricingTerms;
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
                + $"{nameof(PricingTerms)}: {PricingTerms?.ToString(Newtonsoft.Json.Formatting.None)}, "
                + $"{nameof(AndroidBasePlanId)}: {AndroidBasePlanId}, "
                + $"{nameof(AndroidOfferId)}: {AndroidOfferId}";
        }
    }
}
