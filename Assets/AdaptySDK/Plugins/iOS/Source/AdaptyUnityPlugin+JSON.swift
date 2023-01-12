//
//  AdaptyUnityPlugin+JSON.swift
//  Adapty
//
//  Created by Aleksei Valiano on 23.12.2022.
//

import Adapty
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

    static let decoder: JSONDecoder = {
        let decoder = JSONDecoder()
        decoder.dateDecodingStrategy = .formatted(dateFormatter)
        decoder.dataDecodingStrategy = .base64
        return decoder
    }()

    static func encodeToData<T>(_ value: T) throws -> Data where T: Encodable {
        try encoder.encode(value)
    }

    static func encodeToString<T>(_ value: T) throws -> String where T: Encodable {
        String(decoding: try encodeToData(value), as: UTF8.self)
    }

    static func encodeToString<Success: Encodable>(result: AdaptyResult<Success>) -> String {
        encodeToString(result: PluginResult<Success, AdaptyError>.from(result))
    }

    static func encodeToString<Success: Encodable, Failure: Error & Encodable>(result: AdaptyUnityPlugin.PluginResult<Success, Failure>) -> String {
        do {
            return try encodeToString(result)
        } catch {
            return try! encodeToString(PluginError.encodingFailed(error))
        }
    }

    static func encodeToString<Failure: Error & Encodable>(result error: Failure?) -> String {
        encodeToString(result: PluginResult<Bool, AdaptyError>.from(error))
    }

    static func decode<T>(_ type: T.Type, from data: Data) throws -> T where T: Decodable {
        try decoder.decode(type, from: data)
    }

    static func decode<T>(_ type: T.Type, from str: String) throws -> T where T: Decodable {
        try decode(type, from: str.data(using: .utf8) ?? Data())
    }
}
