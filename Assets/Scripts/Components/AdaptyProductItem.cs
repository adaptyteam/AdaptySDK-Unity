using UnityEngine;
using UnityEngine.UI;
using AdaptySDK;

public class AdaptyProductItem: MonoBehaviour {

    public Text DescriptionText;

    public void ConfigurePaywall(Adapty.Paywall paywall) {
        DescriptionText.text = string.Format("Paywall Id: {0}\nProduts: {1}", paywall.DeveloperId, paywall.Products.Length);
    }

    public void ConfigureProduct(Adapty.Product product) {
        DescriptionText.text = string.Format("Product Id: {0}\nPrice: {1}\nPeriod: {2}", product.VendorProductId, product.LocalizedPrice, product.LocalizedSubscriptionPeriod);
    }
}
