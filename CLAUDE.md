# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Adapty Unity SDK — a C# wrapper around native [Adapty iOS SDK](https://github.com/adaptyteam/AdaptySDK-iOS) (Swift/SPM) and [Adapty Android SDK](https://github.com/adaptyteam/AdaptySDK-Android) (Kotlin/Maven). Provides in-app purchase management, flow (paywall) rendering, onboarding flows, and subscription analytics for Unity apps. Current SDK version is defined in `Packages/com.adapty.unity-sdk/Runtime/Adapty.cs` (`Adapty.SDKVersion`).

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

**Native dependency versions:** iOS is declared in `Packages/com.adapty.unity-sdk/Runtime/Editor/AdaptySDKDependencies.xml` (Swift Package Manager via External Dependency Manager 1.2.187+; iOS deployment target 15.0+ is enforced by `Packages/com.adapty.unity-sdk/Editor/AdaptyIOSBuildValidator.cs`). Android is declared in `Packages/com.adapty.unity-sdk/Runtime/Plugins/Android/AdaptySDKDependencies.androidlib/build.gradle`. Update both when bumping native SDK versions.

## Architecture

### Cross-Platform Bridge Pattern

All SDK calls follow a single JSON-based bridge:

1. **C# public API** (`Packages/com.adapty.unity-sdk/Runtime/Adapty.cs`, `Adapty.Overloads.cs`) — `static partial class Adapty` with methods like `GetFlow`, `MakePurchase`, etc.
2. Each method serializes parameters to JSON via `Request.Send()` (bottom of `Adapty.cs`), which adds the `method` key and calls `_Adapty.Invoke(method, json, callback)`.
3. **`_Adapty`** is compile-time aliased per platform:
   - `AdaptySDK.iOS.AdaptyIOS` — P/Invoke `[DllImport("__Internal")]` to Swift plugin
   - `AdaptySDK.Android.AdaptyAndroid` — `AndroidJavaClass` calling `com.adapty.unity.AdaptyAndroidWrapper`
   - `AdaptySDK.Noop.AdaptyNoop` — no-op for Editor/unsupported platforms
4. Native side processes the JSON request and returns a JSON response string via callback.
5. Response is parsed back into C# models via `+JSON.cs` extension methods.

### Key Directory Layout

- **`Packages/com.adapty.unity-sdk/`** — The SDK package distributed to users (UPM layout):
  - `Runtime/Adapty.cs` — Main API (all public methods + internal `Request` class)
  - `Runtime/Adapty.Overloads.cs` — Convenience overloads with fewer parameters
  - `Runtime/AdaptyEventListener.cs` — Event listener interfaces (`AdaptyEventListener`, `AdaptyFlowsEventsListener`, `AdaptyUISystemRequestsHandler`, `AdaptyUIObserverModeResolver`, `AdaptyOnboardingsEventsListener`) and the `OnMessage` dispatcher
  - `Runtime/Models/` — C# data models (one file per type, e.g. `AdaptyFlow.cs`)
  - `Runtime/JSON/` — JSON serialization/deserialization extensions (one `+JSON.cs` per model, plus `SimpleJSON.cs` library)
  - `Runtime/Plugins/iOS/` — `AdaptyIOS.cs` (P/Invoke bridge) + `Source/` (Swift/ObjC native plugin code)
  - `Runtime/Plugins/Android/` — `AdaptyAndroid.cs` (JNI bridge) + `Local/` (local AAR maven repo) + `AdaptySDKDependencies.androidlib` (Android maven dependencies)
  - `Runtime/Plugins/AdaptyNoop.cs` — Editor/no-op stub
  - `Runtime/Editor/AdaptySDKDependencies.xml` — iOS Swift Package declaration for External Dependency Manager
  - `Editor/` — Editor-only assembly (iOS build validation)
- **`adaptyandroidwrapper/`** — Standalone Android Gradle project:
  - `unitywrapper/src/main/java/com/adapty/unity/` — `AdaptyAndroidWrapper.java` (entry point), callback handler, message handler
- **`Assets/Scripts/`** — Demo app scripts (not part of distributed SDK)
- **`cross_platform.yaml`** — Cross-platform API contract schema defining all request/response JSON formats and data types shared across iOS/Android/Unity

### Event System

Native SDKs push events (profile updates, flow view lifecycle, onboarding events) via the same JSON bridge. `Adapty.OnMessage(id, json)` in `AdaptyEventListener.cs` dispatches by event `id` string to the registered listener interfaces. Two event families are round-trips: flow permission requests are answered via `flow_view_did_answer_permission` (keyed by `event_id`), and Observer-mode purchases/restores report back via `observer_*_did_start/finish`.

### Model + JSON Convention

Each model has two files:
- `Runtime/Models/AdaptyFoo.cs` — C# class/struct definition
- `Runtime/JSON/AdaptyFoo+JSON.cs` — `ToJSONNode()` serialization and `GetAdaptyFoo()` deserialization extension methods

When adding a new model, create both files following this pattern. The JSON keys must match `cross_platform.yaml` definitions, including which fields are required vs optional (optional fields parse with `*IfPresent` accessors).

## Version Bumping

When releasing a new version, update:
1. `Adapty.SDKVersion` in `Packages/com.adapty.unity-sdk/Runtime/Adapty.cs`
2. `version` in `Packages/com.adapty.unity-sdk/package.json`
3. Native dependency versions: iOS in `Runtime/Editor/AdaptySDKDependencies.xml`, Android in `Runtime/Plugins/Android/AdaptySDKDependencies.androidlib/build.gradle` and `adaptyandroidwrapper/unitywrapper/build.gradle` (then rebuild the AAR into `Runtime/Plugins/Android/Local/io/adapty/internal/unity-wrapper/<version>/`)
4. `cross_platform.yaml` schema `$id` version
