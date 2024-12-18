//
//  AdaptyPurchaseResult+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPurchaseResult
    {
        internal AdaptyPurchaseResult(JSONObject jsonNode)
        {
            Type = jsonNode.GetAdaptyPurchaseResultType("type");
            Profile = jsonNode.GetAdaptyProfileIfPresent("profile");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPurchaseResult GetAdaptyPurchaseResult(this JSONNode node) =>
           new AdaptyPurchaseResult(GetObject(node));

        internal static AdaptyPurchaseResult GetAdaptyPurchaseResult(this JSONNode node, string aKey) =>
            new AdaptyPurchaseResult(GetObject(node, aKey));

        internal static AdaptyPurchaseResult GetAdaptyPrGetAdaptyPurchaseResultIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyPurchaseResult(obj);
        }
    }
}