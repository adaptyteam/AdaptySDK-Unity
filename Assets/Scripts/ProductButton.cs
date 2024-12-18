using AdaptySDK;
using TMPro;
using UnityEngine;
using static AdaptySDK.Adapty;

public class ProductButton : MonoBehaviour
{
    public TextMeshProUGUI ProductIdText;
    public TextMeshProUGUI PriceText;
    public TextMeshProUGUI EligibilityText;

    public void UpdateProduct(AdaptyPaywallProduct product)
    {
        this.ProductIdText.SetText(product.VendorProductId);
        this.PriceText.SetText(product.Price.LocalizedString);
        // this.EligibilityText.SetText(eligibility.ToString());
        this.EligibilityText.SetText("TODO");
    }
}
