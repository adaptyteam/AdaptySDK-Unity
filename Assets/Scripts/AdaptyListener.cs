using System;
using System.Collections.Generic;
using System.Reflection;
using AdaptySDK;
using UnityEngine;
using UnityEngine.UI;
using static AdaptySDK.Adapty;

namespace AdaptyExample
{
    public class AdaptyListener : MonoBehaviour, AdaptyEventListener
    {
        AdaptyRouter Router;

        void Start()
        {
            this.Router = this.GetComponent<AdaptyRouter>();

            Adapty.SetLogLevel(AdaptyLogLevel.Verbose);
            Adapty.SetEventListener(this);
            this.SetFallBackPaywalls();
            this.GetProfile();
        }

        private void SetFallBackPaywalls()
        {
            this.LogMethodRequest("SetFallBackPaywalls");
            Adapty.SetFallbackPaywalls(AdaptyFallbackPaywalls.Value, (error) =>
            {
                this.LogMethodResult("SetFallBackPaywalls", error);
            });
        }

        public void GetProfile()
        {
            this.LogMethodRequest("GetProfile");

            Adapty.GetProfile((profile, error) =>
            {
                this.LogMethodResult("GetProfile", error);

                if (profile != null)
                {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void GetPaywall(string id, string locale, AdaptyPaywallFetchPolicy fetchPolicy, Action<AdaptyPaywall> completionHandler)
        {
            this.LogMethodRequest("GetPaywall");

            Adapty.GetPaywall(id, locale, fetchPolicy, new TimeSpan(0, 0, 4), (paywall, error) =>
            {
                this.LogMethodResult("GetPaywall", error);
                completionHandler.Invoke(paywall);
            });
        }

        public void GetPaywallProducts(AdaptyPaywall paywall, Action<IList<AdaptyPaywallProduct>> completionHandler)
        {
            this.LogMethodRequest("GetPaywallProducts");

            Adapty.GetPaywallProducts(paywall, (products, error) =>
            {
                this.LogMethodResult("GetPaywallProducts", error);
                completionHandler.Invoke(products);
            });

        }

        public void GetProductsIntroductoryOfferEligibility(IList<AdaptyPaywallProduct> products, Action<IDictionary<string, Eligibility>> completionHandler)
        {
            this.LogMethodRequest("GetProductsIntroductoryOfferEligibility");
            
            Adapty.GetProductsIntroductoryOfferEligibility(products, (eligibilities, error) =>
            {
                this.LogMethodResult("GetProductsIntroductoryOfferEligibility", error);
                completionHandler.Invoke(eligibilities);
            });
        }

        public void MakePurchase(AdaptyPaywallProduct product, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("MakePurchase");

            Adapty.MakePurchase(product, (profile, error) =>
            {
                this.LogMethodResult("MakePurchase", error);
                completionHandler.Invoke(error);

                if (profile != null)
                {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void RestorePurchases(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("RestorePurchases");

            Adapty.RestorePurchases((profile, error) =>
            {
                this.LogMethodResult("RestorePurchases", error);
                completionHandler.Invoke(error);

                if (profile != null)
                {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void Identify(string customerUserId, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("Identify");

            Adapty.Identify(customerUserId, (error) =>
            {
                this.LogMethodResult("Identify", error);
                completionHandler.Invoke(error);
            });
        }

        public void UpdateProfile(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("UpdateProfile");

            var builder = new Adapty.ProfileParameters.Builder()
                .SetFirstName("John")
                .SetLastName("Appleseed")
                .SetBirthday(new DateTime(1990, 5, 14))
                .SetGender(AdaptyProfileGender.Female)
                .SetEmail("example@adapty.io")
                .SetAirbridgeDeviceId("D7203965-6A2E-4F4C-A6E0-E3944EA9EAD1");

            builder = builder.SetAnalyticsDisabled(true);

            Debug.Log("#AdaptyListener# UpdateProfile Test [0]: no exception");
            try
            {
                builder = builder.SetCustomStringAttribute("string_key", "string_value");
                builder = builder.SetCustomStringAttribute("key_to_remove", "test");
                builder = builder.SetCustomDoubleAttribute("double_key", 123.0f);
                builder = builder.RemoveCustomAttribute("key_to_remove");
                Debug.Log("#AdaptyListener# UpdateProfile Test [0]: DONE");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: FAIL");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: value.length > 50");
                builder = builder.SetCustomStringAttribute("string_key", "01234567890123456789012345678901234567890123456789_");
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [1]: DONE");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [2]: key.length > 30");
                builder = builder.SetCustomStringAttribute("012345678901234567890123456789_1", "value");
                Debug.Log("#AdaptyListener# UpdateProfile Test [2]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [2]: DONE");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [3]: key wrong symbols");
                builder = builder.SetCustomStringAttribute("key{}``", "value");
                Debug.Log("#AdaptyListener# UpdateProfile Test [3]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [3]: DONE");
            }

            try
            {
                Debug.Log("#AdaptyListener# UpdateProfile Test [4]: attributes.count > 30");

                for (var i = 1; i <= 31; ++i)
                {
                    builder = builder.SetCustomStringAttribute(string.Format("key_{0}", i), string.Format("value_{0}", i));
                }

                Debug.Log("#AdaptyListener# UpdateProfile Test [4]: FAIL");
            }
            catch (Exception e)
            {
                Debug.Log(string.Format("#AdaptyListener# UpdateProfile Exception: {0}", e));
                Debug.Log("#AdaptyListener# UpdateProfile Test [4]: DONE");
            }

            Adapty.UpdateProfile(builder.Build(), (error) =>
            {
                this.LogMethodResult("UpdateProfile", error);
                completionHandler.Invoke(error);
            });
        }

        public void UpdateAttribution(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("UpdateAttribution");

            Adapty.UpdateAttribution("{\"test_key\": \"test_value\"}", AttributionSource.Custom, (error) =>
            {
                this.LogMethodResult("UpdateAttribution", error);
                completionHandler.Invoke(error);
            });
        }

        public void LogShowPaywall(AdaptyPaywall paywall, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("LogShowPaywall");

            Adapty.LogShowPaywall(paywall, (error) =>
            {
                this.LogMethodResult("LogShowPaywall", error);
                completionHandler.Invoke(error);
            });
        }

        public void LogShowOnboarding(int value, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("LogShowOnboarding");

            Adapty.LogShowOnboarding("test_onboarding", string.Format("test_screen_{0}", value), (uint)value, (error) =>
            {
                this.LogMethodResult("LogShowOnboarding", error);
                completionHandler.Invoke(error);
            });
        }

        public void PresentCodeRedemptionSheet()
        {
            Adapty.PresentCodeRedemptionSheet();
        }

        public void Logout(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("Logout");

            Adapty.Logout((error) =>
            {
                this.LogMethodResult("Logout", error);
                completionHandler.Invoke(error);
            });
        }

        // - Logging

        private void LogMethodRequest(string methodName)
        {
            Debug.Log(string.Format("#AdaptyListener# --> {0}", methodName));
        }

        private void LogMethodResult(string methodName, AdaptyError error)
        {
            if (error != null)
            {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} error {1}", methodName, error));

                this.Router.ShowAlertPanel(error.ToString());
            }
            else
            {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} success", methodName));
            }
        }

        // – AdaptyEventListener

        public void OnLoadLatestProfile(AdaptyProfile profile)
        {
            Debug.Log("#AdaptyListener# OnReceiveUpdatedProfile called");

            this.Router.SetProfile(profile);
        }
    }

}