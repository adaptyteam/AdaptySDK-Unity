//
//  PaywallProduct.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class PaywallProduct
        {
            /// Unique identifier of the product.
            public readonly string VendorProductId;

            /// A description of the product.
            public readonly string LocalizedDescription;

            /// The name of the product.
            public readonly string LocalizedTitle;

            /// Product locale region code.
            ///
            /// [Nullable]
            public readonly string RegionCode;

            /// Indicates whether the product is available for family sharing in App Store Connect.
            public readonly bool IsFamilyShareable;

            /// The identifier of the variation, used to attribute purchases to the paywall.
            public readonly string PaywallVariationId;

            /// Parent A/B test name
            public readonly string PaywallABTestName;

            /// Parent paywall name
            public readonly string PaywallName;

            /// The object which represents the main price for the product.
            public readonly Price Price;

            /// Detailed information about subscription (intro, offers, etc.)
            public readonly SubscriptionDetails SubscriptionDetails; //nullable

            private string _PayloadData;

            public override string ToString() => $"{nameof(VendorProductId)}: {VendorProductId}, " +
                       $"{nameof(LocalizedDescription)}: {LocalizedDescription}, " +
                       $"{nameof(LocalizedTitle)}: {LocalizedTitle}, " +
                       $"{nameof(RegionCode)}: {RegionCode}, " +
                       $"{nameof(IsFamilyShareable)}: {IsFamilyShareable}, " +
                       $"{nameof(PaywallVariationId)}: {PaywallVariationId}, " +
                       $"{nameof(PaywallABTestName)}: {PaywallABTestName}, " +
                       $"{nameof(PaywallName)}: {PaywallName}, " +
                       $"{nameof(Price)}: {Price}, " +
                       $"{nameof(SubscriptionDetails)}: {SubscriptionDetails}";
        }
    }
}