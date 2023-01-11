//
//  PaymentMode+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.PaymentMode GetPaymentMode(this JSONNode node, string aKey)
            => GetString(node, aKey).ToPaymentMode();
        internal static Adapty.PaymentMode? GetPaymentModeIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToPaymentMode();

        internal static Adapty.PaymentMode ToPaymentMode(this string value)
            => value switch
            {
                "pay_as_you_go" => Adapty.PaymentMode.PayAsYouGo,
                "pay_up_front" => Adapty.PaymentMode.PayUpFront,
                "free_trial" => Adapty.PaymentMode.FreeTrial,
                _ => Adapty.PaymentMode.Unknown,
            };
    }
}

