//
//  AdaptySubscriptionRenewalType.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptySubscriptionRenewalType
    {
        [EnumMember(Value = "prepaid")]
        Prepaid = 0,
        [EnumMember(Value = "autorenewable")]
        Autorenewable = 1,
    }
}