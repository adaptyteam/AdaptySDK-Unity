//
//  Price+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class Price
        {
            internal Price(JSONObject jsonNode)
            {
                Amount = jsonNode.GetDouble("amount");
                CurrencyCode = jsonNode.GetStringIfPresent("currency_code");
                CurrencySymbol = jsonNode.GetStringIfPresent("currency_symbol");
                LocalizedString = jsonNode.GetStringIfPresent("localized_string");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Price GetPrice(this JSONNode node, string aKey)
             => new Adapty.Price(GetObject(node, aKey));

        internal static Adapty.Price GetPriceIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.Price(obj);
        }
    }
}