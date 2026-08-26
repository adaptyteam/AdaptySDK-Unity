using System;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// An external attribution provider recognized by the Adapty backend.
    /// </summary>
    /// <remarks>
    /// The shared instances are a snapshot of the providers known when this SDK version was
    /// released, not the limit: the backend adds providers without an SDK release, and one added
    /// later is built with <see cref="AdaptyExternalAttributionProvider(string)"/>.
    /// </remarks>
    [Preserve]
    public sealed class AdaptyExternalAttributionProvider
        : IEquatable<AdaptyExternalAttributionProvider>
    {
        /// <summary>
        /// The identifier the Adapty backend knows the provider by.
        /// </summary>
        public readonly string RawValue;

        /// <summary>
        /// Creates a provider from its backend identifier, with surrounding whitespace trimmed —
        /// for a provider the backend added after the shared instances were snapshotted.
        /// </summary>
        /// <param name="rawValue">The identifier the Adapty backend knows the provider by.</param>
        /// <exception cref="ArgumentNullException">The identifier is null.</exception>
        public AdaptyExternalAttributionProvider(string rawValue)
        {
            if (rawValue is null)
            {
                throw new ArgumentNullException(nameof(rawValue));
            }
            RawValue = rawValue.Trim();
        }

        /// <summary>
        /// Apple Search Ads.
        /// </summary>
        public static readonly AdaptyExternalAttributionProvider AppleAds = new("apple_search_ads");

        /// <summary>
        /// Adjust.
        /// </summary>
        public static readonly AdaptyExternalAttributionProvider Adjust = new("adjust");

        /// <summary>
        /// AppsFlyer.
        /// </summary>
        public static readonly AdaptyExternalAttributionProvider Appsflyer = new("appsflyer");

        /// <summary>
        /// Branch.
        /// </summary>
        public static readonly AdaptyExternalAttributionProvider Branch = new("branch");

        /// <summary>
        /// Tenjin.
        /// </summary>
        public static readonly AdaptyExternalAttributionProvider Tenjin = new("tenjin");

        /// <summary>
        /// A custom attribution integration configured in the Adapty Dashboard.
        /// </summary>
        public static readonly AdaptyExternalAttributionProvider Custom = new("custom");

        /// <summary>
        /// Whether the other provider carries the same identifier.
        /// </summary>
        /// <param name="other">The provider to compare with.</param>
        public bool Equals(AdaptyExternalAttributionProvider other) =>
            other is not null && RawValue == other.RawValue;

        /// <summary>
        /// Whether the other object is a provider carrying the same identifier.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        public override bool Equals(object obj) =>
            Equals(obj as AdaptyExternalAttributionProvider);

        /// <summary>
        /// The hash code of the identifier.
        /// </summary>
        public override int GetHashCode() => RawValue.GetHashCode();

        // Without the operators a sealed class compares references, and comparing a profile's
        // provider against a known value is the main thing this type is read for.
        /// <summary>
        /// Whether both providers carry the same identifier, or both are null.
        /// </summary>
        /// <param name="a">The left provider.</param>
        /// <param name="b">The right provider.</param>
        public static bool operator ==(
            AdaptyExternalAttributionProvider a,
            AdaptyExternalAttributionProvider b
        ) => a is null ? b is null : a.Equals(b);

        /// <summary>
        /// Whether the providers carry different identifiers, or exactly one is null.
        /// </summary>
        /// <param name="a">The left provider.</param>
        /// <param name="b">The right provider.</param>
        public static bool operator !=(
            AdaptyExternalAttributionProvider a,
            AdaptyExternalAttributionProvider b
        ) => !(a == b);

        /// <summary>
        /// The identifier itself.
        /// </summary>
        public override string ToString() => RawValue;
    }
}
