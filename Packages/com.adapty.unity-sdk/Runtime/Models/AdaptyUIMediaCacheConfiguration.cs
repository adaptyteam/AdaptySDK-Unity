//
//  AdaptyUIMediaCacheConfiguration.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public partial class AdaptyUIMediaCacheConfiguration
    {
        [DataMember(Name = "memory_storage_total_cost_limit")]
        public int? MemoryStorageTotalCostLimit;
        [DataMember(Name = "memory_storage_count_limit")]
        public int? MemoryStorageCountLimit;
        [DataMember(Name = "disk_storage_size_limit")]
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