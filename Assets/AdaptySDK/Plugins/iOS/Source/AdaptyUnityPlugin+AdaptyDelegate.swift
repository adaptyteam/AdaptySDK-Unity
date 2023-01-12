//
//  AdaptyUnityPlugin+AdaptyDelegate.swift
//  Adapty
//
//  Created by Aleksei Valiano on 24.12.2022.
//

import Adapty

extension AdaptyUnityPlugin {
    static let delegate = AdaptyDelegateWrapper()
    public typealias MessageDelegate = (String, String) -> Void
    private static var _messageDelegate: MessageDelegate?

    @objc public func registerMessageDelegate(_ delegate: MessageDelegate? = nil) {
        AdaptyUnityPlugin._messageDelegate = delegate
    }

    class AdaptyDelegateWrapper: AdaptyDelegate {
        public func didLoadLatestProfile(_ profile: AdaptyProfile) {
            guard let jsonString: String = try? AdaptyUnityPlugin.encode(profile) else { return }
            AdaptyUnityPlugin._messageDelegate?("did_load_latest_profile", jsonString)
        }
    }
}
