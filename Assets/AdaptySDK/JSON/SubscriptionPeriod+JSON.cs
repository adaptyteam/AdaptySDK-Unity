//
//  SubscriptionPeriod+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class SubscriptionPeriod
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                node.Add("unit", Unit.ToJSON());
                node.Add("number_of_units", NumberOfUnits);
                return node;
            }

            internal SubscriptionPeriod(JSONObject jsonNode)
            {
                Unit = jsonNode.GetPeriodUnit("unit");
                NumberOfUnits = jsonNode.GetInteger("number_of_units");
            }
        }
    }
}


namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.SubscriptionPeriod GetSubscriptionPeriod(this JSONNode node, string aKey)
             => new Adapty.SubscriptionPeriod(GetObject(node, aKey));

        internal static Adapty.SubscriptionPeriod GetSubscriptionPeriodIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.SubscriptionPeriod(obj);
        }
    }
}