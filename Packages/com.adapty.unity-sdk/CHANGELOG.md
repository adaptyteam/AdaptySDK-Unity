# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.1.0] - 2026-08-20

Upgrading from 4.0: see [MIGRATION-v4.0-to-v4.1.md](https://github.com/adaptyteam/AdaptySDK-Unity/blob/4.1.0/MIGRATION-v4.0-to-v4.1.md).

### Breaking Changes

- `Adapty.UpdateAttribution(...)` is renamed to `Adapty.UpdateExternalAttribution(...)`, with the
  `source` parameter renamed to `provider`, matching the native 4.1 attribution API rename. The old
  name is removed without a deprecated alias.
- `AdaptyProfile.AppliedAttributionSources` is renamed to
  `AdaptyProfile.AppliedExternalAttributionProviders`. The wire key is unchanged.
- `IAdaptyEventListener` gained `OnReceivePromotedPurchase(AdaptyPromotedProduct)`; implementations
  must add the method.
- The 4.1 natives read **fallback file format 11** and reject the format 10 files v4.0 shipped
  against, on both platforms: `Adapty.SetFallback` reports `DecodingFailed` (`adapty_code: 2006`,
  *"The fallback paywalls version is not correct"*) on iOS, and `WrongParam` (`adapty_code: 3001`,
  *"The fallback file version is not correct"*) on Android. Re-download both fallback files from
  the Adapty Dashboard.
- **Adapty Attribution is opt-in, and installation details come with it.** The 4.1 natives collect
  installation details only when the configuration asks them to, where the 4.0 natives collected
  them unconditionally: `IAdaptyEventListener.OnInstallationDetailsSuccess` stops firing and
  `Adapty.GetCurrentInstallationStatus` stops reporting
  `AdaptyInstallationStatusType.Determined` until the app activates with
  `AdaptyConfiguration.Builder.SetAdaptyAttributionEnabled(true)`. Both platforms behave this way,
  and nothing in the API changes shape — an app that reads installation details and does not set
  the flag simply stops receiving them.

### Added

- `AdaptyConfiguration.Builder.SetAdaptyAttributionEnabled(bool)` — enables the Adapty Attribution
  service. Not sent unless set, leaving the native default (off), which in 4.1 is also what turns
  installation details off; see the breaking entry above.
- `Adapty.MakePromotedPurchase(AdaptyPromotedProduct, ...)` and
  `IAdaptyEventListener.OnReceivePromotedPurchase` — App Store promoted in-app purchases (iOS only;
  on other platforms the completion handler reports an error). At native iOS 4.1.0 the event is not
  yet emitted — the native SDK still completes promoted purchases by itself, and the exact pin keeps
  it that way — so the listener method starts firing only once a future SDK release moves the pin to
  a native that reports them.
- `AdaptyFlow` carries the 4.1 `ui_schema` (custom flow layouts, UIBuilder 5.1) through to the
  renderer. It is not public API: the schema is the renderer's own data, and the SDK only makes sure
  a flow handed back to the native side keeps it.

### Native and Protocol

- The native dependencies are pinned to AdaptySDK-iOS 4.1.0, which includes the 4.0.3 hotfix, and
  on Android to crossplatform 4.1.0 / android-sdk 4.1.0 / android-ui 4.1.0, with the bundled
  unity-wrapper AAR rebuilt at 4.1.0 from `adaptyandroidwrapper/`.
- [Android] `Adapty.RestorePurchases` no longer reports `NoPurchasesToRestore` (1004): the 4.1
  native completes with the current profile when there is nothing to restore. iOS never produced
  the code, so nothing sends it now; the `AdaptyErrorCode` member stays.
- The cross-platform contract is 4.1.0: `update_external_attribution_data`,
  `make_promoted_purchase`, `did_receive_promoted_purchase`, `adapty_attribution_enabled` and
  `ui_schema` are new, and the offer identifier of a product request now travels nested as
  `subscription.offer.offer_identifier` — the natives read both forms, the flat
  `subscription_offer_identifier` is no longer written.

## [4.0.0-beta.2] - 2026-08-15

> Never published on its own: 4.1.0 is the first release to carry these changes. The section stays
> because the work of coming from 3.x is described here, and the 4.1.0 section above states only
> what moved since 4.0.

Upgrading from 3.x: see [MIGRATION-v3.17-to-v4.0.md](https://github.com/adaptyteam/AdaptySDK-Unity/blob/4.1.0/MIGRATION-v3.17-to-v4.0.md).
If you install from a `.unitypackage`, delete `Assets/AdaptySDK` and add
`com.unity.nuget.newtonsoft-json` **before** importing. A `.unitypackage` never removes files, so the
62 sources this release drops would otherwise stay behind and compile alongside the new ones, which
they collide with. Until Newtonsoft is there the SDK assembly is skipped, so code that calls Adapty
will not compile — and once it does not, Unity stops loading the Editor assembly the installer menu
lives in.

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
  already in the project are left as they are, apart from an External Dependency Manager older than
  the SDK needs, which is upgraded.
- `AdaptyErrorCode.NoPurchasesToRestore` (1004) — restored. The member was commented out in December
  2024 while the native Android SDK kept sending the code, so `RestorePurchases` on a profile with
  nothing to restore returned an error that could only be matched against a literal `1004`. Nothing
  about the error changes; it now has its name back. The code is Android-only — iOS does not define
  it.
- Seven more `AdaptyErrorCode` members the native SDKs declare but this enum never named:
  `UnidentifiedUserLogout` (3020, both platforms, from `Logout` on an unidentified profile),
  `PaymentPendingError` (1050, iOS), `BillingNetworkError` (112, Android), and `WrongAssetType`
  (4104), `JsException` (4105), `NavigatorNotFound` (4106), `InvalidActionUrl` (4107) — the four
  the Android flow renderer reports through `FlowViewDidReceiveError`. `AdaptyErrorCode` carries
  the native number, so these codes already arrived; they simply had no constant to match against.
  Each one was traced in the iOS 4.0.2 and Android 4.0.1 sources to the place the native SDK
  produces it — a throw site for all but 112, which comes out of the `fromBilling` mapping.

  `PaymentPendingError` is the one exception to "already arrived", and is named for completeness
  rather than to be handled: its only throw site is an iOS overload taking StoreKit's own purchase
  result, which the Unity bridge never calls — `ReportTransaction` goes to the overload taking a
  transaction id, which has no pending branch. A pending purchase made through the SDK arrives as
  `AdaptyPurchaseResultType.Pending`, on the result rather than the error.

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
- **Breaking:** `Adapty.GetLoglevel` is spelled `Adapty.GetLogLevel`. The typo was in the v3 surface
  too, out of step with its own `SetLogLevel` and with `get_log_level`, the operation the
  cross-platform contract names. Nothing else changes — same signature, same wire method.
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

- The `respond` delegate of `IAdaptyUISystemRequestsHandler.FlowViewDidAskPermission` and the report
  callbacks of `IAdaptyUIObserverModeResolver` are now safe to invoke from any thread, which is where
  they are invoked from in practice — an OS permission callback, a billing implementation's own
  thread. Each of them sends a request when invoked, and the SDK now sends it from the Unity main
  thread. On Android the bridge is JNI, which a thread must be attached to the JVM to enter, and a
  C# worker thread is not: on Unity 2022.3 — the declared floor, measured on 2022.3.62f3 — the call
  throws into the app's thread and the flow stays blocked waiting for an answer that never went
  out. Unity 6 attaches the thread on demand, so it happened to work there; the hop makes it work
  everywhere, and ordered with the SDK's other callbacks. iOS was unaffected.
- Requests call back on a device even when the app never sets an event listener. The platform
  callback bridge was registered by the four listener setters and by nothing else, so an app that
  subscribes to no events got no completion handler called at all — `Activate` included — on either
  platform, and every iOS request additionally leaked the handle meant to carry its reply. The
  bridge is now registered at player startup, before the first scene loads — so every call made from
  the MonoBehaviour lifecycle onwards is covered. Present since 3.x: the setters are documented as
  subscriptions, nothing ever said they were a precondition, and the demo app in this repository
  calls all four before activating, which is why it was never hit there.
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
- **The server cluster selected through the configuration builder now reaches the native SDK.**
  `ServerCluster` was the one builder field `AdaptyConfiguration`'s constructor did not copy, so
  `server_cluster` was never sent and every app ran against the default cluster whatever it chose.
  Selecting EU or CN did nothing in v3 and takes effect now, so an app that selected one starts
  talking to that region on upgrade. The builder field is `AdaptyServerCluster?` rather than
  `AdaptyServerCluster`, which is what keeps an unset cluster out of the request.
- `AdaptyPlacementFetchPolicy.Default` is no longer null. It aliases `ReloadRevalidatingCacheData`
  but was declared above it, and static field initializers run in declaration order, so passing
  `Default` explicitly raised a `NullReferenceException` when the request was built.
- `UpdateAttribution` sends `bool` and `DateTime` values instead of dropping them. The dictionary
  serializer had branches for strings, numbers, nested dictionaries, lists and null, and none for
  those two, so `UpdateAttribution(new Dictionary<string, object> { { "flag", true } }, ...)` went
  out as an empty object.
- `AdaptyCustomerIdentity.IsEmpty` reports an empty identity. `IosAppAccountToken` is a
  non-nullable `Guid`, so comparing it to null was always false and the property never returned
  true, which left the guard that keeps an empty identity out of the configuration dead.
- A partially filled date in an onboarding `date_picker` event no longer throws. The helpers reading
  the optional `day`, `month` and `year` cast the nullable they received straight to `int`, which
  raises `InvalidOperationException` when the key is absent — and the contract marks all three
  optional.
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
- `UpdateAttribution` reports an attribution graph it cannot encode through the completion handler,
  as `EncodingFailed`, instead of throwing at the call site. The overload taking a dictionary is the
  only public method that has to encode an argument before it can build a request, so it was the
  only one whose failure escaped the transport's guard — a reference loop or a throwing getter in
  the provider's data reached the caller as an exception while every other method reported an error.
- **Adapty SDK > Install Dependencies** stops on two loaded copies of Newtonsoft.Json, which is the
  state the SDK's own validator already reports as an error. It examined the first copy only, and
  the order loaded assemblies come back in is not specified, so the same project could be told its
  dependencies were complete on one run and be sent to fix them on the next.
- **Adapty SDK > Install Dependencies** upgrades an External Dependency Manager older than the SDK
  needs, instead of reporting the project complete. It checked only that a copy was loaded, so a
  project coming from v3 — which declared 1.2.187 — kept it, and the iOS build resolved through a
  version that gets the Xcode project path wrong for the Swift project type. Package Manager is the
  only thing that can tell those versions apart: every 1.2.x build of `Google.VersionHandler`
  reports the same assembly version, so a copy installed from Google's own `.unitypackage` under
  `Assets/` is now reported rather than replaced — adding the package over it would leave two.
- `AdaptyConfiguration.Builder.ToString()` includes `GoogleEnablePendingPrepaidPlans`. It was the
  one member missing from the description, so two builders differing only in whether Android
  reports pending prepaid transactions printed identically in logs.

### Known issues

- **Custom color and linear gradient assets are not rendered on iOS.** The pinned AdaptySDK-iOS
  4.0.2 discards the values it receives and substitutes a transparent color and an empty gradient,
  so a flow view shows neither. Nothing on the Unity side is involved: `AdaptyCustomAsset.Color` and
  `AdaptyCustomAsset.LinearGradient` serialize the actual RGBA and the actual stops, and the same
  substitution is in the later 4.0.3 and 4.1.0 native releases, so there is no version to move the
  pin to. Custom image and video assets are unaffected. Whether Android is affected has not been
  established.

## [3.17.0] - 2026-07-02

### Added

- `AdaptyProfile.AppliedAttributionSources` — attribution sources applied to the profile (e.g. Apple Search Ads), available for segmentation.
- `AdaptyUIUserAction.OpenIn` — for `OpenUrl` actions, indicates whether the link should open in an in-app or external browser.

### Changed

- Migrated the SDK to a Unity Package Manager layout; install via Git URL with `?path=/Packages/com.adapty.unity-sdk`.
- Updated native SDK dependencies and the cross-platform contract to 3.17.2 (iOS and Android).
