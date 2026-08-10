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
  - `Runtime/I*.cs` — one public interface per file, named after it: `IAdaptyEventListener`, `IAdaptyFlowsEventsListener`, `IAdaptyUISystemRequestsHandler`, `IAdaptyUIObserverModeResolver`. The deprecated `IAdaptyOnboardingsEventsListener` is under `Obsolete/` instead, by the rule below.
  - `Runtime/Adapty.Events.cs` — the implementation behind them: listener registration, `OnMessage` and the `Dispatch` switch. Kept out of the interface files, so a contract and the code that calls it are not the same file.
  - `Runtime/Models/` — C# data models (one file per type, e.g. `AdaptyFlow.cs`)
  - `Runtime/Obsolete/` — everything `[Obsolete]`, in a tree mirroring `Runtime/` (`Models/`, `Serialization/Converters/`). The point is that removing the deprecated API is a directory deletion plus the references that then fail to compile, so **nothing outside this folder may carry the attribute** — `EveryObsoleteMemberLivesUnderObsolete` in `SourceConventionTests` is the check. Members of a live `partial class` live here as their own part: `Adapty.Obsolete.cs`, `AdaptyUI.Obsolete.cs`, `Adapty.Events.Obsolete.cs`. The two csproj globs are **not** recursive, so a new subfolder here needs its own `<Compile Include>` line in both `tests/surface/package` and `tests/AdaptySDK.NextTests` or the SDK silently compiles without it.
  - `Runtime/Serialization/` — the Newtonsoft JSON layer: `AdaptyJson` (the single entry point), `AdaptyContractResolver`, `JsonRequire`, and `Converters/`. Every converter lives in that folder, one per file, named `AdaptyConverter<What>` after what it converts, and is registered in `AdaptyJson.Settings` — with one exception. `AdaptyConverterLooseJson` is deliberately **not** in the shared settings, so an ordinary `Dictionary<string, object>` keeps Newtonsoft's own `JObject`/`JArray`/`Int64`. Three public payloads the contract types as a bare object must instead stay the CLR graph of doubles they were in 3.x, and each reaches the converter its own way: `AdaptyProfile.CustomAttributes` is a **member** and names it in a `[JsonConverter]` of its own — the one place a converter is declared outside `AdaptyJson`, and the reason that converter carries `[Preserve]`, since Newtonsoft then builds it by reflection — `AdaptyJson.DeserializeRemoteConfigDictionary` covers `AdaptyRemoteConfig.Dictionary`, which is a **string** parsed on demand, and `AdaptyJson.CreateSerializerFor` covers the dispatcher's `Required`/`Optional`, where `flow_view_did_receive_analytic_event` hands `params` straight to a listener. `CanConvert` is the single definition of "loose" that all three consult — do not restate the type list. Neither the fixtures nor the snapshots can see any of this: an integral `double` and a `long` print alike, and the profile fixture's only number is `12.5`. All three are pinned by type assertions instead.
  - `Runtime/Plugins/iOS/` — `AdaptyIOS.cs` (P/Invoke bridge) + `Source/` (Swift/ObjC native plugin code)
  - `Runtime/Plugins/Android/` — `AdaptyAndroid.cs` (JNI bridge) + `Local/` (local AAR maven repo) + `AdaptySDKDependencies.androidlib` (Android maven dependencies)
  - `Runtime/Plugins/AdaptyNoop.cs` — Editor/no-op stub
  - `Runtime/Editor/AdaptySDKDependencies.xml` — iOS Swift Package declaration for External Dependency Manager
  - `Editor/` — Editor-only assembly (iOS build validation, Newtonsoft presence check, the `Adapty SDK > Install Dependencies` menu item)
- **`adaptyandroidwrapper/`** — Standalone Android Gradle project:
  - `unitywrapper/src/main/java/com/adapty/unity/` — `AdaptyAndroidWrapper.java` (entry point), callback handler, message handler
