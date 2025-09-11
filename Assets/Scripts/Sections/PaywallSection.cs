using System;
using System.Collections;
using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static AdaptySDK.Adapty;

public class PaywallSection : MonoBehaviour
{
    public AdaptyListener Listener;
    public AdaptyRouter Router;

    public GameObject ProductButtonPrefab;

    public RectTransform ContainerTransform;
    public RectTransform ProductsContainerTransform;
    public TextMeshProUGUI PaywallNameText;
    public TextMeshProUGUI LoadingStatusText;
    public TextMeshProUGUI VariationIdText;
    public TextMeshProUGUI RevisionText;
    public TextMeshProUGUI LocaleText;
    public TextMeshProUGUI AudienceNameText;

    private string m_paywallId = "example_ab_test";
    private string m_localeId = "fr";

    private AdaptyPaywall m_paywall;

    private List<ProductButton> m_productButtons = new List<ProductButton>(3);

    [Header("Mock Settings")]
    [SerializeField]
    private bool UseMockProducts = false;

    [SerializeField]
    private int MockProductsCount = 3;

    void Start()
    {
        EnsureLayout();
        this.PaywallNameText.SetText(this.m_paywallId);
        this.Listener.OnInitializeFinished += OnAdaptyInitialized;

#if UNITY_EDITOR
        GenerateMockProducts();
#endif
    }

    private void OnAdaptyInitialized()
    {
        this.Listener.OnInitializeFinished -= OnAdaptyInitialized;
        this.LoadPaywallForDefaultAudience();
        this.LoadPaywall();
    }

    private void OnDestroy()
    {
        this.Listener.OnInitializeFinished -= OnAdaptyInitialized;
    }

    public void LogShowPaywallPressed()
    {
        if (m_paywall != null)
        {
            this.Router.SetIsLoading(true);
            this.Listener.LogShowPaywall(
                m_paywall,
                (error) =>
                {
                    this.Router.SetIsLoading(false);
                }
            );
        }
    }

