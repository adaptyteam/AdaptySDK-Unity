//
//  Result+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Result<Adapty.Profile> ExtractProfileOrError(this string json)
        {
            Adapty.Error error = null;
            Adapty.Profile profile = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetErrorIfPresent("error");
                if (error is null)
                {
                    profile = response.GetProfile("success");
                }

            }
            catch (Exception ex)
            {
                error = new Adapty.Error(Adapty.ErrorCode.DecodingFailed, "Failed decoding Adapty.Profile Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<Adapty.Profile>(profile, error);
        }

        internal static Adapty.Result<Adapty.Paywall> ExtractPaywallOrError(this string json)
        {
            Adapty.Error error = null;
            Adapty.Paywall paywall = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetErrorIfPresent("error");
                if (error is null)
                {
                    paywall = response.GetPaywall("success");
                }
            }
            catch (Exception ex)
            {
                error = new Adapty.Error(Adapty.ErrorCode.DecodingFailed, "Failed decoding Adapty.Paywall Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<Adapty.Paywall>(paywall, error);
        }

        internal static Adapty.Result<IList<Adapty.PaywallProduct>> ExtractPaywallProductListOrError(this string json)
        {
            Adapty.Error error = null;
            IList<Adapty.PaywallProduct> list = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetErrorIfPresent("error");
                if (error is null)
                {
                    list = response.GetPaywallProductList("success");
                }
            }
            catch (Exception ex)
            {
                error = new Adapty.Error(Adapty.ErrorCode.DecodingFailed, "Failed decoding list of Adapty.PaywallProduct Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<IList<Adapty.PaywallProduct>>(list, error);
        }

        internal static Adapty.Result<IDictionary<string, Adapty.Eligibility>> ExtractProductEligibilityDictionaryOrError(this string json)
        {
            Adapty.Error error = null;
            IDictionary<string, Adapty.Eligibility> dic = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetErrorIfPresent("error");
                if (error is null)
                {
                    var obj = response.GetObject("success");
                    if (obj != null)
                    {
                        foreach (var item in obj)
                        {
                            JSONNode valueNode = item.Value;
                            if (!valueNode.IsString) throw new Exception($"Value by key: {item.Key} is not String");
                            dic.Add(item.Key, valueNode.Value.ToEligibility());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = new Adapty.Error(Adapty.ErrorCode.DecodingFailed, "Failed decoding dictionary of Adapty.Eligibility Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<IDictionary<string, Adapty.Eligibility>>(dic, error);
        }
    }
}