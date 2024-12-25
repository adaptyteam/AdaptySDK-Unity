using System;
using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;

namespace AdaptyExample
{
    public class AdaptyListener : MonoBehaviour, AdaptyEventListener
    {
        public event Action OnInitializeFinished;
        AdaptyRouter Router;

        void Start()
        {
            this.Router = this.GetComponent<AdaptyRouter>();

            this.InitializeAdapty();
            this.SetFallBackPaywalls();
        }

        private void InitializeAdapty()
        {
            Adapty.SetEventListener(this);

            this.LogMethodRequest("SetLogLevel");

            Adapty.SetLogLevel(AdaptyLogLevel.Verbose, (error) =>
            {
                this.LogMethodResult("SetLogLevel", error);
            });


            var builder = new AdaptyConfiguration.Builder("public_live_iNuUlSsN.83zcTTR8D5Y8FI9cGUI6")
                    .SetCustomerUserId(null)
                    .SetObserverMode(false)
                    .SetServerCluster(AdaptyServerCluster.Default)
                    .SetIPAddressCollectionDisabled(false)
                    .SetIDFACollectionDisabled(false)
                    .SetActivateUI(true)
                    .SetAdaptyUIMediaCache(
                        100 * 1024 * 1024, // 100MB
                        null,
                        100 * 1024 * 1024 // 100MB
                    );

            this.LogMethodRequest("Activate");

            Adapty.Activate(builder.Build(), (error) =>
            {
                this.LogMethodResult("Activate", error);
                this.OnInitializeFinished?.Invoke();
                this.GetProfile();
            });
        }

