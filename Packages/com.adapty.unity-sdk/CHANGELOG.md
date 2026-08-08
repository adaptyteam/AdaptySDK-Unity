# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0-beta.2] - 2026-08-04

Upgrading from 3.x: see [MIGRATION.md](https://github.com/adaptyteam/AdaptySDK-Unity/blob/main/MIGRATION.md).
If you install from a `.unitypackage`, add `com.unity.nuget.newtonsoft-json` **before** importing —
until it is there the SDK assembly is skipped, so code that calls Adapty will not compile, and the
Editor menu that installs it is unavailable for the same reason.

### Added

- **iOS Kids Mode** for the App Store Kids Category / COPPA. Setting the `ADAPTY_KIDS_MODE` scripting
  define enables the `KidsMode` trait on the AdaptySDK-iOS Swift package, so IDFA, AdSupport and
  AppTrackingTransparency are compiled out of the binary, and forces
  `apple_idfa_collection_disabled` in the runtime configuration. Requires Xcode 26 or newer
  (Swift package traits). Set the define in Player Settings; a build profile's scripting defines
  work too, but only once Unity has recompiled the Editor assemblies for them, and the iOS build
  fails if the SDK detects that it is running against stale ones.
  `BuildPlayerOptions.extraScriptingDefines` is not supported at all, because it reaches the player
  assemblies only and would leave the trait unapplied while the runtime reports Kids Mode.
- `AdaptyUICreateFlowViewParameters.Locale` — the localization to render the flow with. A flow is
  localized when its view is built, so this is the only place that selects the localization.
  Requires the native iOS 4.0.2 and Android 4.0.1 releases, both of which are now the pinned
  dependencies.
- `AdaptyUIFlowView.Locale` — the localization the view was actually built with: the requested one
  when it resolved, and the flow's default localization otherwise. Null when paired with a native SDK
  that does not report it.
- `AdaptyUICreateFlowViewParameters.EnableSafeAreaPaddings` (Android only) — lays the flow view out
  without safe area paddings when set to `false`.
- **Adapty SDK > Install Dependencies** — installs whichever of the SDK's package dependencies are
  missing: Newtonsoft.Json, and External Dependency Manager along with the OpenUPM scoped registry it
  is published on. A `.unitypackage` carries assets only and can bring neither, and External
  Dependency Manager has always had to be installed by hand even alongside Package Manager. Packages
  already in the project are left as they are.
- `AdaptyErrorCode.NoPurchasesToRestore` (1004) — restored. The member was commented out in December
  2024 while the native SDKs kept sending the code, so `RestorePurchases` on a profile with nothing
  to restore returned an error that could only be matched against a literal `1004`. Nothing about the
  error changes; it now has its name back.

### Changed

- **The minimum supported Unity version is now declared: 2022.3.** It was never stated before, in
  `package.json` or anywhere else. Nothing was dropped — Package Manager now states the floor
  instead of letting an older Editor install a package it cannot compile.
- **The JSON layer now uses Newtonsoft.Json instead of the bundled SimpleJSON.** The package depends
  on `com.unity.nuget.newtonsoft-json` 3.2.2, which Package Manager installs for you and
  **Adapty SDK > Install Dependencies** installs for everyone else. While Newtonsoft is absent the
  SDK assembly is skipped by a define constraint instead of failing to compile. The SDK reports the
  reason in the Editor console — as long as its Editor assembly loads, which it does not while your
  own scripts fail to compile. A second copy of Newtonsoft is reported the same way, since it makes
  its types ambiguous.
  The models, their members and every method signature are unchanged, so calling code is unaffected.
  **Breaking for anything that used `AdaptySDK.SimpleJSON` directly:** the namespace is gone, and its
  public types (`JSON`, `JSONNode`, `JSONObject`, `JSONArray` and the rest) went with it.
- **Breaking:** migrated the paywall API to flows — `AdaptyPaywall` → `AdaptyFlow`,
  `GetPaywall` → `GetFlow`, `CreatePaywallView` → `CreateFlowView`, and the corresponding models,
  events and view controllers. Event listener interfaces now carry the `I` prefix
  (`IAdaptyEventListener`, `IAdaptyFlowsEventsListener`, `IAdaptyUISystemRequestsHandler`,
  `IAdaptyUIObserverModeResolver`, `IAdaptyOnboardingsEventsListener`).
- The legacy onboarding API is deprecated in favor of flows.
- Updated the cross-platform contract to 4.0.2 and the native SDK dependencies to
  iOS 4.0.2 and Android 4.0.1. `MakePurchase`'s purchase parameters and
  `AdaptyUICreateFlowViewParameters.ProductPurchaseParameters` are now documented as Android only,
  matching what the native SDKs actually do with them.
- Errors returned by `flow_view_did_answer_permission` and by the observer-mode round trips are now
  logged instead of being swallowed.

### Fixed

- `ReportTransaction` no longer reports `DecodingFailed`. It decoded the response as a profile,
  while the native side returns `{"success": true}`, so the completion handler always received a
  decoding error even though the transaction had been reported successfully.
- Calling the SDK in the Editor now returns a readable "not supported on this platform" error
  instead of failing to parse a null response.
- Custom linear gradient assets are now serialized from every color and alpha key of the Unity
  `Gradient`. Gradients whose color and alpha keys differed in count threw
  `"Color keys and alpha keys arrays must have the same length"`, and gradients whose alpha keys sat
  at different times than the color keys silently lost the shape of the alpha ramp, because keys were
  paired by index.
- `AdaptyProductIdentifier` now implements value equality, so identifiers built from a flow work as
  keys in the dictionary passed to
  `AdaptyUICreateFlowViewParameters.SetProductPurchaseParameters`; previously they were compared by
  reference and the parameters silently applied to nothing.
- `AdaptyFlow.VendorProductIds` and `AdaptyFlow.ProductIdentifiers` no longer return duplicates when
  several paywall variations of the flow offer the same product.

## [3.17.0] - 2026-07-02

### Added

- `AdaptyProfile.AppliedAttributionSources` — attribution sources applied to the profile (e.g. Apple Search Ads), available for segmentation.
- `AdaptyUIUserAction.OpenIn` — for `OpenUrl` actions, indicates whether the link should open in an in-app or external browser.

### Changed

- Migrated the SDK to a Unity Package Manager layout; install via Git URL with `?path=/Packages/com.adapty.unity-sdk`.
- Updated native SDK dependencies and the cross-platform contract to 3.17.2 (iOS and Android).
