<h1 align="center">
<img src="https://raw.githubusercontent.com/adaptyteam/AdaptySDK-iOS/master/adapty.png"><br />
Adapty Unity SDK
</h1>

* [Requirements](#requirements)
* [Installation](#installation)
* [Initialization](#initialization)
* [Usage](#usage)
* [Documentation](#documentation)
* [License](#license)

## Requirements
* Unity 2020.1.15f1+
* [External Dependency Manager plugin](https://github.com/googlesamples/unity-jar-resolver) (included in the adapty unitypackage file)

## Installation

To install the SDK you can either find it in the Asset Store or you can download from github `adapty-unity-plugin-*.unitypackage` and import it into your project. If you download it from the Asset Store, please also download and import the [External Dependency Manager plugin](https://github.com/googlesamples/unity-jar-resolver).

The SDK uses the "External Dependency Manager" plugin to handle iOS Cocoapods dependencies and Android gradle dependencies. After the installation you may need to invoke the dependency manager

`Assets -> External Dependency Manager -> Android Resolver -> Force Resolve`

and

`Assets -> External Dependency Manager -> iOS Resolver -> Install Cocoapods`

When building your Unity project for iOS, you would get `Unity-iPhone.xcworkspace` file, which you have to open instead of `Unity-iPhone.xcodeproj`, otherwise Cocoapods dependencies won't be used.

## Initialization

After importing the plugin, add `AdaptyObject` prefab located at `Assets/Adapty/` into your starting scene.

Select the object in your scene and enter your Adapty key.

You can also set the desired log level and enable the observe mode.

Read the usage documentation on what you can do with Adapty.

## Usage

Adapty uses `AdaptySDK` namespace. You may add at the top of your script files that would use Adapty SDK:

```c#
using AdaptySDK;
```

To identyify your user call `Adapty.identify()` after the plugin was initialized (after the `Start()` method of the `AdaptyObject` is called by Unity).

```c#
Adapty.identify("my_customer_id", this);
```

Adapty invokes callbacks to indicate success or to pass results. If there was an error, the `AdaptyError error` variable would not be `null`.

```c#
public void OnIdentify(AdaptyError error) {
	if (error != null) {
		Debug.Log("Error message: " + error.message + ", code: " + error.code);
	}
}
```

For the most part, you would be using API methods like `Adapty.getPaywalls()` and receive results in callbacks like `void OnGetPaywalls(PaywallModel[] paywalls, ProductModel[] products, DataState state, AdaptyError error)`.

```c#
public void OnGetPaywalls(PaywallModel[] paywalls, ProductModel[] products, DataState state, AdaptyError error) {
	if (error != null) {
		Debug.Log("Error message: " + error.message + ", code: " + error.code);
	} else {
		// Cached or synced?
		Debug.Log("State: " + state);
		// Retrieved products.
		foreach (ProductModel product in products) {
			Debug.Log("Product price: " + product.localizedPrice + ", vendorProductId: " + product.vendorProductId);
		}
	}
}
```

## Documentation

You can find the API documentation here.
- [API](/docs/API.md)

## License

Adapty is available under the MIT license. See [LICENSE](https://github.com/adaptyteam/AdaptySDK-Unity/blob/main/LICENSE) for details.