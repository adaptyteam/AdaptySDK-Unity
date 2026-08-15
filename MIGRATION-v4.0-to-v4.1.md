# Migrate Adapty Unity SDK to v4.1

v4.1 renames the external attribution API to match the native SDKs, adds App Store promoted
purchases, and moves the native iOS dependency to AdaptySDK-iOS 4.1.0. It is a small move compared
to v4.0: three renames the compiler catches, one new listener method, and one file to re-download.
Everything this guide does not cover — why each change was made, and what was fixed along the way —
is in [CHANGELOG.md](Packages/com.adapty.unity-sdk/CHANGELOG.md).

> **This version is an iOS-integration snapshot.** The Android SDK has no 4.1 release yet, so the
> Android side stays on the 4.0.x natives (crossplatform 4.0.2 / android-sdk 4.0.1) and behaves as
> it did in v4.0. The coordinated release is cut once Android exposes the matching 4.1 contract.

1. [Before you upgrade](#before-you-upgrade)
2. [Rename the attribution API](#rename-the-attribution-api)
3. [Implement the new listener method](#implement-the-new-listener-method)
4. [Re-download your fallback file](#re-download-your-fallback-file)
5. [Optional](#optional)

## Before you upgrade

Nothing changes in the toolchain: the requirements are exactly v4.0's — Unity 2022.3 or later,
`com.unity.nuget.newtonsoft-json`, External Dependency Manager 1.2.188+ for iOS, **Xcode 26 or
later** and an iOS deployment target of 15.0 or later. If you are coming from 3.x, follow
[MIGRATION-v3.17-to-v4.0.md](MIGRATION-v3.17-to-v4.0.md) first; this guide starts where it ends.

The native iOS dependency moves to AdaptySDK-iOS **4.1.0** (declared in the package, resolved by
External Dependency Manager at build time — nothing to do on your side).

## Rename the attribution API

The native SDKs renamed their attribution APIs in 4.1, and the Unity SDK follows. There are no
deprecated aliases — the old names are gone, and every call site is a compile error until renamed:

| v4.0 | v4.1 |
|---|---|
| `Adapty.UpdateAttribution(jsonString, source, handler)` | `Adapty.UpdateExternalAttribution(jsonString, provider, handler)` |
| `Adapty.UpdateAttribution(dictionary, source, handler)` | `Adapty.UpdateExternalAttribution(dictionary, provider, handler)` |
| `AdaptyProfile.AppliedAttributionSources` | `AdaptyProfile.AppliedExternalAttributionProviders` |

Only the names move. The parameters keep their types — the provider is still an open `string`
(`"appsflyer"`, `"adjust"`, `"branch"`, `"tenjin"`, `"apple_search_ads"`, `"custom"`), and the
profile member is still an `IReadOnlyList<string>`.

## Implement the new listener method

`IAdaptyEventListener` gained a member, so every implementation stops compiling until it adds:

```csharp
public void OnReceivePromotedPurchase(AdaptyPromotedProduct product)
{
    // The user started a purchase from the App Store product page. Complete it:
    Adapty.MakePromotedPurchase(product, (result, error) => { /* ... */ });
}
```

Promoted in-app purchases are an App Store feature, so the method is called on iOS only. An empty
body is a valid choice if your app does not promote products in the App Store.

One thing to know before relying on it: the pinned AdaptySDK-iOS 4.1.0 does not yet report promoted
purchases to wrappers — it completes them by itself, natively. The Unity API is wired for the native
release that starts reporting them, so implementing the handler now costs nothing and starts working
without an SDK update on your side.

## Re-download your fallback file

AdaptySDK-iOS 4.1 reworked fallback placements and reads **fallback file format 11**; the format 10
file that v4.0 shipped against is rejected at `Adapty.SetFallback` with `DecodingFailed`
(`adapty_code: 2006`, *"The fallback paywalls version is not correct"*). Download a fresh iOS
fallback file from the Adapty Dashboard and replace the one in `Assets/StreamingAssets/`.

The Android fallback file is untouched: the Android native stays on 4.0.x in this release and keeps
reading the format it always did.

## Optional

- `AdaptyConfiguration.Builder.SetAdaptyAttributionEnabled(true)` enables the new
  [Adapty Attribution](https://adapty.io/docs/attribution-integration) service. Off by default, and
  not sent at all unless you set it.
- Nothing else at the API surface changed. The v4.0 additions
  (`AdaptyUICreateFlowViewParameters.Locale`, `AdaptyUIFlowView.Locale`) are now part of the
  cross-platform contract, and the 4.1 wire-format changes — the nested offer identifier a purchase
  sends back, the `ui_schema` a flow carries for the renderer — are internal to the SDK.