- **`tests/`** — .NET test projects for the JSON layer: `AdaptySDK.NextTests` (the suite), `shared/` (fixtures and snapshot helpers), `surface/` (the SDK compiled as a library to assert against), `aot-probe/`. Most of the suite asks the compiled assembly; `SourceConventionTests` and two checks in `ContractEnforcementTests` read the **sources** instead, for the things metadata cannot answer — `partial` does not survive compilation, an explicit enum value is indistinguishable from a counted one, and a file's directory is not a property of its types.
- **`Assets/Scripts/`** — Demo app scripts (not part of distributed SDK)
- **`cross_platform.yaml`** — Cross-platform API contract schema defining all request/response JSON formats and data types shared across iOS/Android/Unity

### Event System

Native SDKs push events (profile updates, flow view lifecycle, onboarding events) via the same JSON bridge. `Adapty.OnMessage(id, json)` in `Adapty.Events.cs` parses the payload and hands it to `Dispatch`, which switches on the event `id` and calls the registered listener interfaces. Nothing may escape `OnMessage`: the call arrives from native code with no handler behind it, so an exception takes the process down on IL2CPP rather than surfacing as a C# error. Every call into the app from the **live** API — a completion handler or a listener method — goes through `Callbacks.InvokeSafe`, which is one policy rather than 57 copies of it; the deprecated onboarding API keeps its own hand-written wrappers, and one call there has none at all: safe means the app's exception is rethrown carrying the context of the call and the original as `InnerException`, not that it is swallowed. `OnMessage`'s own guard is what actually contains it, and stays hand-written for that reason. Two event families are round-trips: flow permission requests are answered via `flow_view_did_answer_permission` (keyed by `event_id`), and Observer-mode purchases/restores report back via `observer_*_did_start/finish`.

The seven `onboarding_*` ids are the exception to the one-switch rule: they leave `Dispatch` through `OnLegacyOnboardingMessage`, which is `[Obsolete]`. That is what keeps the deprecation of the legacy onboarding API from raising `CS0618` on every case of the main switch — folding them back multiplies the warnings by about thirty. `LegacyOnboardingDispatchTests` pins the routing of all seven.

### Model Convention

One file per model in `Runtime/Models/`, and no `partial` unless the type really is split — six are, each for a nested part or for its deprecated half under `Obsolete/`. Every concrete public class is `sealed`; the only open ones are the four abstract roots a converter picks between, and the approved public surface is what catches a new class that forgets. Serialization is declared with attributes, not written by hand:

