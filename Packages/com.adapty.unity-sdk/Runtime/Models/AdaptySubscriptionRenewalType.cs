using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Android only. Whether the subscription renews by itself.
    /// </summary>
    [Preserve]
    public enum AdaptySubscriptionRenewalType
    {
        /// <summary>
        /// A prepaid plan: paid for a fixed span and not renewed unless the user tops it up.
        /// </summary>
        [EnumMember(Value = "prepaid")]
        Prepaid = 0,
        /// <summary>
        /// Renews on its own until cancelled. The default for the contract.
        /// </summary>
        [EnumMember(Value = "autorenewable")]
        Autorenewable = 1,
    }
}