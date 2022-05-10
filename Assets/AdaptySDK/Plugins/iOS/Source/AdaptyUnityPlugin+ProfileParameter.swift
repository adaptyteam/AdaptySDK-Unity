
import Adapty
import Foundation
#if canImport(AppTrackingTransparency)
    import AppTrackingTransparency
#endif

extension Parameters {
    enum CodingKeys: String {
        case email
        case phoneNumber = "phone_number"
        case facebookUserId = "facebook_user_id"
        case facebookAnonymousId = "facebook_anonymous_id"
        case amplitudeUserId = "amplitude_user_id"
        case amplitudeDeviceId = "amplitude_device_id"
        case mixpanelUserId = "mixpanel_user_id"
        case appmetricaProfileId = "appmetrica_profile_id"
        case appmetricaDeviceId = "appmetrica_device_id"
        case firstName = "first_name"
        case lastName = "last_name"
        case gender
        case birthday
        case customAttributes = "custom_attributes"
        case appTrackingTransparencyStatus = "att_status"
    }

    subscript(_ i: CodingKeys) -> Any? {
        get { self[i.rawValue] }
        set { self[i.rawValue] = newValue }
    }
}

extension ProfileParameterBuilder {
    @objc func withBirthday(_ value: String) -> Self {
        let dateFormatter = DateFormatter()
        dateFormatter.dateFormat = "yyyy-MM-dd"
        dateFormatter.timeZone = TimeZone(identifier: "UTC")
        if let birthday = dateFormatter.date(from: value) {
            _ = withBirthday(birthday)
        }
        return self
    }

    @objc func with(_ value: Parameters) -> Self {
        if let value = value[.email] as? String {
            _ = withEmail(value)
        }

        if let value = value[.phoneNumber] as? String {
            _ = withPhoneNumber(value)
        }

        if let value = value[.facebookUserId] as? String {
            _ = withFacebookUserId(value)
        }

        if let value = value[.facebookAnonymousId] as? String {
            _ = withFacebookAnonymousId(value)
        }

        if let value = value[.amplitudeUserId] as? String {
            _ = withAmplitudeUserId(value)
        }

        if let value = value[.amplitudeDeviceId] as? String {
            _ = withAmplitudeDeviceId(value)
        }

        if let value = value[.mixpanelUserId] as? String {
            _ = withMixpanelUserId(value)
        }

        if let value = value[.appmetricaProfileId] as? String {
            _ = withAppmetricaProfileId(value)
        }

        if let value = value[.appmetricaDeviceId] as? String {
            _ = withAppmetricaDeviceId(value)
        }

        if let value = value[.firstName] as? String {
            _ = withFirstName(value)
        }

        if let value = value[.lastName] as? String {
            _ = withLastName(value)
        }

        if let string = value[.gender] as? String,
           let value = try? Gender(serializingString: string) {
            _ = withGender(value)
        }

        if let value = value[.birthday] as? String {
            _ = withBirthday(value)
        }

        if let value = value[.customAttributes] as? Parameters {
            _ = withCustomAttributes(value)
        }

        if #available(iOS 14, macOS 11.0, tvOS 14, *),
           let string = value[.appTrackingTransparencyStatus] as? String,
           let value = try? ATTrackingManager.AuthorizationStatus(serializingString: string) {
            _ = withAppTrackingTransparencyStatus(value)
        }
        
        return self
    }
}
