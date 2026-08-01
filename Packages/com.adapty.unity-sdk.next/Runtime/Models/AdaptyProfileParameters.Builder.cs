//
//  AdaptyProfileParameters.Builder.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{

    public partial class AdaptyProfileParameters
    {
        public class Builder
        {
            private AdaptyProfileParameters _Parameters = new AdaptyProfileParameters();

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

            public Builder SetGender(AdaptyProfileGender? value)
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


            public Builder SetAppTrackingTransparencyStatus(AppTrackingTransparencyStatus? value)
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

            public AdaptyProfileParameters Build() => _Parameters;
        }
    }
}