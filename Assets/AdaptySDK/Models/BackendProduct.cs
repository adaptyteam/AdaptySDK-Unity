//
//  BackendProduct.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        internal partial class BackendProduct
        {
            internal string VendorId;
            internal bool PromotionalOfferEligibility;
            internal Eligibility IntroductoryOfferEligibility;
            internal string PromotionalOfferId;
            private int _Version;

            public override string ToString()
            {
                return $"{nameof(VendorId)}: {VendorId}, " +
                       $"{nameof(PromotionalOfferEligibility)}: {PromotionalOfferEligibility}, " +
                       $"{nameof(IntroductoryOfferEligibility)}: {IntroductoryOfferEligibility}, " +
                       $"{nameof(PromotionalOfferId)}: {PromotionalOfferId}, " +
                       $"{nameof(_Version)}: {_Version}";
            }
        }
    }
}
