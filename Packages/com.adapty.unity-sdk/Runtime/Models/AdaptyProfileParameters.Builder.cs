using System;
using UnityEngine.Scripting;

namespace AdaptySDK
{

    [Preserve]
    public sealed partial class AdaptyProfileParameters
    {
        /// <summary>
        /// Assembles an <see cref="AdaptyProfileParameters"/>. Every setter returns the builder,
        /// so calls chain; what is never set is not sent, and is therefore left as it is on the
        /// server rather than cleared.
        /// </summary>
        public sealed class Builder
        {
            private AdaptyProfileParameters _Parameters = new AdaptyProfileParameters();

            /// <summary>
            /// Sets <see cref="FirstName"/>.
            /// </summary>
            /// <param name="value">The user's first name.</param>
            public Builder SetFirstName(string value)
            {
                _Parameters.FirstName = value;
                return this;
            }

            /// <summary>
            /// Sets <see cref="LastName"/>.
            /// </summary>
            /// <param name="value">The user's last name.</param>
            public Builder SetLastName(string value)
            {
                _Parameters.LastName = value;
                return this;
            }

            /// <summary>
            /// Sets <see cref="Gender"/>.
            /// </summary>
            /// <param name="value">The user's gender.</param>
            public Builder SetGender(AdaptyProfileGender? value)
            {
                _Parameters.Gender = value;
                return this;
            }

            /// <summary>
            /// Sets <see cref="Birthday"/>. Sent as a calendar date, so the time of day is ignored.
            /// </summary>
            /// <param name="value">The user's date of birth.</param>
            public Builder SetBirthday(DateTime? value)
            {
                _Parameters.Birthday = value;
                return this;
            }

            /// <summary>
            /// Sets <see cref="Email"/>.
            /// </summary>
            /// <param name="value">The user's email address.</param>
            public Builder SetEmail(string value)
            {
                _Parameters.Email = value;
                return this;
            }

            /// <summary>
            /// Sets <see cref="PhoneNumber"/>.
            /// </summary>
            /// <param name="value">The user's phone number.</param>
            public Builder SetPhoneNumber(string value)
            {
                _Parameters.PhoneNumber = value;
                return this;
            }


            /// <summary>
            /// Sets <see cref="AppTrackingTransparencyStatus"/>. iOS only.
            /// </summary>
            /// <param name="value">What the user answered to the tracking prompt.</param>
            public Builder SetAppTrackingTransparencyStatus(AppTrackingTransparencyStatus? value)
            {
                _Parameters.AppTrackingTransparencyStatus = value;
                return this;
            }

            /// <summary>
            /// Sets <see cref="AnalyticsDisabled"/>.
            /// </summary>
            /// <param name="value">True to switch analytics off for this profile.</param>
            public Builder SetAnalyticsDisabled(bool? value)
            {
                _Parameters.AnalyticsDisabled = value;
                return this;
            }

            /// <summary>
            /// Sets a custom attribute to a string value. Same limits as
            /// <see cref="AdaptyProfileParameters.SetCustomStringAttribute"/>, and the same
            /// exception when they are broken.
            /// </summary>
            /// <param name="key">Up to 30 characters of letters, digits, dashes, points and underscores.</param>
            /// <param name="value">Between 1 and 50 characters.</param>
            public Builder SetCustomStringAttribute(string key, string value)
            {
                _Parameters.SetCustomStringAttribute(key, value);
                return this;
            }

            /// <summary>
            /// Sets a custom attribute to a numeric value. Same limits as
            /// <see cref="AdaptyProfileParameters.SetCustomDoubleAttribute"/>.
            /// </summary>
            /// <param name="key">Up to 30 characters of letters, digits, dashes, points and underscores.</param>
            /// <param name="value">The value to store.</param>
            public Builder SetCustomDoubleAttribute(string key, double value)
            {
                _Parameters.SetCustomDoubleAttribute(key, value);
                return this;
            }

            /// <summary>
            /// Clears a custom attribute, the way
            /// <see cref="AdaptyProfileParameters.RemoveCustomAttribute"/> does — sent as an
            /// explicit removal rather than left out.
            /// </summary>
            /// <param name="key">The key to clear.</param>
            public Builder RemoveCustomAttribute(string key)
            {
                _Parameters.RemoveCustomAttribute(key);
                return this;
            }

            /// <summary>
            /// The parameters described by this builder.
            /// </summary>
            public AdaptyProfileParameters Build() => _Parameters;
        }
    }
}
