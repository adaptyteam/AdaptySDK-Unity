//
//  AdaptyUICreateFlowViewParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 18.12.2024.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public partial class AdaptyUICreateFlowViewParameters
    {
        /// <summary>
        /// The identifier of the localization to render the flow with, e.g. "en", "es", "fr".
        /// When null, the flow's default localization is used.
        /// </summary>
        /// <remarks>
        /// A flow is localized when its view is built, not when the flow is fetched — this is the only place
        /// that selects the localization. Requires the native iOS 4.0.2 / Android 4.0.1 SDKs or newer.
        /// </remarks>
        public string Locale;

        public TimeSpan? LoadTimeout;
        public bool? PreloadProducts;
        public Dictionary<string, string> CustomTags;
        public Dictionary<string, DateTime> CustomTimers;
        public Dictionary<string, AdaptyCustomAsset> CustomAssets;

        /// <summary>
        /// Android only. Purchase parameters applied to the products the flow offers. Ignored on iOS.
        /// </summary>
        public Dictionary<
            AdaptyProductIdentifier,
            AdaptyPurchaseParameters
        > ProductPurchaseParameters;

        /// <summary>
        /// Android only. When false, the flow view is laid out without safe area paddings. Defaults to true.
        /// </summary>
        public bool? EnableSafeAreaPaddings;

        public override string ToString() =>
            $"{nameof(Locale)}: {Locale}, "
            + $"{nameof(LoadTimeout)}: {LoadTimeout}, "
            + $"{nameof(PreloadProducts)}: {PreloadProducts}, "
            + $"{nameof(CustomTags)}: {CustomTags}, "
            + $"{nameof(CustomTimers)}: {CustomTimers}, "
            + $"{nameof(CustomAssets)}: {CustomAssets}, "
            + $"{nameof(ProductPurchaseParameters)}: {ProductPurchaseParameters}, "
            + $"{nameof(EnableSafeAreaPaddings)}: {EnableSafeAreaPaddings}";

        public AdaptyUICreateFlowViewParameters SetLocale(string locale)
        {
            Locale = locale;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetLoadTimeout(TimeSpan? loadTimeout)
        {
            LoadTimeout = loadTimeout;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetPreloadProducts(bool? preloadProducts)
        {
            PreloadProducts = preloadProducts;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetCustomTags(
            Dictionary<string, string> customTags
        )
        {
            CustomTags = customTags;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetCustomTimers(
            Dictionary<string, DateTime> customTimers
        )
        {
            CustomTimers = customTimers;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetCustomAssets(
            Dictionary<string, AdaptyCustomAsset> customAssets
        )
        {
            CustomAssets = customAssets;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetProductPurchaseParameters(
            Dictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters> productPurchaseParameters
        )
        {
            ProductPurchaseParameters = productPurchaseParameters;
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetEnableSafeAreaPaddings(
            bool? enableSafeAreaPaddings
        )
        {
            EnableSafeAreaPaddings = enableSafeAreaPaddings;
            return this;
        }
    }
}
