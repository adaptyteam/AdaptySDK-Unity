using System;
using System.Collections.Generic;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public class ProfileParameterBuilder
        {
            private Dictionary<string, dynamic> m_Params = new Dictionary<string, dynamic>();

            public string Email
            {
                get { return m_Params["email"] as string;  }
                set { m_Params["email"] = value; }
            }

            public string PhoneNumber
            {
                get { return m_Params["phone_number"] as string ; }
                set { m_Params["phone_number"] = value; }
            }

            public string FacebookUserId
            {
                get { return m_Params["facebook_user_id"] as string; }
                set { m_Params["facebook_user_id"] = value; }
            }

            public string FacebookAnonymousId
            {
                get { return m_Params["facebook_anonymous_id"] as string; }
                set { m_Params["facebook_anonymous_id"] = value; }
            }

            public string AmplitudeUserId
            {
                get { return m_Params["amplitude_user_id"] as string; }
                set { m_Params["amplitude_user_id"] = value; }
            }

            public string AmplitudeDeviceId
            {
                get { return m_Params["amplitude_device_id"] as string; }
                set { m_Params["amplitude_device_id"] = value; }
            }

            public string MixpanelUserId
            {
                get { return m_Params["mixpanel_user_id"] as string; }
                set { m_Params["mixpanel_user_id"] = value; }
            }

            public string AppmetricaProfileId
            {
                get { return m_Params["appmetrica_profile_id"] as string; }
                set { m_Params["appmetrica_profile_id"] = value; }
            }

            public string AppmetricaDeviceId
            {
                get { return m_Params["appmetrica_device_id"] as string; }
                set { m_Params["appmetrica_device_id"] = value; }
            }

            public string FirstName
            {
                get { return m_Params["first_name"] as string; }
                set { m_Params["first_name"] = value; }
            }

            public string LastName
            {
                get { return m_Params["last_name"] as string; }
                set { m_Params["last_name"] = value; }
            }

            public Gender? Gender
            {
                get
                {
                    var v = m_Params["gender"] as string;
                    return v is null ? null : (Gender?)GenderFromString(v);
                }
                set
                {
                    if (value.HasValue)
                    {
                        m_Params["gender"] = GenderToString(value.Value);
                    }
                    else
                    {
                        m_Params.Remove("gender");
                    }
                }
            }

            public string Birthday
            {
                get { return m_Params["birthday"] as string; }
                set { m_Params["birthday"] = value; }
            }

            public void SetBirthday(int year, int month, int day )
            {
                m_Params["birthday"] = $"{year}-{month}-{day}"; 
            }

            public Dictionary<string, dynamic> CustomAttributes
            {
                get { return m_Params["custom_attributes"] as Dictionary<string, dynamic>; }
                set { m_Params["custom_attributes"] = value; }
            }

            /// iOS 14 and newer
            public AppTrackingTransparency? AppTrackingTransparencyStatus
            {
                get {
                    var v = m_Params["att_status"] as string;
                    return v is null ? null : (AppTrackingTransparency?)AppTrackingTransparencyFromString(v);
                }
                set {
                    if (value.HasValue)
                    {
                        m_Params["att_status"] = AppTrackingTransparencyToString(value.Value);
                    } else
                    {
                        m_Params.Remove("att_status");
                    }
                }
            }

            public override string ToString()
            {
                return $"{nameof(Email)}: {Email}, " +
                       $"{nameof(PhoneNumber)}: {PhoneNumber}, " +
                       $"{nameof(FacebookUserId)}: {FacebookUserId}, " +
                       $"{nameof(FacebookAnonymousId)}: {FacebookAnonymousId}, " +
                       $"{nameof(AmplitudeUserId)}: {AmplitudeUserId}, " +
                       $"{nameof(AmplitudeDeviceId)}: {AmplitudeDeviceId}, " +
                       $"{nameof(MixpanelUserId)}: {MixpanelUserId}, " +
                       $"{nameof(AppmetricaProfileId)}: {AppmetricaProfileId}, " +
                       $"{nameof(AppmetricaDeviceId)}: {AppmetricaDeviceId}, " +
                       $"{nameof(FirstName)}: {FirstName}, " +
                       $"{nameof(LastName)}: {LastName}, " +
                       $"{nameof(Gender)}: {Gender}, " +
                       $"{nameof(Birthday)}: {Birthday}, " +
                       $"{nameof(CustomAttributes)}: {CustomAttributes}, " +
                       $"{nameof(AppTrackingTransparencyStatus)}: {AppTrackingTransparencyStatus}";
            }

            public string ToJSONString()
            {
                try
                {
                    JSONNode node = DictionaryExtensions.ToJSON(m_Params);
                    return node.ToString();
                }
                catch (Exception e)
                {
                    Debug.LogError($"Exception on encoding ProfileParameterBuilder: {e} source: {ToString()}");
                    return null;
                }
            }
        }
    }
}

