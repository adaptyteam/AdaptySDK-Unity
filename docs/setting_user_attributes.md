# Setting User Attributes
*Learn how to set user attributes using Adapty SDK. You can then use attributes to segment profiles*

You can set optional attributes such as email, phone number, etc, to the user of your app. You can then use attributes to create user [segments](https://docs.adapty.io/docs/segments) or just view them in CRM.

## Setting user attributes

To set user attributes, call .updateProfile() method:

```c#
var profileBuilder = new Adapty.ProfileParameterBuilder();
profileBuilder.FirstName = "TestFirstName";
profileBuilder.LastName = "TestLastName";
profileBuilder.SetBirthday(1984, 11, 11);

AdaptySDKExampleWrapper.UpdateProfile(profileBuilder, (error) => { 
    if (error != null) {
        // handle error
        return;
	}

    // successful update
});
```

## The allowed keys list

The allowed keys `<Key>` of ProfileParameterBuilder() and the values `<Value>` are listed below:

Key | Value
--|--
email |	String up to 30 characters
phoneNumber | String up to 15 characters
facebookUserId | String up to 30 characters
facebookAnonymousId | String up to 30 characters
amplitudeUserId | String up to 30 characters
amplitudeDeviceId | String up to 30 characters
mixpanelUserId | String up to 30 characters
appmetricaProfileId | String up to 30 characters
appmetricaDeviceId | String up to 30 characters
firstName | String up to 30 characters
lastName | String up to 30 characters
gender | Enum, allowed values are: female, male, other
birthday | string (year-month-date) – or use `SetBirthday` method
appTrackingTransparencyStatus (iOS only) | ATTrackingManager.AuthorizationStatus, app tracking transparency status you can receive starting from iOS 14. To receive it just call let status = ATTrackingManager.AuthorizationStatus. You should send this specific property to Adapty as soon as it changes. `profileBuilder.AppTrackingTransparencyStatus = status`
customAttributes | Dictionary

## Customer user attributes

You can set your own custom attributes. These are usually related to your app usage. For example, for fitness applications, they might be the number of training per week, for language learning app user's knowledge level, and so on. You can use them in segments to create targeted paywalls and offers, and you can also use them in analytics to figure out which product metrics affect the revenue most.

You can set up to 10 custom attributes for the user, the attribute's key and value should be up to 30 characters. The keys can be only strings of letters, numbers, dashes, points, and underscores. The values can be both strings and numbers. Boolean values will be converted to integers.

Contact us if you need higher limits for custom user attributes.