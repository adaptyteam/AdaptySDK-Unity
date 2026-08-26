# Migrate Adapty Unity SDK to v4.1

v4 introduces flows and renames the paywall APIs accordingly; 4.1 renames the attribution API after
the natives, adds App Store promoted purchases, and makes Adapty Attribution opt-in. No stable 4.0
was ever published, so this is the one guide: it takes a v3.17 project straight to v4.1. The new
APIs work with both the new Flow Builder and the existing Paywall Builder, and nothing changes on
the Adapty Dashboard side.

Read **Before you upgrade** first and sort its prerequisites by when they bite: Unity and
Newtonsoft.Json have to be in place before your C# compiles at all, while External Dependency
Manager, Xcode and the iOS deployment target are only needed by the time you build for iOS. The
sections after it are independent of each other; take them in whatever order suits your project,
but do not stop at the last compile error:
[Re-download your fallback files](#re-download-your-fallback-files) and
[Turn Adapty Attribution on if you read installation details](#turn-adapty-attribution-on-if-you-read-installation-details)
are the two steps the compiler cannot know about, and the ones you can genuinely forget. Everything
this guide does not cover — why each change was made, and what was fixed along the way — is in
[CHANGELOG.md](Packages/com.adapty.unity-sdk/CHANGELOG.md).

1. [Before you upgrade](#before-you-upgrade)
2. [Rename the paywall APIs to flows](#rename-the-paywall-apis-to-flows)
3. [Rename the attribution API](#rename-the-attribution-api)
4. [Update listeners and handlers](#update-listeners-and-handlers)
5. [Fix the compile errors](#fix-the-compile-errors)
6. [Review the runtime behavior changes](#review-the-runtime-behavior-changes)
7. [Re-download your fallback files](#re-download-your-fallback-files)
8. [Turn Adapty Attribution on if you read installation details](#turn-adapty-attribution-on-if-you-read-installation-details)
9. [Optional](#optional)

## Before you upgrade

**Unity 2022.3 or later**, and two packages:

| Package | Comes from | Installed for you | Needed by |
|---|---|---|---|
| `com.unity.nuget.newtonsoft-json` 3.2.2 | Unity registry | Yes, with Package Manager | compile time — the SDK assembly is gated on it |
| `com.google.external-dependency-manager` 1.2.188 | OpenUPM | No — a peer dependency, as in v3 | iOS build — it resolves the Swift package. Android does not go through it |

Newtonsoft.Json replaces the JSON parser that used to ship inside the SDK, so it is new in v4. One
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
`.unitypackage` never removes files, and v4 drops 62 sources that 3.17 shipped — the whole
`Assets/AdaptySDK/JSON/` folder, plus `AdaptyPaywall.cs` and its neighbours. Importing over them
keeps the folder and the assembly definition GUIDs, so the leftovers compile into the same assembly
as the new sources: 35 of them declare a `partial` half of a type the new sources also declare —
`AdaptyPlacement`, `AdaptyProfile`, `AdaptyPaywallProduct` among them, and v4 declares most of
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

Both native dependencies come with the package and move on their own: iOS is declared inside the
package as a Swift package — AdaptySDK-iOS 4.1.2 — and External Dependency Manager resolves it at
build time; Android ships its 4.1.0 dependencies in an `.androidlib` module that Unity includes in
the Gradle build on its own. There is nothing for you to update by hand on either platform.

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
| — | `flow.Paywalls`, the paywall variations |
| — | `AdaptyUI.OpenUrl` and `AdaptyUI.RequestAppReview` are new |

`Placement`, `InstanceIdentity`, `Name`, `VariationId`, `HasViewConfiguration`,
`ProductIdentifiers` and `VendorProductIds` keep their names on `AdaptyFlow`.
`AdaptyPaywallProduct` keeps its name and gains `FlowProductId`, nullable.

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

## Rename the attribution API

The native SDKs renamed their attribution APIs in 4.1, and the Unity SDK follows. There are no
deprecated aliases — deliberately: the old and new names would otherwise sit side by side in
autocomplete for a release cycle, and every existing call site is a small edit the compiler
finds for you anyway.

| v3 | v4.1 |
|---|---|
| `Adapty.UpdateAttribution(jsonString, source, handler)` | `Adapty.UpdateExternalAttribution(jsonString, provider, handler)` |
| `Adapty.UpdateAttribution(dictionary, source, handler)` | `Adapty.UpdateExternalAttribution(dictionary, provider, handler)` |
| `AdaptyProfile.AppliedAttributionSources` | `AdaptyProfile.AppliedExternalAttributionProviders` |

The names are not the only move: the provider is now a value of its own,
`AdaptyExternalAttributionProvider`, mirroring the native 4.1 API. Where you passed `"appsflyer"`,
pass `AdaptyExternalAttributionProvider.Appsflyer` — the providers the backend knew at release are
shared instances, and one it added later is a constructor call away:

```csharp
Adapty.UpdateExternalAttribution(conversionData, AdaptyExternalAttributionProvider.Appsflyer, handler);
// a provider the backend added after this release:
Adapty.UpdateExternalAttribution(conversionData, new AdaptyExternalAttributionProvider("singular"), handler);
```

`AppliedExternalAttributionProviders` on the profile is typed the same way, so compare its entries
against the shared instances rather than string literals. The provider identifiers are the strings
you were already using, and the data you were sending keeps working unchanged.

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

### Implement `OnReceivePromotedPurchase`

`IAdaptyEventListener` also has one member its v3 predecessor did not, so the listener you just
renamed stops compiling until you add:

```csharp
public void OnReceivePromotedPurchase(AdaptyPromotedProduct product)
{
    // The user tapped one of your in-app purchases on your App Store product page.
    // Hand it back to Adapty to complete the purchase:
    Adapty.MakePromotedPurchase(product, (result, error) => { /* ... */ });
}
```

The body above is the right default even if you have never set up a promoted purchase — they are
the ones you configure in App Store Connect to appear on your App Store product page. It does what
the native iOS SDK does when an app leaves the choice to it: completes the purchase the user
already started by tapping buy on that page. The event *is* the purchase, so an empty body silently
drops something the user explicitly asked for — leave it empty only as a deliberate decision to
ignore promoted purchases. If a purchase must wait for your own flow — a sign-in, a parental gate —
hold on to the product and call `MakePromotedPurchase` when the flow allows it. On Android the
method is never called.

The same applies one level up: an app that never calls `Adapty.SetEventListener` has no handler
for the event, and a promoted purchase is dropped silently there too.

One thing worth knowing: this works because the pin is AdaptySDK-iOS 4.1.2. Native 4.1.0 completed
promoted purchases by itself, without telling anyone, so on that version the handler was never
called; 4.1.1 changed it to hand the purchase to the wrapper and expect `MakePromotedPurchase` to
finish it, and 4.1.2 keeps that. The native dependency is pinned to exactly 4.1.2 — deliberately,
so native behaviour never changes underneath a wrapper that was not built for it.

### The new handler interfaces

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
| The four `AdaptyUICreateFlowViewParameters` setters and `UpdateExternalAttribution` | take an `IReadOnlyDictionary` and **copy** it, so filling your dictionary afterwards no longer changes the view. The four matching members are read-only properties — assign through the setters |
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
- **`Adapty.RestorePurchases` never reports `NoPurchasesToRestore` (1004) at the 4.1 pins.** The
  Android native completes with the current profile when there is nothing to restore, and iOS never
  produced the code, so a branch checking for it is dead. The `AdaptyErrorCode` member stays.

## Re-download your fallback files

This is a step the compiler cannot catch, and the symptom shows up at runtime looking like a
broken integration: right after upgrading, `Adapty.SetFallback` — `SetFallbackPaywalls`, renamed —
fails. On iOS it reports `DecodingFailed` (`adapty_code: 2006`, *"The fallback paywalls version is
not correct. Download a new one from the Adapty Dashboard."*); on Android, `WrongParam`
(`adapty_code: 3001`, *"The fallback file version is not correct. Download a new one from the
Adapty Dashboard."*). Your integration is fine — the file is stale.

The 4.1 natives expect **fallback file format 11**, and the files you exported for 3.x no longer
pass. The fix is exactly what the error says: download fresh fallback files for both platforms from
the Adapty Dashboard and replace the ones in `Assets/StreamingAssets/`.

## Turn Adapty Attribution on if you read installation details

The other step nothing warns you about, and the one that looks least like a migration: in 4.1 the
natives collect installation details only when you ask them to. In v3 they always did.

So if your app implements `IAdaptyEventListener.OnInstallationDetailsSuccess` or calls
`Adapty.GetCurrentInstallationStatus`, the callback stops arriving and the status stops reporting
`AdaptyInstallationStatusType.Determined` — on both platforms — until you activate the service:

```csharp
var builder = new AdaptyConfiguration.Builder("PUBLIC_SDK_KEY")
    .SetAdaptyAttributionEnabled(true);
```

Nothing else about it changed: the same details arrive in the same shape, on the same callback. If
your app never looked at installation details, there is nothing to do here — leaving the flag unset
is the same as before, minus the collection you were not using.

## Optional

**Remove workarounds you no longer need.** Three v3 defects are fixed, so code written around them
can go: `ReportTransaction` no longer reports a decoding error on success, `AdaptyProductIdentifier`
compares by value so identifiers from a flow work as dictionary keys, and a call in the Editor
returns a readable "not supported on this platform" error instead of a null one that looked like
success. The full list of fixes is in the changelog.

**Move off the legacy onboarding API.** `GetOnboarding`, `AdaptyUI.CreateOnboardingView` and the
rest still work and now warn at compile time. Build onboardings as flows instead.

**Kids Mode.** If your app ships in the App Store Kids Category, v4 adds the `ADAPTY_KIDS_MODE`
scripting define, which compiles IDFA, AdSupport and AppTrackingTransparency out of the iOS binary.
See the [README](README.md#kids-mode-on-ios) for how to set it.
