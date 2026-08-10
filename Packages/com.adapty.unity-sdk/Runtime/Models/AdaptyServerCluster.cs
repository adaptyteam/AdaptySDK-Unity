using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyServerCluster
    {
        [EnumMember(Value = "default")]
        Default = 0,
        [EnumMember(Value = "eu")]
        EU = 1,
        [EnumMember(Value = "cn")]
        CN = 2,
    }
}
