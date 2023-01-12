//
//  AdaptyUnityPlugin+JSON.swift
//  Adapty
//
//  Created by Aleksei Valiano on 23.12.2022.
//

import Foundation

extension AdaptyUnityPlugin {
    private static var dateFormatter: DateFormatter = {
        let formatter = DateFormatter()
        formatter.calendar = Calendar(identifier: .iso8601)
        formatter.locale = Locale(identifier: "en_US_POSIX")
        formatter.dateFormat = "yyyy-MM-dd'T'HH:mm:ss.SSSZ"
        return formatter
    }()

    private static let encoder: JSONEncoder = {
        let encoder = JSONEncoder()
        encoder.dateEncodingStrategy = .formatted(dateFormatter)
        encoder.dataEncodingStrategy = .base64
        return encoder
    }()

    private static let decoder: JSONDecoder = {
        let decoder = JSONDecoder()
        decoder.dateDecodingStrategy = .formatted(dateFormatter)
        decoder.dataDecodingStrategy = .base64
        return decoder
    }()

    static func encode<T>(_ value: T) throws -> Data where T: Encodable {
        try encoder.encode(value)
    }

    static func encode<T>(_ value: T) throws -> String where T: Encodable {
        String(decoding: try encode(value), as: UTF8.self)
    }

    static func decode<T>(_ type: T.Type, from data: Data) throws -> T where T: Decodable {
        try decoder.decode(T.self, from: data)
    }
}
