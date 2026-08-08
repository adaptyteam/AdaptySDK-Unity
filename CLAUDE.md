# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Adapty Unity SDK — a C# wrapper around native [Adapty iOS SDK](https://github.com/adaptyteam/AdaptySDK-iOS) (Swift/SPM) and [Adapty Android SDK](https://github.com/adaptyteam/AdaptySDK-Android) (Kotlin/Maven). Provides in-app purchase management, flow (paywall) rendering, onboarding flows, and subscription analytics for Unity apps. Current SDK version is defined in `Packages/com.adapty.unity-sdk/Runtime/Adapty.cs` (`Adapty.SDKVersion`).

## Build & Development

This is a **Unity project** (Unity 6000.x) — the player is built and tested through the Unity Editor. The JSON layer is the exception: `tests/` links the SDK sources into a plain .NET project, so it needs neither the Editor nor a licence.

The package declares **Unity 2022.3 and newer** as `unity` in `package.json`, and that floor is what Editor-facing code may assume: `AdaptyDependencies` uses `Client.AddAndRemove` and `PackageInfo.FindForAssembly`, neither of which exists all the way back (`AddAndRemove` arrived after 2020.3).

The install path is verified on the floor — `.unitypackage` import into a clean project, then `Adapty SDK > Install Dependencies`, then a compile, all on 2022.3.62f3. Everything else runs on Unity 6. Keep the changelog and `MIGRATION.md` wording matching that split; do not widen it to claim device or build coverage on 2022.3. One trap when re-verifying: recent 2022.3 builds are Extended LTS and refuse to launch without an Industry or Enterprise licence, so pick a build below that cutoff (62f3 works).

**Run the JSON layer tests:**
```bash
dotnet test tests/AdaptySDK.NextTests/AdaptySDK.NextTests.csproj
```
The layer branches on `UNITY_IOS` / `UNITY_ANDROID` and each platform has its own approved snapshots, so a change to it has to pass all three: add `-p:AdaptyPlatform=UNITY_IOS` or `-p:AdaptyPlatform=UNITY_ANDROID` for the other two. `ADAPTY_UPDATE_SNAPSHOTS=1` rewrites the approved files instead of failing. CI runs the same matrix in `.github/workflows/json-layer-tests.yml`.

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
5. Response is parsed back into C# models by Newtonsoft, through `AdaptySDK.Serialization.AdaptyJson`.

### Newtonsoft Dependency

`com.unity.nuget.newtonsoft-json` is a UPM dependency of the package, and it is not part of any stock Unity template — a `.unitypackage` carries assets only, so it cannot bring it along. Three rules follow, and all are load-bearing:

- The **Runtime** assembly declares `ADAPTY_NEWTONSOFT` in `versionDefines` and requires it in `defineConstraints`. Without the package the assembly is skipped rather than failing to compile, which is what keeps a fresh import from spilling hundreds of `CS0246`.
- The **Editor** assembly must compile in every state, so it carries no constraint and no define of its own: it is what reports the problem (`AdaptyNewtonsoftValidator`) and installs the fix (`AdaptyDependencies`, the `Adapty SDK > Install Dependencies` menu item, which also installs External Dependency Manager and writes the OpenUPM registry it comes from into `Packages/manifest.json` — scoped registries have no public API).
- Presence is judged against the **package**, not the assembly, everywhere — `PackageInfo.FindForAssembly`. A `Newtonsoft.Json.dll` sitting in `Assets/` does not set the version define, so the SDK would silently not compile; the installer refuses to add a second copy on top of it and the validator names that state instead. EDM carries no define constraint, so for it any copy counts.

Editor code therefore cannot reference Runtime types, and the Editor asmdef's empty `references` is what enforces that.

### Key Directory Layout

- **`Packages/com.adapty.unity-sdk/`** — The SDK package distributed to users (UPM layout):
  - `Runtime/Adapty.cs` — Main API (all public methods + internal `Request` class)
  - `Runtime/Adapty.Overloads.cs` — Convenience overloads with fewer parameters
  - `Runtime/IAdaptyEventListener.cs` — Event listener interfaces (`IAdaptyEventListener`, `IAdaptyFlowsEventsListener`, `IAdaptyUISystemRequestsHandler`, `IAdaptyUIObserverModeResolver`, `IAdaptyOnboardingsEventsListener`) and the `OnMessage` dispatcher
  - `Runtime/Models/` — C# data models (one file per type, e.g. `AdaptyFlow.cs`)
  - `Runtime/Serialization/` — the Newtonsoft JSON layer: `AdaptyJson` (the single entry point), `AdaptyContractResolver`, and the converters
  - `Runtime/Plugins/iOS/` — `AdaptyIOS.cs` (P/Invoke bridge) + `Source/` (Swift/ObjC native plugin code)
  - `Runtime/Plugins/Android/` — `AdaptyAndroid.cs` (JNI bridge) + `Local/` (local AAR maven repo) + `AdaptySDKDependencies.androidlib` (Android maven dependencies)
  - `Runtime/Plugins/AdaptyNoop.cs` — Editor/no-op stub
  - `Runtime/Editor/AdaptySDKDependencies.xml` — iOS Swift Package declaration for External Dependency Manager
  - `Editor/` — Editor-only assembly (iOS build validation, Newtonsoft presence check, the `Adapty SDK > Install Dependencies` menu item)
