# Subscription Status

*Learn how to get info about user subscription status and grant access to the premium features of the app using Adapty Unity SDK*

With Adapty you don't have to hardcode product ids to check subscription status. You just have to verify that the user has an active access level. To do this, you have to call `GetPurchaserInfo()` method:

```c#
Adapty.GetPurchaserInfo(forceUpdate, (purchaserInfo, error) => {
    if (error == null) {
        // handle error
        return;
    }

    // check the accesss 
});
```

Request parameters:

- **Force update** (optional): a boolean indicating whether Adapty should fetch the data from API or get it from the local cache. Adapty SDK regularly refreshes the local data, so most of the time you don't have to use this flag. By default, it's set to false.

Response parameters:

- **Purchaser info**: a PurchaserInfoModel object. This model contains info about access levels, subscriptions, and non-subscription purchases. Generally, you have to check only access level status to determine whether the user has premium access to the app.

Below is a complete example of checking the user's access level.

```c#
Adapty.GetPurchaserInfo(forceUpdate, (purchaserInfo, error) => {
    if (error != null) {
        // handle error
        return;
    }

    // "premium" is an identifier of default access level
    var accessLevel = purchaserInfo.AccessLevels["premium"];
    if (accessLevel != null && accessLevel.IsActive) {
        // grant access to premium features
    }
});
```

> 📘 You can have multiple access levels per app. For example, if you have a newspaper app and sell subscriptions to different topics independently, you can create access levels "sports" and "science". But most of the time, you will only need one access level, in that case, you can just use the default "premium" access level.
>
> Read more about access levels in the [Access Level](https://docs.adapty.io/docs/access-level) section.

## Listening for subscription status updates

Whenever the user's subscription changes, Adapty will fire an event. To receive subscription updates, extend `AdaptyEventListener` with `OnReceiveUpdatedPurchaserInfo` method:

```c#
public class AdaptyListener : MonoBehaviour, AdaptyEventListener {
	public void OnReceiveUpdatedPurchaserInfo(Adapty.PurchaserInfo purchaserInfo) {
		// handle any changes to subscription state
	}
}
```

> 🚧 Make sure to set up [App Store Server Notifications](https://docs.adapty.io/docs/app-store-server-notifications) to receive subscription updates without significant delays.