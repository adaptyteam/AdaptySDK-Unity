# Configuring
*Learn how to import Adapty Unity SDK in your app, configure it, and set up logging*

## iOS

The Adapty Unity Plugin on iOS is initialized automatically. To make it work properly, you need to manually create an Adapty-Info.plist file and add it to the `/Assets` folder of your Unity project (it will be automatically copied to the Xcode project during the build phase).
This is how this file should look like:

```
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>AdaptyPublicSdkKey</key>
    <string>insert_here_your_Adapty_public_key#</string>
</dict>
</plist>

```

For iOS, you can optionally set the flag AdaptyObserverMode to true, if you want Adapty to run in Observer mode. Usually, it means, that you handle purchases and subscription status yourself and use Adapty for sending subscription events and analytics.

```
<key>AdaptyObserverMode</key>
<false/>
```

```
<key>AdaptyIDFACollectionDisabled</key>
<false/>
```

### Android

The Adapty Unity Plugin on Android is initialized automatically. To make it work properly, you need to add `<meta-data` section with "AdaptyPublicSdkKey" as a direct child of the `<application` section to your project's AndroidManifest.xml file (if you don't have one, it can be easily created in Project Settings -> Player -> Settings for Android -> Publishing settings -> Custom Main Manifest checkbox). Basically it will look like this:
```
<?xml version="1.0" encoding="utf-8"?>
<manifest ...>
    <application ...>
        ...

        <meta-data
            android:name="AdaptyPublicSdkKey"
            android:value="PUBLIC_SDK_KEY"/>
    </application>
</manifest>
```

<!-- After importing the plugin, add `AdaptyObject` prefab located at `Assets/Adapty/` into your starting scene.

Select the object in your scene and enter your Adapty key.

You can also set the desired log level and enable the observe mode.

Read the usage documentation on what you can do with Adapty. -->

## Usage

First of all you need to create a script which will be responsible for listening of Adapty events. Let's call it `AdaptyListener` and place on any object of your scene. We recommed to call `DontDestroyOnLoad` method for this object to make it live forever.

![AdaptyListenerObject](/docs/images/create_adapty_listener.png "AdaptyListenerObject")


Adapty uses `AdaptySDK` namespace. You may add at the top of your script files that would use Adapty SDK:

```c#
using AdaptySDK;
```

Next you need to subscribe for Adapty events:

```c#
using UnityEngine;
using AdaptySDK;

public class AdaptyListener : MonoBehaviour, AdaptyEventListener {
	void Start() {
		DontDestroyOnLoad(this.gameObject);
		Adapty.SetEventListener(this);
	}

	public void OnReceiveUpdatedPurchaserInfo(Adapty.PurchaserInfo purchaserInfo) {
	}

	public void OnReceivePromo(Adapty.Promo promo) {
	}

	public void OnDeferredPurchasesProduct(Adapty.Product product) {
	}

	public void OnReceivePaywallsForConfig(Adapty.Paywall[] paywalls) {
	}
}
```


## Logging

Adapty logs errors and other important information to help you understand what is going on. There are three levels available:

1) `None` (default): won't log anything
2) `Errors`: only errors will be logged
3) `Verbose`: method invocations, API requests/responses, and errors will be logged

You can call `SetLogLevel()` method in your app before configuring Adapty.