    public void LoadPaywallForDefaultAudience()
    {
        if (this.m_paywallId == null)
            return;

        this.Router.SetIsLoading(true);

        this.Listener.GetPaywallForDefaultAudience(
            this.m_paywallId,
            this.m_localeId,
            AdaptyPlacementFetchPolicy.ReloadRevalidatingCacheData,
            (paywall) =>
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
            }
        );
    }

    public void LoadPaywall()
    {
        var fetchPolicy = AdaptyPlacementFetchPolicy.ReloadRevalidatingCacheData;
        var paywallId = this.m_paywallId;
        var locale = this.m_localeId;

        if (locale != null && locale.Length == 0)
        {
            locale = null;
        }

        if (paywallId == null || paywallId.Length == 0)
        {
            return;
        }

        this.Router.SetIsLoading(true);

        this.Listener.GetPaywall(
            paywallId,
            locale,
            fetchPolicy,
            (paywall) =>
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
            }
        );
    }

    void LoadProducts(AdaptyPaywall paywall)
    {
        this.Listener.GetPaywallProducts(
            paywall,
            (products) =>
            {
                if (products != null)
                {
                    StartCoroutine(DelayedUpdate(paywall, products));
                }
                else
                {
                    this.UpdatePaywallFail();
                }

                this.Router.SetIsLoading(false);
            }
        );
    }

    private IEnumerator DelayedUpdate(AdaptyPaywall paywall, IList<AdaptyPaywallProduct> products)
    {
        yield return new WaitForEndOfFrame();
        this.UpdatePaywallData(paywall, products);
    }

    public void PresentPaywall()
    {
        if (m_paywall == null)
            return;

        this.Listener.CreatePaywallView(
            this.m_paywall,
            preloadProducts: false,
            (view) =>
            {
                if (view == null)
                {
                    //this.UpdateViewFail(paywall);
                }
                else
                {
                    view.Present((error) => { });
                }
            }
        );
    }

    private void UpdatePaywallFail()
    {
        this.LoadingStatusText.SetText("FAIL");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
        this.AudienceNameText.SetText("null");
    }

    private void UpdatePaywallData(AdaptyPaywall paywall, IList<AdaptyPaywallProduct> products)
    {
        this.LoadingStatusText.SetText("OK");
        this.VariationIdText.SetText(paywall.VariationId);
        this.RevisionText.SetText(paywall.Revision.ToString());
        this.LocaleText.SetText(paywall.Locale);
        this.AudienceNameText.SetText(paywall.AudienceName);

        m_productButtons.ForEach(
            (button) =>
            {
                Destroy(button.gameObject);
            }
        );
        m_productButtons.Clear();

        for (var i = 0; i < products.Count; ++i)
        {
            var product = products[i];
            var productButton = this.CreateProductButton(product, i);
            m_productButtons.Add(productButton);
        }

        // Rebuild layout so parent scroll view adapts to new preferred height
        var productsParent =
            this.ProductsContainerTransform != null
                ? this.ProductsContainerTransform
                : this.ContainerTransform;
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(productsParent);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.ContainerTransform);
    }

    private ProductButton CreateProductButton(AdaptyPaywallProduct product, float index)
    {
        var productButtonObject = Instantiate(this.ProductButtonPrefab);
        var productButtonRect = productButtonObject.GetComponent<RectTransform>();

        EnsureLayout();
        var productsParent =
            this.ProductsContainerTransform != null
                ? this.ProductsContainerTransform
                : this.ContainerTransform;
        productButtonRect.SetParent(productsParent, false);
        productButtonRect.anchorMin = new Vector2(0, 1);
        productButtonRect.anchorMax = new Vector2(1, 1);
        productButtonRect.offsetMin = new Vector2(0, productButtonRect.offsetMin.y);
        productButtonRect.offsetMax = new Vector2(0, productButtonRect.offsetMax.y);
        productButtonRect.localScale = Vector3.one;

        var le = productButtonRect.GetComponent<UnityEngine.UI.LayoutElement>();
        if (le == null)
        {
            le = productButtonRect.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        }
        le.preferredHeight = 140.0f;
        le.flexibleWidth = 1.0f;

        productButtonObject.GetComponent<ProductButton>().UpdateProduct(product);
        productButtonObject
            .GetComponent<UnityEngine.UI.Button>()
            .onClick.AddListener(() =>
            {
                this.Router.SetIsLoading(true);
                this.Listener.MakePurchase(
                    product,
                    (error) =>
                    {
                        this.Router.SetIsLoading(false);
                    }
                );
            });
        return productButtonObject.GetComponent<ProductButton>();
    }

    private void GenerateMockProducts()
    {
        Debug.Log(
            "#PaywallSection# GenerateMockProducts called, details = "
                + this.MockProductsCount.ToString()
        );
        // Clear any existing buttons (both runtime and previous mock)
        m_productButtons.ForEach(
            (button) =>
            {
                if (button != null)
                    DestroyImmediate(button.gameObject);
            }
        );
        m_productButtons.Clear();

        EnsureLayout();
        var productsParent =
            this.ProductsContainerTransform != null
                ? this.ProductsContainerTransform
                : this.ContainerTransform;

        for (int i = 0; i < Mathf.Max(0, this.MockProductsCount); i++)
        {
            var go = Instantiate(this.ProductButtonPrefab);
            go.name = $"MockProduct_{i + 1}";

            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(productsParent, false);

            var le = rect.GetComponent<UnityEngine.UI.LayoutElement>();
            if (le == null)
            {
                le = rect.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
            }
            le.preferredHeight = 140.0f;
            le.flexibleWidth = 1.0f;

            var pb = go.GetComponent<ProductButton>();
            if (pb != null)
            {
                pb.UpdateProductMock($"mock.product.{i + 1}", "$0.99", "N/A");
                m_productButtons.Add(pb);
            }
        }

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(productsParent);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(this.ContainerTransform);
    }

    private void EnsureLayout()
    {
        //     if (this.ContainerTransform == null)
        //     {
        //         return;
        //     }

        //     var productsParent =
        //         this.ProductsContainerTransform != null
        //             ? this.ProductsContainerTransform
        //             : this.ContainerTransform;
        //     var v = productsParent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        //     if (v == null)
        //     {
        //         v = productsParent.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        //         v.childControlHeight = true;
        //         v.childControlWidth = true;
        //         v.childForceExpandHeight = false;
        //         v.childForceExpandWidth = true;
        //         v.spacing = 10.0f;
        //         v.padding = new RectOffset(20, 20, 300, 20);
        //     }

        //     var f = productsParent.GetComponent<UnityEngine.UI.ContentSizeFitter>();
        //     if (f == null)
        //     {
        //         f = productsParent.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        //     }
        //     f.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

        //     var selfFitter = GetComponent<UnityEngine.UI.ContentSizeFitter>();
        //     if (selfFitter == null)
        //     {
        //         selfFitter = gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        //     }
        //     selfFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

        //     var selfLE = GetComponent<UnityEngine.UI.LayoutElement>();
        //     if (selfLE == null)
        //     {
        //         selfLE = gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        //     }
        //     selfLE.flexibleWidth = 1.0f;
    }
}
