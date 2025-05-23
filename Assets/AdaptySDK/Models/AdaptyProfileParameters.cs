﻿//
//  AdaptyProfileParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AdaptySDK
{
    public partial class AdaptyProfileParameters
    {
        public string FirstName;
        public string LastName;
        public AdaptyProfileGender? Gender;
        public DateTime? Birthday;
        public string Email;
        public string PhoneNumber;


        public AppTrackingTransparencyStatus? AppTrackingTransparencyStatus;
        public bool? AnalyticsDisabled;

        private Dictionary<string, dynamic> _CustomAttributes = new Dictionary<string, dynamic>();

        public Dictionary<string, dynamic> CustomAttributes => _CustomAttributes;

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