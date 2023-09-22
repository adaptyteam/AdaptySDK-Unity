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
    
    @objc public func setBackendEnvironment(_ url: URL) {
        Adapty.setBackendEnvironment(baseUrl: url)
    }

    @objc public func activate(_ apikey: String,
                               observerMode: Bool,
                               enableUsageLogs: Bool,
                               storeKit2UsageString: String?) {
        let storeKit2Usage: StoreKit2Usage
        
        if let storeKit2UsageString = storeKit2UsageString {
            switch storeKit2UsageString {
            case "intro_eligibility_check":
                storeKit2Usage = .forIntroEligibilityCheck
            default:
                storeKit2Usage = .disabled
            }
        } else {
            storeKit2Usage = .disabled
        }
        
        Adapty.activate(apikey,
                        observerMode: observerMode,
                        enableUsageLogs: enableUsageLogs,
                        storeKit2Usage: storeKit2Usage)
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

    @objc public func getPaywallProducts(_ paywallJson: String, completion: JSONStringCompletion? = nil) {
        let paywall: AdaptyPaywall
        do {
            paywall = try AdaptyUnityPlugin.decode(AdaptyPaywall.self, from: paywallJson)
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.getPaywallProducts(paywall: paywall) { result in
            completion?(AdaptyUnityPlugin.encodeToString(result: result))
        }
    }

    @objc public func getProductsIntroductoryOfferEligibility(_ arrayJson: String, completion: JSONStringCompletion? = nil) {
        let vendorProductIds: [String]
        do {
            vendorProductIds = try AdaptyUnityPlugin.decode([String].self, from: arrayJson)
        } catch {
            let error = PluginError.decodingFailed(error)
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
            return
        }

        Adapty.getProductsIntroductoryOfferEligibility(vendorProductIds: vendorProductIds) { result in
            let resultString = AdaptyUnityPlugin.encodeToString(result: result)
            completion?(resultString)
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
                    completion?(AdaptyUnityPlugin.encodeToString(result: result.map { $0.profile }))
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

    @objc public func setVariationForTransaction(_ parameters: String, completion: JSONStringCompletion? = nil) {
        Adapty.setVariationId(from: AdaptyUnityPlugin.decoder, data: parameters.data(using: .utf8) ?? Data()) { error in
            completion?(AdaptyUnityPlugin.encodeToString(result: error))
        }
    }

    @objc public func presentCodeRedemptionSheet() {
        Adapty.presentCodeRedemptionSheet()
    }
}
