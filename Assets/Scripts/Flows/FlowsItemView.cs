using System;
using System.Collections;
using System.Collections.Generic;
using AdaptySDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AdaptyExample
{
    public class FlowsItemView : MonoBehaviour
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
        public TextMeshProUGUI StatusText;
        public RectTransform DetailsContainerTransform;
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI AudienceNameText;
        public TextMeshProUGUI VariationIdText;
        public TextMeshProUGUI RevisionText;
        public TextMeshProUGUI RemoteConfigText;
        public TextMeshProUGUI RequestLocaleText;
        public TextMeshProUGUI ErrorText;

        public Toggle Toggle;

        void Start()
        {
            this.SetLoading(false);
        }

        void Update()
        {
            this.PlacementIdText.SetText(this.PlacementId);
        }

        void SetLoading(bool loading)
        {
            this.LoadingTransform.gameObject.SetActive(loading);
        }

        private string RequestedLocale =>
            string.IsNullOrEmpty(this.PlacementLocale) ? "null" : this.PlacementLocale;

        private AdaptyFlow m_flow;
        private List<ProductButton> m_productButtons = new List<ProductButton>(3);
        private List<GameObject> m_openWebPaywallButtons = new List<GameObject>(3);

        public void LoadFlow(PlacementLoadStrategy loadStrategy, bool isDefaultAudience)
        {
            if (string.IsNullOrEmpty(this.PlacementId))
            {
                this.UpdateFlowError("PlacementId is empty");
                this.SetLoading(false);
                return;
            }

            this.SetLoading(true);

            var fetchPolicy = loadStrategy.ToFetchPolicy();

            Action<AdaptyFlow, AdaptyError> onLoadFlow = (flow, error) =>
            {
                if (error != null)
                {
                    this.UpdateFlowError(error.Message);
                    this.SetLoading(false);
                }
                else
                {
                    this.m_flow = flow;
                    StartCoroutine(DelayedUpdateFlow(flow));
                    this.LoadProducts(flow);
                }
            };

            if (isDefaultAudience)
            {
                Adapty.GetFlowForDefaultAudience(this.PlacementId, fetchPolicy, onLoadFlow);
            }
            else
            {
                Adapty.GetFlow(this.PlacementId, fetchPolicy, null, onLoadFlow);
            }
        }

        private IEnumerator DelayedUpdateFlow(AdaptyFlow flow)
        {
            yield return new WaitForEndOfFrame();
            this.UpdateFlowData(flow);
        }

        void LoadProducts(AdaptyFlow flow)
        {
            Adapty.GetPaywallProducts(
                flow,
                (products, error) =>
                {
                    if (products != null)
                    {
                        StartCoroutine(DelayedUpdateProducts(products));
                    }
                    else
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }

                    this.SetLoading(false);
                }
            );
        }

        private IEnumerator DelayedUpdateProducts(IReadOnlyList<AdaptyPaywallProduct> products)
        {
            yield return new WaitForEndOfFrame();
            this.UpdateProductsData(products);
        }

        public void LogShowFlowPressed()
        {
            if (this.m_flow == null)
            {
                return;
            }

            this.Listener.LogShowFlow(this.m_flow, (error) => { });
        }

        public void PresentFlowPressed(bool fullScreen)
        {
            if (this.m_flow == null)
            {
                return;
            }

            this.Listener.CreateFlowView(
                this.m_flow,
                true,
                this.PlacementLocale,
                (view) =>
                {
                    this.RequestLocaleText.SetText(
                        string.Format("{0} -> {1}", this.RequestedLocale, view.Locale ?? "null")
                    );

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
            if (this.m_flow == null || this.m_flow.Paywalls.Count == 0)
            {
                return;
            }

            var flowPaywall = this.m_flow.Paywalls[0];

            Adapty.CreateWebPaywallUrl(
                flowPaywall,
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
                flowPaywall,
                this.Toggle.isOn
                    ? AdaptyWebPresentation.InAppBrowser
                    : AdaptyWebPresentation.ExternalBrowser,
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
                this.Toggle.isOn
                    ? AdaptyWebPresentation.InAppBrowser
                    : AdaptyWebPresentation.ExternalBrowser,
                (error) =>
                {
                    if (error != null)
                    {
                        this.Listener.Router.ShowAlertPanel(error.ToString());
                    }
                }
            );
        }

        private void UpdateFlowData(AdaptyFlow flow)
        {
            this.StatusText.SetText("OK");
            this.StatusText.color = Color.green;

            this.DetailsContainerTransform.gameObject.SetActive(true);
            this.NameText.SetText(flow.Name);
            this.AudienceNameText.SetText(flow.Placement.AudienceName);
            this.VariationIdText.SetText(flow.VariationId);
            this.RemoteConfigText.SetText(flow.RemoteConfig?.Locale ?? "null");
            this.RequestLocaleText.SetText(this.RequestedLocale);

            this.ErrorText.gameObject.SetActive(false);
        }

        private void UpdateFlowError(string error)
        {
            this.StatusText.SetText("FAIL");
            this.StatusText.color = Color.red;

            this.DetailsContainerTransform.gameObject.SetActive(false);
            this.ErrorText.gameObject.SetActive(true);

            this.ErrorText.SetText("Error: " + error);
        }

        private void UpdateProductsData(IReadOnlyList<AdaptyPaywallProduct> products)
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
