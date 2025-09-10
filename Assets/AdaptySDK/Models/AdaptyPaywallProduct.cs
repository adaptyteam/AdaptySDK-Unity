//
//  AdaptyPaywallProduct.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public partial class AdaptyPaywallProduct
    {
        /// Unique identifier of the product.
        public readonly string VendorProductId;

        public readonly string AdaptyProductId;

        /// The identifier of the variation, used to attribute purchases to the paywall.
        public readonly string PaywallVariationId;

        /// Parent A/B test name
        public readonly string PaywallABTestName;

        /// Parent paywall name
        public readonly string PaywallName;

        /// A description of the product.
        public readonly string LocalizedDescription;

        /// The name of the product.
        public readonly string LocalizedTitle;

        /// Indicates whether the product is available for family sharing in App Store Connect.
        public readonly bool IsFamilyShareable;

        /// Product locale region code.
        ///
        /// [Nullable]
        public readonly string RegionCode;

        /// The object which represents the main price for the product.
        public readonly AdaptyPrice Price;

        /// Detailed information about subscription (intro, offers, etc.)
        public readonly AdaptySubscription Subscription; //nullable

        /// The index of the product in the paywall.
        public readonly int PaywallProductIndex;

        private readonly string _PayloadData;
        private readonly string _WebPurchaseUrl;

        public override string ToString() =>
            $"{nameof(VendorProductId)}: {VendorProductId}, "
            + $"{nameof(LocalizedDescription)}: {LocalizedDescription}, "
            + $"{nameof(LocalizedTitle)}: {LocalizedTitle}, "
            + $"{nameof(RegionCode)}: {RegionCode}, "
            + $"{nameof(IsFamilyShareable)}: {IsFamilyShareable}, "
            + $"{nameof(PaywallVariationId)}: {PaywallVariationId}, "
            + $"{nameof(PaywallABTestName)}: {PaywallABTestName}, "
            + $"{nameof(PaywallName)}: {PaywallName}, "
            + $"{nameof(Price)}: {Price}, "
            + $"{nameof(Subscription)}: {Subscription}, "
            + $"{nameof(PaywallProductIndex)}: {PaywallProductIndex}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}, "
            + $"{nameof(_WebPurchaseUrl)}: {_WebPurchaseUrl}";
    }
}
