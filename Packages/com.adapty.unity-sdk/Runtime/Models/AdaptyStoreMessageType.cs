using System;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The kind of an App Store message waiting to be shown. iOS only.
    /// </summary>
    /// <remarks>
    /// The shared instances are the kinds known when this SDK version was released, not the
    /// limit: a reason StoreKit adds later arrives as <c>storekit_&lt;n&gt;</c> with its own
    /// <see cref="RawValue"/>.
    /// </remarks>
    [Preserve]
    public sealed class AdaptyStoreMessageType : IEquatable<AdaptyStoreMessageType>
    {
        /// <summary>
        /// The identifier the native SDK reports the kind by.
        /// </summary>
        public readonly string RawValue;

        /// <summary>
        /// Creates a kind from its identifier — for a kind added after the shared instances were
        /// snapshotted.
        /// </summary>
        /// <param name="rawValue">The identifier the native SDK reports the kind by.</param>
        /// <exception cref="ArgumentNullException">The identifier is null.</exception>
        public AdaptyStoreMessageType(string rawValue)
        {
            RawValue = rawValue ?? throw new ArgumentNullException(nameof(rawValue));
        }

        /// <summary>
        /// A general App Store message.
        /// </summary>
        public static readonly AdaptyStoreMessageType Generic = new("generic");

        /// <summary>
        /// A request to consent to a subscription price increase.
        /// </summary>
        public static readonly AdaptyStoreMessageType PriceIncreaseConsent = new("price_increase_consent");

        /// <summary>
        /// A subscription billing issue.
        /// </summary>
        public static readonly AdaptyStoreMessageType BillingIssue = new("billing_issue");

        /// <summary>
        /// A win-back offer the user is eligible for.
        /// </summary>
        public static readonly AdaptyStoreMessageType WinBackOffer = new("win_back_offer");

        /// <summary>
        /// Whether the other kind carries the same identifier.
        /// </summary>
        /// <param name="other">The kind to compare with.</param>
        public bool Equals(AdaptyStoreMessageType other) =>
            other is not null && RawValue == other.RawValue;

        /// <summary>
        /// Whether the other object is a kind carrying the same identifier.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        public override bool Equals(object obj) => Equals(obj as AdaptyStoreMessageType);

        /// <summary>
        /// The hash code of the identifier.
        /// </summary>
        public override int GetHashCode() => RawValue.GetHashCode();

        /// <summary>
        /// Whether both kinds carry the same identifier, or both are null.
        /// </summary>
        /// <param name="a">The left kind.</param>
        /// <param name="b">The right kind.</param>
        public static bool operator ==(AdaptyStoreMessageType a, AdaptyStoreMessageType b) =>
            a is null ? b is null : a.Equals(b);

        /// <summary>
        /// Whether the kinds carry different identifiers, or exactly one is null.
        /// </summary>
        /// <param name="a">The left kind.</param>
        /// <param name="b">The right kind.</param>
        public static bool operator !=(AdaptyStoreMessageType a, AdaptyStoreMessageType b) =>
            !(a == b);

        /// <summary>
        /// The identifier itself.
        /// </summary>
        public override string ToString() => RawValue;
    }
}
