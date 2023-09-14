//
//  ProductReference.cs
//  Adapty
//
//  Created by Aleksei Valiano on 11.09.2023.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        internal partial class ProductReference
        {
            internal readonly string VendorId;
            internal readonly string AndroidBasePlanId; //nullable
            internal readonly string AndroidOfferId; //nullable
            internal readonly string IOSDiscountId; //nullable

            public override string ToString() => $"{nameof(VendorId)}: {VendorId}, " +
                       $"{nameof(AndroidBasePlanId)}: {AndroidBasePlanId}, " +
                       $"{nameof(AndroidOfferId)}: {AndroidOfferId}, " +
                       $"{nameof(IOSDiscountId)}: {IOSDiscountId}";
        }
    }
}