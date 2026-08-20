using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How a discounted subscription phase is paid for.
    /// </summary>
    [Preserve]
    public enum AdaptyPaymentMode
    {
        /// <summary>
        /// Reduced price, charged each period of the offer.
        /// </summary>
        [EnumMember(Value = "pay_as_you_go")]
        PayAsYouGo = 0,
        /// <summary>
        /// The whole offer paid once, at its start.
        /// </summary>
        [EnumMember(Value = "pay_up_front")]
        PayUpFront = 1,
        /// <summary>
        /// Nothing is charged for the phase.
        /// </summary>
        [EnumMember(Value = "free_trial")]
        FreeTrial = 2,
        /// <summary>
        /// The store reported a mode the contract does not list. One of the two enums that keep an unknown value, because the contract lists <c>"unknown"</c> among theirs.
        /// </summary>
        [EnumMember(Value = "unknown")]
        Unknown = 3,
    }
}
