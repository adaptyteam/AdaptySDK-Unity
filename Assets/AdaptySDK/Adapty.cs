using System;
using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using _Adapty = AdaptySDK.iOS.AdaptyIOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _Adapty = AdaptySDK.Android.AdaptyAndroid;
#else
using _Adapty = AdaptySDK.Noop.AdaptyNoop;
#endif

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;
    using static AdaptySDK.Adapty;

    public static partial class Adapty
    {
        public static readonly string sdkVersion = "2.9.0";

        public static void SetLogLevel(AdaptyLogLevel level)
            => _Adapty.SetLogLevel(level.ToJSON());

        public static void Identify(string customerUserId, Action<AdaptyError> completionHandler) =>
            _Adapty.Identify(customerUserId, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtractAdaptyErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in AdaptyIOS.Identify(..)", e);
            }
        });

        public static void Logout(Action<AdaptyError> completionHandler)
            => _Adapty.Logout((json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtractAdaptyErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.Logout(..)", e);
            }
        });

        public static void GetPaywall(string placementId, Action<AdaptyPaywall, AdaptyError> completionHandler)
            => GetPaywall(placementId, null, null, null, completionHandler);

        public static void GetPaywall(string placementId, AdaptyPaywallFetchPolicy fetchPolicy, TimeSpan? loadTimeout, Action<AdaptyPaywall, AdaptyError> completionHandler)
          => GetPaywall(placementId, null, fetchPolicy, loadTimeout, completionHandler);

        public static void GetPaywall(string placementId, string locale, Action<AdaptyPaywall, AdaptyError> completionHandler)
            => GetPaywall(placementId, locale, null, null, completionHandler);

        public static void GetPaywall(string placementId, string locale, AdaptyPaywallFetchPolicy fetchPolicy, TimeSpan? loadTimeout, Action<AdaptyPaywall, AdaptyError> completionHandler)
        {
            string fetchPolicyJson;
            try
            {
                fetchPolicyJson = fetchPolicy?.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Adapty.PaywallFetchPolicy", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(null, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<Adapty.Paywall,Adapty.Error> completionHandler in Adapty.GetPaywall(..)", e);
                }
                return;
            }

            Int64? timeoutInMilliseconds = loadTimeout.HasValue ? (Int64)loadTimeout.Value.TotalMilliseconds : null;
            _Adapty.GetPaywall(placementId, locale, fetchPolicyJson, timeoutInMilliseconds, (json) =>
                {
                    if (completionHandler == null) return;
                    var response = json.ExtractPaywallOrError();
                    try
                    {
                        completionHandler(response.Value, response.Error);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Failed to invoke Action<Adapty.Paywall,Adapty.Error> completionHandler in Adapty.GetPaywall(..)", e);
                    }
                });
        }

        public static void GetPaywallProducts(AdaptyPaywall paywall, Action<IList<AdaptyPaywallProduct>, AdaptyError> completionHandler)
        {
            string paywallJson;

            try
            {
                paywallJson = paywall.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Adapty.Paywall", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(null, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<IList<AdaptyPaywallProduct>,Adapty.Error> completionHandler in Adapty.GetPaywallProducts(..)", e);
                }
                return;
            }

            _Adapty.GetPaywallProducts(paywallJson, (json) =>
            {
                if (completionHandler == null) return;
                var response = json.ExtractPaywallProductListOrError();
                try
                {
                    completionHandler(response.Value, response.Error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<IList<AdaptyPaywallProduct>,Adapty.Error> completionHandler in Adapty.GetPaywallProducts(..)", e);
                }
            });
        }

        public static void GetProductsIntroductoryOfferEligibility(IList<AdaptyPaywallProduct> products, Action<IDictionary<string, Eligibility>, AdaptyError> completionHandler)
        {
#if UNITY_ANDROID
            var result = new Dictionary<string, Eligibility>();
            foreach (var product in products)
            {
                result.Add(product.VendorProductId, product.SubscriptionDetails?.AndroidIntroductoryOfferEligibility ?? Eligibility.Ineligible);
            }
            completionHandler(result, null);
            return;
#endif
            string productsJson;
            try
            {
                var array = new JSONArray();
                foreach (var product in products)
                {
                    array.Add(product.VendorProductId);
                }

                productsJson = array.ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Array of VendorProductId", $"AdaptyUnityError.EncodingFailed({ex})");
                try
                {
                    completionHandler(null, error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<IDictionary<string, Adapty.Eligibility>,Adapty.Error> completionHandler in Adapty.GetProductsIntroductoryOfferEligibility(..)", e);
                }
                return;
            }

            _Adapty.GetProductsIntroductoryOfferEligibility(productsJson, (json) =>
            {
                if (completionHandler == null) return;
                var response = json.ExtractProductEligibilityDictionaryOrError();
                try
                {
                    completionHandler(response.Value, response.Error);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to invoke Action<IDictionary<string, AdaptyPaywallProduct>,Adapty.Error> completionHandler in Adapty.GetProductsIntroductoryOfferEligibility(..)", e);
                }
            });
        }


        public static void GetProfile(Action<AdaptyProfile, AdaptyError> completionHandler)
            => _Adapty.GetProfile((json) =>
        {
            if (completionHandler == null) return;
            var response = json.ExtractProfileOrError();
            try
            {
                completionHandler(response.Value, response.Error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in Adapty.GetProfile(..)", e);
            }
        });

        public static void RestorePurchases(Action<AdaptyProfile, AdaptyError> completionHandler)
            => _Adapty.RestorePurchases((json) =>
        {
            if (completionHandler == null) return;
            var response = json.ExtractProfileOrError();

            try
            {
                completionHandler(response.Value, response.Error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Profile,Adapty.Error> completionHandler in Adapty.RestorePurchases(..)", e);
            }
        });

        public static void MakePurchase(AdaptyPaywallProduct product, Action<AdaptyProfile, AdaptyError> completionHandler)
            => MakePurchase(product, null, null, completionHandler);

        public static void MakePurchase(AdaptyPaywallProduct product, AndroidSubscriptionUpdateParameters subscriptionUpdate, Action<AdaptyProfile, AdaptyError> completionHandler)
            => MakePurchase(product, subscriptionUpdate, null, completionHandler);

        public static void MakePurchase(AdaptyPaywallProduct product, AndroidSubscriptionUpdateParameters subscriptionUpdate, bool? isOfferPersonalized, Action<AdaptyProfile, AdaptyError> completionHandler)
        {
            AdaptyError error = null;
            string productJson;
            try
            {
                productJson = product.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                productJson = null;
                error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding AdaptyPaywallProduct", $"AdaptyUnityError.EncodingFailed({ex})");
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
                    error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Adapty.AndroidSubscriptionUpdateParameters", $"AdaptyUnityError.EncodingFailed({ex})");
                }
            }

            if (error != null)
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

            _Adapty.MakePurchase(productJson, androidSubscriptionUpdateJson, isOfferPersonalized, (json) =>
            {
                if (completionHandler == null) return;
                var response = json.ExtractProfileOrError();
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

        public static void LogShowPaywall(AdaptyPaywall paywall, Action<AdaptyError> completionHandler)
        {
            string paywallJson;
            try
            {
                paywallJson = paywall.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Adapty.Paywall", $"AdaptyUnityError.EncodingFailed({ex})");
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
                var error = json.ExtractAdaptyErrorIfPresent();
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

        public static void LogShowOnboarding(string name, string screenName, uint screenOrder, Action<AdaptyError> completionHandler)
        {
            string parametersJson;
            try
            {
                parametersJson = new AdaptyOnboardingScreenParameters(name, screenName, screenOrder).ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Adapty.OnboardingScreenParameters", $"AdaptyUnityError.EncodingFailed({ex})");
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
                var error = json.ExtractAdaptyErrorIfPresent();
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

        public static void SetFallbackPaywalls(string paywalls, Action<AdaptyError> completionHandler)
            => _Adapty.SetFallbackPaywalls(paywalls, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtractAdaptyErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.SetFallbackPaywalls(..)", e);
            }
        });

        public static void UpdateProfile(ProfileParameters param, Action<AdaptyError> completionHandler)
        {
            string parametersJson;
            try
            {
                parametersJson = param.ToJSONNode().ToString();
            }
            catch (Exception ex)
            {
                var error = new AdaptyError(AdaptyErrorCode.EncodingFailed, "Failed encoding Adapty.ProfileParameters", $"AdaptyUnityError.EncodingFailed({ex})");
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
                var error = json.ExtractAdaptyErrorIfPresent();
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

        public static void UpdateAttribution(string jsonString, AttributionSource source, Action<AdaptyError> completionHandler)
            => UpdateAttribution(jsonString, source, null, completionHandler);

        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, string networkUserId, Action<AdaptyError> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, networkUserId, completionHandler);

        public static void UpdateAttribution(Dictionary<string, dynamic> attribution, AttributionSource source, Action<AdaptyError> completionHandler)
            => UpdateAttribution(attribution.ToJSONObject().ToString(), source, null, completionHandler);

        public static void UpdateAttribution(string jsonString, AttributionSource source, string networkUserId, Action<AdaptyError> completionHandler)
            => _Adapty.UpdateAttribution(jsonString, source.ToJSON(), networkUserId, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtractAdaptyErrorIfPresent();
            try
            {
                completionHandler(error);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to invoke Action<Adapty.Error> completionHandler in Adapty.UpdateAttribution(..)", e);
            }
        });

        public static void SetVariationForTransaction(string variationId, string transactionId, Action<AdaptyError> completionHandler)
            => _Adapty.SetVariationForTransaction(variationId, transactionId, (json) =>
        {
            if (completionHandler == null) return;
            var error = json.ExtractAdaptyErrorIfPresent();
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