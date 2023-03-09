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
        {
            switch (value)
            {
                case "pay_as_you_go": return Adapty.PaymentMode.PayAsYouGo;
                case "pay_up_front": return Adapty.PaymentMode.PayUpFront;
                case "free_trial": return Adapty.PaymentMode.FreeTrial;
                default: return Adapty.PaymentMode.Unknown;
            }
        }
    }
}

