import Adapty

extension AdaptyUnityPlugin {
    static let productCache = ProductCache()
    static var deferredPurchases = [String: DeferredPurchaseCompletion]()
    
    class ProductCache {
        private var paywalls = [PaywallModel]()
        private var products = [ProductModel]()

        func clear() {
            paywalls.removeAll()
            products.removeAll()
        }

        func add(_ products: [ProductModel]?) {
            self.products.removeAll()
            if let products = products {
                self.products.append(contentsOf: products)
            }
        }

        func add(_ paywalls: [PaywallModel]?) {
            self.paywalls.removeAll()
            if let paywalls = paywalls {
                self.paywalls.append(contentsOf: paywalls)
            }
        }

        func paywall(byVariationId variationId: String?) -> PaywallModel? {
            guard let variationId = variationId else { return nil }
            return paywalls.first(where: { $0.variationId == variationId })
        }
        
        func product(byId productId: String, withVariationId variationId: String?) -> ProductModel? {
            guard let paywall = paywall(byVariationId: variationId) else {
                return products.first(where: { $0.vendorProductId == productId })
            }

            return paywall.products.first(where: { $0.vendorProductId == productId })
        }
    }
}
