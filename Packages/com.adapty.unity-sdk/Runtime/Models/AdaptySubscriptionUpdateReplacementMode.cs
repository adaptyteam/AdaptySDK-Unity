using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptySubscriptionUpdateReplacementMode
    {
        [EnumMember(Value = "with_time_proration")]
        WithTimeProration = 0,
        [EnumMember(Value = "charge_prorated_price")]
        ChargeProratedPrice = 1,
        [EnumMember(Value = "without_proration")]
        WithoutProration = 2,
        [EnumMember(Value = "deferred")]
        Deferred = 3,
        [EnumMember(Value = "charge_full_price")]
        ChargeFullPrice = 4,
    }
}
