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
            => value switch
            {
                Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateWithTimeProration => "immediate_with_time_proration",
                Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateAndChargeProratedPrice => "immediate_and_charge_prorated_price",
                Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateWithoutProration => "immediate_without_proration",
                Adapty.AndroidSubscriptionUpdateProrationMode.Deferred => "deferred",
                Adapty.AndroidSubscriptionUpdateProrationMode.ImmediateAndChargeFullPrice => "immediate_and_charge_full_price",
                _ => throw new Exception($"AndroidSubscriptionUpdateProrationMode unknown value: {value}"),
            };
    }
}
