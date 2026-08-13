# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.0.0-beta.2] - 2026-08-08

Upgrading from 3.x: see [MIGRATION.md](https://github.com/adaptyteam/AdaptySDK-Unity/blob/main/MIGRATION.md).
If you install from a `.unitypackage`, add `com.unity.nuget.newtonsoft-json` **before** importing —
until it is there the SDK assembly is skipped, so code that calls Adapty will not compile, and the
Editor menu that installs it is unavailable for the same reason.

### Added

- **iOS Kids Mode** for the App Store Kids Category / COPPA. Setting the `ADAPTY_KIDS_MODE` scripting
  define enables the `KidsMode` trait on the AdaptySDK-iOS Swift package, so IDFA, AdSupport and
  AppTrackingTransparency are compiled out of the binary, and forces
  `apple_idfa_collection_disabled` in the runtime configuration. Swift package traits need Xcode 26,
  which v4 requires anyway. Set the define in Player Settings; a build profile's scripting defines
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
  2024 while the native Android SDK kept sending the code, so `RestorePurchases` on a profile with
  nothing to restore returned an error that could only be matched against a literal `1004`. Nothing
  about the error changes; it now has its name back. The code is Android-only — iOS does not define
  it.
- Seven more `AdaptyErrorCode` members the native SDKs emit but this enum never named:
  `UnidentifiedUserLogout` (3020, both platforms, from `Logout` on an unidentified profile),
  `PaymentPendingError` (1050, iOS, from `ReportTransaction`), `BillingNetworkError` (112, Android),
  and `WrongAssetType` (4104), `JsException` (4105), `NavigatorNotFound` (4106),
  `InvalidActionUrl` (4107) — the four the Android flow renderer reports through
  `FlowViewDidReceiveError`. `AdaptyErrorCode` carries the native number, so these codes always
  arrived; they simply had no constant to match against. Each one was traced in the iOS 4.0.2 and
  Android 4.0.1 sources to the place the native SDK produces it — a throw site for all but 112, which
  comes out of the `fromBilling` mapping.

### Changed

- **The minimum supported Unity version is now declared: 2022.3.** It was never stated before, in
  `package.json` or anywhere else, so Package Manager let any Editor install a package it might not
  be able to compile. Nothing was dropped — the floor is now stated.

  Installing on the floor is verified: a clean `.unitypackage` import and
  **Adapty SDK > Install Dependencies** were run end to end on 2022.3, and the SDK compiles
  afterwards. Player builds, device runs and the rest of the acceptance matrix were done on Unity 6,
  which is what the SDK is developed against.
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
- The legacy onboarding API is deprecated in favor of flows, and `[Obsolete]` now covers the whole
  of it rather than only its entry points: `IAdaptyOnboardingsEventsListener`, `AdaptyOnboarding`,
  `AdaptyUIOnboardingView`, `AdaptyUIOnboardingMeta`, the `AdaptyOnboardingsAnalyticsEvent`,
  `AdaptyOnboardingsStateUpdatedParams` and `AdaptyOnboardingsInput` hierarchies, and the
  `AdaptyUI.ShowDialog` overload taking an `AdaptyUIOnboardingView`. Naming any of them warns now,
  where before only calling one of the six entry points did.
- **Breaking:** removed two members that only forwarded to another one —
  `AdaptyProfile.NonSubscription.IsOneTime` (returned `IsConsumable` unchanged; its summary had
  called it deprecated for several versions without an attribute to back that up) and
  `AdaptyPlacement.GetIsTrackingPurchases` (wrapped the public `IsTrackingPurchases` field, whose
  `null` case cannot occur).
- **Breaking:** a string the contract does not list now fails the read instead of degrading to
  `Unknown`, and the six members that existed only to catch one are gone —
  `AdaptyPurchaseResultType.Unknown`, `AdaptySubscriptionOfferType.Unknown`,
  `AdaptySubscriptionRenewalType.Unknown`, `AdaptyUIDialogActionType.Unknown`,
  `AdaptyUIUserActionType.Unknown` and `AdaptyWebPresentation.Unknown`. The SDK ships pinned to the
  native SDKs it is built against, so an unlisted value is a broken payload rather than one from the
  future, which is what v3 did too; the fallback was carried over from the beta and the contract
  never allowed an arbitrary string in these positions. `AdaptyPaymentMode.Unknown` and
  `AdaptySubscriptionPeriodUnit.Unknown` stay, because the contract lists `"unknown"` among their
  values. No surviving member changed its numeric value. `AdaptySubscriptionOfferType.Unknown` was
  also the one fallback that could be sent — as `"unknown"`, which no branch of the contract's offer
  identifier accepts — so a purchase of such an offer failed on the native side instead of here.
  A JSON number is no longer accepted for a string enum either; it used to read as `Unknown`.
- **iOS builds now require Xcode 26 or newer.** AdaptySDK-iOS 4.0 declares
  `swift-tools-version: 6.2`, where the 3.17.2 that v3 pinned declared 6.0, and Swift Package Manager
  refuses a package whose tools version is newer than the installed toolchain. On Xcode 16 the build
  fails while resolving the dependency, before anything is compiled. This is the floor for the whole
  SDK, not only for Kids Mode. Nothing in Unity can check it — the Editor never sees which Xcode will
  open the generated project.
- Updated the cross-platform contract to 4.0.2 and the native SDK dependencies to
  iOS 4.0.2 and Android 4.0.1. `MakePurchase`'s purchase parameters and
  `AdaptyUICreateFlowViewParameters.ProductPurchaseParameters` are now documented as Android only,
  matching what the native SDKs actually do with them.
- Errors returned by `flow_view_did_answer_permission` and by the observer-mode round trips are now
  logged instead of being swallowed.
- **Breaking:** removed `AdaptyErrorCode.InvalidJson` (23) and `AdaptyErrorCode.PendingPurchase`
  (25). Neither native SDK has these codes: iOS 4.0 declares no 23 or 25 at all, and the Android
  enum runs 20, 22, 24, 97 — the two numbers are gaps left where the members were deleted, while
  their neighbours stayed. Nothing can raise them, so nothing can match on them; a pending
  purchase is reported as `AdaptyPurchaseResultType.Pending` rather than as an error. Removing a
  public member is a breaking change, which is why it happens in a major.
- **Breaking:** removed what the old JSON layer left behind.
  `AdaptyRefundPreferenceExtensions.ToJSONNode` was the last of the `ToJSONNode` extension classes —
  the other three went with `AdaptySDK.SimpleJSON`, while this one survived because it sat in
  `Models/`. The SDK does not call it: the refund preference is serialized through its
  `[EnumMember]` mapping like every other enum. Two constructors that only the hand-written parser
  ever called are gone too, though those were never public.
- **Breaking:** the collections on a response model are read-only. `AdaptyProfile`, `AdaptyFlow`,
  `AdaptyFlowPaywall`, `AdaptySubscriptionOffer` and `AdaptyRemoteConfig` hand back
  `IReadOnlyList<T>` and `IReadOnlyDictionary<K, V>` instead of `IList<T>` and `IDictionary<K, V>`,
  and `AdaptyProfile.NonSubscriptions` is read-only at both levels. A `readonly` field never made
  these models immutable: the reference could not be replaced, but the contents could, and the SDK
  handed out its own storage. The views refuse to write — casting one back to `IDictionary` still
  compiles, because `ReadOnlyDictionary` implements it, and every mutating call throws
  `NotSupportedException`. `GetPaywallProducts` reports an `IReadOnlyList<AdaptyPaywallProduct>` for
  the same reason. The deprecated onboarding API is the exception and keeps its old shapes —
  `AdaptyOnboardingsMultiSelectParams.Params` is still an `IList` handed over as it was received.
  It is maintained rather than improved until it is removed, so do not read the sentence above as
  covering it.
- **Breaking:** the parameter objects take the narrowest abstraction and copy it.
  `AdaptyUICreateFlowViewParameters.SetCustomTags`, `SetCustomTimers`, `SetCustomAssets` and
  `SetProductPurchaseParameters` accept an `IReadOnlyDictionary` and copy it, so a caller that keeps
  writing to its own dictionary afterwards no longer changes what the view is built with; the four
  matching members are now read-only properties rather than public fields. `UpdateAttribution` takes
  an `IReadOnlyDictionary<string, object>`, and `AdaptyProfileParameters.CustomAttributes` exposes a
  view rather than the dictionary the builder writes into.
- **Breaking, at the call site only:** the analytics-event parameter of
  `IAdaptyFlowsEventsListener.FlowViewDidReceiveAnalyticEvent` is named `parameters` rather than
  `@params`. The CLR signature is unchanged, so an implementation keeps compiling and only a call
  passing it as a named argument — `@params:` — has to be renamed. The deprecated onboarding
  listener keeps its own `@params`.
- **Breaking:** `IAdaptyFlowsEventsListener.FlowViewDidReceiveAnalyticEvent` and
  `IAdaptyUISystemRequestsHandler.FlowViewDidAskPermission` receive `IReadOnlyDictionary` instead of
  `IDictionary`. Implementations need the signature updated; nothing else about them changes.
- **Breaking:** `AdaptyPlacementFetchPolicy.Default`, `.ReloadRevalidatingCacheData` and
  `.ReturnCacheDataElseLoad` are `readonly`. They were public mutable statics, so any code could
  repoint the SDK's shared defaults for every other caller — and with Domain Reload disabled the
  change outlived Play Mode.
- Registered listeners no longer survive Play Mode. With Domain Reload disabled — the default for
  fast iteration — Unity keeps static fields between runs, so the event listener, flows listener,
  system request handler and observer-mode resolver a previous run registered were still there for
  the next one, which then delivered its events to objects belonging to a session that had ended.
  The SDK now clears them, and the no-op bridge's test hook with them, at
  `RuntimeInitializeLoadType.SubsystemRegistration`. Call `SetEventListener` and friends on start as
  you always should; nothing changes when Domain Reload is on.
- **Breaking:** every concrete public class is now `sealed`; the four abstract roots the wire
  contract needs — `AdaptyCustomAsset` and the three legacy onboarding hierarchies — stay open. For
  most of them this states what was already true: a response model has no constructor reachable from
  outside the SDK — private, or `internal` as on `AdaptySubscriptionOffer` — so no type of yours
  could derive from one in the first place. Eleven could, all of them inputs rather than responses —
  the parameter objects, the two identity types, the three builders — and for those this is a real
  restriction. Nothing was designed for extension: no model declares a
  `virtual` or `protected` member, and a subclass of a parameter object would have been a trap,
  since the SDK serializes the declared contract and silently drops whatever the subclass added.
  `AdaptyProductIdentifier` is the one where it also fixes something: it compares by value and is
  used as a dictionary key, and a subclass would have broken the symmetry of `Equals`.
- **Breaking:** `AdaptyInstallationStatus` is one sealed type carrying a `Status` and a `Details`,
  instead of a base class and the three subclasses `AdaptyInstallationStatusNotAvailable`,
  `AdaptyInstallationStatusNotDetermined` and `AdaptyInstallationStatusDetermined`, which are gone
  along with their public constructors — the type is a response, and only the SDK builds one. The
  new `AdaptyInstallationStatusType` names the same three states the contract lists, so a caller
  switches on a value rather than testing for a type. `Details` is non-null exactly when `Status` is
  `Determined`: the determined branch is rejected without it, and on the other two branches a stray
  one is dropped, which is what the removed subclasses did with it. Nothing about the wire format
  changes, and the polymorphic converter the hierarchy needed is gone with it.
- **Breaking:** `AdaptyFlowPaywall.ProductReference` is now `internal`. Its constructor was private
  and every one of its members was already `internal`, so no instance of it could be obtained or
  read from outside the SDK; the type was public only because in 3.x it was the top-level
  `AdaptyProductReference`.

### Fixed

- An exception thrown by the completion handler passed to the deprecated `Adapty.GetOnboarding` now
  arrives with the name of the call that raised it, and the original as `InnerException`, the way
  every other Adapty call already reported one. That single method handed the exception on untouched,
  so it surfaced with no indication of which callback had failed. Only `GetOnboarding` was affected.
- `AdaptyProfileParameters.SetBirthday` now sends the date the contract asks for. The key is
  declared `YYYY-MM-dd` and was built by hand from the parts, without padding, so 7 March 1990 went
  out as `1990-3-7` rather than `1990-03-07`. Every birthday whose month or day is below the tenth
  was affected, on every platform, since 3.x. No test caught it because the only date in the
  fixtures is 10 December 1815, whose month and day are both two digits.
- An offer whose branch of the contract requires an `id` is now rejected without one, at the point
  it is read: promotional and win-back everywhere, and introductory on Android. The converter read
  the identifier leniently for every branch, so a missing one became a null that `NullValueHandling`
  then dropped from the purchase request — leaving the native side to fail the decode instead. The
  error now names the missing key where it went missing.
- A subscription offer without `phases` is now rejected instead of being handed over half built.
  The contract requires the key, and the converter that reads it enforced `offer_identifier` and
  `type` but not this one, so a payload without it produced an offer whose phases were null — an
  object that looks valid and has no prices in it. Neither native SDK can currently send such a
  payload: iOS always encodes the key, and Android builds the offer only when its phases are not
  empty.
- Dates reach your code as local time again, as they did in 3.x. The wire is UTC and the public API
  is local — a subscription's expiry compares against `DateTime.Now` — but the payload from the
  native side was being turned into a document by a reader that recognises dates while it builds the
  tree, so it settled their kind before anything else had a say and every date arrived as
  `DateTimeKind.Utc`. Both the event callbacks and the reply to every method were affected; only
  4.0.0-beta.1 ever behaved that way. Call `ToUniversalTime()` where you need the instant.
- A date-looking string inside an untyped payload survives as it was sent. The same reader turned
  `params` of `FlowViewDidReceiveAnalyticEvent` into dates and back into strings, so
  `"2026-07-30T10:00:00.000Z"` reached the listener as `"07/30/2026 10:00:00"`.
- `AdaptyPurchaseResult.ToString()` no longer throws. The contract carries `profile` in the success
  branch only, and the method dereferenced it unconditionally, so describing a pending or cancelled
  purchase — logging one, for instance — raised a `NullReferenceException`.
- `ReportTransaction` no longer reports `DecodingFailed`. It decoded the response as a profile,
  while the native side returns `{"success": true}`, so the completion handler always received a
  decoding error even though the transaction had been reported successfully.
- Calling the SDK in the Editor now returns a readable "not supported on this platform" error
  instead of failing to parse a null response. That holds for the whole surface now:
  `UpdateAppStoreCollectingRefundDataConsent`, `UpdateAppStoreRefundPreference` and
  `PresentCodeRedemptionSheet` were guarded so that the Editor took their off-iOS branch, which
  reports a null error — indistinguishable from success — so testing them in the Editor looked like
  they had worked. On an Android device they still report `null`, which is unchanged.
- Custom linear gradient assets are now serialized from every color and alpha key of the Unity
  `Gradient`. Gradients whose color and alpha keys differed in count threw
  `"Color keys and alpha keys arrays must have the same length"`, and gradients whose alpha keys sat
  at different times than the color keys silently lost the shape of the alpha ramp, because keys were
  paired by index.
- `AdaptyProductIdentifier` now implements value equality, so identifiers built from a flow work as
  keys in the dictionary passed to
  `AdaptyUICreateFlowViewParameters.SetProductPurchaseParameters`; previously they were compared by
  reference and the parameters silently applied to nothing. An empty base plan id is now the same as
  none, at construction, so two identifiers that always went on the wire identically are equal and
  hash alike — a base plan read out of an empty text field no longer produces a key that matches
  nothing.
- `AdaptyFlow.VendorProductIds` and `AdaptyFlow.ProductIdentifiers` no longer return duplicates when
  several paywall variations of the flow offer the same product.

## [3.17.0] - 2026-07-02

### Added

- `AdaptyProfile.AppliedAttributionSources` — attribution sources applied to the profile (e.g. Apple Search Ads), available for segmentation.
- `AdaptyUIUserAction.OpenIn` — for `OpenUrl` actions, indicates whether the link should open in an in-app or external browser.

### Changed

- Migrated the SDK to a Unity Package Manager layout; install via Git URL with `?path=/Packages/com.adapty.unity-sdk`.
- Updated native SDK dependencies and the cross-platform contract to 3.17.2 (iOS and Android).
