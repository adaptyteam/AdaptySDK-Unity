using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
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

        public static AdaptyPlacementFetchPolicy ReloadRevalidatingCacheData = new(
            "reload_revalidating_cache_data",
            null
        );
        public static AdaptyPlacementFetchPolicy ReturnCacheDataElseLoad = new(
            "return_cache_data_else_load",
            null
        );

        // Declared after the policy it aliases: a static field initializer runs in declaration
        // order, so the other way round leaves Default null.
        public static AdaptyPlacementFetchPolicy Default = ReloadRevalidatingCacheData;

        public static AdaptyPlacementFetchPolicy ReturnCacheDataIfNotExpiredElseLoad(
            TimeSpan maxAge
        ) => new("return_cache_data_if_not_expired_else_load", maxAge);

        public override string ToString() =>
            $"{nameof(_Type)}: {_Type}, " + $"{nameof(_MaxAge)}: {_MaxAge}";
    }
}
