import Adapty
import AdaptyPlugin

enum Log {
    typealias Category = AdaptyPlugin.LogCategory

    static let wrapper = Category(subsystem: "io.adapty.unity", name: "wrapper")
}

@objc public final class AdaptyUnityPlugin: NSObject {
    public typealias JSONStringCompletion = (String?) -> Void
    public typealias MessageDelegate = (String, String) -> Void
    
    @objc public static let shared = AdaptyUnityPlugin()
    static var messageDelegate: MessageDelegate?
    
    @objc public static func setup(_ delegate: MessageDelegate? = nil) {
        messageDelegate = delegate
        
        Task { @MainActor in
            AdaptyPlugin.reqister(eventHandler: shared)
        }
    }

    @objc public func method(_ method: String, request: String, completion: JSONStringCompletion? = nil) {
        Task {
            let result = await AdaptyPlugin.execute(method: method, withJson: request)
            completion?(result.asAdaptyJsonString)
        }
    }
    
    func invokeMethod(_ method: String, arguments: String) {
        guard let delegate = AdaptyUnityPlugin.messageDelegate else {
            Log.wrapper.error("No messageDelegate registered!")
            return
        }
        
        delegate( method, arguments )
    }
}

extension AdaptyUnityPlugin: EventHandler {
    public func handle(event: AdaptyPluginEvent) {
        do {
            try invokeMethod(
                event.id,
                arguments: event.asAdaptyJsonData.asAdaptyJsonString
            )
        } catch {
            Log.wrapper.error("Plugin encoding error: \(error.localizedDescription)")
        }
    }
}
