//
//  AdaptyServerCluster.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 10.12.2024.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyServerCluster
    {
        [EnumMember(Value = "default")]
        Default,
        [EnumMember(Value = "eu")]
        EU,
        [EnumMember(Value = "cn")]
        CN,
    }
}
