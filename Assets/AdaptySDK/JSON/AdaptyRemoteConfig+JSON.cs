//
//  AdaptyRemoteConfig+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Goncharov on 09.09.2025.

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyRemoteConfig
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("lang", Locale);
            node.Add("data", Data);
            return node;
        }

        internal AdaptyRemoteConfig(JSONObject jsonNode)
        {
            Locale = jsonNode.GetString("lang");
            Data = jsonNode.GetString("data");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyRemoteConfig GetRemoteConfig(this JSONNode node) =>
            new AdaptyRemoteConfig(GetObject(node));

        internal static AdaptyRemoteConfig GetRemoteConfig(this JSONNode node, string aKey) =>
            new AdaptyRemoteConfig(GetObject(node, aKey));

        internal static AdaptyRemoteConfig GetRemoteConfigIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null)
                return null;
            return new AdaptyRemoteConfig(obj);
        }
    }
}
