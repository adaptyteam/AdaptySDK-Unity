import Adapty

protocol StringSerializable {
    init(serializingString value: String) throws
    var serializingString: String { get throws }
}

extension AdaptyLogLevel: StringSerializable, Codable {
    private enum CodingValues: String {
        case none
        case verbose
        case errors
        case all
    }

    init(serializingString value: String) throws {
        guard let value = CodingValues(rawValue: value) else {
            throw DecodingError.dataCorrupted(DecodingError.Context(codingPath: [], debugDescription: "unknown value"))
        }
        switch value {
        case .none: self = .none
        case .verbose: self = .verbose
        case .errors: self = .errors
        case .all: self = .all
        }
    }

    var serializingString: String {
        let value: CodingValues
        switch self {
        case .none: value = .none
        case .errors: value = .errors
        case .verbose: value = .verbose
        case .all: value = .all
        }
        return value.rawValue
    }

    public init(from decoder: Decoder) throws {
        try self.init(serializingString: try decoder.singleValueContainer().decode(String.self))
    }

    public func encode(to encoder: Encoder) throws {
        var container = encoder.singleValueContainer()
        try container.encode(serializingString)
    }
}

extension AdaptyError: Encodable {
    enum CodingKeys: String, CodingKey {
        case code
        case message
        case domain
        case adaptyCode = "adapty_code"
    }

    public func encode(to encoder: Encoder) throws {
        var container = encoder.container(keyedBy: CodingKeys.self)

        try container.encode(code, forKey: .code)
        try container.encode(localizedDescription, forKey: .message)
        try container.encode(domain, forKey: .domain)
        try container.encode(adaptyErrorCode.rawValue, forKey: .adaptyCode)
    }
}

extension AttributionNetwork: StringSerializable, Codable {
    private enum CodingValues: String {
        case adjust
        case appsflyer
        case branch
        case appleSearchAds = "apple_search_ads"
        case custom
    }

    init(serializingString value: String) throws {
        guard let value = CodingValues(rawValue: value) else {
            throw DecodingError.dataCorrupted(DecodingError.Context(codingPath: [], debugDescription: "unknown value"))
        }

        switch value {
        case .adjust: self = .adjust
        case .appsflyer: self = .appsflyer
        case .branch: self = .branch
        case .appleSearchAds: self = .appleSearchAds
        case .custom: self = .custom
        }
    }

    var serializingString: String {
        let value: CodingValues
        switch self {
        case .adjust: value = .adjust
        case .appsflyer: value = .appsflyer
        case .branch: value = .branch
        case .appleSearchAds: value = .appleSearchAds
        case .custom: value = .custom
        }
        return value.rawValue
    }

    public init(from decoder: Decoder) throws {
        try self.init(serializingString: try decoder.singleValueContainer().decode(String.self))
    }

    public func encode(to encoder: Encoder) throws {
        var container = encoder.singleValueContainer()
        try container.encode(serializingString)
    }
}

extension Gender: StringSerializable, Codable {
    private enum CodingValues: String {
        case female = "f"
        case male = "m"
        case other = "o"
    }

    init(serializingString value: String) throws {
        guard let value = CodingValues(rawValue: value) else {
            throw DecodingError.dataCorrupted(DecodingError.Context(codingPath: [], debugDescription: "unknown value"))
        }
        switch value {
        case .female: self = .female
        case .male: self = .male
        case .other: self = .other
        }
    }

    var serializingString: String {
        let value: CodingValues
        switch self {
        case .female: value = .female
        case .male: value = .male
        case .other: value = .other
        }
        return value.rawValue
    }

    public init(from decoder: Decoder) throws {
        try self.init(serializingString: try decoder.singleValueContainer().decode(String.self))
    }

    public func encode(to encoder: Encoder) throws {
        var container = encoder.singleValueContainer()
        try container.encode(serializingString)
    }
}

#if canImport(AppTrackingTransparency)
    import AppTrackingTransparency

    @available(iOS 14, macOS 11.0, *)
    extension ATTrackingManager.AuthorizationStatus: StringSerializable, Codable {
        private enum CodingValues: String {
            case notDetermined = "not_determined"
            case restricted
            case denied
            case authorized
        }

        init(serializingString value: String) throws {
            guard let value = CodingValues(rawValue: value) else {
                throw DecodingError.dataCorrupted(DecodingError.Context(codingPath: [], debugDescription: "unknown value"))
            }
            switch value {
            case .notDetermined: self = .notDetermined
            case .restricted: self = .restricted
            case .denied: self = .denied
            case .authorized: self = .authorized
            }
        }

        var serializingString: String {
            get throws {
                let value: CodingValues
                switch self {
                case .notDetermined: value = .notDetermined
                case .restricted: value = .restricted
                case .denied: value = .denied
                case .authorized: value = .authorized
                @unknown default:
                    throw EncodingError.invalidValue(self, EncodingError.Context(codingPath: [], debugDescription: "unknown value"))
                }
                return value.rawValue
            }
        }

        public init(from decoder: Decoder) throws {
            try self.init(serializingString: try decoder.singleValueContainer().decode(String.self))
        }

        public func encode(to encoder: Encoder) throws {
            var container = encoder.singleValueContainer()
            try container.encode(try serializingString)
        }
    }
#endif
