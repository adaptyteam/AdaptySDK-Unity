using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Android only. When a subscription change takes effect and what happens to the time the user has already paid for.
    /// </summary>
    /// <remarks>
    /// These are Google Play's replacement modes; see <see href="https://developer.android.com/google/play/billing/subscriptions#replacement-modes">its documentation</see> for which ones a given change allows.
    /// </remarks>
    [Preserve]
    public enum AdaptySubscriptionUpdateReplacementMode
    {
        /// <summary>
        /// The change is immediate and the remaining time is credited as time: the next billing date moves to pay for what is left. Google Play's <c>WITH_TIME_PRORATION</c>.
        /// </summary>
        [EnumMember(Value = "with_time_proration")]
        WithTimeProration = 0,
        /// <summary>
        /// The change is immediate and the user is charged the difference for the rest of the current period. The billing date does not move. Only for an upgrade. Google Play's <c>CHARGE_PRORATED_PRICE</c>.
        /// </summary>
        [EnumMember(Value = "charge_prorated_price")]
        ChargeProratedPrice = 1,
        /// <summary>
        /// The change is immediate and nothing is credited or charged until the next billing date, which does not move. Google Play's <c>WITHOUT_PRORATION</c>.
        /// </summary>
        [EnumMember(Value = "without_proration")]
        WithoutProration = 2,
        /// <summary>
        /// The change waits for the next billing date; until then the user keeps what they had. Google Play's <c>DEFERRED</c>.
        /// </summary>
        [EnumMember(Value = "deferred")]
        Deferred = 3,
        /// <summary>
        /// The change is immediate and the user is charged the full price of the new plan at once, starting a new billing period. Google Play's <c>CHARGE_FULL_PRICE</c>.
        /// </summary>
        [EnumMember(Value = "charge_full_price")]
        ChargeFullPrice = 4,
    }
}
