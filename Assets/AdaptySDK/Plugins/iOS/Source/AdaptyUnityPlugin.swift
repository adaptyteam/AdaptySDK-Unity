import Adapty

@objc public class AdaptyUnityPlugin: NSObject {
    @objc public static let shared = AdaptyUnityPlugin()

    public typealias JSONStringCompletion = (String?) -> Void
    
    public override init() {
        super.init()
        Adapty.delegate = AdaptyUnityPlugin.delegate
    }
    
    @objc public func setIdfaCollectionDisabled(_ value: Bool) {
        Adapty.idfaCollectionDisabled = value
    }
    
    
    @objc public func activate(_ apikey: String, observerMode: Bool, customerUserId: String?) {
        Adapty.activate(apikey, observerMode: observerMode, customerUserId: customerUserId)
    }

    @objc public func getLogLevel() -> String {
        Adapty.logLevel.serializingString
    }

    @objc public func setLogLevel(_ value: String) {
        Adapty.logLevel = (try? AdaptyLogLevel(serializingString: value)) ?? .none
    }

    @objc public func identify(_ customerUserId: String, completion: JSONStringCompletion? = nil) {
        Adapty.identify(customerUserId) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func logout(_ completion: JSONStringCompletion? = nil) {
        Adapty.logout { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func getPaywalls(_ forceUpdate: Bool, completion: JSONStringCompletion? = nil) {
        Adapty.getPaywalls(forceUpdate: forceUpdate) { paywalls, products, error in
            let result: AdaptyResult<GetPaywallsResponse>
            if let error = error {
                result = .error(error)
            } else {
                AdaptyUnityPlugin.productCache.add(products)
                AdaptyUnityPlugin.productCache.add(paywalls)
                result = .success(GetPaywallsResponse(paywalls: paywalls, products: products))
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func getPurchaserInfo(_ forceUpdate: Bool, completion: JSONStringCompletion? = nil) {
        Adapty.getPurchaserInfo(forceUpdate: forceUpdate) { purchaserInfo, error in
            let result: AdaptyResult<PurchaserInfoModel>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(purchaserInfo)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func restorePurchases(_ completion: JSONStringCompletion? = nil) {
        Adapty.restorePurchases { purchaserInfo, receipt, appleValidationResult, error in
            let result: AdaptyResult<RestorePurchasesResponse>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(RestorePurchasesResponse(
                    purchaserInfo: purchaserInfo,
                    receipt: receipt,
                    appleValidationResult: appleValidationResult
                ))
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func makePurchase(_ productId: String, variationId: String?, offerId: String?, completion: JSONStringCompletion? = nil) {
        guard let product = AdaptyUnityPlugin.productCache.product(byId: productId, withVariationId: variationId) else {
            let error = PluginError.notFoundProduct(productId, withVariationId: variationId)
            completion?(AdaptyUnityPlugin.toJSONString(AdaptyErrorResult(error: error)))
            return
        }

        Adapty.makePurchase(product: product, offerId: offerId) { purchaserInfo, receipt, appleValidationResult, product, error in
            let result: AdaptyResult<MakePurchaseResponse>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(MakePurchaseResponse(
                    purchaserInfo: purchaserInfo,
                    receipt: receipt,
                    appleValidationResult: appleValidationResult,
                    product: product
                ))
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }
    
    @objc public func makeDeferredPurchase(_ productId: String, completion: JSONStringCompletion? = nil) {
        guard let deferredPurchase = AdaptyUnityPlugin.deferredPurchases[productId] else {
            let error = PluginError.notFoundDeferredPurchase(productId)
            completion?(AdaptyUnityPlugin.toJSONString(AdaptyErrorResult(error: error)))
            return
        }

        deferredPurchase { purchaserInfo, receipt, appleValidationResult, product, error in
            let result: AdaptyResult<MakePurchaseResponse>
            if let error = error {
                result = .error(error)
            } else {
                AdaptyUnityPlugin.deferredPurchases.removeValue(forKey: productId)
                result = .success(MakePurchaseResponse(
                    purchaserInfo: purchaserInfo,
                    receipt: receipt,
                    appleValidationResult: appleValidationResult,
                    product: product
                ))
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func logShowPaywall(_ variationId: String, completion: JSONStringCompletion? = nil) {
        guard let paywall = AdaptyUnityPlugin.productCache.paywall(byVariationId: variationId) else {
            let error = PluginError.notFoundPaywall(byVariationId: variationId)
            completion?(AdaptyUnityPlugin.toJSONString(AdaptyErrorResult(error: error)))
            return
        }

        Adapty.logShowPaywall(paywall) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func setFallbackPaywalls(_ paywalls: String, completion: JSONStringCompletion? = nil) {
        Adapty.setFallbackPaywalls(paywalls) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func getPromo(_ completion: JSONStringCompletion? = nil) {
        Adapty.getPromo { promo, error in
            let result: AdaptyResult<PromoModel>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(promo)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func updateProfile(_ jsonString: String, completion: JSONStringCompletion? = nil) {
        let builder = ProfileParameterBuilder()
        if let params = AdaptyUnityPlugin.toDictionary(jsonString) {
            _ = builder.with(params)
        }
        Adapty.updateProfile(params: builder) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func updateAttribution(_ jsonString: String, source: String, networkUserId: String?, completion: JSONStringCompletion? = nil) {
        let attribution = AdaptyUnityPlugin.toDictionary(jsonString) ?? [:]
        let source = (try? AttributionNetwork(serializingString: source)) ?? .custom
        Adapty.updateAttribution(attribution, source: source, networkUserId: networkUserId) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func setExternalAnalyticsEnabled(_ enabled: Bool, completion: JSONStringCompletion? = nil) {
        Adapty.setExternalAnalyticsEnabled(enabled) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func setVariationId(_ variationId: String, forTransactionId: String, completion: JSONStringCompletion? = nil) {
        Adapty.setVariationId(variationId, forTransactionId: forTransactionId) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }

    @objc public func getApnsToken() -> String? {
        Adapty.apnsTokenString
    }

    @objc public func setApnsToken(_ value: String) {
        Adapty.apnsTokenString = value
    }

    @objc public func presentCodeRedemptionSheet() {
        Adapty.presentCodeRedemptionSheet()
    }

    @objc public func handlePushNotification(_ userInfo: String, completion: JSONStringCompletion? = nil) {
        let userInfo = AdaptyUnityPlugin.toDictionary(userInfo) ?? [:]
        Adapty.handlePushNotification(userInfo) { error in
            let result: AdaptyResult<Bool>
            if let error = error {
                result = .error(error)
            } else {
                result = .success(true)
            }
            completion?(AdaptyUnityPlugin.toJSONString(result))
        }
    }
}
