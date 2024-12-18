//
//  AdaptyUIConfiguration.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

namespace AdaptySDK
{
    public partial class AdaptyUIConfiguration
    {
        public int? MemoryStorageTotalCostLimit;
        public int? MemoryStorageCountLimit;
        public int? DiskStorageSizeLimit;

        public override string ToString() =>
            $"{nameof(MemoryStorageTotalCostLimit)}: {MemoryStorageTotalCostLimit}, " +
            $"{nameof(MemoryStorageCountLimit)}: {MemoryStorageCountLimit}, " +
            $"{nameof(DiskStorageSizeLimit)}: {DiskStorageSizeLimit}";


        public AdaptyUIConfiguration SetMemoryStorageTotalCostLimit(int? memoryStorageTotalCostLimit)
        {
            MemoryStorageTotalCostLimit = memoryStorageTotalCostLimit;
            return this;
        }

        public AdaptyUIConfiguration SetMemoryStorageCountLimit(int? memoryStorageCountLimit)
        {
            MemoryStorageCountLimit = memoryStorageCountLimit;
            return this;
        }

        public AdaptyUIConfiguration SetDiskStorageSizeLimit(int? diskStorageSizeLimit)
        {
            DiskStorageSizeLimit = diskStorageSizeLimit;
            return this;
        }
    }
}