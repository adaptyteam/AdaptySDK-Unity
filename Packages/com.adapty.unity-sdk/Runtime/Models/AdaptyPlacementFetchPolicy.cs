using System;
using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// Whether a fetch may answer from the cache, and for how long. Pick one of the shared
    /// instances or build one with <see cref="ReturnCacheDataIfNotExpiredElseLoad"/>.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyPlacementFetchPolicy
    {
        [DataMember(Name = "type", IsRequired = true)]
        private readonly string _Type;

        private readonly TimeSpan? _MaxAge;

        /// <summary>
        /// The contract carries the age in seconds, not as a duration literal.
        /// </summary>
        [DataMember(Name = "max_age")]
        [Preserve]
        private double? MaxAgeInSeconds => _MaxAge?.TotalSeconds;

        private AdaptyPlacementFetchPolicy(string type, TimeSpan? maxAge)
        {
            _Type = type;
            _MaxAge = maxAge;
        }

        /// <summary>
        /// Ask the server, and fall back to the cache when it cannot be reached. The default.
        /// </summary>
        public static readonly AdaptyPlacementFetchPolicy ReloadRevalidatingCacheData = new(
            "reload_revalidating_cache_data",
            null
        );
        /// <summary>
        /// Use the cache when there is anything in it, however old, and only ask the server otherwise.
        /// </summary>
        public static readonly AdaptyPlacementFetchPolicy ReturnCacheDataElseLoad = new(
            "return_cache_data_else_load",
            null
        );

        // Declared after the policy it aliases: a static field initializer runs in declaration
        // order, so the other way round leaves Default null.
        /// <summary>
        /// The policy used when none is given — the same instance as
        /// <see cref="ReloadRevalidatingCacheData"/>.
        /// </summary>
        public static readonly AdaptyPlacementFetchPolicy Default = ReloadRevalidatingCacheData;

        /// <summary>
        /// Use the cache while it is younger than <paramref name="maxAge"/>, and ask the server once it
        /// is older.
        /// </summary>
        public static AdaptyPlacementFetchPolicy ReturnCacheDataIfNotExpiredElseLoad(
            TimeSpan maxAge
        ) => new("return_cache_data_if_not_expired_else_load", maxAge);

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(_Type)}: {_Type}, " + $"{nameof(_MaxAge)}: {_MaxAge}";
    }
}
