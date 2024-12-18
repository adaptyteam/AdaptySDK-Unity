import Adapty
import AdaptyPlugin

@objc public class AdaptyUnityPlugin: NSObject {
    @objc public static let shared = AdaptyUnityPlugin()

    public typealias JSONStringCompletion = (String?) -> Void

    override public init() {
        super.init()
        
        Adapty.delegate = AdaptyUnityPlugin.delegate
    }
    
    
    @objc public func method(_ method: String, request: String, completion: JSONStringCompletion? = nil) {
        Task {
            let result = await AdaptyPlugin.execute(method: method, withJson: request)
            completion?(result.asAdaptyJsonString)
        }
    }
}
