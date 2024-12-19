import Adapty
import AdaptyPlugin

@objc public class AdaptyUnityPlugin: NSObject {
    @objc public static let shared = AdaptyUnityPlugin()

    public typealias JSONStringCompletion = (String?) -> Void

    override public init() {
        super.init()
        
        Adapty.delegate = AdaptyUnityPlugin.delegate
        
        Task { @MainActor in
            AdaptyPlugin.reqister(setFallbackPaywallsRequests: { @MainActor assetId in
                if #available(iOS 16.0, *) {
                    return URL(filePath: assetId)
                } else {
                    return URL(fileURLWithPath: assetId)
                }
            })
        }
    }
    
    
    @objc public func method(_ method: String, request: String, completion: JSONStringCompletion? = nil) {
        Task {
            let result = await AdaptyPlugin.execute(method: method, withJson: request)
            completion?(result.asAdaptyJsonString)
        }
    }
}
