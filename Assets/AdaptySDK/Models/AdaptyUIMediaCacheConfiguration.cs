//
//  AdaptyUIMediaCacheConfiguration.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

namespace AdaptySDK
{
    public partial class AdaptyUIMediaCacheConfiguration
    {
        public int? MemoryStorageTotalCostLimit;
        public int? MemoryStorageCountLimit;
        public int? DiskStorageSizeLimit;

        public AdaptyUIMediaCacheConfiguration(int? memoryStorageTotalCostLimit, int? memoryStorageCountLimit, int? diskStorageSizeLimit)
        {
            MemoryStorageTotalCostLimit = memoryStorageTotalCostLimit;
            MemoryStorageCountLimit = memoryStorageCountLimit;
            DiskStorageSizeLimit = diskStorageSizeLimit;
        }

        public override string ToString() =>
            $"{nameof(MemoryStorageTotalCostLimit)}: {MemoryStorageTotalCostLimit}, " +
            $"{nameof(MemoryStorageCountLimit)}: {MemoryStorageCountLimit}, " +
            $"{nameof(DiskStorageSizeLimit)}: {DiskStorageSizeLimit}";

    }
}