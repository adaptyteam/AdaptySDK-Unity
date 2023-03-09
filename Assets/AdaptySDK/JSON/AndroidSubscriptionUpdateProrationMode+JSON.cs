//
//  AndroidSubscriptionUpdateProrationMode+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 25.11.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.AndroidSubscriptionUpdateProrationMode value)
        {
            switch (value)
            {
                case Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateWithTimeProration: return "immediate_with_time_proration";
                case Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateAndChargeProratedPrice: return "immediate_and_charge_prorated_price";
                case Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateWithoutProration: return "immediate_without_proration";
                case Adapty.AndroidSubscriptionUpdateProrationMode.Deferred: return "deferred";
                case Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateAndChargeFullPrice: return "immediate_and_charge_full_price";
                default: throw new Exception($"AndroidSubscriptionUpdateProrationMode unknown value: {value}");
            }
        }
    }
}
