//
//  AdaptyUnityPlugin+Result.swift
//  Adapty
//
//  Created by Aleksei Valiano on 12.01.2023
//
//

import Adapty
import Foundation

extension AdaptyUnityPlugin {
    @frozen enum PluginResult<Success: Encodable, Failure: Error & Encodable>: Encodable {
        case success(Success)
        case failure(Failure)

        enum CodingKeys: String, CodingKey {
            case success
            case error
        }

        func encode(to encoder: Encoder) throws {
            var container = encoder.container(keyedBy: CodingKeys.self)
            switch self {
            case let .success(value):
                try container.encode(value, forKey: .success)
            case let .failure(value):
                try container.encode(value, forKey: .error)
            }
        }

        static func from<Success: Encodable>(_ result: AdaptyResult<Success>) -> PluginResult<Success, AdaptyError> {
            switch result {
            case let .failure(error):
                return .failure(error)
            case let .success(value):
                return .success(value)
            }
        }

        static func from<Failure: Error & Encodable>(_ error: Failure?) -> PluginResult<Bool, Failure> {
            if let error = error {
                return .failure(error)
            } else {
                return .success(true)
            }
        }
    }
}
