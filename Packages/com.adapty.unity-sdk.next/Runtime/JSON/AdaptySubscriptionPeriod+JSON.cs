//
//  AdaptySubscriptionPeriod+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptySubscriptionPeriod
    {
        internal AdaptySubscriptionPeriod(JSONObject jsonNode)
        {
            Unit = jsonNode.GetAdaptySubscriptionPeriodUnit("unit");
            NumberOfUnits = jsonNode.GetInteger("number_of_units");
        }
    }

}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySubscriptionPeriod GetAdaptySubscriptionPeriod(this JSONNode node, string aKey)
             => new AdaptySubscriptionPeriod(GetObject(node, aKey));

        internal static AdaptySubscriptionPeriod GetAdaptySubscriptionPeriodIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptySubscriptionPeriod(obj);
        }
    }
}