//
//  AdaptyPaymentMode+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPaymentMode GetAdaptyPaymentMode(this JSONNode node, string aKey)
            => GetString(node, aKey).ToAdaptyPaymentMode();

        internal static AdaptyPaymentMode? GetAdaptyPaymentModeIfPresent(this JSONNode node, string aKey)
            => GetStringIfPresent(node, aKey)?.ToAdaptyPaymentMode();

        internal static AdaptyPaymentMode ToAdaptyPaymentMode(this string value)
        {
            switch (value)
            {
                case "pay_as_you_go": return AdaptyPaymentMode.PayAsYouGo;
                case "pay_up_front": return AdaptyPaymentMode.PayUpFront;
                case "free_trial": return AdaptyPaymentMode.FreeTrial;
                default: return AdaptyPaymentMode.Unknown;
            }
        }
    }
}