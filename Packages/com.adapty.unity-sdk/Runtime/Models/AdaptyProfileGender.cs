using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The gender recorded on a profile.
    /// </summary>
    [Preserve]
    public enum AdaptyProfileGender
    {
        /// <summary>
        /// Female.
        /// </summary>
        [EnumMember(Value = "f")]
        Female = 0,
        /// <summary>
        /// Male.
        /// </summary>
        [EnumMember(Value = "m")]
        Male = 1,
        /// <summary>
        /// Anything else, including a user who prefers not to say.
        /// </summary>
        [EnumMember(Value = "o")]
        Other = 2,
    }
}
