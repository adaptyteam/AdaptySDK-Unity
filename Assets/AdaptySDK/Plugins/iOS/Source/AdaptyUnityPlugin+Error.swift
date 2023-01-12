//
//  AdaptyUnityPlugin+Error.swift
//  Adapty
//
//  Created by Aleksei Valiano on 23.04.2022.
//

import Foundation

extension AdaptyUnityPlugin {
    struct PluginError: Error, Encodable {
        enum Code: Int {
            case none = 0
        }

        let errorCode: Code
        let message: String
        let detail: String

        enum CodingKeys: String, CodingKey {
            case errorCode = "adapty_code"
            case message
            case detail
        }

        func encode(to encoder: Encoder) throws {
            var container = encoder.container(keyedBy: CodingKeys.self)
            try container.encode(errorCode.rawValue, forKey: .errorCode)
            try container.encode(message, forKey: .message)
            try container.encode(detail, forKey: .detail)
        }
    }
}
