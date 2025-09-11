using System.Collections;
using System.Collections.Generic;
using AdaptyExample;
using AdaptySDK;
using TMPro;
using UnityEngine;
using static AdaptySDK.Adapty;

public class CustomPaywallSection : MonoBehaviour
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

    public TMP_Dropdown FetchPolicyDropdown;
    public TMP_InputField PaywallIdTextField;
    public TMP_InputField LocaleTextField;

    private AdaptyPaywall m_paywall;

    private List<ProductButton> m_productButtons = new List<ProductButton>(3);

    void Start()
    {
        EnsureLayout();
    }

    public void LogShowPaywallPressed()
    {
        if (m_paywall == null)
        {
            return;
        }

        this.Router.SetIsLoading(true);
        this.Listener.LogShowPaywall(
            m_paywall,
            (error) =>
            {
                this.Router.SetIsLoading(false);
            }
        );
    }

    AdaptyPlacementFetchPolicy CurrentFetchPolicy()
    {
        Debug.Log(
            string.Format("#CustomPaywallSection# value {0}", this.FetchPolicyDropdown.value)
        );

        switch (this.FetchPolicyDropdown.value)
        {
            case 0:
                return AdaptyPlacementFetchPolicy.ReloadRevalidatingCacheData;
            case 1:
                return AdaptyPlacementFetchPolicy.ReturnCacheDataElseLoad;
            case 2:
                return AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                    new System.TimeSpan(0, 0, 10)
                );
            case 3:
                return AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                    new System.TimeSpan(0, 0, 30)
                );
            case 4:
                return AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                    new System.TimeSpan(0, 0, 120)
                );
            default:
                return AdaptyPlacementFetchPolicy.ReloadRevalidatingCacheData;
        }
    }

    public void LoadPaywall()
    {
        var fetchPolicy = this.CurrentFetchPolicy();
        var paywallId = this.PaywallIdTextField.text;
        var locale = this.LocaleTextField.text;

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

    private void UpdatePaywallInitial()
    {
        this.PaywallNameText.SetText("null");
        this.LoadingStatusText.SetText("WAIT");
        this.VariationIdText.SetText("null");
        this.RevisionText.SetText("null");
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
        this.PaywallNameText.SetText(paywall.PlacementId);
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

    private void EnsureLayout()
    {
        if (this.ContainerTransform == null)
        {
            return;
        }

        var v = this.ContainerTransform.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (v == null)
        {
            v =
                this.ContainerTransform.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            v.childControlHeight = true;
            v.childControlWidth = true;
            v.childForceExpandHeight = false;
            v.childForceExpandWidth = true;
            v.spacing = 10.0f;
            v.padding = new RectOffset(20, 20, 300, 20);
        }

        var f = this.ContainerTransform.GetComponent<UnityEngine.UI.ContentSizeFitter>();
        if (f == null)
        {
            f = this.ContainerTransform.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        }
        f.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

        var selfFitter = GetComponent<UnityEngine.UI.ContentSizeFitter>();
        if (selfFitter == null)
        {
            selfFitter = gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        }
        selfFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;

        var selfLE = GetComponent<UnityEngine.UI.LayoutElement>();
        if (selfLE == null)
        {
            selfLE = gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        }
        selfLE.flexibleWidth = 1.0f;
    }
}
