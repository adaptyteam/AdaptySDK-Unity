//
//  AndroidSubscriptionUpdateReplacementMode+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 25.11.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.AndroidSubscriptionUpdateReplacementMode value)
        {
            switch (value)
            {
                case Adapty.AndroidSubscriptionUpdateReplacementMode.WithTimeProration: return "with_time_proration";
                case Adapty.AndroidSubscriptionUpdateReplacementMode.ChargeProratedPrice: return "charge_prorated_price";
                case Adapty.AndroidSubscriptionUpdateReplacementMode.WithoutProration: return "without_proration";
                case Adapty.AndroidSubscriptionUpdateReplacementMode.Deferred: return "deferred";
                case Adapty.AndroidSubscriptionUpdateReplacementMode.ChargeFullPrice: return "charge_full_price";
                default: throw new Exception($"AndroidSubscriptionUpdateReplacementMode unknown value: {value}");
            }
        }
    }
}