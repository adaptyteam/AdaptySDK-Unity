//
//  AdaptySubscriptionUpdateReplacementMode+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 25.11.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static JSONNode ToJSONNode(this AdaptySubscriptionUpdateReplacementMode value)
        {
            switch (value)
            {
                case AdaptySubscriptionUpdateReplacementMode.WithTimeProration: return "with_time_proration";
                case AdaptySubscriptionUpdateReplacementMode.ChargeProratedPrice: return "charge_prorated_price";
                case AdaptySubscriptionUpdateReplacementMode.WithoutProration: return "without_proration";
                case AdaptySubscriptionUpdateReplacementMode.Deferred: return "deferred";
                case AdaptySubscriptionUpdateReplacementMode.ChargeFullPrice: return "charge_full_price";
                default: throw new Exception($"AdaptySubscriptionUpdateReplacementMode unknown value: {value}");
            }
        }
    }
}