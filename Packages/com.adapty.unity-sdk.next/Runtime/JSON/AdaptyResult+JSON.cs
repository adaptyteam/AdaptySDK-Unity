//
//  AdaptyResult+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
         internal static AdaptyResult<T> GetAdaptyResult<T>(this string json, Func<JSONNode, T> map)
        {
            AdaptyError error;
            var value = default(T);
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetAdaptyErrorIfPresent("error");
                if (error is null)
                {
                    value = map(response.GetJSONNode("success"));
                } 
            }
            catch (Exception ex)
            {
                error = new AdaptyError(AdaptyErrorCode.DecodingFailed, "Failed decoding result ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new AdaptyResult<T>(value, error);
        }
    }
}