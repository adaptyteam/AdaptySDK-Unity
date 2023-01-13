using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;

public class PaywallSection : MonoBehaviour {
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public GameObject ProductButtonPrefab;

    public RectTransform ContainerTransform;
    public TextMeshProUGUI PaywallNameText;
    public TextMeshProUGUI LoadingStatusText;
    public TextMeshProUGUI VariationIdText;
    public TextMeshProUGUI RevisionText;

    private string m_paywallId = "example_ab_test";
    private List<ProductButton> m_productButtons = new List<ProductButton>(3);

    void Start() {
        this.PaywallNameText.SetText(this.m_paywallId);
        this.LoadPaywall();
    }

    public void LoadPaywall() {
        this.Router.SetIsLoading(true);

        this.Listener.GetPaywall(this.m_paywallId, (paywall) => {
            if (paywall == null) {
                this.UpdatePaywallFail();
                this.Router.SetIsLoading(false);
            } else {
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

    private void UpdatePaywallFail() {
        this.LoadingStatusText.SetText("FAIL");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
    }

    private void UpdatePaywallData(Adapty.Paywall paywall, IList<Adapty.PaywallProduct> products) {
        this.LoadingStatusText.SetText("OK");
        this.VariationIdText.SetText(paywall.VariationId);
        this.RevisionText.SetText(paywall.Revision.ToString());

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
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 380.0f + products.Count * 80.0f);
    }

    private ProductButton CreateProductButton(Adapty.PaywallProduct product, float index) {
        var productButtonObject = Instantiate(this.ProductButtonPrefab);
        var productButtonRect = productButtonObject.GetComponent<RectTransform>();

        productButtonRect.SetParent(this.ContainerTransform);
        productButtonRect.anchoredPosition = new Vector3(productButtonRect.position.x, -230.0f - 80.0f * index);
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