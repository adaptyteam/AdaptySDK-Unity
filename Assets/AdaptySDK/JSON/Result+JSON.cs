//
//  Result+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using static AdaptySDK.Adapty;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static Adapty.Result<Adapty.Profile> ExtructProfileOrError(this string json)
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

        internal static Adapty.Result<Adapty.Paywall> ExtructPaywalleOrError(this string json)
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

        internal static Adapty.Result<IList<PaywallProduct>> ExtructPaywallProductListOrError(this string json)
        {
            Adapty.Error error = null;
            IList<PaywallProduct> list = null;
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

            return new Adapty.Result<IList<PaywallProduct>>(list, error);
        }
    }
}