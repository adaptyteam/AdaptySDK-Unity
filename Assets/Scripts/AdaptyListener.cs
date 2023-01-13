using System;
using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;
using UnityEngine.UI;
using static AdaptySDK.Adapty;

namespace AdaptyExample {
    public class AdaptyListener : MonoBehaviour, AdaptyEventListener {
        AdaptyRouter Router;

        void Start() {
            this.Router = this.GetComponent<AdaptyRouter>();

            Adapty.SetLogLevel(Adapty.LogLevel.Verbose);
            Adapty.SetEventListener(this);
            this.SetFallBackPaywalls();
        }

        private void SetFallBackPaywalls() {
            this.LogMethodRequest("SetFallBackPaywalls");
            Adapty.SetFallbackPaywalls(AdaptyFallbackPaywalls.Value, (error) => {
                this.LogMethodResult("SetFallBackPaywalls", error);
            });
        }

        public void GetProfile() {
            this.LogMethodRequest("GetProfile");

            Adapty.GetProfile((profile, error) => {
                this.LogMethodResult("GetProfile", error);

                if (profile != null) {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void GetPaywall(string id, Action<Adapty.Paywall> completionHandler) {
            this.LogMethodRequest("GetPaywall");

            Adapty.GetPaywall(id, (paywall, error) => {
                this.LogMethodResult("GetPaywall", error);
                completionHandler.Invoke(paywall);
            });
        }

        public void GetPaywallProducts(Adapty.Paywall paywall, Action<IList<PaywallProduct>> completionHandler) {
            this.LogMethodRequest("GetPaywallProducts");

            Adapty.GetPaywallProducts(paywall, (products, error) => {
                this.LogMethodResult("GetPaywallProducts", error);
                completionHandler.Invoke(products);
            });

        }

        public void MakePurchase(Adapty.PaywallProduct product, Action<Error> completionHandler) {
            this.LogMethodRequest("MakePurchase");

            Adapty.MakePurchase(product, null, (profile, error) => {
                this.LogMethodResult("MakePurchase", error);
                completionHandler.Invoke(error);

                if (profile != null) {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void RestorePurchases(Action<Error> completionHandler) {
            this.LogMethodRequest("RestorePurchases");

            Adapty.RestorePurchases((profile, error) => {
                this.LogMethodResult("RestorePurchases", error);
                completionHandler.Invoke(error);

                if (profile != null) {
                    this.Router.SetProfile(profile);
                }
            });
        }

        public void UpdateProfile(Action<Error> completionHandler) {
            this.LogMethodRequest("UpdateProfile");

            var builder = new Adapty.ProfileParameters.Builder();

            builder.SetFirstName("John")
                .SetLastName("Appleseed")
                .SetBirthday(new DateTime(1990, 5, 14))
                .SetGender(ProfileGender.Female)
                .SetEmail("example@adapty.io");

            Adapty.UpdateProfile(builder.Build(), (error) => {
                this.LogMethodResult("UpdateProfile", error);
                completionHandler.Invoke(error);
            });
        }

        public void UpdateAttribution(Action<Error> completionHandler) {
            this.LogMethodRequest("UpdateAttribution");

            Adapty.UpdateAttribution("{\"test_key\": \"test_value\"}", AttributionSource.Custom, (error) => {
                this.LogMethodResult("UpdateAttribution", error);
                completionHandler.Invoke(error);
            });
        }

        public void LogShowOnboarding(int value, Action<Error> completionHandler) {
            this.LogMethodRequest("LogShowOnboarding");

            Adapty.LogShowOnboarding("test_onboarding", string.Format("test_screen_{0}", value), (uint)value, (error) => {
                this.LogMethodResult("LogShowOnboarding", error);
                completionHandler.Invoke(error);
            });
        }

        public void PresentCodeRedemptionSheet() {
            Adapty.PresentCodeRedemptionSheet();
        }

        public void Logout(Action<Error> completionHandler) {
            this.LogMethodRequest("Logout");

            Adapty.Logout((error) => {
                this.LogMethodResult("Logout", error);
                completionHandler.Invoke(error);
            });
        }

        // - Logging

        private void LogMethodRequest(string methodName) {
            Debug.Log(string.Format("#AdaptyListener# --> {0}", methodName));
        }

        private void LogMethodResult(string methodName, Error error) {
            if (error != null) {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} error {1}", methodName, error));
            } else {
                Debug.Log(string.Format("#AdaptyListener# <-- {0} success", methodName));
            }
        }

        // – AdaptyEventListener

        public void OnLoadLatestProfile(Adapty.Profile profile) {
            Debug.Log("#AdaptyListener# OnReceiveUpdatedProfile called");

            this.Router.SetProfile(profile);
        }
    }

}