- `[DataContract]` on the type and `[DataMember(Name = "json_key")]` on each member, with `IsRequired = true` where the contract says the key is required. The JSON keys must match `cross_platform.yaml`, including which fields are required vs optional.
- `[Preserve]` on the type. Managed stripping otherwise removes it, and the failure shows only on a device, the first time a response carries the type. A nested type is covered by its declaring type's attribute; a member the serializer reaches through a method is not — a `[DataMember]` property, read through its getter, and an `[OnDeserialized]` callback — and needs its own. `StrippingGuardTests` asks the metadata for both.
- Conditional emission is a question for the model's own constructor, not for the serializer. A value the contract omits rather than sends empty is normalized to null where the object is built — `AdaptyConfiguration`'s builder does it for an identity carrying neither value, `AdaptyProductIdentifier`'s constructor for an empty base plan — and `NullValueHandling.Ignore` does the rest. There is no `ShouldSerializeX` convention: it existed in the resolver for three methods and was removed, so writing one now would silently do nothing for a field or a non-public member.
- A `oneOf` whose branches are one public object differing only by a discriminator and what that branch adds is **one flattened type** — a `Type`/`Status` enum plus the members, as `AdaptyPurchaseResult` and `AdaptyInstallationStatus` do. Where the branches are genuinely different shapes, the base class and its converter stay; that is the whole of `PolymorphicRoots` in `StrippingGuardTests` — `AdaptyCustomAsset`, `AdaptyOnboardingsAnalyticsEvent`, `AdaptyOnboardingsStateUpdatedParams`, `AdaptyOnboardingsInput` — and flattening one of those is not an improvement. A key the contract requires on one branch only is what no attribute can state: `AdaptyInstallationStatus` says it in a `[Preserve] [OnDeserialized]` method, which rejects a determined status without `details` and **drops** a `details` arriving on either other branch, matching what the subclass that had no such member did. Neither half is a rule about `oneOf` in general — `AdaptyPurchaseResult` normalizes nothing. Two things to know before writing a second such callback: Newtonsoft calls it through `MethodInfo.Invoke`, so whatever it throws arrives wrapped in a `TargetInvocationException` — harmless, since both boundaries a payload crosses catch `Exception` and print the inner one — and `StrippingGuardTests` will demand the `[Preserve]`.
- A collection a model hands back is a **read-only view over private concrete storage**: `[DataMember]` sits on a `private List<T>`/`Dictionary<K,V>`, and the public member is an `IReadOnlyList`/`IReadOnlyDictionary` wrapping it. The declared interface alone would not do — `ReadOnlyCollection` and `ReadOnlyDictionary` implement the mutable interfaces too, so the cast back compiles; what stops the write is that it yields the wrapper, which throws. The views are built in a `Freeze()` called from the constructor **and** from `[OnDeserialized]`, not beside the field: `ObjectCreationHandling.Replace` hands the deserializer a new collection, so a wrapper made in the initializer would wrap the discarded one and read empty forever. On the way in the rule is the mirror — take `IReadOnlyDictionary` and copy, so a caller still writing to its own dictionary cannot change what was handed over.
- **The wire is UTC; the public API is local.** A `DateTime` the SDK hands back is the same instant expressed on the machine's clock (`Kind == Local`), because these dates — a subscription's expiry, an access level's activation — are shown to end users, and `expiresAt > DateTime.Now` is what an app naturally writes. The write path is the mirror: anything the app supplies goes out as UTC, and a `Kind == Unspecified` value is read as local, since `new DateTime(2026, 7, 30, 22, 0, 0)` for a custom timer means 22:00 on the user's clock. `AdaptyConverterDateTime` owns both halves and cannot be replaced by `DateTimeZoneHandling.Utc`, which reads correctly but *relabels* an unspecified value on write instead of converting it — measured, and pinned by `DatesTheAppSuppliesAreWrittenAsUtc`. That test only discriminates off UTC, so CI sets `TZ`; on a UTC host it ignores itself rather than passing.
- Every public enum member states its number explicitly, and the numbers never move — they are public API even where the wire format is a string, and an inserted member would otherwise renumber everything below it. `EveryPublicEnumMemberStatesItsValue` reads the model sources for this, since metadata cannot tell an explicit value from a counted one; the approved public surface catches a number that moves.
- Enums follow one of two contracts, and the choice is part of the wire format. A **string** enum maps **every** member with `[EnumMember(Value = "...")]`, and a value outside that set fails the read: the SDK ships pinned to the native SDKs, so an unlisted string is a broken payload rather than one from the future. There is no `Unknown` fallback — only `AdaptyPaymentMode` and `AdaptySubscriptionPeriodUnit` have such a member, because the contract lists `"unknown"` among *their* values. Where the contract does want an open set it says so, and the model holds a `string`: a flow permission, an onboarding event name. A **numeric** enum — `AdaptyErrorCode`, `AppTrackingTransparencyStatus` — carries the native number and declares no `[EnumMember]` at all: `AdaptyConverterStringEnum.CanConvert` skips it and Newtonsoft's default numeric handling applies. Adding `[EnumMember]` to a numeric enum silently switches it to strings. `AdaptyConverterStringEnum` writes through stock `StringEnumConverter` but **reads with its own ordinal map**, because stock reading is lenient in three ways the contract does not allow: it accepts the C# member name as well as the `[EnumMember]` one, ignores case, and trims the value — so `"UserCancelled"`, `"USER_CANCELLED"` and `" user_cancelled "` would all pass for `"user_cancelled"`. Neither half of the converter can report a member with no `[EnumMember]` (stock writing falls back to the C# name) or two members sharing one, so `EveryMemberOfAContractNamedEnumHasItsName` asserts both over the metadata.

