using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;

public class CustomPaywallSection : MonoBehaviour {
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public GameObject ProductButtonPrefab;

    public RectTransform ContainerTransform;
    public TextMeshProUGUI PaywallNameText;
    public TextMeshProUGUI LoadingStatusText;
    public TextMeshProUGUI VariationIdText;
    public TextMeshProUGUI RevisionText;
    public TextMeshProUGUI LocaleText;

    public TMP_InputField PaywallIdTextField;
    public TMP_InputField LocaleTextField;

    private Adapty.Paywall m_paywall;

    private List<ProductButton> m_productButtons = new List<ProductButton>(3);

    void Start() {
        
    }

    public void LogShowPaywallPressed() {
        if (m_paywall == null) {
            return;
        }

        this.Router.SetIsLoading(true);
        this.Listener.LogShowPaywall(m_paywall, (error) => {
            this.Router.SetIsLoading(false);
        });
    }

    public void LoadPaywall() {
        var paywallId = this.PaywallIdTextField.text;
        var locale = this.LocaleTextField.text;

        if (paywallId == null || paywallId.Length == 0) {
            return;
        }

        this.Router.SetIsLoading(true);

        this.Listener.GetPaywall(paywallId, locale, (paywall) => {
            if (paywall == null) {
                this.UpdatePaywallFail();
                this.Router.SetIsLoading(false);
            } else {
                this.m_paywall = paywall;
                this.LoadProducts(paywall);
            }
        });
    }

    void LoadProducts(Adapty.Paywall paywall) {
        this.Listener.GetPaywallProducts(paywall, (products) => {
            if (products != null) {
                this.UpdatePaywallData(paywall, products);
            } else {
                this.UpdatePaywallFail();
            }

            this.Router.SetIsLoading(false);
        });
    }


    private void UpdatePaywallInitial() {
        this.PaywallNameText.SetText("null");
        this.LoadingStatusText.SetText("WAIT");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
    }

    private void UpdatePaywallFail() {
        this.LoadingStatusText.SetText("FAIL");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
    }

    private void UpdatePaywallData(Adapty.Paywall paywall, IList<Adapty.PaywallProduct> products) {
        this.LoadingStatusText.SetText("OK");
        this.PaywallNameText.SetText(paywall.Id);
        this.VariationIdText.SetText(paywall.VariationId);
        this.RevisionText.SetText(paywall.Revision.ToString());
        this.LocaleText.SetText(paywall.Locale);

        m_productButtons.ForEach((button) => {
            Destroy(button.gameObject);
        });
        m_productButtons.Clear();

        for (var i = 0; i < products.Count; ++i) {
            var product = products[i];
            var productButton = this.CreateProductButton(product, i);
            m_productButtons.Add(productButton);
        }

        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 780.0f + products.Count * 80.0f);
    }

    private ProductButton CreateProductButton(Adapty.PaywallProduct product, float index) {
        var productButtonObject = Instantiate(this.ProductButtonPrefab);
        var productButtonRect = productButtonObject.GetComponent<RectTransform>();

        productButtonRect.SetParent(this.ContainerTransform);
        productButtonRect.anchoredPosition = new Vector3(productButtonRect.position.x, -300.0f - 80.0f * index);
        productButtonRect.sizeDelta = new Vector2(this.ContainerTransform.sizeDelta.x - 40.0f, 70.0f);

        productButtonObject.GetComponent<ProductButton>().UpdateProduct(product);
        productButtonObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => {
            this.Router.SetIsLoading(true);
            this.Listener.MakePurchase(product, (error) => {
                this.Router.SetIsLoading(false);
            });
        });
        return productButtonObject.GetComponent<ProductButton>();
    }
}