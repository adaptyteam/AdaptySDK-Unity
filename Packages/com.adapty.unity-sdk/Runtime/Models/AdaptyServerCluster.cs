using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// Which Adapty server region the SDK talks to. Set it in the configuration to match where your account's data is held.
    /// </summary>
    [Preserve]
    public enum AdaptyServerCluster
    {
        /// <summary>
        /// The default cluster.
        /// </summary>
        [EnumMember(Value = "default")]
        Default = 0,
        /// <summary>
        /// The European Union cluster.
        /// </summary>
        [EnumMember(Value = "eu")]
        EU = 1,
        /// <summary>
        /// The mainland China cluster.
        /// </summary>
        [EnumMember(Value = "cn")]
        CN = 2,
    }
}
