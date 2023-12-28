//
//  PaywallFetchPolicy.cs
//  Adapty
//
//  Created by Aleksei Valiano on 26.12.2023.
//

namespace AdaptySDK
{
    using System;

    public static partial class Adapty
    {
        public partial class PaywallFetchPolicy
        {
            private readonly string _Type;
            private readonly TimeSpan? _MaxAge;

            private PaywallFetchPolicy(string type, TimeSpan? maxAge)
            {
                _Type = type;
                _MaxAge = maxAge;
            }

            public static PaywallFetchPolicy ReloadRevalidatingCacheData = new("reload_revalidating_cache_data", null);
            public static PaywallFetchPolicy ReturnCacheDataElseLoad = new PaywallFetchPolicy("return_cache_data_else_load", null);
            public static PaywallFetchPolicy ReturnCacheDataIfNotExpiredElseLoad(TimeSpan maxAge)
            {
                return new PaywallFetchPolicy("return_cache_data_if_not_expired_else_load", maxAge);
            }


            public override string ToString() => $"{nameof(_Type)}: {_Type}, " +
                       $"{nameof(_MaxAge)}: {_MaxAge}";
        }
    }
}