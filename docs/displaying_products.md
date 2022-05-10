# Displaying Products

*Learn how to fetch and display products from paywalls in your app using Adapty Flutter SDK. With Adapty you can dynamically change the products visible to your users without new app releases*

Adapty allows you to remotely configure the products that will be displayed in your app. This way you don't have to hardcode the products and can dynamically change offers or run A/B tests without app releases.

> 🚧 Make sure you've added the products and paywalls in Adapty Dashboard before fetching the products.

To fetch the products, you have to call `.GetPaywalls()` method:

```c#
Adapty.GetPaywalls(forceUpdate, (result, error) => {
	if(error != null) {
		// handle error
		return;
	}

	var paywalls = result.Paywalls;
    var products = result.Products;
	
	// Dispay paywalls or products
});
```

Request parameters:

- **Force update** (optional): a boolean indicating whether Adapty should fetch the data from API or get it from the local cache. Adapty SDK regularly refreshes the local data, so most of the time you don't have to use this flag. By default, it's set to false.


Response parameters:
- **Paywalls**: an array of AdaptyPaywall objects. This model contains the list of the products, paywall's identifier, custom payload, and several other properties. All you need to display the products in your app is to get a paywall by its identifier, retrieve the products, and display them.
- **Products**: an array of AdaptyProduct objects. This model contains product identifier, price, currency, title, and many other product options. Generally, you don't have to use this response parameter as all the products should be stored in the paywalls.

> 📘 Every time the data is fetched from a remote server, it will be stored in the local cache. This way, you can display the products even when the user is offline.

> 🚧 Since the paywalls are configured remotely, the available products, the number of the products, and the special offers (eg free trials) can change over time. Make sure your code handles these scenarios. For example, if you get 2 products, the 2 products will be displayed, but when you get 3, all of them should be displayed without code changes.
>
> **Don't hard code product ids**, you won't need them.

## Paywall analytics

Adapty helps you to measure the performance of the paywalls. We automatically collect all the metrics related to purchases except for paywall views. This is because only you know when the paywall was shown to a customer.
Whenever you show a paywall to your user, call `.LogShowPaywall(paywall)` to log the event, and it will be accumulated in the paywall metrics.

```c#
Adapty.LogShowPaywall(paywall, (error) => {
	if(error != null) {
		// handle error
	}
});
```

Request parameters:

- **Paywall** (required): an `Adapty.Paywall` object.


## Fallback paywalls

Adapty allows you to provide fallback paywalls that will be used when a user opens the app for the first time and there's no internet connection. Or in the rare case when Adapty backend is down and there's no cache on the device.
To set fallback paywalls, use `.SetFallbackPaywalls(paywalls)` method. You should pass exactly the same payload you're getting from Adapty backend. You can copy it from Adapty Dashboard.

```c#
Adapty.SetFallbackPaywalls(paywalls, (error) => {
	if(error != null) {
		// handle error
	}
});
```

Request parameters:

- **Paywalls** (required): a JSON representation of your paywalls/products list in the exact same format as provided by Adapty backend.

> 📘 You can also hardcode fallback paywall data or receive it from your remote server.

## Paywall Remote Config

There is a remote config available with Adapty which [can be built](https://docs.adapty.io/docs/paywall#paywall-remote-config) right through the dashboard and then used inside your app. To get such config, just access customPayload property and extract needed values.

```c#
Adapty.GetPaywalls(forceUpdate, (result, error) => {
	if (error != null) {
		// handle error
		return;
	}

	var paywall = result.Paywalls[0];
	var headerText = (string)paywall.CustomPayload["header_text"];
});
```

You can also access customPayload through delegate method. It's faster, since it doesn't require additional StoreKit request as for fulfilled paywalls.

> 🚧 **Don't use this delegate method to display products**
> 
> Since no StoreKit requests are made at this point, data in products (like price) may be wrong. Use this delegate only to access remote config early. For example, to configure onboarding before paywall

```c#
public class AdaptyListener : MonoBehaviour, AdaptyEventListener {
    public void OnReceivePaywallsForConfig(Adapty.Paywall[] paywalls) {
        var paywall = paywalls[0];
        var headerText = (string)paywall.CustomPayload["header_text"];
    }
}
```