`tests/AdaptySDK.NextTests` enforces all of this: a model added without `[Preserve]` fails `StrippingGuardTests`, and one whose output changes fails its approved snapshot.

### Overloads

The completion handler is the last parameter of every public method, is never optional, and never has a default — the SDK does not offer a fire-and-forget call, so no method taking a completion handler has an optional parameter at all. The two defaults that do exist are on a constructor, `AdaptyPurchaseParameters`, where both are trailing and stable, which is the one shape defaults are for. That is what forces the convenience forms to be **overloads** rather than trailing defaults: in every group the optional-looking argument (`fetchPolicy`, `purchaseParameters`, `variationId`, a presentation style) sits *before* the callback, and C# has no required parameter after an optional one. Collapsing them would mean making the callback optional or moving it, and neither is on the table.

Each short form is a one-line forward to the canonical method — audited, none carries logic of its own. Five groups instead overload by the *type* of the first argument (`Activate`, `OpenWebPaywall`, `CreateWebPaywallUrl`, `ShowDialog`, `UpdateAttribution`); passing a literal `null` there is ambiguous and fails as `CS0121` — measured on two of them — which is loud and acceptable.

### Static state and Play Mode

With Domain Reload disabled — the default for fast iteration — Unity keeps static fields between Play Mode runs. Anything the SDK holds **on the developer's behalf** has to be cleared at `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`, which runs before the first scene of every run: today the four listeners and the no-op bridge's test hook. The legacy onboarding listener is cleared from its own part under `Obsolete/`, marked `[Obsolete]` itself, so a live method does not reference a deprecated field and raise `CS0618` where the caller has nothing to act on.

Infrastructure is **not** reset — the contract resolver, the settings, the converters' type caches. It is derived from the assembly, identical every run, and rebuilding it would only cost startup time. Nor is the native bridge's registration flag: clearing it separately from the native side would register a callback twice.

`EveryResetIsRegisteredWithUnity` is the guard — it checks the load type too, not just the attribute: `AfterSceneLoad` runs once a scene has already had the chance to register a listener, so a reset there would clear the new run's own. The Editor assembly is out of reach of all this, and keeps its own `[InitializeOnEnterPlayMode]` in `AdaptyDependencies` for the one subscription that does not clean itself up — a Package Manager request left in flight. What no desktop test can show is that Unity calls it at all; that is two consecutive Play Mode runs in the acceptance pass, on the 2022.3 floor and on Unity 6.

### Platform conditionals

There are 26 `#if` in the package, and each belongs to one of three kinds. **Compilation boundary** — a native symbol exists only there: the `_Adapty` and `_AdaptyCallbackAction` aliases, everything under `Plugins/iOS` and `Plugins/Android`, the Kids Mode post-processor. **Wire contract** — the contract itself differs: a `[DataMember]` the schema marks platform-only, `offer_tags` read on Android alone, the offer id required on one more branch there. **Public API behaviour** — three iOS-only methods that off iOS report `null`, meaning success.

Nothing else qualifies. A constructor must not re-decide by define what the layer above already decided: `AdaptySubscriptionOffer` used to null `OfferTags` off Android although its only caller, the converter, passes null there anyway — the approved snapshots did not move when it went, which is what redundant means.

The public surface is byte-identical on all three platforms; the conditionals change what is read and written, never what is declared. Keep it that way — `diff`ing the three approved surface files is the check.

**Open decision, deliberately not taken here:** off iOS, `UpdateAppStoreCollectingRefundDataConsent`, `UpdateAppStoreRefundPreference` and `PresentCodeRedemptionSheet` report `null` — indistinguishable from success. On Android that is a silent no-op, not an unsupported-platform error, and unlike the Editor path it never reaches the no-op bridge that would say so. Changing it is a behaviour change for callers and belongs in a release that expects one.

