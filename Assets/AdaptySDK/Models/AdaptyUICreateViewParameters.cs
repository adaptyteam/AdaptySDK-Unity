//
//  AdaptyUICreatePaywallViewParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 18.12.2024.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public partial class AdaptyUICreatePaywallViewParameters
    {
        public TimeSpan? LoadTimeout;
        public bool? PreloadProducts;
        public Dictionary<string, string> CustomTags;
        public Dictionary<string, DateTime> CustomTimers;
        public Dictionary<string, AdaptyCustomAsset> CustomAssets;

        public Dictionary<
            AdaptyProductIdentifier,
            AdaptyPurchaseParameters
        > ProductPurchaseParameters;

        public override string ToString() =>
            $"{nameof(LoadTimeout)}: {LoadTimeout}, "
            + $"{nameof(PreloadProducts)}: {PreloadProducts}, "
            + $"{nameof(CustomTags)}: {CustomTags}, "
            + $"{nameof(CustomTimers)}: {CustomTimers}, "
            + $"{nameof(CustomAssets)}: {CustomAssets}, "
            + $"{nameof(ProductPurchaseParameters)}: {ProductPurchaseParameters}";

        public AdaptyUICreatePaywallViewParameters SetLoadTimeout(TimeSpan? loadTimeout)
        {
            LoadTimeout = loadTimeout;
            return this;
        }

        public AdaptyUICreatePaywallViewParameters SetPreloadProducts(bool? preloadProducts)
        {
            PreloadProducts = preloadProducts;
            return this;
        }

        public AdaptyUICreatePaywallViewParameters SetCustomTags(
            Dictionary<string, string> customTags
        )
        {
            CustomTags = customTags;
            return this;
        }

        public AdaptyUICreatePaywallViewParameters SetCustomTimers(
            Dictionary<string, DateTime> customTimers
        )
        {
            CustomTimers = customTimers;
            return this;
        }

        public AdaptyUICreatePaywallViewParameters SetCustomAssets(
            Dictionary<string, AdaptyCustomAsset> customAssets
        )
        {
            CustomAssets = customAssets;
            return this;
        }

        public AdaptyUICreatePaywallViewParameters SetProductPurchaseParameters(
            Dictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters> productPurchaseParameters
        )
        {
            ProductPurchaseParameters = productPurchaseParameters;
            return this;
        }
    }
}
