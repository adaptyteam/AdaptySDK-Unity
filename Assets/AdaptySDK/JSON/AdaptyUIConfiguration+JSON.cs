//
//  AdaptyUIConfiguration+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIConfiguration
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            if (MemoryStorageTotalCostLimit.HasValue) node.Add("title", MemoryStorageTotalCostLimit.Value);
            if (MemoryStorageCountLimit.HasValue) node.Add("content", MemoryStorageCountLimit.Value);
            if (DiskStorageSizeLimit.HasValue) node.Add("content", DiskStorageSizeLimit.Value);

            return node;
        }
    }
}
