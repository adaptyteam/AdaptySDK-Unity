//
//  AdaptyPaymentMode.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyPaymentMode
    {
        [EnumMember(Value = "pay_as_you_go")]
        PayAsYouGo = 0,
        [EnumMember(Value = "pay_up_front")]
        PayUpFront = 1,
        [EnumMember(Value = "free_trial")]
        FreeTrial = 2,
        [EnumMember(Value = "unknown")]
        Unknown = 3,
    }
}