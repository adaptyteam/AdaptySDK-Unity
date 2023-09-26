//
//  ProfileParameters+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class ProfileParameters
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();

                if (FirstName != null) node.Add("first_name", FirstName);
                if (LastName != null) node.Add("last_name", LastName);
                if (Gender != null) node.Add("gender", Gender.Value.ToJSON());
                if (Birthday != null) node.Add("birthday", $"{Birthday.Value.Year}-{Birthday.Value.Month}-{Birthday.Value.Day}");
                if (Email != null) node.Add("email", Email);
                if (PhoneNumber != null) node.Add("phone_number", PhoneNumber);
                if (FacebookAnonymousId != null) node.Add("facebook_anonymous_id", FacebookAnonymousId);
                if (AmplitudeUserId != null) node.Add("amplitude_user_id", AmplitudeUserId);
                if (AmplitudeDeviceId != null) node.Add("amplitude_device_id", AmplitudeDeviceId);
                if (MixpanelUserId != null) node.Add("mixpanel_user_id", MixpanelUserId);
                if (AppmetricaProfileId != null) node.Add("appmetrica_profile_id", AppmetricaProfileId);
                if (AppmetricaDeviceId != null) node.Add("appmetrica_device_id", AppmetricaDeviceId);
                if (OneSignalPlayerId != null) node.Add("one_signal_player_id", OneSignalPlayerId);
                if (OneSignalSubscriptionId != null) node.Add("one_signal_subscription_id", OneSignalSubscriptionId);
                if (PushwooshHWID != null) node.Add("pushwoosh_hwid", PushwooshHWID);
                if (FirebaseAppInstanceId != null) node.Add("firebase_app_instance_id", FirebaseAppInstanceId);
                if (AirbridgeDeviceId != null) node.Add("airbridge_device_id", AirbridgeDeviceId);
#if UNITY_IOS
                if (AppTrackingTransparencyStatus != null) node.Add("att_status", AppTrackingTransparencyStatus.Value.ToJSON());
#endif
                if (AnalyticsDisabled != null) node.Add("analytics_disabled", AnalyticsDisabled.Value);
                if (CustomAttributes.Count > 0) node.Add("custom_attributes", CustomAttributes.ToJSONObject());

                return node;
            }
        }
    }
}