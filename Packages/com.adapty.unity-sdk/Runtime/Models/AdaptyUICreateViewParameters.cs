//
//  AdaptyUICreateFlowViewParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 18.12.2024.
//

using UnityEngine.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public sealed class AdaptyUICreateFlowViewParameters
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
        [Preserve]
        private double? LoadTimeoutInSeconds => LoadTimeout?.TotalSeconds;

        [DataMember(Name = "preload_products")]
        public bool? PreloadProducts;

        [DataMember(Name = "custom_tags")]
        private Dictionary<string, string> _CustomTags;

        [Preserve]
        public IReadOnlyDictionary<string, string> CustomTags =>
            _CustomTags is null ? null : new ReadOnlyDictionary<string, string>(_CustomTags);

        [DataMember(Name = "custom_timers")]
        private Dictionary<string, DateTime> _CustomTimers;

        [Preserve]
        public IReadOnlyDictionary<string, DateTime> CustomTimers =>
            _CustomTimers is null ? null : new ReadOnlyDictionary<string, DateTime>(_CustomTimers);

        [DataMember(Name = "custom_assets")]
        private Dictionary<string, AdaptyCustomAsset> _CustomAssets;

        [Preserve]
        public IReadOnlyDictionary<string, AdaptyCustomAsset> CustomAssets =>
            _CustomAssets is null ? null : new ReadOnlyDictionary<string, AdaptyCustomAsset>(_CustomAssets);

        /// <summary>
        /// Android only. Purchase parameters applied to the products the flow offers. Ignored on iOS.
        /// </summary>
        private Dictionary<
            AdaptyProductIdentifier,
            AdaptyPurchaseParameters
        > _ProductPurchaseParameters;

        [Preserve]
        public IReadOnlyDictionary<
            AdaptyProductIdentifier,
            AdaptyPurchaseParameters
        > ProductPurchaseParameters =>
            _ProductPurchaseParameters is null
                ? null
                : new ReadOnlyDictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters>(
                    _ProductPurchaseParameters
                );

        /// <summary>
        /// The contract keys these by the product identifier the store knows, not by the composite
        /// identifier the app passes.
        /// </summary>
        [DataMember(Name = "product_purchase_parameters")]
        [Preserve]
        private Dictionary<string, AdaptyPurchaseParameters> ProductPurchaseParametersForRequest
        {
            get
            {
                if (_ProductPurchaseParameters is null)
                {
                    return null;
                }

                var result = new Dictionary<string, AdaptyPurchaseParameters>();
                foreach (var entry in _ProductPurchaseParameters)
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
            IReadOnlyDictionary<string, string> customTags
        )
        {
            _CustomTags = Copy(customTags);
            return this;
        }

        // Copied, so a caller that keeps writing to its own dictionary after handing it over does
        // not change what the view will be built with.
        private static Dictionary<TKey, TValue> Copy<TKey, TValue>(
            IReadOnlyDictionary<TKey, TValue> source
        )
        {
            if (source is null)
            {
                return null;
            }

            var copy = new Dictionary<TKey, TValue>();
            foreach (var entry in source)
            {
                copy[entry.Key] = entry.Value;
            }
            return copy;
        }

        /// <param name="customTimers">
        /// When each timer ends. A <see cref="DateTime"/> with no <see cref="DateTimeKind"/> of its
        /// own is read as the user's local clock, so
        /// <c>new DateTime(2026, 7, 30, 22, 0, 0)</c> means 22:00 where the user is; pass a
        /// <see cref="DateTimeKind.Utc"/> value to mean 22:00 UTC.
        /// </param>
        public AdaptyUICreateFlowViewParameters SetCustomTimers(
            IReadOnlyDictionary<string, DateTime> customTimers
        )
        {
            _CustomTimers = Copy(customTimers);
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetCustomAssets(
            IReadOnlyDictionary<string, AdaptyCustomAsset> customAssets
        )
        {
            _CustomAssets = Copy(customAssets);
            return this;
        }

        public AdaptyUICreateFlowViewParameters SetProductPurchaseParameters(
            IReadOnlyDictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters> productPurchaseParameters
        )
        {
            _ProductPurchaseParameters = Copy(productPurchaseParameters);
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
