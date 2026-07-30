# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0-beta.1] - 2026-07-30

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

### Changed

- **Breaking:** migrated the paywall API to flows — `AdaptyPaywall` → `AdaptyFlow`,
  `GetPaywall` → `GetFlow`, `CreatePaywallView` → `CreateFlowView`, and the corresponding models,
  events and view controllers. Event listener interfaces now carry the `I` prefix
  (`IAdaptyEventListener`, `IAdaptyFlowsEventsListener`, `IAdaptyUISystemRequestsHandler`,
  `IAdaptyUIObserverModeResolver`, `IAdaptyOnboardingsEventsListener`).
- The legacy onboarding API is deprecated in favor of flows.
- Updated the cross-platform contract to 4.0.2 and the native SDK dependencies to 4.x
  (iOS 4.0.2, Android 4.0.1). `MakePurchase`'s purchase parameters and
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
