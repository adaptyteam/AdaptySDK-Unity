using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Limits for the cache the flow renderer keeps for images and video. Any limit left null
    /// keeps the native default.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyUIMediaCacheConfiguration
    {
        /// <summary>
        /// How much the in-memory cache may hold, in bytes.
        /// </summary>
        [DataMember(Name = "memory_storage_total_cost_limit")]
        public int? MemoryStorageTotalCostLimit;
        /// <summary>
        /// How many items the in-memory cache may hold.
        /// </summary>
        [DataMember(Name = "memory_storage_count_limit")]
        public int? MemoryStorageCountLimit;
        /// <summary>
        /// How much the on-disk cache may hold, in bytes.
        /// </summary>
        [DataMember(Name = "disk_storage_size_limit")]
        public int? DiskStorageSizeLimit;

        /// <summary>
        /// Sets the cache limits. Any of them null keeps the native default.
        /// </summary>
        /// <param name="memoryStorageTotalCostLimit">In-memory cache limit, in bytes.</param>
        /// <param name="memoryStorageCountLimit">How many items the in-memory cache may hold.</param>
        /// <param name="diskStorageSizeLimit">On-disk cache limit, in bytes.</param>
        public AdaptyUIMediaCacheConfiguration(int? memoryStorageTotalCostLimit, int? memoryStorageCountLimit, int? diskStorageSizeLimit)
        {
            MemoryStorageTotalCostLimit = memoryStorageTotalCostLimit;
            MemoryStorageCountLimit = memoryStorageCountLimit;
            DiskStorageSizeLimit = diskStorageSizeLimit;
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(MemoryStorageTotalCostLimit)}: {MemoryStorageTotalCostLimit}, " +
            $"{nameof(MemoryStorageCountLimit)}: {MemoryStorageCountLimit}, " +
            $"{nameof(DiskStorageSizeLimit)}: {DiskStorageSizeLimit}";

    }
}