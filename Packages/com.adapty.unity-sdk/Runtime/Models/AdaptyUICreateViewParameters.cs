using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The optional extras of <see cref="AdaptyUI.CreateFlowView(AdaptySDK.AdaptyFlow, AdaptySDK.AdaptyUICreateFlowViewParameters, System.Action{AdaptySDK.AdaptyUIFlowView, AdaptySDK.AdaptyError})"/>: which localization to render,
    /// how long to wait, and the tags, timers and assets the flow substitutes into its layout.
    /// </summary>
    /// <remarks>
    /// A dictionary handed to a setter is copied, so writing into your own copy afterwards does
    /// not change what the view is built with.
    /// </remarks>
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

        /// <summary>
        /// How long to wait for the flow's assets before giving up. Null leaves the native
        /// default.
        /// </summary>
        public TimeSpan? LoadTimeout;

        /// <summary>
        /// The contract carries the timeout in seconds, not as a duration literal.
        /// </summary>
        [DataMember(Name = "load_timeout")]
        [Preserve]
        private double? LoadTimeoutInSeconds => LoadTimeout?.TotalSeconds;

        /// <summary>
        /// Fetches the flow's products while the view is being built, so the first frame already
        /// has prices. Null leaves the native default.
        /// </summary>
        [DataMember(Name = "preload_products")]
        public bool? PreloadProducts;

        [DataMember(Name = "custom_tags")]
        private Dictionary<string, string> _CustomTags;

        /// <summary>
        /// The values the flow substitutes for its custom tags, keyed by tag name. Null when none were set.
        /// </summary>
        [Preserve]
        public IReadOnlyDictionary<string, string> CustomTags =>
            _CustomTags is null ? null : new ReadOnlyDictionary<string, string>(_CustomTags);

        [DataMember(Name = "custom_timers")]
        private Dictionary<string, DateTime> _CustomTimers;

        /// <summary>
        /// When each of the flow's custom timers ends, keyed by timer name. A value with no <see cref="DateTimeKind"/> of its own is read as the user's local clock. Null when none were set.
        /// </summary>
        [Preserve]
        public IReadOnlyDictionary<string, DateTime> CustomTimers =>
            _CustomTimers is null ? null : new ReadOnlyDictionary<string, DateTime>(_CustomTimers);

        [DataMember(Name = "custom_assets")]
        private Dictionary<string, AdaptyCustomAsset> _CustomAssets;

        /// <summary>
        /// The assets the flow uses in place of its own, keyed by the asset id in the layout. Null when none were set.
        /// </summary>
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

        /// <summary>
        /// Android only. The purchase extras to apply to each product the flow offers, keyed by
        /// identifier. Null when none were set; ignored on iOS.
        /// </summary>
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
        /// Android only. Lays the view out without safe area paddings when false. Null leaves the
        /// native default, which is true. Ignored on iOS.
        /// </summary>
        [DataMember(Name = "enable_safe_area_paddings")]
        public bool? EnableSafeAreaPaddings;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Locale)}: {Locale}, "
            + $"{nameof(LoadTimeout)}: {LoadTimeout}, "
            + $"{nameof(PreloadProducts)}: {PreloadProducts}, "
            + $"{nameof(CustomTags)}: {CustomTags}, "
            + $"{nameof(CustomTimers)}: {CustomTimers}, "
            + $"{nameof(CustomAssets)}: {CustomAssets}, "
            + $"{nameof(ProductPurchaseParameters)}: {ProductPurchaseParameters}, "
            + $"{nameof(EnableSafeAreaPaddings)}: {EnableSafeAreaPaddings}";

        /// <summary>
        /// Sets <see cref="Locale"/>.
        /// </summary>
        /// <param name="locale">The localization to render the flow with, such as "en" or "es".</param>
        public AdaptyUICreateFlowViewParameters SetLocale(string locale)
        {
            Locale = locale;
            return this;
        }

        /// <summary>
        /// Sets <see cref="LoadTimeout"/>.
        /// </summary>
        /// <param name="loadTimeout">How long to wait for the flow's assets.</param>
        public AdaptyUICreateFlowViewParameters SetLoadTimeout(TimeSpan? loadTimeout)
        {
            LoadTimeout = loadTimeout;
            return this;
        }

        /// <summary>
        /// Sets <see cref="PreloadProducts"/>.
        /// </summary>
        /// <param name="preloadProducts">True to fetch the products while the view is built.</param>
        public AdaptyUICreateFlowViewParameters SetPreloadProducts(bool? preloadProducts)
        {
            PreloadProducts = preloadProducts;
            return this;
        }

        /// <summary>
        /// Sets <see cref="CustomTags"/>, copying the dictionary.
        /// </summary>
        /// <param name="customTags">The value for each custom tag, keyed by tag name.</param>
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
        /// <summary>
        /// Sets <see cref="CustomTimers"/>, copying the dictionary.
        /// </summary>
        public AdaptyUICreateFlowViewParameters SetCustomTimers(
            IReadOnlyDictionary<string, DateTime> customTimers
        )
        {
            _CustomTimers = Copy(customTimers);
            return this;
        }

        /// <summary>
        /// Sets <see cref="CustomAssets"/>, copying the dictionary.
        /// </summary>
        /// <param name="customAssets">The asset to use for each id in the layout.</param>
        public AdaptyUICreateFlowViewParameters SetCustomAssets(
            IReadOnlyDictionary<string, AdaptyCustomAsset> customAssets
        )
        {
            _CustomAssets = Copy(customAssets);
            return this;
        }

        /// <summary>
        /// Sets <see cref="ProductPurchaseParameters"/>, copying the dictionary. Android only.
        /// </summary>
        /// <param name="productPurchaseParameters">The purchase extras for each product.</param>
        public AdaptyUICreateFlowViewParameters SetProductPurchaseParameters(
            IReadOnlyDictionary<AdaptyProductIdentifier, AdaptyPurchaseParameters> productPurchaseParameters
        )
        {
            _ProductPurchaseParameters = Copy(productPurchaseParameters);
            return this;
        }

        /// <summary>
        /// Sets <see cref="EnableSafeAreaPaddings"/>. Android only.
        /// </summary>
        /// <param name="enableSafeAreaPaddings">False to lay the view out without safe area paddings.</param>
        public AdaptyUICreateFlowViewParameters SetEnableSafeAreaPaddings(
            bool? enableSafeAreaPaddings
        )
        {
            EnableSafeAreaPaddings = enableSafeAreaPaddings;
            return this;
        }
    }
}
