//
//  AdaptyUICreateViewOptional.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 18.12.2024.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public partial class AdaptyUICreateViewOptional
    {
        public TimeSpan? LoadTimeout;
        public bool? PreloadProducts;
        public Dictionary<string, string> CustomTags;
        public Dictionary<string, DateTime> CustomTimers;
        public Dictionary<string, bool> AndroidPersonalizedOffers;


        public override string ToString() =>
            $"{nameof(LoadTimeout)}: {LoadTimeout}, " +
            $"{nameof(PreloadProducts)}: {PreloadProducts}, " +
            $"{nameof(CustomTags)}: {CustomTags}, " +
            $"{nameof(CustomTimers)}: {CustomTimers}, " +
            $"{nameof(AndroidPersonalizedOffers)}: {AndroidPersonalizedOffers}";

        public AdaptyUICreateViewOptional SetLoadTimeout(TimeSpan? loadTimeout)
        {
            LoadTimeout = loadTimeout;
            return this;
        }

        public AdaptyUICreateViewOptional SetPreloadProducts(bool? preloadProducts)
        {
            PreloadProducts = preloadProducts;
            return this;
        }

        public AdaptyUICreateViewOptional SetCustomTags(Dictionary<string, string> customTags)
        {
            CustomTags = customTags;
            return this;
        }

        public AdaptyUICreateViewOptional SetCustomTimers(Dictionary<string, DateTime> customTimers)
        {
            CustomTimers = customTimers;
            return this;
        }

        public AdaptyUICreateViewOptional SetAndroidPersonalizedOffers(Dictionary<string, bool> androidPersonalizedOffers)
        {
            AndroidPersonalizedOffers = androidPersonalizedOffers;
            return this;
        }
    }
}