# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Adapty Unity SDK — a C# wrapper around native [Adapty iOS SDK](https://github.com/adaptyteam/AdaptySDK-iOS) (Swift/CocoaPods) and [Adapty Android SDK](https://github.com/adaptyteam/AdaptySDK-Android) (Kotlin/Maven). Provides in-app purchase management, paywall rendering, onboarding flows, and subscription analytics for Unity apps. Current SDK version is defined in `Assets/AdaptySDK/Adapty.cs` (`Adapty.SDKVersion`).

## Build & Development

This is a **Unity project** (Unity 6000.x). There is no standalone CLI build or test command — the project is built and tested through the Unity Editor.

**Build .unitypackage for distribution:**
```bash
cd deploy && ./build_unitypackage.sh      # dev mode (keeps Library/)
cd deploy && ./build_unitypackage.sh -p    # production (cleans generated files, moves .unitypackage to root)
```

**Android wrapper (Java):** Built separately via Gradle in `adaptyandroidwrapper/`:
```bash
cd adaptyandroidwrapper && ./gradlew :unitywrapper:build
```

**Native dependency versions** are declared in `Assets/AdaptySDK/Editor/AdaptySDKDependencies.xml` (iOS CocoaPods + Android Maven). Update this file when bumping native SDK versions.

## Architecture

### Cross-Platform Bridge Pattern

All SDK calls follow a single JSON-based bridge:

1. **C# public API** (`Assets/AdaptySDK/Adapty.cs`, `Adapty.Overloads.cs`) — `static partial class Adapty` with methods like `GetPaywall`, `MakePurchase`, etc.
2. Each method serializes parameters to JSON via `Request.Send()` (bottom of `Adapty.cs`), which adds the `method` key and calls `_Adapty.Invoke(method, json, callback)`.
3. **`_Adapty`** is compile-time aliased per platform:
   - `AdaptySDK.iOS.AdaptyIOS` — P/Invoke `[DllImport("__Internal")]` to Swift plugin
   - `AdaptySDK.Android.AdaptyAndroid` — `AndroidJavaClass` calling `com.adapty.unity.AdaptyAndroidWrapper`
   - `AdaptySDK.Noop.AdaptyNoop` — no-op for Editor/unsupported platforms
4. Native side processes the JSON request and returns a JSON response string via callback.
5. Response is parsed back into C# models via `+JSON.cs` extension methods.

### Key Directory Layout

- **`Assets/AdaptySDK/`** — The SDK package distributed to users:
  - `Adapty.cs` — Main API (all public methods + internal `Request` class)
  - `Adapty.Overloads.cs` — Convenience overloads with fewer parameters
  - `AdaptyEventListener.cs` — Event listener interfaces (`AdaptyEventListener`, `AdaptyPaywallsEventsListener`, `AdaptyOnboardingsEventsListener`) and the `OnMessage` dispatcher
  - `Models/` — C# data models (one file per type, e.g. `AdaptyPaywall.cs`)
  - `JSON/` — JSON serialization/deserialization extensions (one `+JSON.cs` per model, plus `SimpleJSON.cs` library)
  - `Plugins/iOS/` — `AdaptyIOS.cs` (P/Invoke bridge) + `Source/` (Swift/ObjC native plugin code)
  - `Plugins/Android/` — `AdaptyAndroid.cs` (JNI bridge) + `Local/` (local AAR)
  - `Plugins/AdaptyNoop.cs` — Editor/no-op stub
  - `Editor/AdaptySDKDependencies.xml` — Native dependency declarations for External Dependency Manager
- **`adaptyandroidwrapper/`** — Standalone Android Gradle project:
  - `unitywrapper/src/main/java/com/adapty/unity/` — `AdaptyAndroidWrapper.java` (entry point), callback handler, message handler
- **`Assets/Scripts/`** — Demo app scripts (not part of distributed SDK)
- **`cross_platform.yaml`** — Cross-platform API contract schema defining all request/response JSON formats and data types shared across iOS/Android/Unity

### Event System

Native SDKs push events (profile updates, paywall view lifecycle, onboarding events) via the same JSON bridge. `Adapty.OnMessage(id, json)` in `AdaptyEventListener.cs` dispatches by event `id` string to the registered listener interfaces.

### Model + JSON Convention

Each model has two files:
- `Models/AdaptyFoo.cs` — C# class/struct definition
- `JSON/AdaptyFoo+JSON.cs` — `ToJSONNode()` serialization and `GetAdaptyFoo()` deserialization extension methods

When adding a new model, create both files following this pattern. The JSON keys must match `cross_platform.yaml` definitions.

## Version Bumping

When releasing a new version, update:
1. `Adapty.SDKVersion` in `Assets/AdaptySDK/Adapty.cs`
2. Native dependency versions in `Assets/AdaptySDK/Editor/AdaptySDKDependencies.xml`
3. `cross_platform.yaml` schema `$id` version
