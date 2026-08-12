using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Which kind of discounted offer a subscription phase belongs to.
    /// </summary>
    [Preserve]
    public enum AdaptySubscriptionOfferType
    {
        /// <summary>
        /// The offer for a user who has never subscribed to this product.
        /// </summary>
        [EnumMember(Value = "introductory")]
        Introductory = 0,
        /// <summary>
        /// An offer aimed at an existing or lapsed subscriber, identified by an offer id.
        /// </summary>
        [EnumMember(Value = "promotional")]
        Promotional = 1,
        /// <summary>
        /// An offer aimed at a user whose subscription has ended.
        /// </summary>
        [EnumMember(Value = "win_back")]
        WinBack = 2,
        /// <summary>
        /// iOS only. An offer redeemed through an App Store offer code.
        /// </summary>
        [EnumMember(Value = "code")]
        Code = 3,
    }
}
