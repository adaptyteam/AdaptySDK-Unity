//
//  Error+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class Error
        {
            internal Error(JSONObject jsonNode)
            {
                Message = jsonNode.GetString("message");
#if UNITY_IOS
                Detail = jsonNode.GetString("detail");
#else
                Detail = jsonNode.GetStringIfPresent("detail");
#endif
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

        internal static Adapty.Error ExtractErrorIfPresent(this string json)
        {
            Adapty.Error error;
            try
            {
                error = JSONNode.Parse(json).GetErrorIfPresent("error");
            }
            catch (Exception ex)
            {
                error = new Adapty.Error(Adapty.ErrorCode.DecodingFailed, "Failed decoding Adapty.Error", $"AdaptyUnityError.DecodingFailed({ex})");
            }
            return error;
        }
    }
}