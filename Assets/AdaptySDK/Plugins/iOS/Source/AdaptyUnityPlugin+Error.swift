//
//  AdaptyUnityPlugin+Error.swift
//  Adapty
//
//  Created by Aleksei Valiano on 23.04.2022.
//

import Adapty
import Foundation

extension AdaptyUnityPlugin {
    struct PluginError: Error, Encodable {
        let errorCode: Int
        let message: String
        let detail: String

        enum CodingKeys: String, CodingKey {
            case errorCode = "adapty_code"
            case message
            case detail
        }

        func encode(to encoder: Encoder) throws {
            var container = encoder.container(keyedBy: CodingKeys.self)
            try container.encode(errorCode, forKey: .errorCode)
            try container.encode(message, forKey: .message)
            try container.encode(detail, forKey: .detail)
        }

        static func encodingFailed(_ error: String) -> PluginError {
            PluginError(errorCode: AdaptyErrorCode.encodingFailed.rawValue,
                        message: "Encoding failed",
                        detail: "AdaptyPluginError.encodingFailed(\(error))")
        }

        static func encodingFailed(_ error: Error) -> PluginError {
            encodingFailed("\(error)")
        }

        static func decodingFailed(_ error: String) -> PluginError {
            PluginError(errorCode: AdaptyErrorCode.decodingFailed.rawValue,
                        message: "Decoding failed",
                        detail: "AdaptyPluginError.decodingFailed(\(error))")
        }

        static func decodingFailed(_ error: Error) -> PluginError {
            decodingFailed("\(error)")
        }
    }
}
