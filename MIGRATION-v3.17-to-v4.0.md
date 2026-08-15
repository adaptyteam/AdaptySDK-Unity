# Migrate Adapty Unity SDK to v4.0

v4.0 introduces flows and renames the paywall APIs accordingly. The new APIs work with both the new
Flow Builder and the existing Paywall Builder, and nothing changes on the Adapty Dashboard side.

This guide is the move from v3.17 to v4.0. Read **Before you upgrade** first and sort its
prerequisites by when they bite: Unity and Newtonsoft.Json have to be in place before your C#
compiles at all, while External Dependency Manager, Xcode and the iOS deployment target are only
needed by the time you build for iOS. The other sections are independent of each other; take them
in whatever order suits your project. Everything
this guide does not cover — why each change was made, and what was fixed along the way — is in
[CHANGELOG.md](Packages/com.adapty.unity-sdk/CHANGELOG.md).

1. [Before you upgrade](#before-you-upgrade)
2. [Rename the paywall APIs to flows](#rename-the-paywall-apis-to-flows)
3. [Update listeners and handlers](#update-listeners-and-handlers)
4. [Fix the compile errors](#fix-the-compile-errors)
5. [Review the runtime behavior changes](#review-the-runtime-behavior-changes)
6. [Optional](#optional)

## Before you upgrade

**Unity 2022.3 or later**, and two packages:

| Package | Comes from | Installed for you | Needed by |
|---|---|---|---|
| `com.unity.nuget.newtonsoft-json` 3.2.2 | Unity registry | Yes, with Package Manager | compile time — the SDK assembly is gated on it |
| `com.google.external-dependency-manager` 1.2.188 | OpenUPM | No — a peer dependency, as in v3 | iOS build — it resolves the Swift package. Android does not go through it |

Newtonsoft.Json replaces the JSON parser that used to ship inside the SDK, so it is new in v4.0. One
menu item installs whichever is missing, adds the OpenUPM registry, and upgrades an External
Dependency Manager below 1.2.188 — v3 declared 1.2.187, so a project coming from it has one:

> **Adapty SDK > Install Dependencies**

A copy of External Dependency Manager installed from Google's own `.unitypackage` under `Assets/`
has no version Package Manager can read. It is left alone with a warning, and you update it
yourself.

**Installing from a `.unitypackage`: add `com.unity.nuget.newtonsoft-json` before you import.** A
`.unitypackage` carries assets only and cannot touch your project manifest. Without Newtonsoft the
SDK assembly is skipped, your calls into Adapty stop compiling (`error CS0103: The name 'Adapty'
does not exist in the current context`), and the menu item above is unavailable — Unity does not
load the Editor assembly it lives in while your scripts fail to compile. Recover through **Window >
Package Manager > + > Add package by name**. It has to be that package: a `Newtonsoft.Json.dll`
dropped into `Assets/` does not satisfy the SDK, and installing the package on top of one leaves two
copies. Delete the DLL first.

**Installing from a `.unitypackage`: delete `Assets/AdaptySDK` before you import.** A
`.unitypackage` never removes files, and 4.0 drops 62 sources that 3.17 shipped — the whole
`Assets/AdaptySDK/JSON/` folder, plus `AdaptyPaywall.cs` and its neighbours. Importing over them
keeps the folder and the assembly definition GUIDs, so the leftovers compile into the same assembly
as the new sources: 35 of them declare a `partial` half of a type the new sources also declare —
`AdaptyPlacement`, `AdaptyProfile`, `AdaptyPaywallProduct` among them, and 4.0 declares most of
those `sealed` — while the rest call constructors and a `SimpleJSON` namespace that are gone. The
errors do not appear at import — the assembly is gated on Newtonsoft — but the
moment Newtonsoft is in place the project stops compiling, and from there the menu item above is out
of reach too. Deleting the folder first costs nothing: everything in it is replaced.

**iOS requirements changed — needed to build for iOS, not to compile.** Nothing below blocks the
rest of this guide; the deployment target is checked by a build validator when an iOS build starts,
and Xcode only comes in after the export, when it resolves the Swift package.

| | v3 | v4 |
|---|---|---|
| Xcode | any recent | **26 or later** |
| Deployment target | 13.0 | **15.0 or later**, enforced by a build validator |
| Native dependency | CocoaPods (`iosPods`) | Swift Package Manager |

Stop running **Assets > External Dependency Manager > iOS Resolver > Install Cocoapods** for Adapty;
no pod of ours appears in the Podfile any more. **Keep building
`Unity-iPhone.xcworkspace`,** exactly as in v3 — External Dependency Manager still generates it and
still wires `Pods_UnityFramework` into the Unity target, so building `Unity-iPhone.xcodeproj`
directly fails with `ld: framework 'Pods_UnityFramework' not found`.

Android needs nothing from you: the dependencies are declared in an `.androidlib` module Unity
includes in the Gradle build on its own.

## Rename the paywall APIs to flows

`AdaptyPaywall` becomes `AdaptyFlow`, and a flow is not a paywall — it holds the paywall variations.

```csharp
- Adapty.GetPaywall("YOUR_PLACEMENT_ID", "en", (paywall, error) => { });
+ Adapty.GetFlow("YOUR_PLACEMENT_ID", (flow, error) => { });

- AdaptyUI.CreatePaywallView(paywall, parameters, (view, error) => { });
+ AdaptyUI.CreateFlowView(flow, parameters, (view, error) => { });

- AdaptyUI.PresentPaywallView(view, (error) => { });
+ AdaptyUI.PresentFlowView(view, (error) => { });

- AdaptyUI.DismissPaywallView(view, (error) => { });
+ AdaptyUI.DismissFlowView(view, (error) => { });
```

| v3 | v4 |
|---|---|
| `Adapty.GetPaywall` | `Adapty.GetFlow` — **no locale argument**; it moved to `AdaptyUICreateFlowViewParameters.Locale`, since a flow is localized when its view is built |
| `Adapty.GetPaywallForDefaultAudience` | `Adapty.GetFlowForDefaultAudience` |
| `Adapty.GetPaywallProducts(paywall, ...)` | same name, takes an `AdaptyFlow` |
| `Adapty.LogShowPaywall` | `Adapty.LogShowFlow`, takes an `AdaptyFlow`. Same variation, so funnels and A/B tests carry over |
| `AdaptyUIPaywallView` | `AdaptyUIFlowView` |
| `view.PaywallVariationId` | `view.VariationId` — same value, shorter name now that the view is a flow's. The view also gains `view.Locale`, the localization it was built with |
| `AdaptyUICreatePaywallViewParameters` | `AdaptyUICreateFlowViewParameters` — same fields, plus `Locale` and `EnableSafeAreaPaddings` (Android only, defaults to `true`) |
| `paywall.RemoteConfig` | `flow.RemoteConfigs`, one per configured language. `RemoteConfig` still exists and returns the first |
| `paywall.Products` | `flow.ProductIdentifiers`, or `GetPaywallProducts(flow, ...)` |
| `paywall.HasViewConfiguration` | removed — `CreateFlowView` returns an error instead |
| — | `flow.Paywalls`, the paywall variations; `flow.FlowVersionId`, nullable |
| — | `AdaptyUI.OpenUrl` and `AdaptyUI.RequestAppReview` are new |

`Placement`, `InstanceIdentity`, `Name`, `VariationId`, `ProductIdentifiers` and `VendorProductIds`
keep their names on `AdaptyFlow`. `AdaptyPaywallProduct` keeps its name and gains `FlowProductId`,
nullable.

The members deprecated in v3.14 are gone with the type:

| Removed from `AdaptyPaywall` | Use instead |
|---|---|
| `PlacementId`, `AudienceName`, `ABTestName`, `Revision` | `flow.Placement.Id`, `.AudienceName`, `.ABTestName`, `.Revision` |
| `RemoteConfigString`, `Locale` | `flow.RemoteConfig.Data`, `flow.RemoteConfig.Locale` |

### Product references have no replacement

`paywall.Products` and the public `AdaptyProductReference` are both gone.
`AdaptyFlowPaywall.ProductReference` is not that type renamed — it is internal, and no public member
returns one, so you cannot write that code. Migrate by what you read the reference for:

| You needed | In v4 |
|---|---|
| The product ids of a flow | `flow.ProductIdentifiers` or `flow.VendorProductIds` |
| The products themselves | `Adapty.GetPaywallProducts(flow, ...)`, giving `AdaptyPaywallProduct` |
| Access level, product type | `product.AccessLevelId`, `product.ProductType` |
| Offer id and kind | `product.Subscription.Offer.Identifier` and `.Type` |
| Android base plan | `product.Subscription.BasePlanId`, or `identifier.BasePlanId` |

`AdaptyProductIdentifier` carries `VendorProductId` and `BasePlanId` only; anything else comes from
the fetched product.

### Web paywalls take one variation

`CreateWebPaywallUrl` and `OpenWebPaywall` took an `AdaptyPaywall` and now take an
`AdaptyFlowPaywall` — one variation out of the flow, not the flow itself. Pick the one you mean:

```csharp
- AdaptyPaywall paywall = ...;
+ AdaptyFlowPaywall paywall = flow.Paywalls[index];

  Adapty.CreateWebPaywallUrl(paywall, (url, error) => { });
```

The overloads taking an `AdaptyPaywallProduct` are unchanged.

## Update listeners and handlers

The interfaces carry the C# `I` prefix, and the paywall one is about flows:

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
| `void PaywallViewDid…(AdaptyUIPaywallView view, …)` | `void FlowViewDid…(AdaptyUIFlowView view, …)` — every callback |
| `PaywallViewDidFailRendering` | `FlowViewDidReceiveError`, which also fires for other runtime errors |

Two handler interfaces are new, each with two callbacks you implement, plus one new callback on the
flows listener:

| Interface | Registered with | Callbacks |
|---|---|---|
| `IAdaptyUISystemRequestsHandler` | `Adapty.SetSystemRequestsHandler` | `FlowViewDidAskPermission`, `FlowViewDidRequestAppReview` |
| `IAdaptyUIObserverModeResolver` | `Adapty.SetObserverModeResolver` | `FlowViewDidInitiatePurchase`, `FlowViewDidInitiateRestore` |
| `IAdaptyFlowsEventsListener` | `Adapty.SetFlowsEventsListener` | `FlowViewDidReceiveAnalyticEvent` |

Three rules about the new callbacks:

- **Answer a permission request exactly once.** Until `respond` runs the flow stays pending;
  dismissing the view resolves it as denied.
- **`FlowViewDidRequestAppReview` must call `AdaptyUI.RequestAppReview`** to keep the default
  behavior. An empty body is worse than registering no handler, because with no handler the SDK
  makes that call for you.
- **Implement `IAdaptyUIObserverModeResolver` only if you run in Observer mode.**

`FlowViewDidReceiveAnalyticEvent` is the one you may leave empty — it is a live event you are
dropping, not a placeholder.

## Fix the compile errors

Renames, first:

| v3 | v4 |
|---|---|
| `Adapty.SetFallbackPaywalls` | `Adapty.SetFallback` |
| `builder.SetIDFACollectionDisabled` | `builder.SetAppleIDFACollectionDisabled` |
| `Adapty.GetLoglevel` | `Adapty.GetLogLevel` — a typo fixed, so there was no v3 warning for this one |
| `builder.IdfaCollectionDisabled` | `builder.AppleIdfaCollectionDisabled` — the property matching the method above. It was already `[Obsolete]` in v3, naming this same replacement |

Removed with a replacement:

| Removed | Use instead |
|---|---|
| `AdaptyProfile.NonSubscription.IsOneTime` | `IsConsumable`, which it returned unchanged |
| `AdaptyPlacement.GetIsTrackingPurchases` | `IsTrackingPurchases`, the field it wrapped — but that field is `bool?`, where the removed member returned `bool`. Write `IsTrackingPurchases ?? false` to keep the old expression's type |
| `AdaptyInstallationStatusNotAvailable`, `AdaptyInstallationStatusNotDetermined`, `AdaptyInstallationStatusDetermined` | `AdaptyInstallationStatus.Status` and `.Details`, see below |
| `AdaptyErrorCode.PendingPurchase` (25) | `AdaptyPurchaseResultType.Pending`, see below |
| `AdaptyErrorCode.InvalidJson` (23) | nothing — no native SDK can raise it |
| The `AdaptySDK.SimpleJSON` namespace, and the `ToJSONNode` extension classes `AdaptyRefundPreferenceExtensions`, `AdaptyUIIOSPresentationStyleExtensions`, `AdaptyUIOnboardingMetaExtensions`, `AdaptyWebPresentationExtensions` | Newtonsoft.Json, now a dependency of the package and available to your assemblies |

Changed types and shapes:

| Member | Change |
|---|---|
| Collections on `AdaptyProfile`, `AdaptyFlow`, `AdaptyFlowPaywall`, `AdaptySubscriptionOffer`, `AdaptyRemoteConfig`, and the `GetPaywallProducts` callback | `IList<T>` → `IReadOnlyList<T>`, `IDictionary<K, V>` → `IReadOnlyDictionary<K, V>`. `AdaptyProfile.NonSubscriptions` is read-only at both levels |
| The four `AdaptyUICreateFlowViewParameters` setters and `UpdateAttribution` | take an `IReadOnlyDictionary` and **copy** it, so filling your dictionary afterwards no longer changes the view. The four matching members are read-only properties — assign through the setters |
| `AdaptyProfileParameters.CustomAttributes` | was a `Dictionary` you could write into and is now a read-only view over the builder's own storage. There is no setter taking a dictionary: use `SetCustomStringAttribute`, `SetCustomDoubleAttribute` and `RemoveCustomAttribute`. Copying this one does not reach the request |
| `FlowViewDidReceiveAnalyticEvent`, `FlowViewDidAskPermission` | take `IReadOnlyDictionary` instead of `IDictionary`. The analytics parameter is renamed `@params` → `parameters`, which matters only for a named argument |
| Every concrete public class | `sealed`. If you derived from one, hold it as a field instead of inheriting it |
| `AdaptyPlacementFetchPolicy.Default`, `.ReloadRevalidatingCacheData`, `.ReturnCacheDataElseLoad` | `readonly` — reading is unchanged, assigning no longer compiles |
| `AdaptyConfiguration.Builder.ServerCluster` | `AdaptyServerCluster?` where it was `AdaptyServerCluster`. `SetServerCluster` is unchanged |

Reading a read-only collection is unaffected — `foreach` and LINQ included. Writing needs a copy
first, which `ToDictionary` from `System.Linq` does in one line:

```csharp
- profile.CustomAttributes["seen_intro"] = true;
+ var attributes = profile.CustomAttributes.ToDictionary(pair => pair.Key, pair => pair.Value);
+ attributes["seen_intro"] = true;
```

`AdaptyInstallationStatus` is one sealed type instead of a base class and three subclasses.
`GetCurrentInstallationStatus` still hands back an `AdaptyInstallationStatus`; you switch on its
`Status`, and `Details` is non-null exactly when that is `Determined`:

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

A pending purchase is a result rather than an error, so the check moves to the other branch of the
callback — on the error path the result is null:

```csharp
Adapty.MakePurchase(product, (result, error) =>
{
    if (error != null)
    {
        // AdaptyErrorCode.PendingPurchase used to be checked here.
        return;
    }

    if (result.Type == AdaptyPurchaseResultType.Pending) { /* ... */ }
});
```

## Review the runtime behavior changes

These compile as they are and behave differently at runtime, so the compiler will not point them
out:

- **A flow view stays open after a purchase.** Dismiss it yourself from `FlowViewDidFinishPurchase`
  when that is what you want.
- **A view is single use.** After `DismissFlowView` it is destroyed; call `CreateFlowView` again to
  show the flow again.
- **The Android system back button no longer closes the view on its own.** It arrives in
  `FlowViewDidPerformAction` as a `SystemBack` action, which is what iOS already did.
- **`AdaptySubscriptionOfferType` gained `Code`** (iOS only). Existing members keep their values, but
  a `switch` that was exhaustive in v3 is not exhaustive now — give it a default branch.

## Optional

**Remove workarounds you no longer need.** Three v3 defects are fixed, so code written around them
can go: `ReportTransaction` no longer reports a decoding error on success, `AdaptyProductIdentifier`
compares by value so identifiers from a flow work as dictionary keys, and a call in the Editor
returns a readable "not supported on this platform" error instead of a null one that looked like
success. The full list of fixes is in the changelog.

**Move off the legacy onboarding API.** `GetOnboarding`, `AdaptyUI.CreateOnboardingView` and the
rest still work and now warn at compile time. Build onboardings as flows instead.

**Kids Mode.** If your app ships in the App Store Kids Category, v4.0 adds the `ADAPTY_KIDS_MODE`
scripting define, which compiles IDFA, AdSupport and AppTrackingTransparency out of the iOS binary.
See the [README](README.md#kids-mode-on-ios) for how to set it.
