import Foundation

extension AdaptyUnityPlugin {
    private static let encoder: JSONEncoder = {
        let encoder = JSONEncoder()
        encoder.dateEncodingStrategy = .iso8601
        encoder.keyEncodingStrategy = .convertToSnakeCase
        return encoder
    }()
    private static let decoder: JSONDecoder = {
        let decoder = JSONDecoder()
        decoder.dateDecodingStrategy = .iso8601
        decoder.keyDecodingStrategy = .convertFromSnakeCase
        return decoder
    }()

    static func toJSONData<T>(_ value: T?) -> Data? where T: Encodable {
        guard let value = value else { return nil }

        do {
            return try AdaptyUnityPlugin.encoder.encode(value)
        } catch {
            // TODO
            return nil
        }
    }

    static func toJSONString<T>(_ value: T?) -> String? where T: Encodable {
        guard let data = toJSONData(value) else { return nil }
        return String(decoding: data, as: UTF8.self)
    }

    static func toDictionary(_ value: Data?) -> [String: Any]? {
        guard let value = value else { return nil }
        do {
            return try JSONSerialization.jsonObject(with: value, options: []) as? [String: Any]
        } catch {
            // TODO
            return nil
        }
    }

    static func toDictionary(_ value: String?) -> [String: Any]? {
        toDictionary(value?.data(using: .utf8))
    }
}
