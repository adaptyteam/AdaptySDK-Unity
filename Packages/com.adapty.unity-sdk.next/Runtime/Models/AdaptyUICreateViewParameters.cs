//
//  AdaptyUICreateFlowViewParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 18.12.2024.
//

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
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
        [DataMember(Name = "locale")]
        public string Locale;

        public TimeSpan? LoadTimeout;

        /// <summary>
        /// The contract carries the timeout in seconds, not as a duration literal.
        /// </summary>
        [DataMember(Name = "load_timeout")]
        private double? LoadTimeoutInSeconds => LoadTimeout?.TotalSeconds;

        [DataMember(Name = "preload_products")]
        public bool? PreloadProducts;

        [DataMember(Name = "custom_tags")]
        public Dictionary<string, string> CustomTags;

        [DataMember(Name = "custom_timers")]
        public Dictionary<string, DateTime> CustomTimers;

        [DataMember(Name = "custom_assets")]
        public Dictionary<string, AdaptyCustomAsset> CustomAssets;

        /// <summary>
        /// Android only. Purchase parameters applied to the products the flow offers. Ignored on iOS.
        /// </summary>
        public Dictionary<
            AdaptyProductIdentifier,
            AdaptyPurchaseParameters
        > ProductPurchaseParameters;

        /// <summary>
        /// The contract keys these by the product identifier the store knows, not by the composite
        /// identifier the app passes.
        /// </summary>
        [DataMember(Name = "product_purchase_parameters")]
        private Dictionary<string, AdaptyPurchaseParameters> ProductPurchaseParametersForRequest
        {
            get
            {
                if (ProductPurchaseParameters is null)
                {
                    return null;
                }

                var result = new Dictionary<string, AdaptyPurchaseParameters>();
                foreach (var entry in ProductPurchaseParameters)
                {
                    result[entry.Key._AdaptyProductId] = entry.Value;
                }
                return result;
            }
        }

        /// <summary>
        /// Android only. When false, the flow view is laid out without safe area paddings. Defaults to true.
        /// </summary>
        [DataMember(Name = "enable_safe_area_paddings")]
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
