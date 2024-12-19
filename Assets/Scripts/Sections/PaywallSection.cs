using System;
using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;
using static AdaptySDK.Adapty;

public class PaywallSection : MonoBehaviour
{
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public GameObject ProductButtonPrefab;

    public RectTransform ContainerTransform;
    public TextMeshProUGUI PaywallNameText;
    public TextMeshProUGUI LoadingStatusText;
    public TextMeshProUGUI VariationIdText;
    public TextMeshProUGUI RevisionText;
    public TextMeshProUGUI LocaleText;

    private string m_paywallId = "example_ab_test";
    private string m_localeId = "fr";

    private AdaptyPaywall m_paywall;

    private List<ProductButton> m_productButtons = new List<ProductButton>(3);

    void Start()
    {
        this.PaywallNameText.SetText(this.m_paywallId);
        this.LoadPaywall();
    }

    public void LogShowPaywallPressed()
    {
        if (m_paywall != null)
        {
            this.Router.SetIsLoading(true);
            this.Listener.LogShowPaywall(m_paywall, (error) =>
            {
                this.Router.SetIsLoading(false);
            });
        }
    }

    public void LoadPaywall()
    {
        this.Router.SetIsLoading(true);

        this.Listener.GetPaywall(this.m_paywallId, this.m_localeId, AdaptyPaywallFetchPolicy.ReloadRevalidatingCacheData, (paywall) =>
        {
            if (paywall == null)
            {
                this.UpdatePaywallFail();
                this.Router.SetIsLoading(false);
            }
            else
            {
                this.m_paywall = paywall;
                this.LoadProducts(paywall);
            }
        });
    }

    void LoadProducts(AdaptyPaywall paywall)
    {
        this.Listener.GetPaywallProducts(paywall, (products) =>
        {
            if (products != null)
            {
                this.UpdatePaywallData(paywall, products);
            }
            else
            {
                this.UpdatePaywallFail();
            }

            this.Router.SetIsLoading(false);
        });
    }

    public void PresentPaywall()
    {
        if (m_paywall == null) return;

        this.Listener.CreatePaywallView(this.m_paywall, preloadProducts: false, (view) =>
        {
            if (view == null)
            {

                //this.UpdateViewFail(paywall);
            }
            else
            {
                view.Present((error) => { });
            }
        });
    }

    private void UpdatePaywallFail()
    {
        this.LoadingStatusText.SetText("FAIL");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
    }

    private void UpdatePaywallData(AdaptyPaywall paywall, IList<AdaptyPaywallProduct> products)
    {
        this.LoadingStatusText.SetText("OK");
        this.VariationIdText.SetText(paywall.VariationId);
        this.RevisionText.SetText(paywall.Revision.ToString());
        this.LocaleText.SetText(paywall.Locale);

        m_productButtons.ForEach((button) =>
        {
            Destroy(button.gameObject);
        });
        m_productButtons.Clear();

        for (var i = 0; i < products.Count; ++i)
        {
            var product = products[i];
            var productButton = this.CreateProductButton(product, i);
            m_productButtons.Add(productButton);
        }

        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 610.0f + products.Count * 150.0f);
    }

    private ProductButton CreateProductButton(AdaptyPaywallProduct product, float index)
    {
        var productButtonObject = Instantiate(this.ProductButtonPrefab);
        var productButtonRect = productButtonObject.GetComponent<RectTransform>();

        productButtonRect.SetParent(this.ContainerTransform);
        productButtonRect.anchoredPosition = new Vector3(productButtonRect.position.x, -300.0f - 150.0f * index);
        productButtonRect.sizeDelta = new Vector2(this.ContainerTransform.sizeDelta.x - 40.0f, 140.0f);

        productButtonObject.GetComponent<ProductButton>().UpdateProduct(product);
        productButtonObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            this.Router.SetIsLoading(true);
            this.Listener.MakePurchase(product, (error) =>
            {
                this.Router.SetIsLoading(false);
            });
        });
        return productButtonObject.GetComponent<ProductButton>();
    }
}