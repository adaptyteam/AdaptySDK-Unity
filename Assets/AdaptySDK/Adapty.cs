using System;
using System.Collections.Generic;
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
        public static readonly string sdkVersion = "2.2.2";

        public static void SetLogLevel(LogLevel level)
            => _Adapty.SetLogLevel(level.ToJSON());

        public static void Identify(string customerUserId, Action<Error> completionHandler) =>
            _Adapty.Identify(customerUserId, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtructErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.Identify(..)", e);
            }
        });

        public static void Logout(Action<Error> completionHandler)
            => _Adapty.Logout((json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtructErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.Logout(..)", e);
            }
        });

        public static void GetPaywall(string id, Action<Paywall, Error> completionHandler)
            => _Adapty.GetPaywall(id, (json) =>
        {
            if (completionHandler == null) return;
            var response = json.ExtructPaywalleOrError();
            try
            {
                completionHandler(response.Value, response.Error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Paywall,Adapty.Error> completionHandler in Adapty.GetPaywall(..)", e);
            }
        });

        public static void GetPaywallProducts(Paywall paywall, Action<IList<PaywallProduct>, Error> completionHandler)
            => GetPaywallProducts(paywall, IOSProductsFetchPolicy.Default, completionHandler);

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

            _Adapty.GetPaywallProducts(paywallJson, fetchPolicy.ToJSON(), (json) =>
            {
                if (completionHandler == null) return;
                var response = json.ExtructPaywallProductListOrError();
                try
                {
                    completionHandler(response.Value, response.Error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<IList<Adapty.PaywallProduct>,Adapty.Error> completionHandler in Adapty.GetPaywallProducts(..)", e);
                }
            });
        }

        public static void GetProfile(Action<Profile, Error> completionHandler)
            => _Adapty.GetProfile((json) =>
        {
            if (completionHandler == null) return;
            var response = json.ExtructProfileOrError();
            try
            {
                completionHandler(response.Value, response.Error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in Adapty.GetProfile(..)", e);
            }
        });

        public static void RestorePurchases(Action<Profile, Error> completionHandler)
            => _Adapty.RestorePurchases((json) =>
        {
            if (completionHandler == null) return;
            var response = json.ExtructProfileOrError();

            try
            {
                completionHandler(response.Value, response.Error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in Adapty.RestorePurchases(..)", e);
            }
        });

        public static void MakePurchase(PaywallProduct product, Action<Profile, Error> completionHandler)
            => MakePurchase(product, null, completionHandler);

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

            string androidSubscriptionUpdateJson;
            if (subscriptionUpdate is null)
            {
                androidSubscriptionUpdateJson = null;
            }
            else
            {
                try
                {
                    androidSubscriptionUpdateJson = subscriptionUpdate.ToJSONNode().ToString();
                }
                catch (Exception ex)
                {
                    androidSubscriptionUpdateJson = null;
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

            _Adapty.MakePurchase(productJson, androidSubscriptionUpdateJson, (json) =>
            {
                if (completionHandler == null) return;
                var response = json.ExtructProfileOrError();
                try
                {
                    completionHandler(response.Value, response.Error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in Adapty.MakePurchase(..)", e);
                }
            });

        }

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

            _Adapty.LogShowPaywall(paywallJson, (json) =>
            {
                if (completionHandler == null) return;
                var error = json.ExtructErrorIfPresent();
                try
                {
                    completionHandler(error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.LogShowPaywall(..)", e);
                }
            });
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

            _Adapty.LogShowOnboarding(parametersJson, (json) =>
            {
                if (completionHandler == null) return;
                var error = json.ExtructErrorIfPresent();
                try
                {
                    completionHandler(error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.LogShowOnboarding(..)", e);
                }
            });
        }

        public static void SetFallbackPaywalls(string paywalls, Action<Error> completionHandler)
            => _Adapty.SetFallbackPaywalls(paywalls, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtructErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.SetFallbackPaywalls(..)", e);
            }
        });

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

            _Adapty.UpdateProfile(parametersJson, (json) =>
            {
                if (completionHandler == null) return;
                var error = json.ExtructErrorIfPresent();
                try
                {
                    completionHandler(error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.UpdateProfile(..)", e);
                }
            });
        }

        public static void UpdateAttribution(string jsonstring, AttributionSource source, Action<Error> completionHandler)
            => UpdateAttribution(jsonstring, source, null, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, string networkUserId, Action<Error> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, networkUserId, completionHandler);
        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, Action<Error> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, null, completionHandler);

        public static void UpdateAttribution(string jsonstring, AttributionSource source, string networkUserId, Action<Error> completionHandler)
            => _Adapty.UpdateAttribution(jsonstring, source.ToJSON(), networkUserId, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtructErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.UpdateAttribution(..)", e);
            }
        });

        public static void SetVariationForTransaction(string variationId, string transactionId, Action<Error> completionHandler)
            => _Adapty.SetVariationForTransaction(variationId, transactionId, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtructErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.SetVariationForTransaction(..)", e);
            }
        });

        public static void PresentCodeRedemptionSheet()
            => _Adapty.PresentCodeRedemptionSheet();
    }
}