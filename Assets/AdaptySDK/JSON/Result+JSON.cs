//
//  Result+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Result<AdaptyProfile> ExtractProfileOrError(this string json)
        {
            AdaptyError error = null;
            AdaptyProfile profile = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetAdaptyErrorIfPresent("error");
                if (error is null)
                {
                    profile = response.GetAdaptyProfile("success");
                }

            }
            catch (Exception ex)
            {
                error = new AdaptyError(AdaptyErrorCode.DecodingFailed, "Failed decoding Adapty.Profile Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<AdaptyProfile>(profile, error);
        }

        internal static Adapty.Result<AdaptyPaywall> ExtractPaywallOrError(this string json)
        {
            AdaptyError error = null;
            AdaptyPaywall paywall = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetAdaptyErrorIfPresent("error");
                if (error is null)
                {
                    paywall = response.GetPaywall("success");
                }
            }
            catch (Exception ex)
            {
                error = new AdaptyError(AdaptyErrorCode.DecodingFailed, "Failed decoding Adapty.Paywall Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<AdaptyPaywall>(paywall, error);
        }

        internal static Adapty.Result<IList<AdaptyPaywallProduct>> ExtractPaywallProductListOrError(this string json)
        {
            AdaptyError error = null;
            IList<AdaptyPaywallProduct> list = null;
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetAdaptyErrorIfPresent("error");
                if (error is null)
                {
                    list = response.GetAdaptyPaywallProductList("success");
                }
            }
            catch (Exception ex)
            {
                error = new AdaptyError(AdaptyErrorCode.DecodingFailed, "Failed decoding list of AdaptyPaywallProduct Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<IList<AdaptyPaywallProduct>>(list, error);
        }

        internal static Adapty.Result<IDictionary<string, Adapty.Eligibility>> ExtractProductEligibilityDictionaryOrError(this string json)
        {
            AdaptyError error = null;
            var dic = new Dictionary<string, Adapty.Eligibility>();
            try
            {
                var response = JSONNode.Parse(json);
                error = response.GetAdaptyErrorIfPresent("error");

                if (error is null) {
                    var obj = response.GetObject("success");
                    if (obj != null) {
                        foreach (var item in obj) {
                            var key = item.Key;
                            var eligibility = obj.GetEligibility(key);
                            dic.Add(key, eligibility);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = new AdaptyError(AdaptyErrorCode.DecodingFailed, "Failed decoding dictionary of Adapty.Eligibility Or Adapty.Error ", $"AdaptyUnityError.DecodingFailed({ex})");
            }

            return new Adapty.Result<IDictionary<string, Adapty.Eligibility>>(dic, error);
        }
    }
}