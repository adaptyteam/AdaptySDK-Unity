import Adapty

extension AdaptyUnityPlugin {
    enum AdaptyResult<T>: Encodable where T: Encodable {
        case success(T?)
        case error(AdaptyError)

        enum CodingKeys: String, CodingKey {
            case success
            case error
        }

        public func encode(to encoder: Encoder) throws {
            var container = encoder.container(keyedBy: CodingKeys.self)
            switch self {
            case let .success(value):
                if let value = value {
                    try container.encode(value, forKey: .success)
                } else {
                    try container.encodeNil(forKey: .success)
                }
            case let .error(value):
                try container.encode(value, forKey: .error)
            }
        }
    }
}
