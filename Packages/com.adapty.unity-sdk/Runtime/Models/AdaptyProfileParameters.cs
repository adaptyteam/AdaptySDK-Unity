using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The profile attributes <see cref="Adapty.UpdateProfile(AdaptySDK.AdaptyProfileParameters, System.Action{AdaptySDK.AdaptyError})"/> sends. Build one with
    /// <see cref="Builder"/>.
    /// </summary>
    /// <remarks>
    /// Only what is set is sent — a field left null is not cleared on the server, it is left
    /// alone. To clear a custom attribute use <see cref="RemoveCustomAttribute"/>, which sends an
    /// explicit removal.
    /// </remarks>
    [DataContract]
    public sealed partial class AdaptyProfileParameters
    {
        /// <summary>
        /// The user's first name. Null leaves whatever the profile already has.
        /// </summary>
        [DataMember(Name = "first_name")]
        public string FirstName;
        /// <summary>
        /// The user's last name. Null leaves whatever the profile already has.
        /// </summary>
        [DataMember(Name = "last_name")]
        public string LastName;
        /// <summary>
        /// The user's gender. Null leaves whatever the profile already has.
        /// </summary>
        [DataMember(Name = "gender")]
        public AdaptyProfileGender? Gender;
        /// <summary>
        /// The user's date of birth. Sent as a calendar date — <c>yyyy-MM-dd</c> — so the time of
        /// day and the <see cref="DateTimeKind"/> are ignored, unlike the dates the SDK hands back.
        /// </summary>
        public DateTime? Birthday;
        /// <summary>
        /// The user's email address. Null leaves whatever the profile already has.
        /// </summary>
        [DataMember(Name = "email")]
        public string Email;
        /// <summary>
        /// The user's phone number. Null leaves whatever the profile already has.
        /// </summary>
        [DataMember(Name = "phone_number")]
        public string PhoneNumber;


        /// <summary>
        /// iOS only. What the user answered to the App Tracking Transparency prompt. Sent on iOS
        /// alone — the contract has no such key for Android.
        /// </summary>
#if UNITY_IOS
        [DataMember(Name = "att_status")]
#endif
        public AppTrackingTransparencyStatus? AppTrackingTransparencyStatus;
        /// <summary>
        /// Switches analytics off for this profile. Calls that need analytics then fail with
        /// <see cref="AdaptyErrorCode.AnalyticsDisabled"/>.
        /// </summary>
        [DataMember(Name = "analytics_disabled")]
        public bool? AnalyticsDisabled;

        private Dictionary<string, object> _CustomAttributes = new Dictionary<string, object>();

        /// <summary>
        /// The custom attributes set so far, as a read-only view. A key removed with
        /// <see cref="RemoveCustomAttribute"/> is present here with a null value, which is what
        /// tells the server to clear it.
        /// </summary>
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

        /// <summary>
        /// Sets a custom attribute to a string value.
        /// </summary>
        /// <param name="key">
        /// Up to 30 characters of letters, digits, dashes, points and underscores.
        /// </param>
        /// <param name="value">Between 1 and 50 characters.</param>
        /// <exception cref="ArgumentException">
        /// The key or the value breaks those limits, or the profile would end up with more than 30
        /// custom attributes.
        /// </exception>
        public void SetCustomStringAttribute(string key, string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length > 50)
            {
                throw new ArgumentException(
                    "The value must not be empty and not more than 50 characters.",
                    nameof(value)
                );
            }
            _validateCustomAttributeKey(key, true);
            _CustomAttributes[key] = value;
        }

        /// <summary>
        /// Sets a custom attribute to a numeric value.
        /// </summary>
        /// <param name="key">
        /// Up to 30 characters of letters, digits, dashes, points and underscores.
        /// </param>
        /// <param name="value">The value to store.</param>
        /// <exception cref="ArgumentException">
        /// The key breaks those limits, or the profile would end up with more than 30 custom
        /// attributes.
        /// </exception>
        public void SetCustomDoubleAttribute(string key, double value)
        {
            _validateCustomAttributeKey(key, true);
            _CustomAttributes[key] = value;
        }

        /// <summary>
        /// Clears a custom attribute. The key is sent with a null value rather than left out, so
        /// the server removes it instead of leaving it as it was.
        /// </summary>
        /// <param name="key">The key to clear.</param>
        /// <exception cref="ArgumentException">The key is not a valid custom attribute key.</exception>
        public void RemoveCustomAttribute(string key)
        {
            _validateCustomAttributeKey(key, false);
            _CustomAttributes[key] = null;
        }

        void _validateCustomAttributeKey(String addingKey, bool testCount)
        {
            if (string.IsNullOrEmpty(addingKey) || addingKey.Length > 30 || !Regex.IsMatch(addingKey, "^[A-Za-z0-9._-]+$"))
            {
                throw new ArgumentException("The key must be string not more than 30 characters. Only letters, numbers, dashes, points and underscores allowed");
            }

            if (!testCount)
            {
                return;
            }

            var count = 1;
            foreach (var item in _CustomAttributes)
            {
                if (item.Value is null || item.Key == addingKey) continue;
                count += 1;
            }

            if (count > 30)
            {
                throw new ArgumentException("The total number of custom attributes must be no more than 30");
            }
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(FirstName)}: {FirstName}, " +
            $"{nameof(LastName)}: {LastName}, " +
            $"{nameof(Gender)}: {Gender}, " +
            $"{nameof(Birthday)}: {Birthday}, " +
            $"{nameof(Email)}: {Email}, " +
            $"{nameof(PhoneNumber)}: {PhoneNumber}, " +
            $"{nameof(AppTrackingTransparencyStatus)}: {AppTrackingTransparencyStatus}, " +
            $"{nameof(AnalyticsDisabled)}: {AnalyticsDisabled}, " +
            $"{nameof(CustomAttributes)}: " +
            "{" + string.Join(", ", CustomAttributes.Select(kv => $"{kv.Key}: {kv.Value}")) + "}";
    }

}
