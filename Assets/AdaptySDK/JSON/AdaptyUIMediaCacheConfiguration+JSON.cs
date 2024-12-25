//
//  AdaptyUIMediaCacheConfiguration+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIMediaCacheConfiguration
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            if (MemoryStorageTotalCostLimit.HasValue) node.Add("memory_storage_total_cost_limit", MemoryStorageTotalCostLimit.Value);
            if (MemoryStorageCountLimit.HasValue) node.Add("memory_storage_count_limit", MemoryStorageCountLimit.Value);
            if (DiskStorageSizeLimit.HasValue) node.Add("disk_storage_size_limit", DiskStorageSizeLimit.Value);

            return node;
        }
    }
}
