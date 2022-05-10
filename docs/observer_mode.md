# Observer Mode
*Learn how to use Adapty Unity SDK in Observer mode along with existing purchase infrastructure*

If you have a functioning subscription system and want to give Adapty SDK a quick try, you can use Observer mode. With just one line of code you can:

- get insights by using our top-class [analytics](https://docs.adapty.io/docs/analytics-charts);
- send [subscription events](https://docs.adapty.io/docs/events) to your server and 3rd party services;
- view and analyze customers in Adapty [CRM](https://docs.adapty.io/docs/profilescrm).

## Important

> ❗️ When running in Observer mode, Adapty SDK won't close any transactions, so make sure you're handling it.

## Activating Observer Mode
### iOS
```
// in Adapty-Info.plist
<key>AdaptyObserverMode</key>
<false/>
```
### Android
If you don't need observer mode, you can skip this step

// in AndroidManifest.xml, on the same level as meta-data for activation was
```
<?xml version="1.0" encoding="utf-8"?>
<manifest ...>
    <application ...>
        ...

        <meta-data
            android:name="AdaptyPublicSdkKey"
            android:value="PUBLIC_SDK_KEY"/>

        <meta-data
            android:name="AdaptyObserverMode"
            android:value="true"/>
    </application>
</manifest>
```

## A/B tests analytics

In Observer mode, Adapty SDK doesn't know, where the purchase was made from. If you display products using our Paywalls or A/B Tests, you can manually assign variation to the purchase. After doing this, you'll be able to see metrics in Adapty Dashboard.

```c#
Adapty.SetVariationForTransaction("test_variation_id", "test_transaction_id", (error) => { 
    if(error != null) {
        // handle error
        return;
    }

    // successful binding
});
```

Request parameters:

- **variationId** (required): a string identifier of variation. You can get it using variationId property of AdaptyPaywall
- **transactionId** (required): a string identifier of your purchased transaction SKPaymentTransaction for iOS or a string identifier (purchase.getOrderId()) of the purchase, where the purchase is an instance of the billing library Purchase class for Android