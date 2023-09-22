using AdaptySDK;
using TMPro;
using UnityEngine;
using static AdaptySDK.Adapty;

public class ProductButton : MonoBehaviour
{
    public TextMeshProUGUI ProductIdText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI EligibilityText;

    public void UpdateProduct(Adapty.PaywallProduct product, Eligibility eligibility)
    {
        this.ProductIdText.SetText(product.VendorProductId);
        this.PriceText.SetText(product.Price.LocalizedString);
        this.EligibilityText.SetText(eligibility.ToString());
    }
}
