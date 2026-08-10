using UnityEngine.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    public sealed partial class AdaptyProfileParameters
    {
        [DataMember(Name = "first_name")]
        public string FirstName;
        [DataMember(Name = "last_name")]
        public string LastName;
        [DataMember(Name = "gender")]
        public AdaptyProfileGender? Gender;
        public DateTime? Birthday;
        [DataMember(Name = "email")]
        public string Email;
        [DataMember(Name = "phone_number")]
        public string PhoneNumber;


        #if UNITY_IOS
        [DataMember(Name = "att_status")]
#endif
        public AppTrackingTransparencyStatus? AppTrackingTransparencyStatus;
        [DataMember(Name = "analytics_disabled")]
        public bool? AnalyticsDisabled;

        private Dictionary<string, object> _CustomAttributes = new Dictionary<string, object>();

        [Preserve]
        public IReadOnlyDictionary<string, object> CustomAttributes =>
            new ReadOnlyDictionary<string, object>(_CustomAttributes);


        /// <remarks>
        /// The contract wants a plain calendar date here, not the timestamp format of the other
        /// dates, so this one is written by hand rather than through the date converter.
        /// </remarks>
        [DataMember(Name = "birthday")]
        [Preserve]
        private string BirthdayForRequest =>
            Birthday?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        [DataMember(Name = "custom_attributes")]
        [Preserve]
        private System.Collections.Generic.Dictionary<string, object> CustomAttributesForRequest =>
            _CustomAttributes.Count > 0 ? _CustomAttributes : null;

        public void SetCustomStringAttribute(string key, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 50)
            {
                throw new Exception($"The value must not be empty and not more than 50 characters.");
            }
            if (!_validateCustomAttributeKey(key, true))
            {
                return;
            }
            _CustomAttributes[key] = value;

        }

        public void SetCustomDoubleAttribute(string key, double value)
        {
            if (!_validateCustomAttributeKey(key, true))
            {
                return;
            }
            _CustomAttributes[key] = value;
        }

        public void RemoveCustomAttribute(string key)
        {
            if (!_validateCustomAttributeKey(key, false))
            {
                return;
            }
            _CustomAttributes[key] = null;
        }

        bool _validateCustomAttributeKey(String addingKey, bool testCount)
        {

            if (string.IsNullOrEmpty(addingKey) || addingKey.Length > 30 || !Regex.IsMatch(addingKey, "^[A-Za-z0-9._-]+$"))
            {
                throw new Exception("The key must be string not more than 30 characters. Only letters, numbers, dashes, points and underscores allowed");
            }

            if (!testCount)
            {
                return true;
            }

            var count = 1;
            foreach (var item in _CustomAttributes)
            {
                if (item.Value is null || item.Key == addingKey) continue;
                count += 1;
            }

            if (count > 30)
            {
                throw new Exception("The total number of custom attributes must be no more than 30");
            }

            return true;
        }

        public override string ToString() =>
            $"{nameof(FirstName)}: {FirstName}, " +
            $"{nameof(LastName)}: {LastName}, " +
            $"{nameof(Gender)}: {Gender}, " +
            $"{nameof(Birthday)}: {Birthday}, " +
            $"{nameof(Email)}: {Email}, " +
            $"{nameof(PhoneNumber)}: {PhoneNumber}, " +
            $"{nameof(AppTrackingTransparencyStatus)}: {AppTrackingTransparencyStatus}, " +
            $"{nameof(AnalyticsDisabled)}: {AnalyticsDisabled}, " +
            $"{nameof(CustomAttributes)}: {CustomAttributes}";
    }

}