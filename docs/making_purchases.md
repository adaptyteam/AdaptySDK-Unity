# Making Purchases

*Learn how to make and restore mobile purchases using Adapty iOS SDK. Adapty handles server-side validation, including renewals and grace periods*

To make the purchase, you have to call `MakePurchase()` method:

```c#
Adapty.MakePurchase(productId, variationId, offerId, (result, error) => {
    if(error != null) {
        // handle error
        return;
    }

    var product = result.Product;
    var purchaserInfo = result.PurchaserInfo;
    var receipt = result.Receipt;
});
```

Request parameters:

- **Product Id**: an `Adapty.Product` object `VendorProductId` field.
- **Variation Id** (optional):  a `VariationId` field from the paywall, from which this product was obtained.
- **Offer Id** (optional): a string identifier of promotional offer from App Store Connect. Adapty signs the request according to Apple guidelines, please make sure you've uploaded Subscription Key in Adapty Dashboard when using promotional offers.


Response parameters:

- **Purchaser info**: a `Adapty.PurchaserInfo` object. This model contains info about access levels, subscriptions, and non-subscription purchases. Generally, you have to check only access level status to determine whether the user has premium access to the app.
- **Receipt**: a string representation of Apple's receipt.
Apple validation result: a dictionary containing raw validation result from Apple.
- **Product**: a `Adapty.Product` object you've passed to this method.

> 🚧 Make sure you've added [App Store Shared Secret](https://docs.adapty.io/docs/app-store-shared-secret) in Adapty Dashboard, without it, we can't validate purchases.

Below is a complete example of making the purchase and checking the user's access level.

```c#
Adapty.MakePurchase(productId, variationId, offerId, (result, error) => {
    if(error != null) {
        // handle error
    return;
    }

    var purchaserInfo = result.PurchaserInfo;

    // "premium" is an identifier of default access level
    var accessLevel = purchaserInfo.AccessLevels["premium"];
    if (accessLevel != null && accessLevel.IsActive) {
        // grant access to premium features
    }
});
```

> 🚧 Make sure to set up [App Store Server Notifications](https://docs.adapty.io/docs/app-store-server-notifications) to receive subscription updates without significant delays.

## Restoring purchases

To restore purchases, you have to call .restorePurchases() method:

```c#
Adapty.RestorePurchases((result, error) => {
    if (error != null) {
        // handle error
        return;
    }

    // successful restore
    var purchaserInfo = result.PurchaserInfo;
    var receipt = result.Receipt;
});
```

Response parameters:

- **Purchaser info**: an `Adapty.PurchaserInfo` object. This model contains info about access levels, subscriptions, and non-subscription purchases. Generally, you have to check only access level status to determine whether the user has premium access to the app.
- **Receipt**: a string representation of Apple's receipt.


## Deferred purchases

For deferred purchases, Adapty SDK has an optional delegate method, which is called when the user starts the purchase in the App Store, and the transaction continues in your app. Just store makeDeferredPurchase and call it later if you want to hold your purchase for now. Then show the paywall to your user. To continue purchase, call makeDeferredPurchase.

```c#
public class AdaptyListener : MonoBehaviour, AdaptyEventListener {

	public void OnDeferredPurchasesProduct(Adapty.Product product) {
		// you can store product.VendorProductId and call MakeDeferredPurchase with it later

		// or you can call it right away
		Adapty.MakeDeferredPurchase(product.VendorProductId, (result, error) => {
			if (error != null) {
				// handle error
				return;
			}

			// check the purchase
		});
	}

}
```

## Redeeming an Offer Code

Since iOS 14.0 your users can redeem Offer Codes. To allow them to do so, you can present the Offer Code redemption sheet by calling the related SDK method.

```c#
Adapty.PresentCodeRedemptionSheet();
```