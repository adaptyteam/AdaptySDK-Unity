using System;
using AdaptySDK;

namespace AdaptyExample
{
    public enum PlacementLoadStrategy
    {
        LoadElseCache,
        CacheElseLoad,
        CacheElseLoadIfExperied_10sec,
        CacheElseLoadIfExperied_60sec,
        CacheElseLoadIfExperied_600sec,
    }

    public static class PlacementLoadStrategyExtensions
    {
        public static AdaptyPlacementFetchPolicy ToFetchPolicy(
            this PlacementLoadStrategy loadStrategy
        )
        {
            switch (loadStrategy)
            {
                case PlacementLoadStrategy.LoadElseCache:
                    return AdaptyPlacementFetchPolicy.ReloadRevalidatingCacheData;
                case PlacementLoadStrategy.CacheElseLoad:
                    return AdaptyPlacementFetchPolicy.ReturnCacheDataElseLoad;
                case PlacementLoadStrategy.CacheElseLoadIfExperied_10sec:
                    return AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                        TimeSpan.FromSeconds(10)
                    );
                case PlacementLoadStrategy.CacheElseLoadIfExperied_60sec:
                    return AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                        TimeSpan.FromSeconds(60)
                    );
                case PlacementLoadStrategy.CacheElseLoadIfExperied_600sec:
                    return AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                        TimeSpan.FromSeconds(600)
                    );
                default:
                    return AdaptyPlacementFetchPolicy.Default;
            }
        }
    }
}
