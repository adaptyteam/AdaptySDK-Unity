using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public enum PaymentMode
        {
            PayAsYouGo = 0,
            PayUpFront = 1,
            FreeTrial = 2,
            Unknown = 3
        }


        public static PaymentMode PaymentModeFromJSON(JSONNode response)
        {
            if (response.IsNumber)
            {
                return PaymentModeFromInt(response);
            }
            else if (response.IsString)
            {
                return PaymentModeFromString(response);
            }
            else
            {
                return PaymentMode.Unknown;
            }
        }

        public static PaymentMode PaymentModeFromString(string value)
        {
            if (value == null) return PaymentMode.Unknown;
            switch (value)
            {
                case "payAsYouGo":
                case "pay_as_you_go":
                    return PaymentMode.PayAsYouGo;
                case "payUpFront":
                case "pay_up_front":
                    return PaymentMode.PayUpFront;
                case "FreeTrial":
                case "free_trial":
                    return PaymentMode.FreeTrial;
                default:
                    return PaymentMode.Unknown;
            }
        }

        public static PaymentMode PaymentModeFromInt(int value)
        {
            if (value < 0 || value > 3)
            {
                return PaymentMode.Unknown;
            }
            else
            {
                return (PaymentMode)value;
            }
        }
    }
}
