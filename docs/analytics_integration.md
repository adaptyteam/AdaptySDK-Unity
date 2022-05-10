# Analytics Integration
*Learn how to set up integration with external analytics using Adapty SDK. Adapty supports Amplitude, Mixpanel, and AppMetrica*

Adapty sends all subscription events to analytical services, such as Amplitude, Mixpanel, and AppMetrica. We can also send events to your server using webhook integration. The best thing about this is that you don't have to send any of the events, we'll do it for you. Just make sure to configure the integration in Adapty Dashboard.

## Setting the profile's identifier

Set the profile's identifier for the selected analytics using `UpdateProfile()` method. For example, for Amplitude integration, you can set either `AmplitudeUserId`. For Mixpanel integration, you have to set `MixpanelUserId`. When these identifiers are not set, Adapty will use customerUserId instead. If the customerUserId is not set, we will use our internal profile id.

> ❗️ **Avoiding duplication**
> 
> Don't forget to turn off sending subscription events from devices and your server to avoid duplication

## Disabling external analytics for a specific customer

You may want to stop sending analytics events for a specific customer. This is useful if you have an option in your app to opt-out of analytics services.

To disable external analytics for a customer, use `SetExternalAnalyticsEnabled(false)` method. When external analytics is blocked, Adapty won't be sending any events to any integrations for the specific user. If you want to disable an integration for all users of your app, just turn it off in Adapty Dashboard.

## Disable collection of IDFA

### iOS 
You can also disable only IDFA collecting by adding specific key to the `Adapty-Info.plist` file.

```
<key>AdaptyIDFACollectionDisabled</key>
<true/>
```