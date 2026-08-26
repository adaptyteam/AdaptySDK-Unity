using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// iOS only. What to tell the App Store when it consults you about a refund request for this user.
    /// </summary>
    /// <remarks>
    /// A preference, not a decision — the App Store is not obliged to follow it.
    /// </remarks>
    [Preserve]
    public enum AdaptyRefundPreference
    {
        /// <summary>
        /// Express no preference and let the App Store decide.
        /// </summary>
        [EnumMember(Value = "no_preference")]
        NoPreference = 0,
        /// <summary>
        /// Ask the App Store to grant the refund.
        /// </summary>
        [EnumMember(Value = "grant")]
        Grant = 1,
        /// <summary>
        /// Ask the App Store to decline it.
        /// </summary>
        [EnumMember(Value = "decline")]
        Decline = 2,
    }
}