        private void SetFallBackPaywalls()
        {
#if UNITY_IOS
            var assetId = "adapty_fallback_ios.json";
#elif UNITY_ANDROID
            var assetId = "adapty_fallback_android.json";
#else
            var assetId = "";
#endif

            this.LogMethodRequest("SetFallBackPaywalls");
            Adapty.SetFallbackPaywalls(assetId, (error) =>
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

        public void GetPaywallForDefaultAudience(string id, string locale, AdaptyPaywallFetchPolicy fetchPolicy, Action<AdaptyPaywall> completionHandler)
        {
            this.LogMethodRequest("GetPaywallForDefaultAudience");

            Adapty.GetPaywallForDefaultAudience(id, locale, fetchPolicy, (paywall, error) =>
            {
                this.LogMethodResult("GetPaywallForDefaultAudience", error);
                completionHandler.Invoke(paywall);
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

        public void MakePurchase(AdaptyPaywallProduct product, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("MakePurchase");

            Adapty.MakePurchase(product, (result, error) =>
            {
                this.LogMethodResult("MakePurchase", error);
                completionHandler.Invoke(error);

                switch (result.Type)
                {

                    case AdaptyPurchaseResultType.Pending:
                        // handle pending
                        break;
                    case AdaptyPurchaseResultType.UserCancelled:
                        // handle cancelation
                        break;
                    case AdaptyPurchaseResultType.Success:
                        var profile = result.Profile;
                        this.Router.SetProfile(profile);
                        break;
                    default:
                        break;
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

            var builder = new AdaptyProfileParameters.Builder()
                .SetFirstName("John")
                .SetLastName("Appleseed")
                .SetBirthday(new DateTime(1990, 5, 14))
                .SetGender(AdaptyProfileGender.Female)
                .SetEmail("example@adapty.io");

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

        public void SetIntegrationIdentifier(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("SetIntegrationIdentifier");

            Adapty.SetIntegrationIdentifier("test_integration", "test_id", (error) =>
            {
                this.LogMethodResult("SetIntegrationIdentifier", error);
                completionHandler.Invoke(error);
            });
        }

        public void ReportTransaction(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("ReportTransaction");

            Adapty.ReportTransaction("transaction_id", "variation_id", (error) =>
            {
                this.LogMethodResult("ReportTransaction", error);
                completionHandler.Invoke(error);
            });
        }

        public void UpdateAttribution(Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("UpdateAttribution");

            Adapty.UpdateAttribution("{\"test_key\": \"test_value\"}", "custom", (error) =>
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
            this.LogMethodRequest("PresentCodeRedemptionSheet");

            Adapty.PresentCodeRedemptionSheet((error) =>
            {
                this.LogMethodResult("PresentCodeRedemptionSheet", error);
            });
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

        private void LogIncomingCall_AdaptyUI(string methodName, AdaptyUIView view, string meta)
        {
            Debug.Log(string.Format("#AdaptyListener# <-- {0}, viewId = {1}, meta = {2}", methodName, view.Id, meta));
        }

        // – AdaptyEventListener

        public void OnLoadLatestProfile(AdaptyProfile profile)
        {
            Debug.Log("#AdaptyListener# OnReceiveUpdatedProfile called");

            this.Router.SetProfile(profile);
        }

        // AdaptyUI

        public void CreatePaywallView(AdaptyPaywall paywall, bool preloadProducts, Action<AdaptyUIView> completionHandler)
        {
            this.LogMethodRequest("CreatePaywallView");

            var parameters = new AdaptyUICreateViewParameters()
                .SetPreloadProducts(preloadProducts)
                .SetCustomTags(
                    new Dictionary<string, string> {
                        { "CUSTOM_TAG_NAME", "Walter White" },
                        { "CUSTOM_TAG_PHONE", "+1 234 567890" },
                        { "CUSTOM_TAG_CITY", "Albuquerque" },
                        { "CUSTOM_TAG_EMAIL", "walter@white.com" }
                    }
                )
                .SetCustomTimers(
                    new Dictionary<string, DateTime> { 
                        { "CUSTOM_TIMER_24H", DateTime.Now.AddSeconds(86400) }, 
                        { "CUSTOM_TIMER_10H", DateTime.Now.AddSeconds(36000) }, 
                        { "CUSTOM_TIMER_1H", DateTime.Now.AddSeconds(3600) }, 
                        { "CUSTOM_TIMER_10M", DateTime.Now.AddSeconds(600) }, 
                        { "CUSTOM_TIMER_1M", DateTime.Now.AddSeconds(60) }, 
                        { "CUSTOM_TIMER_10S", DateTime.Now.AddSeconds(10) }, 
                        { "CUSTOM_TIMER_5S", DateTime.Now.AddSeconds(5) } 
                    }
                )
                .SetLoadTimeout(new TimeSpan(0, 0, 3));

            AdaptyUI.CreateView(paywall, parameters, (view, error) =>
            {
                this.LogMethodResult("CreatePaywallView", error);
                completionHandler.Invoke(view);
            });
        }

        public void PresentPaywallView(AdaptyUIView view, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("PresentPaywallView");

            AdaptyUI.PresentView(view, (error) =>
            {
                this.LogMethodResult("PresentPaywallView", error);

                if (completionHandler != null)
                {
                    completionHandler.Invoke(error);
                }
            });
        }

        public void DismissPaywallView(AdaptyUIView view, Action<AdaptyError> completionHandler)
        {
            this.LogMethodRequest("DismissPaywallView");

            AdaptyUI.DismissView(view, (error) =>
            {
                this.LogMethodResult("DismissPaywallView", error);

                if (completionHandler != null)
                {
                    completionHandler.Invoke(error);
                }
            });
        }

        // - AdaptyUIEventListener

        public void PaywallViewDidPerformAction(AdaptyUIView view, AdaptyUIUserAction action)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidPerformAction", view, action.Type.ToString());

            switch (action.Type)
            {
                case AdaptyUIUserActionType.Close:
                    view.Dismiss(null);
                    break;
                case AdaptyUIUserActionType.OpenUrl:
                    break;
                default:
                    break;
            }
        }

        public void PaywallViewDidSelectProduct(AdaptyUIView view, string productId)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidSelectProduct", view, productId);
        }

        public void PaywallViewDidStartPurchase(AdaptyUIView view, AdaptyPaywallProduct product)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidStartPurchase", view, product.VendorProductId);
        }

        public void PaywallViewDidFinishPurchase(AdaptyUIView view, AdaptyPaywallProduct product, AdaptyPurchaseResult purchasedResult)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidFinishPurchase", view, product.VendorProductId);

            switch (purchasedResult.Type)
            {

                case AdaptyPurchaseResultType.UserCancelled:
                    // handle user canceled
                    break;
                case AdaptyPurchaseResultType.Pending:
                    // handle pending purchase
                    break;
                case AdaptyPurchaseResultType.Success:
                    var profile = purchasedResult.Profile;
                    var accessLevel = profile.AccessLevels["premium"];
                    if (accessLevel != null && accessLevel.IsActive)
                    {
                        this.DismissPaywallView(view, null);
                    }
                    break;
                default:
                    break;
            }
        }

        public void PaywallViewDidFailPurchase(AdaptyUIView view, AdaptyPaywallProduct product, AdaptyError error)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidFailPurchase", view, string.Format("id: {0}, error: {1}", product.VendorProductId, error.ToString()));
        }

        public void PaywallViewDidStartRestore(AdaptyUIView view)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidStartRestore", view, null);
        }

        public void PaywallViewDidFinishRestore(AdaptyUIView view, AdaptyProfile profile)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidFinishRestore", view, profile.ProfileId);

            var dialog = new AdaptyUIDialogConfiguration()
                .SetContent("Success!")
                .SetContent("Purchases were successfully restored.")
                .SetDefaultActionTitle("OK");


            AdaptyUI.ShowDialog(view, dialog, (action, error) =>
            {

            });
        }

        public void PaywallViewDidFailRestore(AdaptyUIView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUI("aywallViewDidFailRestore", view, error.ToString());
        }

        public void PaywallViewDidFailRendering(AdaptyUIView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidFailRendering", view, error.ToString());
        }

        public void PaywallViewDidFailLoadingProducts(AdaptyUIView view, AdaptyError error)
        {
            LogIncomingCall_AdaptyUI("PaywallViewDidFailLoadingProducts", view, error.ToString());
        }
    }

}