﻿//
//  Error+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class Error
        {
            internal Error(JSONObject jsonNode)
            {
                Message = jsonNode.GetString("message");
                Detail = jsonNode.GetStringIfPresent("detail");
                Code = (ErrorCode)jsonNode.GetInteger("adapty_code");
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Error GetError(this JSONNode node, string aKey)
             => new Adapty.Error(GetObject(node, aKey));

        internal static Adapty.Error GetErrorIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new Adapty.Error(obj);
        }
    }
}