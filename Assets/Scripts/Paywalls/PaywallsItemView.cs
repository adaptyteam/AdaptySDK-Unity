using System;
using System.Collections.Generic;
using AdaptySDK;
using TMPro;
using UnityEngine;

namespace AdaptyExample
{
    public class PaywallsItemView : MonoBehaviour
    {
        [HideInInspector]
        public AdaptyListener Listener;

        [HideInInspector]
        public string PlacementId;

        [HideInInspector]
        public string PlacementLocale;

        public GameObject ProductButtonPrefab;
        public RectTransform LoadingTransform;
        public RectTransform ProductsContainerTransform;

        public TextMeshProUGUI PlacementIdText;
        public TextMeshProUGUI RequestLocaleText;
        public TextMeshProUGUI StatusText;
        public RectTransform DetailsContainerTransform;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI AudienceNameText;
        public TextMeshProUGUI VariationIdText;
        public TextMeshProUGUI RevisionText;
        public TextMeshProUGUI RemoteConfigText;
        public TextMeshProUGUI ErrorText;

        void Start()
        {
            this.SetLoading(false);
            this.LoadPaywall();
        }

        void Update()
        {
            this.PlacementIdText.SetText(this.PlacementId);
            this.RequestLocaleText.SetText(
                string.IsNullOrEmpty(this.PlacementLocale) ? "null" : this.PlacementLocale
            );
        }

        void SetLoading(bool loading)
        {
            this.LoadingTransform.gameObject.SetActive(loading);
        }

        private AdaptyPaywall m_paywall;

        public void LoadPaywall()
        {
            if (string.IsNullOrEmpty(this.PlacementId))
            {
                this.UpdatePaywallError("PaywallId is empty");
                this.SetLoading(false);
                return;
            }

            var placementLocale = string.IsNullOrEmpty(this.PlacementLocale)
                ? null
                : this.PlacementLocale;

            this.SetLoading(true);

            Adapty.GetPaywall(
                this.PlacementId,
                placementLocale,
                AdaptyPlacementFetchPolicy.Default,
                null,
                (paywall, error) =>
                {
                    if (error != null)
                    {
                        this.UpdatePaywallError(error.Message);
                        this.SetLoading(false);
                    }
                    else
                    {
                        this.m_paywall = paywall;
                        this.UpdatePaywallData(paywall);
                        this.LoadProducts(paywall);
                    }
                }
            );
        }

        void LoadProducts(AdaptyPaywall paywall)
        {
            Adapty.GetPaywallProducts(
                paywall,
                (products, error) =>
                {
                    if (products != null)
                    {
                        this.UpdateProductsData(products);
                    }
                    else
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }

                    this.SetLoading(false);
                }
            );
        }

        public void LogShowPaywallPressed()
        {
            if (this.m_paywall == null)
            {
                return;
            }

            this.Listener.LogShowPaywall(this.m_paywall, (error) => { });
        }

        public void PresentPaywallPressed(bool fullScreen)
        {
            if (this.m_paywall == null)
            {
                return;
            }

            this.Listener.CreatePaywallView(
                this.m_paywall,
                true,
                (view) =>
                {
                    view.Present((error) => { });
                }
            );
        }

        private void UpdatePaywallData(AdaptyPaywall paywall)
        {
            this.StatusText.SetText("OK");
            this.StatusText.color = Color.green;

            this.DetailsContainerTransform.gameObject.SetActive(true);
            this.NameText.SetText(paywall.Name);
            this.AudienceNameText.SetText(paywall.Placement.AudienceName);
            this.VariationIdText.SetText(paywall.VariationId);
            this.RemoteConfigText.SetText(paywall.RemoteConfig?.Locale ?? "null");

            this.ErrorText.gameObject.SetActive(false);
        }

        private void UpdatePaywallError(string error)
        {
            this.StatusText.SetText("FAIL");
            this.StatusText.color = Color.red;

            this.DetailsContainerTransform.gameObject.SetActive(false);
            this.ErrorText.gameObject.SetActive(true);

            this.ErrorText.SetText("Error: " + error);
        }

        private void UpdateProductsData(IList<AdaptyPaywallProduct> products) { }
    }
}
