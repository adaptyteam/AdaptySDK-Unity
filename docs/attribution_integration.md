# Attribution Integration

*Learn how to pass attribution data to the dashboard using Adapty SDK. Adapty supports AppsFlyer, Adjust, Branch, Facebook Ads, and Apple Search Ads*

Adapty allows easy integration with the popular attribution services: AppsFlyer, Adjust, Branch, Apple Search Ads, and Facebook Ads. Adapty will send [subscription events](https://docs.adapty.io/docs/events) to these services so you can accurately measure the performance of ad campaigns. You can also filter [charts data](https://docs.adapty.io/docs/analytics-charts) using attribution data.

## Important

> ❗️ **Avoiding events duplication**
> 
> Make sure to turn off sending subscription events from devices and your server to avoid duplication.
>
> If you set up direct integration with Facebook, turn off events forwarding from AppsFlyer, Adjust, or Branch.

> 👍 Be sure you've set up attribution integration in Adapty Dashboard, otherwise, we won't be able to send subscription events.

> 📘 Attribution data is set for every profile one time, we won't override the data once it's been saved.

## Setting attribution data

To set attribution data for the profile, use .updateAttribution() method:

```c#
Adapty.UpdateAttribution("<attributions>", source, "<networkUserId>", (error) => {
    if (error != null) {
        // handle error
        return;
    }

    // succesfull attribution update
});
```

Request parameters

- **Attribution** (required): a dictionary containing attribution (conversion) data.
- **Source** (required): a source of attribution. The allowed values are:
  - AttributionNetwork.Adjust
  - AttributionNetwork.Appsflyer
  - AttributionNetwork.Branch
  - AttributionNetwork.AppleSearchAds
  - AttributionNetwork.Custom
- **Network user Id** (optional): a string profile's identifier from the attribution service.

## Custom

If you use custom attribution system, you can pass the attribution data to Adapty. You can then segment users based on this data.
To set custom attribution, use only the keys from the example below (all keys are optional). Every value in the map should be no longer than 100 characters. status can only be organic, non-organic or unknown. Any additional keys will be omitted.

```c#
var attributions = "{\"status\": \"non_organic|organic|unknown\",\"channel\": \"Google Ads\",\"campaign\": \"Christmas Sale\",\"ad_group\": \"ad group\",\"ad_set\": \"ad_set\",\"creative\": \"creative id\"}";

await Adapty.updateAttribution(attribution, source: AdaptyAttributionNetwork.custom);

Adapty.UpdateAttribution(attributions, Adapty.AttributionNetwork.Custom, "test_user_id", (error) => {
    if (error != null) {
        // handle error
    }
});
```
