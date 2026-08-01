//
//  AdaptySubscriptionOffer+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//
using System.Collections.Generic;
using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptySubscriptionOffer
    {
        internal AdaptySubscriptionOffer(JSONObject jsonNode)
        {
            var subNode = jsonNode.GetObject("offer_identifier");
            Identifier = subNode.GetStringIfPresent("id");
            Type = subNode.GetAdaptySubscriptionOfferType("type");
            Phases = jsonNode.GetAdaptySubscriptionPhaseListIfPresent("phases");
#if UNITY_ANDROID
            OfferTags = jsonNode.GetStringList("offer_tags");
#else
            OfferTags = null;
#endif
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySubscriptionOffer GetAdaptySubscriptionOffer(this JSONNode node, string aKey)
             => new AdaptySubscriptionOffer(GetObject(node, aKey));

        internal static AdaptySubscriptionOffer GetAdaptySubscriptionOfferIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptySubscriptionOffer(obj);
        }

        internal static IList<string> GetStringList(this JSONNode node, string aKey)
        {
            var array = GetArrayIfPresent(node, aKey);
            if (array is null) return null;
            var result = new List<string>();
            foreach (var item in array.Children)
            {
                if (!item.IsString) throw new Exception($"Value by index: {result.Count} is not String");
                result.Add(item.Value);
            }
            return result;
        }
    }
}