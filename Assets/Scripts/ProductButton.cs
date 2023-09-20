using AdaptySDK;
using TMPro;
using UnityEngine;

public class ProductButton : MonoBehaviour
{
    public TextMeshProUGUI ProductIdText;

    public TextMeshProUGUI PriceText;

    public void UpdateProduct(Adapty.PaywallProduct product)
    {
        this.ProductIdText.SetText(product.VendorProductId);
        this.PriceText.SetText(product.Price.LocalizedString);
    }
}
