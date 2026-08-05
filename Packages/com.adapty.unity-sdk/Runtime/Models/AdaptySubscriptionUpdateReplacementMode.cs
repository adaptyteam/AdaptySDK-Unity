//
//  AdaptySubscriptionUpdateReplacementMode.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptySubscriptionUpdateReplacementMode
    {
        [EnumMember(Value = "with_time_proration")]
        WithTimeProration,
        [EnumMember(Value = "charge_prorated_price")]
        ChargeProratedPrice,
        [EnumMember(Value = "without_proration")]
        WithoutProration,
        [EnumMember(Value = "deferred")]
        Deferred,
        [EnumMember(Value = "charge_full_price")]
        ChargeFullPrice,
    }
}
