---
name: ios-sdk-reference
description: Use when you need to read, understand, or reference the native iOS AdaptySDK-iOS source code — for understanding bridge contracts, porting new features, debugging native behavior, or checking JSON request/response formats
---

# iOS SDK Reference

Access the native AdaptySDK-iOS source code locally for reading and cross-referencing when working on the Unity wrapper.

## Setup: Clone if Missing

The iOS SDK is cloned into `.ios-sdk/` at the project root (gitignored).

**Step 1:** Check if `.ios-sdk/` exists:
```bash
ls .ios-sdk/Sources 2>/dev/null
```

**Step 2:** If missing, clone it:
```bash
git clone git@github.com:adaptyteam/AdaptySDK-iOS.git .ios-sdk
```
If the machine's SSH config reaches GitHub through a host alias, use that host instead — check `git remote -v` in this repository for the form that works here.

**Step 3:** Determine which version/branch to use. Parse the current dependency version from `Packages/com.adapty.unity-sdk/Runtime/Editor/AdaptySDKDependencies.xml` (look for `<swiftPackage ... version="X.Y.Z">`). Then **ask the user** which tag or branch to checkout, suggesting the dependency version as default. The user may want an unreleased branch instead.

**Step 4:** Checkout the confirmed version:
```bash
cd .ios-sdk && git fetch --all --tags && git checkout <tag-or-branch>
```

## iOS SDK Directory Map

Verified against tag `4.0.2`. Re-check it after every checkout of a different tag — the layout moves between majors, and a map that lies is worse than no map.

```
.ios-sdk/
├── Sources/                        # Core Adapty SDK
│   ├── Adapty.swift                # Main SDK class
│   ├── Adapty+*.swift              # Only three: Activate, Completion, Shared
│   ├── Backend/                    # HTTP API layer
│   ├── Backend.HTTPSession/        # Network session
│   ├── Configuration/              # SDK configuration
│   ├── Envoriment/                 # Device/environment info - misspelled upstream, glob it that way
│   ├── Errors/                     # AdaptyError and its codes
│   ├── Events/                     # Analytics events
│   ├── Log/                        # Logging
│   ├── Placements/                 # Paywall placements
│   ├── Profile/                    # User profiles
│   ├── Storage/                    # Local caches
│   ├── StoreKit/                   # StoreKit integration, and Adapty+MakePurchase.swift
│   ├── UserAcquisition/            # Install attribution
│   ├── WebPaywall/                 # Web paywall URLs
│   └── LifecycleManager.swift      # App lifecycle
│
├── Sources.AdaptyPlugin/           # Cross-platform bridge (THIS IS THE KEY DIRECTORY)
│   ├── AdaptyPlugin.swift          # Main plugin entry: execute(method:withJson:)
│   ├── cross_platform.yaml         # API contract schema (JSON formats)
│   ├── Requests/                   # One file per SDK method (42 files, incl. AdaptyPluginRequest.swift)
│   │   ├── Request.Activate.swift
│   │   ├── Request.AdaptyUICreateFlowView.swift
│   │   ├── Request.GetPaywallProducts.swift
│   │   └── ...
│   ├── Codable/                    # JSON encoding/decoding for models
│   └── Events/                     # Event definitions pushed to Unity
│
├── Sources.AdaptyUI/               # Visual paywall rendering
├── Sources.UIBuilder/              # Paywall template builder
├── Sources.Codable/                # Shared Codable helpers
├── Sources.Logger/                 # Logging framework
├── Sources.DeveloperTools/         # Debug tools
├── Examples/                       # Sample apps
├── Tests/                          # Unit tests
├── scripts/                        # Repo tooling
└── Package.swift                   # Swift Package Manager manifest, and where the traits live
```

There is no `Sources.KidsMode/`: Kids Mode is a **trait** declared in `Package.swift`, which turns on the `KidsMode` compilation condition. The code it guards is `#if KidsMode` in `Sources/Envoriment/Environment.Device.idfa.swift`, `Sources/Adapty+Activate.swift` and `Sources/Profile/Entities/AdaptyProfileParameters.Builder.swift`.

There is no `Adapty.podspec` either — the package ships through SwiftPM only, which is why the Unity side declares it with `<swiftPackage>`.

## Common Lookup Patterns

### Find how a specific SDK method works on iOS
```
# In Sources.AdaptyPlugin/Requests/ — one file per method
Glob: .ios-sdk/Sources.AdaptyPlugin/Requests/Request.*.swift

# Example: how does GetPaywallProducts work?
Read: .ios-sdk/Sources.AdaptyPlugin/Requests/Request.GetPaywallProducts.swift
```

### Find JSON contract for a method
```
# The cross_platform.yaml defines all request/response JSON schemas
Read: .ios-sdk/Sources.AdaptyPlugin/cross_platform.yaml
# Search for a specific method:
Grep: pattern="get_paywall" path=".ios-sdk/Sources.AdaptyPlugin/cross_platform.yaml"
```

### Find how a model is encoded/decoded
```
# In Sources.AdaptyPlugin/Codable/
Glob: .ios-sdk/Sources.AdaptyPlugin/Codable/*.swift
Grep: pattern="AdaptyPaywall" path=".ios-sdk/Sources.AdaptyPlugin/Codable/"
```

### Find events pushed from iOS to Unity
```
Glob: .ios-sdk/Sources.AdaptyPlugin/Events/*.swift
```

### Find the core SDK implementation (not bridge)
```
# Only three files sit at the root: Adapty+Activate, Adapty+Completion, Adapty+Shared.
Glob: .ios-sdk/Sources/Adapty+*.swift
# The rest live under the feature directory they belong to.
# Example: full purchase flow
Read: .ios-sdk/Sources/StoreKit/Adapty+MakePurchase.swift
# When unsure which directory owns a method, search for it:
Grep: pattern="func makePurchase" path=".ios-sdk/Sources/"
```

### Find model definitions in the core SDK
```
Grep: pattern="struct Adapty" path=".ios-sdk/Sources/"
```

## Cross-Referencing Unity ↔ iOS

When working on the Unity side, the mapping is:

Unity paths are relative to the repository root; everything in the package lives under `Packages/com.adapty.unity-sdk/`.

| Unity (C#) | iOS Bridge | iOS Core |
|---|---|---|
| `Runtime/Adapty.cs` methods | `Sources.AdaptyPlugin/Requests/Request.*.swift` | `Sources/<feature>/Adapty+*.swift` |
| `Runtime/Models/AdaptyFoo.cs` | `Sources.AdaptyPlugin/Codable/` | `Sources/` model files |
| `Runtime/Serialization/` (Newtonsoft layer) | `Sources.AdaptyPlugin/Codable/` | N/A |
| `cross_platform.yaml` (Unity root) | `Sources.AdaptyPlugin/cross_platform.yaml` | N/A |
| `Runtime/IAdaptyEventListener.cs` | `Sources.AdaptyPlugin/Events/` | `Sources/Events/` |
| `Runtime/AdaptyRequest.cs` (transport) | `Sources.AdaptyPlugin/AdaptyPlugin.swift` | N/A |

## Version Alignment

The iOS dependency version is declared in:
```
Packages/com.adapty.unity-sdk/Runtime/Editor/AdaptySDKDependencies.xml
```
Look for: `<swiftPackage ... version="X.Y.Z">`. It is a Swift Package Manager declaration read by External Dependency Manager, not a CocoaPods one.

Always confirm with the user before checking out a tag — they may be working against an unreleased branch.
