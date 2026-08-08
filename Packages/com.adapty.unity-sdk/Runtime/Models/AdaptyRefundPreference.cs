//
//  AdaptyRefundPreference.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 19.03.2025.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyRefundPreference
    {
        [EnumMember(Value = "no_preference")]
        NoPreference,
        [EnumMember(Value = "grant")]
        Grant,
        [EnumMember(Value = "decline")]
        Decline,
    }
}
