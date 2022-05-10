//
//  AdaptyUnityPlugin+Error.swift
//  UnityFramework
//
//  Created by Alexey Valiano on 23.04.2022.
//

import Foundation

extension AdaptyUnityPlugin {
    struct AdaptyErrorResult<T>: Encodable where T: Encodable {
        let error: T
    }
    
    struct PluginError: Error, Encodable {
        enum Code: Int {
            case notFoundProduct = 10001
            case notFoundPaywall = 10002
            case notDeferredPurchase = 10003
        }

        let code: Code
        let message: String

        enum CodingKeys: String, CodingKey {
            case code
            case message
            case domain
            case adaptyCode = "adapty_code"
        }

        public func encode(to encoder: Encoder) throws {
            var container = encoder.container(keyedBy: CodingKeys.self)
            try container.encode(code.rawValue, forKey: .code)
            try container.encode(message, forKey: .message)
            try container.encode("com.adapty.AdaptySDK.UnityPlugin", forKey: .domain)
            try container.encode(code.rawValue, forKey: .adaptyCode)
        }

        static func notFoundProduct(_ productId: String, withVariationId variationId: String?) -> PluginError {
            PluginError(code: .notFoundProduct, message: "Not found product (id: \(productId), variationId: \(variationId ?? "nil"))")
        }
        
        static func notFoundPaywall(byVariationId variationId: String) -> PluginError {
            PluginError(code: .notFoundPaywall, message: "Not found paywall (with variationId: \(variationId ))")
        }
        
        static func notFoundDeferredPurchase(_ productId: String) -> PluginError {
            PluginError(code: .notDeferredPurchase, message: "Not found  deferred purchase for product (id: \(productId))")
        }
    }
}
