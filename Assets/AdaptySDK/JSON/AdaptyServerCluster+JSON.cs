//
//  AdaptyServerCluster+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 10.12.2024.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static JSONNode ToJSONNode(this AdaptyServerCluster value) =>
            value switch
            {
                AdaptyServerCluster.EU => "eu",
                _ => null
            };
    }
}