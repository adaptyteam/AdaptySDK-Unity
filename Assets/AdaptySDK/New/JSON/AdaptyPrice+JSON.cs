//
//  AdaptyPrice+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPrice
    {
        internal AdaptyPrice(JSONObject jsonNode)
        {
            Amount = jsonNode.GetDouble("amount");
            CurrencyCode = jsonNode.GetStringIfPresent("currency_code");
            CurrencySymbol = jsonNode.GetStringIfPresent("currency_symbol");
            LocalizedString = jsonNode.GetStringIfPresent("localized_string");
        }

    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPrice GetAdaptyPrice(this JSONNode node, string aKey) => 
            new AdaptyPrice(GetObject(node, aKey));

        internal static AdaptyPrice GetAdaptyPriceIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyPrice(obj);
        }
    }
}