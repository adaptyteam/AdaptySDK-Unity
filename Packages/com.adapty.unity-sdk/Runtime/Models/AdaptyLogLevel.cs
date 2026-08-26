using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How much the native SDK writes to the platform log.
    /// </summary>
    /// <remarks>
    /// Each level includes the ones before it. Set it in the configuration, or at any time with <see cref="Adapty.SetLogLevel(AdaptySDK.AdaptyLogLevel, System.Action{AdaptySDK.AdaptyError})"/>.
    /// </remarks>
    [Preserve]
    public enum AdaptyLogLevel
    {
        /// <summary>
        /// Failures only.
        /// </summary>
        [EnumMember(Value = "error")]
        Error = 0,
        /// <summary>
        /// Failures, and conditions the SDK could work around.
        /// </summary>
        [EnumMember(Value = "warn")]
        Warn = 1,
        /// <summary>
        /// The above, plus the significant things the SDK does.
        /// </summary>
        [EnumMember(Value = "info")]
        Info = 2,
        /// <summary>
        /// The above, plus the calls made and the requests sent.
        /// </summary>
        [EnumMember(Value = "verbose")]
        Verbose = 3,
        /// <summary>
        /// Everything, including payload bodies. For development, not for a shipped build.
        /// </summary>
        [EnumMember(Value = "debug")]
        Debug = 4
    }
}
