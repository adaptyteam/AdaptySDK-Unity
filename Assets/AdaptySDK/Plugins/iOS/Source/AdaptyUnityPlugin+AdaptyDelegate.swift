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
    static var messageDelegate: MessageDelegate?

    @objc public func registerMessageDelegate(_ delegate: MessageDelegate? = nil) {
        AdaptyUnityPlugin.messageDelegate = delegate
    }

    class AdaptyDelegateWrapper: AdaptyDelegate {
        public func didLoadLatestProfile(_ profile: AdaptyProfile) {
            guard let jsonString = try? AdaptyUnityPlugin.encodeToString(profile) else { return }
            AdaptyUnityPlugin.messageDelegate?("did_load_latest_profile", jsonString)
        }
    }
}
