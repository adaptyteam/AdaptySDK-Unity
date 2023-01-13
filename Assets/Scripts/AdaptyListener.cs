using System;
using System.Collections.Generic;
using AdaptySDK;
using UnityEngine;
using UnityEngine.UI;
using static AdaptySDK.Adapty;

namespace AdaptyExample
{
    public class AdaptyListener : MonoBehaviour, AdaptyEventListener
    {

        AdaptyRouter Router;

        public void Log(string message)
        {
            Debug.Log("#AdaptyListener# " + message);
        }

        void Start()
        {
            this.Router = this.GetComponent<AdaptyRouter>();

            Adapty.SetLogLevel(Adapty.LogLevel.Debug);
            Adapty.SetEventListener(this);
        }

        public void Log(string message, bool clearLog = false)
        {
            Debug.Log($"#AdaptyListener# {message}");
        }

        public void GetProfile()
        {
            this.Log("--> GetPurchaserInfo");
            Adapty.GetProfile((profile, error) =>
            {
                this.Router.UpdateProfile(profile);

                if (profile != null)
                {
                    this.Log("<-- GetPurchaserInfo success");
                }
                else
                {
                    this.Log("<-- GetPurchaserInfo error");
                }
            });
        }

        public void GetPaywall(string id, Action<Adapty.Paywall> completionHandler)
        {
            this.Log("--> GetPaywall");

            this.Router.SetIsLoading(true);

            Adapty.GetPaywall(id, (paywall, error) =>
            {
                this.Router.SetIsLoading(false);

                if (error != null)
                {
                    this.Log("<-- GetPaywall error");
                }
                else if (paywall != null)
                {
                    this.Log("<-- GetPaywall success");

                    completionHandler.Invoke(paywall);
                }

                completionHandler.Invoke(null);
            });
        }

        public void GetPaywallProducts(Adapty.Paywall paywall, Action<IList<PaywallProduct>> completionHandler)
        {

            this.Log("--> GetPaywallProducts");

            Adapty.GetPaywallProducts(paywall, (products, error) =>
            {


                if (error != null)
                {
                    this.Log("<-- GetPaywallProducts error");
                }
                else if (products != null)
                {
                    this.Log("<-- GetPaywallProducts success");
                    completionHandler.Invoke(products);
                }
            });

        }

        public void MakePurchase(Adapty.PaywallProduct product)
        {
            this.Log("--> MakePurchase");

            Adapty.MakePurchase(product, null, (response, error) =>
            {
                if (error != null)
                {
                    this.Log("<-- MakePurchase error");
                }
                else if (response != null)
                {
                    this.Log("<-- MakePurchase success");
                }
            });
        }


        // – AdaptyEventListener

        public void OnLoadLatestProfile(Adapty.Profile profile)
        {
            Debug.Log("#AdaptyListener# OnReceiveUpdatedProfile called");

            this.Router.UpdateProfile(profile);
        }
    }

}