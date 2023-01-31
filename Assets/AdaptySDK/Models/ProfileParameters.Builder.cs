//
//  ProfileParameters.Builder.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class ProfileParameters
        {
            public class Builder
            {
                private ProfileParameters _Parameters = new ProfileParameters();

                public Builder SetFirstName(string value)
                {
                    _Parameters.FirstName = value;
                    return this;
                }

                public Builder SetLastName(string value)
                {
                    _Parameters.LastName = value;
                    return this;
                }

                public Builder SetGender(ProfileGender? value)
                {
                    _Parameters.Gender = value;
                    return this;
                }

                public Builder SetBirthday(DateTime? value)
                {
                    _Parameters.Birthday = value;
                    return this;
                }

                public Builder SetEmail(string value)
                {
                    _Parameters.Email = value;
                    return this;
                }

                public Builder SetPhoneNumber(string value)
                {
                    _Parameters.PhoneNumber = value;
                    return this;
                }

                public Builder SetFacebookAnonymousId(string value)
                {
                    _Parameters.FacebookAnonymousId = value;
                    return this;
                }

                public Builder SetAmplitudeUserId(string value)
                {
                    _Parameters.AmplitudeUserId = value;
                    return this;
                }

                public Builder SetAmplitudeDeviceId(string value)
                {
                    _Parameters.AmplitudeDeviceId = value;
                    return this;
                }

                public Builder SetMixpanelUserId(string value)
                {
                    _Parameters.MixpanelUserId = value;
                    return this;
                }

                public Builder SetAppmetricaProfileId(string value)
                {
                    _Parameters.AppmetricaProfileId = value;
                    return this;
                }

                public Builder SetAppmetricaDeviceId(string value)
                {
                    _Parameters.AppmetricaDeviceId = value;
                    return this;
                }

                public Builder SetOneSignalPlayerId(string value)
                {
                    _Parameters.OneSignalPlayerId = value;
                    return this;
                }

                public Builder SetPushwooshHWID(string value)
                {
                    _Parameters.PushwooshHWID = value;
                    return this;
                }

                public Builder SetFirebaseAppInstanceId(string value)
                {
                    _Parameters.FirebaseAppInstanceId = value;
                    return this;
                }

                public Builder SetAppTrackingTransparencyStatus(IOSAppTrackingTransparencyStatus? value)
                {
                    _Parameters.AppTrackingTransparencyStatus = value;
                    return this;
                }

                public Builder SetAnalyticsDisabled(bool? value)
                {
                    _Parameters.AnalyticsDisabled = value;
                    return this;
                }

                public Builder SetCustomStringAttribute(string key, string value)
                {
                    _Parameters.SetCustomStringAttribute(key, value);
                    return this;
                }

                public Builder SetCustomDoubleAttribute(string key, double value)
                {
                    _Parameters.SetCustomDoubleAttribute(key, value);
                    return this;
                }

                public Builder RemoveCustomAttribute(string key)
                {
                    _Parameters.RemoveCustomAttribute(key);
                    return this;
                }

                public ProfileParameters Build() => _Parameters;

            }
        }
    }
}

