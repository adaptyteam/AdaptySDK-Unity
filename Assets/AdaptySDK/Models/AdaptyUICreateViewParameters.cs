//
//  AdaptyUICreateViewParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 18.12.2024.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public partial class AdaptyUICreateViewParameters
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

        public AdaptyUICreateViewParameters SetLoadTimeout(TimeSpan? loadTimeout)
        {
            LoadTimeout = loadTimeout;
            return this;
        }

        public AdaptyUICreateViewParameters SetPreloadProducts(bool? preloadProducts)
        {
            PreloadProducts = preloadProducts;
            return this;
        }

        public AdaptyUICreateViewParameters SetCustomTags(Dictionary<string, string> customTags)
        {
            CustomTags = customTags;
            return this;
        }

        public AdaptyUICreateViewParameters SetCustomTimers(Dictionary<string, DateTime> customTimers)
        {
            CustomTimers = customTimers;
            return this;
        }

        public AdaptyUICreateViewParameters SetAndroidPersonalizedOffers(Dictionary<string, bool> androidPersonalizedOffers)
        {
            AndroidPersonalizedOffers = androidPersonalizedOffers;
            return this;
        }
    }
}