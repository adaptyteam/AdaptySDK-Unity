using System;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How a subscription is paid for: all at once, or in installments over a commitment.
    /// </summary>
    /// <remarks>
    /// The shared instances are the plans known when this SDK version was released, not the
    /// limit: a plan StoreKit or the native SDK adds later arrives with its own
    /// <see cref="RawValue"/> rather than being mapped to one of them.
    /// </remarks>
    [Preserve]
    public sealed class AdaptySubscriptionBillingPlan : IEquatable<AdaptySubscriptionBillingPlan>
    {
        /// <summary>
        /// The identifier the native SDK reports the plan by.
        /// </summary>
        public readonly string RawValue;

        /// <summary>
        /// Creates a plan from its identifier — for a plan added after the shared instances were
        /// snapshotted.
        /// </summary>
        /// <param name="rawValue">The identifier the native SDK reports the plan by.</param>
        /// <exception cref="ArgumentNullException">The identifier is null.</exception>
        public AdaptySubscriptionBillingPlan(string rawValue)
        {
            RawValue = rawValue ?? throw new ArgumentNullException(nameof(rawValue));
        }

        /// <summary>
        /// The whole subscription period is charged at once.
        /// </summary>
        public static readonly AdaptySubscriptionBillingPlan UpFront = new("up_front");

        /// <summary>
        /// The commitment is charged in monthly installments. iOS 26.4+ only.
        /// </summary>
        public static readonly AdaptySubscriptionBillingPlan Monthly = new("monthly");

        /// <summary>
        /// Whether the other plan carries the same identifier.
        /// </summary>
        /// <param name="other">The plan to compare with.</param>
        public bool Equals(AdaptySubscriptionBillingPlan other) =>
            other is not null && RawValue == other.RawValue;

        /// <summary>
        /// Whether the other object is a plan carrying the same identifier.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        public override bool Equals(object obj) => Equals(obj as AdaptySubscriptionBillingPlan);

        /// <summary>
        /// The hash code of the identifier.
        /// </summary>
        public override int GetHashCode() => RawValue.GetHashCode();

        /// <summary>
        /// Whether both plans carry the same identifier, or both are null.
        /// </summary>
        /// <param name="a">The left plan.</param>
        /// <param name="b">The right plan.</param>
        public static bool operator ==(
            AdaptySubscriptionBillingPlan a,
            AdaptySubscriptionBillingPlan b
        ) => a is null ? b is null : a.Equals(b);

        /// <summary>
        /// Whether the plans carry different identifiers, or exactly one is null.
        /// </summary>
        /// <param name="a">The left plan.</param>
        /// <param name="b">The right plan.</param>
        public static bool operator !=(
            AdaptySubscriptionBillingPlan a,
            AdaptySubscriptionBillingPlan b
        ) => !(a == b);

        /// <summary>
        /// The identifier itself.
        /// </summary>
        public override string ToString() => RawValue;
    }
}
