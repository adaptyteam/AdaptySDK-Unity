using System;
using System.Collections.Generic;
using AdaptySDK.iOS;
using AdaptySDK.Android;
using AdaptySDK.Noop;
using AdaptySDK.SimpleJSON;
using static AdaptySDK.Adapty;

#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public static readonly string sdkVersion = "2.2.0";

        public static void SetLogLevel(LogLevel level)
            => _Adapty.SetLogLevel(level.ToJSON());

        public static void Identify(string customerUserId, Action<Error> completionHandler)
            => _Adapty.Identify(customerUserId, completionHandler);

        public static void Logout(Action<Error> completionHandler)
            => _Adapty.Logout(completionHandler);

        public static void GetPaywall(string id, Action<Paywall, Error> completionHandler)
            => _Adapty.GetPaywall(id, completionHandler);

        public static void GetPaywallProducts(Paywall paywall, IOSProductsFetchPolicy fetchPolicy, Action<IList<PaywallProduct>, Error> completionHandler)
        {
            string paywallJson;
            try
            {
                paywallJson = paywall.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new Error(ErrorCode.EncodingFailed, "Failed encoding Adapty.Paywall", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(null, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<IList<Adapty.PaywallProduct>,Adapty.Error> completionHandler in Adapty.GetPaywallProducts(..)", e);
                }
                return;
            }

            _Adapty.GetPaywallProducts(paywallJson, fetchPolicy.ToJSON(), completionHandler);
        }

        public static void GetPaywallProducts(Paywall paywall, Action<IList<PaywallProduct>, Error> completionHandler)
            => GetPaywallProducts(paywall, IOSProductsFetchPolicy.Default, completionHandler);

        public static void GetProfile(Action<Profile, Error> completionHandler)
            => _Adapty.GetProfile(completionHandler);

        public static void RestorePurchases(Action<Profile, Error> completionHandler)
            => _Adapty.RestorePurchases(completionHandler);

        public static void MakePurchase(PaywallProduct product, AndroidSubscriptionUpdateParameters subscriptionUpdate, Action<Profile, Error> completionHandler)
        {
            Error error = null;
            string productJson;
            try
            {
                productJson = product.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                productJson = null;
                error = new Error(ErrorCode.EncodingFailed, "Failed encoding Adapty.PaywallProduct", $"AdaptyUnityError.EncodingFailed({ex})");
            }

            string subscriptionUpdateJson;
            if (subscriptionUpdate is null)
            {
                subscriptionUpdateJson = null;
            }
            else
            {
                try
                {
                    subscriptionUpdateJson = subscriptionUpdate.ToJSONNode().ToString();
                }
                catch (Exception ex)
                {
                    subscriptionUpdateJson = null;
                    error = new Error(ErrorCode.EncodingFailed, "Failed encoding Adapty.AndroidSubscriptionUpdateParameters", $"AdaptyUnityError.EncodingFailed({ex})");
                }
            }

            if (error is not null)
            {
                try
                {
                    completionHandler(null, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Profile>,Adapty.Error> completionHandler in Adapty.MakePurchase(..)", e);
                }
                return;
            }

#if UNITY_IOS
            AdaptyIOS.MakePurchase(productJson, completionHandler);
#elif UNITY_ANDROID
            AdaptyAndroid.MakePurchase(productJson, subscriptionUpdateJson, completionHandler);
#else
            AdaptyNoop.MakePurchase(productJson, completionHandler);
#endif
        }
        public static void MakePurchase(PaywallProduct product, Action<Profile, Error> completionHandler)
            => MakePurchase(product, null, completionHandler);

        public static void LogShowPaywall(Paywall paywall, Action<Error> completionHandler)
        {
            string paywallJson;
            try
            {
                paywallJson = paywall.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new Error(ErrorCode.EncodingFailed, "Failed encoding Adapty.Paywall", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.LogShowPaywall(..)", e);
                }
                return;
            }

            _Adapty.LogShowPaywall(paywallJson, completionHandler);
        }

        public static void LogShowOnboarding(string name, string screenName, uint screenOrder, Action<Error> completionHandler)
        {
            string parametersJson;
            try
            {
                parametersJson = new OnboardingScreenParameters(name, screenName, screenOrder).ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new Error(ErrorCode.EncodingFailed, "Failed encoding Adapty.OnboardingScreenParameters", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.LogShowOnboarding(..)", e);
                }
                return;
            }

            _Adapty.LogShowOnboarding(parametersJson, completionHandler);
        }

        public static void SetFallbackPaywalls(string paywalls, Action<Error> completionHandler)
            => _Adapty.SetFallbackPaywalls(paywalls, completionHandler);

        public static void UpdateProfile(ProfileParameters param, Action<Error> completionHandler)
        {
            string parametersJson;
            try
            {
                parametersJson = param.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new Error(ErrorCode.EncodingFailed, "Failed encoding Adapty.ProfileParameters", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.UpdateProfile(..)", e);
                }
                return;
            }

            _Adapty.UpdateProfile(parametersJson, completionHandler);
        }

        public static void UpdateAttribution(string jsonstring, AttributionSource source, string networkUserId, Action<Error> completionHandler)
            => _Adapty.UpdateAttribution(jsonstring, source.ToJSON(), networkUserId, completionHandler);

        public static void UpdateAttribution(string jsonstring, AttributionSource source, Action<Error> completionHandler)
            => UpdateAttribution(jsonstring, source, null, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, string networkUserId, Action<Error> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, networkUserId, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, Action<Error> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, null, completionHandler);



        public static void SetVariationForTransaction(string variationId, string transactionId, Action<Error> completionHandler)
            => _Adapty.SetVariationForTransaction(variationId, transactionId, completionHandler);


#if UNITY_IOS
        public static void PresentCodeRedemptionSheet()
            => AdaptyIOS.PresentCodeRedemptionSheet();
#endif
    }
}