### Deprecation

Deprecating one entry point is not enough — mark everything the deprecated API hands back or takes, or the warning only reaches the caller at the registration call and never at the type they wrote. The attribute is written `[System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]`, with the same sentence everywhere. Marking a member is also what decides where it lives: it moves to `Runtime/Obsolete/`, and the two travel together.

**The legacy onboarding API has to keep working, and nothing more.** It is still in the shipped public surface and apps still call it, so a bug in it gets fixed. Everything else is explicitly not owed to it: **its failing to meet a convention this document sets is not a defect**, needs no note and no follow-up item. It kept its polymorphic converters through the converter audit and its mutable `IList` through the read-only collections work, and that is the expected outcome, not debt. The line is what a change touches: repository-wide formatting applies to it like anywhere else — headers, `using` placement, `is not` type patterns — while anything reaching its API, a renamed parameter included, does not. Breaking changes to it are undesirable — the one structural change it did get, the move to `Runtime/Obsolete/`, touched no name, signature or type. When a sweep states a rule in the changelog, say plainly that the deprecated API is the exception rather than implying it was covered: an inaccurate claim about it is a defect, touching it to make the claim true is not.

Marking a public type deprecates it for the SDK's own code too, so expect `CS0618` inside the package, and expect it to reach the console of everyone who installs the SDK: the Editor reports it. `CS0649` is the opposite case and does not — Unity passes `/nowarn:0649` and `/nowarn:0169` to every assembly it compiles, which is why the package's reflection-assigned fields are silent there while `tests/surface` has to suppress them itself. Never silence `CS0618` — `#pragma warning disable` is not used in this repository. Mark the internal parts that serve the deprecated API instead (private fields, helpers, converters): a reference from obsolete code to obsolete code raises nothing, which pushes the warnings back to the boundary where live code really does touch the deprecated API. Those remaining warnings are the point, not a problem to solve.

The public surface snapshots record signatures without attributes, so nothing fails if `[Obsolete]` is dropped from a member.

## Version Bumping

When releasing a new version, update:
1. `Adapty.SDKVersion` in `Packages/com.adapty.unity-sdk/Runtime/Adapty.cs`
2. `version` in `Packages/com.adapty.unity-sdk/package.json`
3. Native dependency versions: iOS in `Runtime/Editor/AdaptySDKDependencies.xml`, Android in `Runtime/Plugins/Android/AdaptySDKDependencies.androidlib/build.gradle` and `adaptyandroidwrapper/unitywrapper/build.gradle` (then rebuild the AAR into `Runtime/Plugins/Android/Local/io/adapty/internal/unity-wrapper/<version>/`, and **delete the previous one** — no `build.gradle` in the package references the artifact, Unity picks up any `.aar` under `Plugins/Android` as a plugin, so two versions side by side both reach the player)
4. `cross_platform.yaml` schema `$id` version — must match the canonical contract in AdaptySDK-iOS (`Sources.AdaptyPlugin/cross_platform.yaml`); diff the two files, not just the version
5. `CHANGELOG.md` and the `_upm.changelog` string in `package.json` — keep both in sync, the latter is what Package Manager shows after an update
6. Managed dependency versions, when they move: `dependencies` and `peerDependencies` in `package.json` **and** the constants in `Editor/AdaptyDependencies.cs`, which is what installs them for `.unitypackage` users

The `contract-conformance` skill compares the contract with the C# that restates it; run it whenever step 3 or step 4 moves. One key is worth naming here because a conformance run will keep reporting it: `CustomerIdentityParameters.obfuscated_profile_id` is declared in the contract, Android only, and implemented by nobody — not by this SDK, not by AdaptySDK-Android 4.0.1, and iOS has no such field at all. It was left unimplemented deliberately rather than guessed at. **Re-check it on every native bump**: the moment Android starts reading it, the Unity side has to carry it too, and that is a public API change to `AdaptyCustomerIdentity`, so it wants to land in a release that expects one.
