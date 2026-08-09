# Migrate Adapty Unity SDK to v4.0

Adapty Unity SDK 4.0 introduces flows and renames the paywall APIs accordingly. The new APIs work
with both the new Flow Builder and the existing Paywall Builder, and nothing changes on the Adapty
Dashboard side.

This guide covers the move from v3.17 to v4.0. Work through it in order — the first step has to
happen before the others, or your project will not compile long enough for you to reach them.

1. [Install the SDK dependencies](#install-the-sdk-dependencies)
2. [Fetch flows instead of paywalls](#fetch-flows-instead-of-paywalls)
3. [Update the flow model](#update-the-flow-model)
4. [Rename view creation and presentation methods](#rename-view-creation-and-presentation-methods)
5. [Update the web paywall methods](#update-the-web-paywall-methods)
6. [Update event listeners](#update-event-listeners)
7. [Replace members removed in v4.0](#replace-members-removed-in-v40)
8. [Move off the legacy onboarding API](#move-off-the-legacy-onboarding-api)
9. [Check the behavior changes](#check-the-behavior-changes)
10. [Update the native dependencies](#update-the-native-dependencies)

## Install the SDK dependencies

v4.0 requires **Unity 2022.3 or later** and two packages:

| Package | Comes from | Installed for you |
|---|---|---|
| `com.unity.nuget.newtonsoft-json` 3.2.2 | Unity registry | Yes, with Package Manager |
| `com.google.external-dependency-manager` 1.2.188 | OpenUPM | No — it is a peer dependency, as in v3 |

Newtonsoft.Json replaces the JSON parser that used to ship inside the SDK, so this dependency is new
in v4.0. The SDK installs whichever package is missing, together with the OpenUPM scoped registry
that External Dependency Manager is published on:

> **Adapty SDK > Install Dependencies**

### Which Unity versions this was verified on

The steps above were run end to end on **2022.3**, the declared floor: a clean `.unitypackage` import
into a project with no Newtonsoft, then **Adapty SDK > Install Dependencies**, then a compile. No
errors at any stage.

Everything else behind v4.0 — player builds, device runs, the full acceptance matrix — was done on
**Unity 6**, which is what the SDK is developed against.

### If you install from a `.unitypackage`

**Install `com.unity.nuget.newtonsoft-json` before you import the package.**

A `.unitypackage` carries assets and nothing else, so it cannot add a package to your project
manifest. Without Newtonsoft.Json the SDK assembly is skipped rather than failing to compile, which
keeps a fresh import quiet — but in a project that already calls Adapty, your own code stops
compiling:

```
Assets/YourScript.cs(8,19): error CS0103: The name 'Adapty' does not exist in the current context
```

The **Adapty SDK > Install Dependencies** menu item is unavailable while that lasts, because Unity
does not load the Editor assembly it lives in while your scripts fail to compile. Recover by hand:
open **Window > Package Manager**, choose **Add package by name** from the **+** menu (newer Editors
label it *Install package by name*), and enter `com.unity.nuget.newtonsoft-json`. Compilation
recovers, and from then on the menu item is available for External Dependency Manager.

Newtonsoft.Json has to arrive as that package. A `Newtonsoft.Json.dll` dropped into `Assets/`, which
some other SDKs ship, does not set the version define the SDK assembly is gated on, so the SDK stays
uncompiled. Delete that DLL first, then add the package — installing the package on top would leave
two copies of Newtonsoft in the project, which breaks compilation a different way.

The SDK reports both states in the Editor console, but only once its Editor assembly loads. While
your own scripts fail to compile, nothing of ours runs and you are on the manual path above.

## Fetch flows instead of paywalls

`GetPaywall` becomes `GetFlow` and no longer takes a locale. A flow is localized when its view is
built, so the locale moved to `AdaptyUICreateFlowViewParameters.Locale`.

```csharp
- Adapty.GetPaywall("YOUR_PLACEMENT_ID", "en", (paywall, error) => { });
+ Adapty.GetFlow("YOUR_PLACEMENT_ID", (flow, error) => { });
```

The overloads that take an `AdaptyPlacementFetchPolicy` and a load timeout keep their shape, minus
the locale:

```csharp
- Adapty.GetPaywall("YOUR_PLACEMENT_ID", null, fetchPolicy, timeout, (paywall, error) => { });
+ Adapty.GetFlow("YOUR_PLACEMENT_ID", fetchPolicy, timeout, (flow, error) => { });
```

`GetPaywallForDefaultAudience` becomes `GetFlowForDefaultAudience`. `GetPaywallProducts` keeps its
name and now takes an `AdaptyFlow`.

## Update the flow model

`AdaptyPaywall` becomes `AdaptyFlow`. A flow is not a paywall — it holds the paywall variations:

```csharp
- IList<AdaptyPaywall.ProductReference> products = paywall.Products;
+ IList<AdaptyFlowPaywall> paywalls = flow.Paywalls;
```

| v3 on `AdaptyPaywall` | v4 on `AdaptyFlow` |
|---|---|
| `RemoteConfig` | `RemoteConfigs`, one per configured language. `RemoteConfig` still exists and returns the first |
| `Products` | `ProductIdentifiers`, or `GetPaywallProducts(flow)` |
| `HasViewConfiguration` | removed — `CreateFlowView` returns an error instead |
| — | `Paywalls`, the paywall variations of the flow |
| — | `FlowVersionId`, nullable |

`Placement`, `InstanceIdentity`, `Name`, `VariationId`, `ProductIdentifiers` and `VendorProductIds`
keep their names.

`AdaptyUIPaywallView` becomes `AdaptyUIFlowView`, and `AdaptyPaywallProduct` keeps its name and
gains `FlowProductId`, nullable.

### Product references are gone, not renamed

Both ways of reading product references off a paywall are removed, and there is no drop-in
replacement:

- `paywall.Products`, the list of `AdaptyPaywall.ProductReference`;
- `AdaptyProductReference`, which in v3 was a public type you could construct yourself, with
  `VendorProductId`, `PromotionalOfferId`, `WinBackOfferId`, `AndroidBasePlanId` and
  `AndroidOfferId` on it.

`AdaptyFlowPaywall.ProductReference` exists in v4, but it is not that type under a new name: its
constructor is private, its fields are internal, and no public member of `AdaptyFlowPaywall` returns
one. Do not migrate to it — you cannot write that code through the public API.

Migrate by what you were reading the reference for:

| You needed | In v4 |
|---|---|
| The product ids of a flow | `flow.ProductIdentifiers` or `flow.VendorProductIds` |
| The products themselves | `Adapty.GetPaywallProducts(flow, ...)`, giving `AdaptyPaywallProduct` |
| Access level, product type | `product.AccessLevelId`, `product.ProductType` |
| Offer id and kind | `product.Subscription.Offer.Identifier` and `.Type` |
| Android base plan | `product.Subscription.BasePlanId`, or `identifier.BasePlanId` |

`AdaptyProductIdentifier` carries `VendorProductId` and `BasePlanId` only, so anything else has to
come from the fetched product.

## Rename view creation and presentation methods

The view methods stay on `AdaptyUI` and take a flow:

```csharp
- AdaptyUI.CreatePaywallView(paywall, parameters, (view, error) => { });
+ AdaptyUI.CreateFlowView(flow, parameters, (view, error) => { });

- AdaptyUI.PresentPaywallView(view, (error) => { });
+ AdaptyUI.PresentFlowView(view, (error) => { });

- AdaptyUI.DismissPaywallView(view, (error) => { });
+ AdaptyUI.DismissFlowView(view, (error) => { });
```

`AdaptyUICreatePaywallViewParameters` becomes `AdaptyUICreateFlowViewParameters`. It keeps its
fields and adds `Locale`, which selects the localization to render, and `EnableSafeAreaPaddings`,
which is Android only and defaults to `true`.

`LogShowPaywall` becomes `LogShowFlow` and takes an `AdaptyFlow`. It logs against the same
variation, so existing funnels and A/B tests carry over.

Two methods are new on `AdaptyUI`: `OpenUrl` and `RequestAppReview`.

## Update the web paywall methods

`CreateWebPaywallUrl` and `OpenWebPaywall` took an `AdaptyPaywall` in v3 and take an
`AdaptyFlowPaywall` in v4 — one variation out of the flow, not the flow itself:

```csharp
- AdaptyPaywall paywall = ...;
+ AdaptyFlowPaywall paywall = flow.Paywalls[index];

  Adapty.CreateWebPaywallUrl(paywall, (url, error) => { });
  Adapty.OpenWebPaywall(paywall, openIn, (error) => { });
```

`flow.Paywalls` holds every variation of the flow, so pick the one you mean to open rather than
assuming there is exactly one.

The overloads that take an `AdaptyPaywallProduct` are unchanged, so code that opened a web paywall
from a product needs no edit.

## Update event listeners

The listener interfaces now carry the C# `I` prefix, and the paywall one is about flows:

```csharp
- public class MyListener : AdaptyEventListener, AdaptyPaywallsEventsListener
+ public class MyListener : IAdaptyEventListener, IAdaptyFlowsEventsListener
```

| v3 | v4 |
|---|---|
| `AdaptyEventListener` | `IAdaptyEventListener` |
| `AdaptyPaywallsEventsListener` | `IAdaptyFlowsEventsListener` |
| `AdaptyOnboardingsEventsListener` | `IAdaptyOnboardingsEventsListener` |
| `Adapty.SetPaywallsEventsListener` | `Adapty.SetFlowsEventsListener` |

Every callback changes its `PaywallView` prefix to `FlowView`, and one changes name outright:

```csharp
- void PaywallViewDidFailRendering(AdaptyUIPaywallView view, AdaptyError error);
+ void FlowViewDidReceiveError(AdaptyUIFlowView view, AdaptyError error);
```

`FlowViewDidReceiveError` fires for rendering errors and for other runtime errors.

Two handler interfaces are new, both registered on `Adapty`, and each brings two callbacks you have
to implement:

| Interface | Registered with | Callbacks |
|---|---|---|
| `IAdaptyUISystemRequestsHandler` | `Adapty.SetSystemRequestsHandler` | `FlowViewDidAskPermission`, `FlowViewDidRequestAppReview` |
| `IAdaptyUIObserverModeResolver` | `Adapty.SetObserverModeResolver` | `FlowViewDidInitiatePurchase`, `FlowViewDidInitiateRestore` |

Implement `IAdaptyUIObserverModeResolver` only if you run in Observer mode. A permission request
must be answered exactly once.

`IAdaptyFlowsEventsListener` gains one callback of its own, `FlowViewDidReceiveAnalyticEvent` — five
new callbacks in total.

`FlowViewDidAskPermission`, `FlowViewDidRequestAppReview` and `FlowViewDidReceiveAnalyticEvent` are
reserved: the interfaces require them, but flows do not emit these events yet. Implement them as
no-ops unless you have a reason not to.

## Replace members removed in v4.0

v4.0 drops what v3 had already deprecated. If you build with warnings visible, you have seen these
already.

```csharp
- Adapty.SetFallbackPaywalls("fallback.json", (error) => { });
+ Adapty.SetFallback("fallback.json", (error) => { });

- builder.SetIDFACollectionDisabled(true);
+ builder.SetAppleIDFACollectionDisabled(true);
```

The `AdaptyPaywall` members deprecated in v3.14 are gone with the type itself. `AdaptyFlow` has no
replacements for them — use what the v3 warnings pointed at:

| Removed | Use instead |
|---|---|
| `PlacementId` | `flow.Placement.Id` |
| `AudienceName` | `flow.Placement.AudienceName` |
| `ABTestName` | `flow.Placement.ABTestName` |
| `Revision` | `flow.Placement.Revision` |
| `RemoteConfigString` | `flow.RemoteConfig.Data` |
| `Locale` | `flow.RemoteConfig.Locale` |

The `AdaptySDK.SimpleJSON` namespace went with the parser it belonged to, together with its public
types — `JSON`, `JSONNode`, `JSONObject`, `JSONArray` and the rest — and with the `ToJSONNode`
extension classes that came with them, `AdaptyUIIOSPresentationStyleExtensions`,
`AdaptyUIOnboardingMetaExtensions`, `AdaptyWebPresentationExtensions` and
`AdaptyRefundPreferenceExtensions`. Code that only calls the
SDK is unaffected; code that used those types directly can move to Newtonsoft.Json, which is now a
dependency of the package and available to your assemblies too.

`AdaptyInstallationStatus` is one sealed type instead of a base class and three subclasses.
`GetCurrentInstallationStatus` still hands back an `AdaptyInstallationStatus`; what you switch on is
now its `Status`, and `Details` is on the same object:

```csharp
- if (status is AdaptyInstallationStatusDetermined determined)
- {
-     Debug.Log(determined.Details.InstallId);
- }
+ if (status.Status == AdaptyInstallationStatusType.Determined)
+ {
+     Debug.Log(status.Details.InstallId);
+ }
```

`AdaptyInstallationStatusNotAvailable`, `AdaptyInstallationStatusNotDetermined` and
`AdaptyInstallationStatusDetermined` are gone, public constructors included — the type is a
response, and only the SDK builds one. `Details` is non-null exactly when `Status` is `Determined`.

Two members that only forwarded to another one are gone:

| Removed | Use instead |
|---|---|
| `AdaptyProfile.NonSubscription.IsOneTime` | `IsConsumable`, which it returned unchanged |
| `AdaptyPlacement.GetIsTrackingPurchases` | `IsTrackingPurchases`, the field it wrapped. It is a `bool?`, but never arrives null: a missing key leaves the declared `false` |

## Move off the legacy onboarding API

`GetOnboarding`, `GetOnboardingForDefaultAudience`, `AdaptyUI.CreateOnboardingView`,
`AdaptyUI.PresentOnboardingView`, `AdaptyUI.DismissOnboardingView` and
`Adapty.SetOnboardingsEventsListener` still work and now warn at compile time. Build onboardings as
flows instead.

The warning now covers the whole API, not only its entry points: `IAdaptyOnboardingsEventsListener`,
`AdaptyOnboarding`, `AdaptyUIOnboardingView`, `AdaptyUIOnboardingMeta`, the
`AdaptyOnboardingsAnalyticsEvent` hierarchy, the `AdaptyOnboardingsStateUpdatedParams` and
`AdaptyOnboardingsInput` hierarchies, and the `AdaptyUI.ShowDialog` overload that takes an
`AdaptyUIOnboardingView`. Naming any of them in your own code — a listener implementation, a field,
a method signature — now warns where before only the call did.

## Check the behavior changes

These compile as they are and behave differently at runtime, so the compiler will not point them
out:

- **A flow view stays open after a purchase.** Dismiss it yourself from
  `FlowViewDidFinishPurchase` when that is what you want.
- **A view is single use.** After `DismissFlowView` it is destroyed; call `CreateFlowView` again to
  show the flow again.
- **The Android system back button no longer closes the view on its own.** It arrives in
  `FlowViewDidPerformAction` as a `SystemBack` action, which is what iOS already did.
- **`ReportTransaction` no longer reports a decoding error on success.** In v3 it decoded the
  response as a profile while the native side returned `{"success": true}`, so the completion
  handler received an error even though the transaction had been reported.
- **`AdaptyProductIdentifier` now compares by value.** Identifiers built from a flow work as
  dictionary keys, which is what `AdaptyUICreateFlowViewParameters.SetProductPurchaseParameters`
  expects; in v3 they were compared by reference and the parameters silently applied to nothing.
- **`AdaptySubscriptionOfferType` gained `Code`** (iOS only). The existing members keep their
  values, but a `switch` that was exhaustive in v3 is not exhaustive now — give it a default branch.
  A string the contract does not list still fails the read, exactly as in v3.

## Update the native dependencies

v4.0 pins iOS 4.0.2 and Android 4.0.1, against cross-platform contract 4.0.2.

**iOS moves from CocoaPods to Swift Package Manager.** In v3 the SDK declared `iosPods`; in v4 it
declares a remote Swift package, which External Dependency Manager adds to the generated Xcode
project as a package reference. You no longer run
**Assets > External Dependency Manager > iOS Resolver > Install Cocoapods** for Adapty, and no pod
of ours appears in the Podfile.

**Keep building the workspace.** External Dependency Manager still generates `Podfile` and
`Unity-iPhone.xcworkspace` — with no pods in them — and still wires `Pods_UnityFramework` into the
Unity target. Building `Unity-iPhone.xcodeproj` directly fails at link time with
`ld: framework 'Pods_UnityFramework' not found`; the same tree builds from
`Unity-iPhone.xcworkspace`. Open and build the workspace exactly as you did in v3.

External Dependency Manager has to be 1.2.188 or later, since earlier versions have no Swift Package
Manager support, and the iOS deployment target moves from 13.0 to **15.0 or later**, which a build
validator enforces in the Editor.

The Android dependencies are declared in an `.androidlib` module that Unity includes in the Gradle
build on its own.

If your app ships in the App Store Kids Category, v4.0 adds the `ADAPTY_KIDS_MODE` scripting define,
which compiles IDFA, AdSupport and AppTrackingTransparency out of the iOS binary. See the
[README](README.md#kids-mode-on-ios) for what it requires.
