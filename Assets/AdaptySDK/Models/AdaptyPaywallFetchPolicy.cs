//
//  AdaptyPaywallFetchPolicy.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 26.12.2023.
//

namespace AdaptySDK
{
    using System;

    public partial class AdaptyPaywallFetchPolicy
    {
        private readonly string _Type;
        private readonly TimeSpan? _MaxAge;

        private AdaptyPaywallFetchPolicy(string type, TimeSpan? maxAge)
        {
            _Type = type;
            _MaxAge = maxAge;
        }

        public static AdaptyPaywallFetchPolicy Default = ReloadRevalidatingCacheData;
        public static AdaptyPaywallFetchPolicy ReloadRevalidatingCacheData = 
            new("reload_revalidating_cache_data", null);
        public static AdaptyPaywallFetchPolicy ReturnCacheDataElseLoad = 
            new("return_cache_data_else_load", null);
        public static AdaptyPaywallFetchPolicy ReturnCacheDataIfNotExpiredElseLoad(TimeSpan maxAge) => 
            new("return_cache_data_if_not_expired_else_load", maxAge);

        public override string ToString() => 
            $"{nameof(_Type)}: {_Type}, " +
            $"{nameof(_MaxAge)}: {_MaxAge}";
    }
}