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
        public GameObject OpenWebPaywallButtonPrefab;
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
        private List<ProductButton> m_productButtons = new List<ProductButton>(3);
        private List<GameObject> m_openWebPaywallButtons = new List<GameObject>(3);

        public void LoadPaywall(PlacementLoadStrategy loadStrategy, bool isDefaultAudience)
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

            var fetchPolicy = loadStrategy.ToFetchPolicy();

            Action<AdaptyPaywall, AdaptyError> onLoadPaywall = (paywall, error) =>
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
            };

            if (isDefaultAudience)
            {
                Adapty.GetPaywallForDefaultAudience(
                    this.PlacementId,
                    placementLocale,
                    fetchPolicy,
                    onLoadPaywall
                );
            }
            else
            {
                Adapty.GetPaywall(
                    this.PlacementId,
                    placementLocale,
                    fetchPolicy,
                    null,
                    onLoadPaywall
                );
            }
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
                    view.Present(
                        fullScreen
                            ? AdaptyUIIOSPresentationStyle.FullScreen
                            : AdaptyUIIOSPresentationStyle.PageSheet,
                        (error) => { }
                    );
                }
            );
        }

        public void OpenWebPaywallPressed()
        {
            if (this.m_paywall == null)
            {
                return;
            }

            Adapty.CreateWebPaywallUrl(
                this.m_paywall,
                (url, error) =>
                {
                    if (error != null)
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }
                    else
                    {
                        Debug.Log("CreateWebPaywallUrl: " + url);
                    }
                }
            );

            Adapty.OpenWebPaywall(
                this.m_paywall,
                (error) =>
                {
                    if (error != null)
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }
                }
            );
        }

        public void OpenWebPaywallProductPressed(AdaptyPaywallProduct product)
        {
            if (product == null)
            {
                return;
            }

            Adapty.CreateWebPaywallUrl(
                product,
                (url, error) =>
                {
                    if (error != null)
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }
                    else
                    {
                        Debug.Log("CreateWebPaywallUrl: " + url);
                    }
                }
            );

            Adapty.OpenWebPaywall(
                product,
                (error) =>
                {
                    if (error != null)
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }
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

        private void UpdateProductsData(IList<AdaptyPaywallProduct> products)
        {
            // Clear existing product buttons
            m_productButtons.ForEach(
                (button) =>
                {
                    if (button != null)
                    {
                        Destroy(button.gameObject);
                    }
                }
            );
            m_productButtons.Clear();
            m_openWebPaywallButtons.ForEach(
                (button) =>
                {
                    if (button != null)
                    {
                        Destroy(button);
                    }
                }
            );
            m_openWebPaywallButtons.Clear();

            // Create product buttons for each product
            for (var i = 0; i < products.Count; ++i)
            {
                var product = products[i];
                var productButton = this.CreateProductButton(product, i);
                m_productButtons.Add(productButton);

                var openWebPaywallButton = this.CreateOpenWebPaywallButton(product);
                m_openWebPaywallButtons.Add(openWebPaywallButton);
            }
        }

        private GameObject CreateOpenWebPaywallButton(AdaptyPaywallProduct product)
        {
            var openWebPaywallButtonObject = Instantiate(this.OpenWebPaywallButtonPrefab);
            var openWebPaywallButtonRect = openWebPaywallButtonObject.GetComponent<RectTransform>();
            openWebPaywallButtonRect.SetParent(this.ProductsContainerTransform, false);
            var openWebPaywallButton =
                openWebPaywallButtonObject.GetComponent<UnityEngine.UI.Button>();
            openWebPaywallButton.onClick.AddListener(() =>
            {
                this.OpenWebPaywallProductPressed(product);
            });

            return openWebPaywallButtonObject;
        }

        private ProductButton CreateProductButton(AdaptyPaywallProduct product, int index)
        {
            var productButtonObject = Instantiate(this.ProductButtonPrefab);
            var productButtonRect = productButtonObject.GetComponent<RectTransform>();

            productButtonRect.SetParent(this.ProductsContainerTransform, false);

            var productButton = productButtonObject.GetComponent<ProductButton>();
            productButton.UpdateProduct(product);

            var button = productButtonObject.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    this.Listener.Router.SetIsLoading(true);
                    this.Listener.MakePurchase(
                        product,
                        (error) =>
                        {
                            this.Listener.Router.SetIsLoading(false);
                        }
                    );
                });
            }

            return productButton;
        }
    }
}
