//
//  AdaptyError+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;
    public partial class AdaptyError
    {
        internal AdaptyError(JSONObject jsonNode)
        {
            Message = jsonNode.GetString("message");
            Detail = jsonNode.GetStringIfPresent("detail");
            Code = (AdaptyErrorCode)jsonNode.GetInteger("adapty_code");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyError GetAdaptyError(this JSONNode node, string aKey) =>
            new AdaptyError(GetObject(node, aKey));

        internal static AdaptyError GetAdaptyErrorIfPresent(this JSONNode node, string aKey)
        {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) return null;
            return new AdaptyError(obj);
        }

        internal static AdaptyError ExtractAdaptyErrorIfPresent(this string json)
        {
            AdaptyError error;
            try
            {
                error = JSONNode.Parse(json).GetAdaptyErrorIfPresent("error");
            }
            catch (Exception ex)
            {
                error = new AdaptyError(AdaptyErrorCode.DecodingFailed, "Failed decoding Adapty.Error", $"AdaptyUnityError.DecodingFailed({ex})");
            }
            return error;
        }
    }
}