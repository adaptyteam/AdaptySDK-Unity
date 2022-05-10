import Adapty

extension AdaptyUnityPlugin {
    struct GetPaywallsResponse: Encodable {
        let paywalls: [PaywallModel]?
        let products: [ProductModel]?
        
        enum CodingKeys: String, CodingKey {
            case paywalls
            case products
        }
    }

    struct RestorePurchasesResponse: Encodable {
        let purchaserInfo: PurchaserInfoModel?
        let receipt: String?
        let appleValidationResult: [String: Any]?

        enum CodingKeys: String, CodingKey {
            case purchaserInfo = "purchaser_info"
            case receipt
        }
    }
    
    struct MakePurchaseResponse: Encodable {
        let purchaserInfo: PurchaserInfoModel?
        let receipt: String?
        let appleValidationResult: [String: Any]?
        let product: ProductModel?

        enum CodingKeys: String, CodingKey {
            case purchaserInfo = "purchaser_info"
            case receipt
            case product
        }
    }
}