- **`adaptyandroidwrapper/`** — Standalone Android Gradle project:
  - `unitywrapper/src/main/java/com/adapty/unity/` — `AdaptyAndroidWrapper.java` (entry point), callback handler, message handler
- **`tests/`** — .NET test projects for the JSON layer: `AdaptySDK.NextTests` (the suite), `shared/` (fixtures and snapshot helpers), `surface/` (the SDK compiled as a library to assert against), `aot-probe/`
- **`Assets/Scripts/`** — Demo app scripts (not part of distributed SDK)
- **`cross_platform.yaml`** — Cross-platform API contract schema defining all request/response JSON formats and data types shared across iOS/Android/Unity

### Event System

Native SDKs push events (profile updates, flow view lifecycle, onboarding events) via the same JSON bridge. `Adapty.OnMessage(id, json)` in `IAdaptyEventListener.cs` dispatches by event `id` string to the registered listener interfaces. Two event families are round-trips: flow permission requests are answered via `flow_view_did_answer_permission` (keyed by `event_id`), and Observer-mode purchases/restores report back via `observer_*_did_start/finish`.

### Model Convention

One file per model in `Runtime/Models/`. Serialization is declared with attributes, not written by hand:

- `[DataContract]` on the type and `[DataMember(Name = "json_key")]` on each member, with `IsRequired = true` where the contract says the key is required. The JSON keys must match `cross_platform.yaml`, including which fields are required vs optional.
- `[Preserve]` on the type. Managed stripping otherwise removes it, and the failure shows only on a device, the first time a response carries the type. A nested type is covered by its declaring type's attribute; a property getter or a `ShouldSerialize*` method is not, and needs its own.
- Enums follow one of two contracts, and the choice is part of the wire format. A **string** enum maps every member with `[EnumMember(Value = "...")]`; declare an `Unknown` member on anything the native side can extend, since an unrecognised value reads as `Unknown` where one exists and throws where it does not. A **numeric** enum — `AdaptyErrorCode`, `AppTrackingTransparencyStatus` — carries the native number and declares no `[EnumMember]` at all: `AdaptyEnumConverter.CanConvert` skips it and Newtonsoft's default numeric handling applies. Adding `[EnumMember]` to a numeric enum silently switches it to strings.

`tests/AdaptySDK.NextTests` enforces all of this: a model added without `[Preserve]` fails `StrippingGuardTests`, and one whose output changes fails its approved snapshot.

## Version Bumping

When releasing a new version, update:
1. `Adapty.SDKVersion` in `Packages/com.adapty.unity-sdk/Runtime/Adapty.cs`
2. `version` in `Packages/com.adapty.unity-sdk/package.json`
3. Native dependency versions: iOS in `Runtime/Editor/AdaptySDKDependencies.xml`, Android in `Runtime/Plugins/Android/AdaptySDKDependencies.androidlib/build.gradle` and `adaptyandroidwrapper/unitywrapper/build.gradle` (then rebuild the AAR into `Runtime/Plugins/Android/Local/io/adapty/internal/unity-wrapper/<version>/`, and **delete the previous one** — no `build.gradle` in the package references the artifact, Unity picks up any `.aar` under `Plugins/Android` as a plugin, so two versions side by side both reach the player)
4. `cross_platform.yaml` schema `$id` version — must match the canonical contract in AdaptySDK-iOS (`Sources.AdaptyPlugin/cross_platform.yaml`); diff the two files, not just the version
5. `CHANGELOG.md` and the `_upm.changelog` string in `package.json` — keep both in sync, the latter is what Package Manager shows after an update
6. Managed dependency versions, when they move: `dependencies` and `peerDependencies` in `package.json` **and** the constants in `Editor/AdaptyDependencies.cs`, which is what installs them for `.unitypackage` users
