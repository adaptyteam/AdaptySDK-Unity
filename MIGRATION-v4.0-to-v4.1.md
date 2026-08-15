# Migrate Adapty Unity SDK to v4.1

The good news first: this is a small migration, and the compiler does most of it for you. Plan for
three renames it will point out, one new interface method it will demand, and one file to
re-download that it cannot know about — that last one is the only step you can genuinely forget, so
if you read just one section, read
[Re-download your fallback file](#re-download-your-fallback-file).

Under the hood, v4.1 moves the native iOS dependency to AdaptySDK-iOS 4.1.0, renames the external
attribution API to match it, and adds App Store promoted purchases. The why behind each change, and
what was fixed along the way, is in
[CHANGELOG.md](Packages/com.adapty.unity-sdk/CHANGELOG.md).

> **One honest caveat before you start.** This version is an iOS-integration snapshot: the Android
> SDK has no 4.1 release yet, so the Android side stays on the 4.0.x natives (crossplatform 4.0.2 /
> android-sdk 4.0.1) and behaves exactly as it did in v4.0. Nothing here breaks your Android build —
> the new APIs compile everywhere — but the 4.1 features arrive on iOS first. The coordinated
> release follows once Android ships its 4.1.

1. [Before you upgrade](#before-you-upgrade)
2. [Rename the attribution API](#rename-the-attribution-api)
3. [Implement the new listener method](#implement-the-new-listener-method)
4. [Re-download your fallback file](#re-download-your-fallback-file)
5. [Optional](#optional)

## Before you upgrade

If v4.0 built for you, v4.1 will too: the toolchain requirements did not move. Unity 2022.3 or
later, `com.unity.nuget.newtonsoft-json`, External Dependency Manager 1.2.188+ for iOS, Xcode 26 or
later, deployment target 15.0 or later — all exactly as in v4.0.

Coming from 3.x? Take [MIGRATION-v3.17-to-v4.0.md](MIGRATION-v3.17-to-v4.0.md) first — it covers
the parts that actually hurt (the paywall-to-flow rename, the Newtonsoft dependency, the install
order). This guide starts where it ends.

The native iOS dependency moves to AdaptySDK-iOS 4.1.0 on its own: it is declared inside the
package and External Dependency Manager resolves it at build time. There is nothing for you to
update by hand.

## Rename the attribution API

The native SDKs renamed their attribution APIs in 4.1, and the Unity SDK follows. There are no
deprecated aliases — deliberately: the old and new names would otherwise sit side by side in
autocomplete for a release cycle, and every existing call site is a two-word edit the compiler
finds for you anyway.

| v4.0 | v4.1 |
|---|---|
| `Adapty.UpdateAttribution(jsonString, source, handler)` | `Adapty.UpdateExternalAttribution(jsonString, provider, handler)` |
| `Adapty.UpdateAttribution(dictionary, source, handler)` | `Adapty.UpdateExternalAttribution(dictionary, provider, handler)` |
| `AdaptyProfile.AppliedAttributionSources` | `AdaptyProfile.AppliedExternalAttributionProviders` |

Only the names move. The provider is the same open `string` it always was (`"appsflyer"`,
`"adjust"`, `"branch"`, `"tenjin"`, `"apple_search_ads"`, `"custom"`), the profile member is still
an `IReadOnlyList<string>`, and the data you were sending keeps working unchanged.

## Implement the new listener method

`IAdaptyEventListener` gained a member, so every class implementing it stops compiling until you
add:

```csharp
public void OnReceivePromotedPurchase(AdaptyPromotedProduct product)
{
    // The user tapped one of your in-app purchases on your App Store product page.
    // Hand it back to Adapty to complete the purchase:
    Adapty.MakePromotedPurchase(product, (result, error) => { /* ... */ });
}
```

Not sure whether you need this? Then you don't — promoted purchases are the ones you set up
manually in App Store Connect to appear on your App Store product page, and if you had, you would
know. An empty body is a perfectly honest implementation in that case, and the method is never
called on Android either way.

One thing worth knowing before you rely on it: the pinned AdaptySDK-iOS 4.1.0 does not yet hand
promoted purchases to wrappers — it completes them by itself, natively, without telling anyone. We
wired the Unity API to the contract anyway, so when a native release starts reporting them, your
handler starts receiving them with no SDK update on your side. Implementing it now costs you five
lines and nothing else.

## Re-download your fallback file

This is the step the compiler cannot catch, and the symptom shows up at runtime looking like a
broken integration: right after upgrading, `Adapty.SetFallback` reports `DecodingFailed`
(`adapty_code: 2006`, *"The fallback paywalls version is not correct. Download a new one from the
Adapty Dashboard."*). Your integration is fine — the file is stale.

AdaptySDK-iOS 4.1 reworked how fallback placements are read and now expects **fallback file format
11**; the format 10 file you exported for v4.0 no longer passes. The fix is exactly what the error
says: download a fresh iOS fallback file from the Adapty Dashboard and replace the one in
`Assets/StreamingAssets/`.

Leave the Android fallback file alone: the Android native stays on 4.0.x in this release and keeps
reading the format it always did.

## Optional

- `AdaptyConfiguration.Builder.SetAdaptyAttributionEnabled(true)` turns on the new
  [Adapty Attribution](https://adapty.io/docs/attribution-integration) service. It is off by
  default and not even sent unless you set it, so ignoring it changes nothing.
- That is the whole list. The v4.0 additions (`AdaptyUICreateFlowViewParameters.Locale`,
  `AdaptyUIFlowView.Locale`) are now officially part of the cross-platform contract, and the rest
  of the 4.1 wire-format changes — the nested offer identifier a purchase sends back, the
  `ui_schema` a flow carries for the renderer — happen inside the SDK, where you never see them.
