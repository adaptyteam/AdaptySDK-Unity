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

**Step 3:** Determine which version/branch to use. Parse the current dependency version from `Assets/AdaptySDK/Editor/AdaptySDKDependencies.xml` (look for `<iosPod name="Adapty" version="X.Y.Z">`). Then **ask the user** which tag or branch to checkout, suggesting the dependency version as default. The user may want an unreleased branch instead.

**Step 4:** Checkout the confirmed version:
```bash
cd .ios-sdk && git fetch --all --tags && git checkout <tag-or-branch>
```

## iOS SDK Directory Map

```
.ios-sdk/
├── Sources/                        # Core Adapty SDK
│   ├── Adapty.swift                # Main SDK class
│   ├── Adapty+*.swift              # Public API extensions (GetPaywall, MakePurchase, etc.)
│   ├── Backend/                    # HTTP API layer
│   ├── Backend.HTTPSession/        # Network session
│   ├── Configuration/              # SDK configuration
│   ├── Environment/                # Device/environment info
│   ├── Events/                     # Analytics events
│   ├── Placements/                 # Paywall placements
│   ├── Profile/                    # User profiles
│   ├── StoreKit/                   # StoreKit integration
│   └── LifecycleManager.swift      # App lifecycle
│
├── Sources.AdaptyPlugin/           # Cross-platform bridge (THIS IS THE KEY DIRECTORY)
│   ├── AdaptyPlugin.swift          # Main plugin entry: execute(method:withJson:)
│   ├── cross_platform.yaml         # API contract schema (JSON formats)
│   ├── Requests/                   # One file per SDK method (37+ handlers)
│   │   ├── Request.Activate.swift
│   │   ├── Request.GetPaywall.swift
│   │   ├── Request.MakePurchase.swift
│   │   └── ...
│   ├── Codable/                    # JSON encoding/decoding for models
│   └── Events/                     # Event definitions pushed to Unity
│
├── Sources.AdaptyUI/               # Visual paywall rendering
├── Sources.UIBuilder/              # Paywall template builder
├── Sources.KidsMode/               # Kids mode support
├── Sources.Logger/                 # Logging framework
├── Sources.DeveloperTools/         # Debug tools
├── Tests/                          # Unit tests
├── Adapty.podspec                  # CocoaPods spec
└── Package.swift                   # Swift Package Manager manifest
```

## Common Lookup Patterns

### Find how a specific SDK method works on iOS
```
# In Sources.AdaptyPlugin/Requests/ — one file per method
Glob: .ios-sdk/Sources.AdaptyPlugin/Requests/Request.*.swift

# Example: how does GetPaywall work?
Read: .ios-sdk/Sources.AdaptyPlugin/Requests/Request.GetPaywall.swift
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
# Public API methods are in Adapty+MethodName.swift
Glob: .ios-sdk/Sources/Adapty+*.swift
# Example: full purchase flow
Read: .ios-sdk/Sources/Adapty+MakePurchase.swift
```

### Find model definitions in the core SDK
```
Grep: pattern="struct Adapty" path=".ios-sdk/Sources/"
```

## Cross-Referencing Unity ↔ iOS

When working on the Unity side, the mapping is:

| Unity (C#) | iOS Bridge | iOS Core |
|---|---|---|
| `Adapty.cs` methods | `Sources.AdaptyPlugin/Requests/Request.*.swift` | `Sources/Adapty+*.swift` |
| `Models/AdaptyFoo.cs` | `Sources.AdaptyPlugin/Codable/` | `Sources/` model files |
| `JSON/AdaptyFoo+JSON.cs` | `Sources.AdaptyPlugin/Codable/` | N/A |
| `cross_platform.yaml` (Unity root) | `Sources.AdaptyPlugin/cross_platform.yaml` | N/A |
| `AdaptyEventListener.cs` | `Sources.AdaptyPlugin/Events/` | `Sources/Events/` |

## Version Alignment

The iOS dependency version is declared in:
```
Assets/AdaptySDK/Editor/AdaptySDKDependencies.xml
```
Look for: `<iosPod name="Adapty" version="X.Y.Z">`

Always confirm with the user before checking out a tag — they may be working against an unreleased branch.
