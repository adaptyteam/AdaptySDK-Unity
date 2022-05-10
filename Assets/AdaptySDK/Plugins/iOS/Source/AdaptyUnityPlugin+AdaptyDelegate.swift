import Adapty

extension AdaptyUnityPlugin {
    static let delegate = AdaptyDelegateWrapper()
    public typealias MessageDelegate = (String, String) -> Void
    private static var _messageDelegate: MessageDelegate?

    @objc public func registerMessageDelegate(_ delegate: MessageDelegate? = nil) {
        AdaptyUnityPlugin._messageDelegate = delegate
    }

    class AdaptyDelegateWrapper: AdaptyDelegate {
        public func didReceiveUpdatedPurchaserInfo(_ purchaserInfo: PurchaserInfoModel) {
            guard let jsonString = AdaptyUnityPlugin.toJSONString(purchaserInfo) else { return }
            AdaptyUnityPlugin._messageDelegate?("purchaser_info_update", jsonString)
        }

        public func didReceivePromo(_ promo: PromoModel) {
            guard let jsonString = AdaptyUnityPlugin.toJSONString(promo) else { return }
            AdaptyUnityPlugin._messageDelegate?("promo_received", jsonString)
        }

        public func paymentQueue(shouldAddStorePaymentFor product: ProductModel, defermentCompletion makeDeferredPurchase: @escaping DeferredPurchaseCompletion) {
            guard let jsonString = AdaptyUnityPlugin.toJSONString(product) else { return }
            AdaptyUnityPlugin.deferredPurchases[product.vendorProductId] = makeDeferredPurchase
            AdaptyUnityPlugin._messageDelegate?("deferred_purchase", jsonString)
        }

        public func didReceivePaywallsForConfig(paywalls: [PaywallModel]) {
            guard let jsonString = AdaptyUnityPlugin.toJSONString(paywalls) else { return }
            AdaptyUnityPlugin._messageDelegate?("remote_config_update", jsonString)
        }
    }
}
