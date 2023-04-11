import Adapty

@objc public class AdaptyUnityPlugin: NSObject {
    @objc public static let shared = AdaptyUnityPlugin()

    public typealias JSONStringCompletion = (String?) -> Void

    override public init() {
        super.init()
        Adapty.delegate = AdaptyUnityPlugin.delegate
    }

    @objc public func setIdfaCollectionDisabled(_ value: Bool) {
        Adapty.idfaCollectionDisabled = value
    }

    @objc public func activate(_ apikey: String, observerMode: Bool) {
        Adapty.activate(apikey, observerMode: observerMode) { error in
            guard let error = error else { return }
        }
    }

    @objc public func getLogLevel() -> String {
        Adapty.logLevel.rawStringValue
    }

    @objc public func setCrossPlatformSDK(_ name: String, version: String) {
        Adapty.setCrossPlatformSDK(version: version, name: name) 
    }

    @objc public func setLogLevel(_ value: String) {
        guard let logLevel = AdaptyLogLevel(rawStringValue: value) else {
            let error = PluginError.decodingFailed("Unknown log level value: \(value)")
            return
        }
        Adapty.logLevel = logLevel
    }

    @objc public func identify(_ customerUserId: String, completion: JSONStringCompletion?) {
        Adapty.identify(customerUserId) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func logout(_ completion: JSONStringCompletion? = nil) {
        Adapty.logout { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func getPaywall(_ id: String, locale: String, completion: JSONStringCompletion? = nil) {
        Adapty.getPaywall(id, locale: locale) { result in
            completion?(AdaptyUnityPlugin.encodeToString(result: result))
        }
    }

    @objc public func getPaywallProducts(_ paywallJson: String, fetchPolicy: String, completion: JSONStringCompletion? = nil) {
        let paywall: AdaptyPaywall
        do {
            paywall = try AdaptyUnityPlugin.decode(AdaptyPaywall.self, from: paywallJson)
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.getPaywallProducts(paywall: paywall, fetchPolicy: fetchPolicy == "wait_for_receipt_validation" ? .waitForReceiptValidation : .default) { result in
            completion?(AdaptyUnityPlugin.encodeToString(result: result))
        }
    }

    @objc public func getProfile(completion: JSONStringCompletion? = nil) {
        Adapty.getProfile { result in
            completion?(AdaptyUnityPlugin.encodeToString(result: result))
        }
    }

    @objc public func restorePurchases(_ completion: JSONStringCompletion? = nil) {
        Adapty.restorePurchases { result in
            completion?(AdaptyUnityPlugin.encodeToString(result: result))
        }
    }

    @objc public func makePurchase(_ productJson: String, completion: JSONStringCompletion? = nil) {
        Adapty.getPaywallProduct(from: AdaptyUnityPlugin.decoder, data: productJson.data(using: .utf8) ?? Data()) { result in
            switch result {
            case let .failure(error):
                completion?(AdaptyUnityPlugin.encodeToString(result: error))
            case let .success(product):
                Adapty.makePurchase(product: product) { result in
                    completion?(AdaptyUnityPlugin.encodeToString(result: result))
                }
            }
        }
    }

    @objc public func logShowPaywall(_ paywallJson: String, completion: JSONStringCompletion? = nil) {
        let paywall: AdaptyPaywall
        do {
            paywall = try AdaptyUnityPlugin.decode(AdaptyPaywall.self, from: paywallJson)
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.logShowPaywall(paywall) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func logShowOnboarding(_ paramJson: String, completion: JSONStringCompletion? = nil) {
        let parameters: AdaptyOnboardingScreenParameters
        do {
            parameters = try AdaptyUnityPlugin.decode(AdaptyOnboardingScreenParameters.self, from: paramJson)
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.logShowOnboarding(parameters) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func setFallbackPaywalls(_ paywalls: String, completion: JSONStringCompletion? = nil) {
        Adapty.setFallbackPaywalls(paywalls.data(using: .utf8) ?? Data()) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func updateProfile(_ paramJson: String, completion: JSONStringCompletion? = nil) {
        let parameters: AdaptyProfileParameters
        do {
            parameters = try AdaptyUnityPlugin.decode(AdaptyProfileParameters.self, from: paramJson)
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.updateProfile(params: parameters) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func updateAttribution(_ jsonString: String, source sourceString: String, networkUserId: String?, completion: JSONStringCompletion? = nil) {
        let attribution: [AnyHashable: Any]
        do {
            attribution = try AdaptyUnityPlugin.decodeToDictionary(jsonString) ?? [:]
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        guard let source = AdaptyAttributionSource(rawValue: sourceString) else {
            let error = PluginError.decodingFailed("AdaptyAttributionSource unknown value: \(sourceString)")
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.updateAttribution(attribution, source: source, networkUserId: networkUserId) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func setVariationId(_ variationId: String, forTransactionId: String, completion: JSONStringCompletion? = nil) {
        Adapty.setVariationId(variationId, forTransactionId: forTransactionId) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func presentCodeRedemptionSheet() {
        Adapty.presentCodeRedemptionSheet()
    }
}
