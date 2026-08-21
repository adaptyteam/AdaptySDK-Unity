# Migrate Adapty Unity SDK to v4.1

The good news first: this is a small migration, and the compiler does most of it for you. Plan for
three renames and one removed member it will point out, one new interface method it will demand,
and two things it cannot know about: the fallback files both natives now reject, and the flag
installation details now need. Those two are the steps you can genuinely forget, so if you read
just two sections, read
[Re-download your fallback files](#re-download-your-fallback-files) and
[Turn Adapty Attribution on if you read installation details](#turn-adapty-attribution-on-if-you-read-installation-details).

Under the hood, v4.1 moves the native dependencies to AdaptySDK-iOS 4.1.1 and AdaptySDK-Android
4.1.0, renames the external attribution API to match them, makes Adapty Attribution opt-in, and
adds App Store promoted purchases. The why behind each change, and what was fixed along the way, is
in [CHANGELOG.md](Packages/com.adapty.unity-sdk/CHANGELOG.md).

1. [Before you upgrade](#before-you-upgrade)
2. [Rename the attribution API](#rename-the-attribution-api)
3. [Replace `FlowVersionId` with `HasViewConfiguration`](#replace-flowversionid-with-hasviewconfiguration)
4. [Implement the new listener method](#implement-the-new-listener-method)
5. [Re-download your fallback files](#re-download-your-fallback-files)
6. [Turn Adapty Attribution on if you read installation details](#turn-adapty-attribution-on-if-you-read-installation-details)
7. [Optional](#optional)

## Before you upgrade

If v4.0 built for you, v4.1 will too: the toolchain requirements did not move. Unity 2022.3 or
later, `com.unity.nuget.newtonsoft-json`, External Dependency Manager 1.2.188+ for iOS, Xcode 26 or
later, deployment target 15.0 or later — all exactly as in v4.0.

Coming from 3.x? Take [MIGRATION-v3.17-to-v4.0.md](MIGRATION-v3.17-to-v4.0.md) first — it covers
the parts that actually hurt (the paywall-to-flow rename, the Newtonsoft dependency, the install
order). This guide starts where it ends.

Both native dependencies move on their own. iOS is declared inside the package as a Swift package
and External Dependency Manager resolves it at build time; Android ships in the bundled
`.androidlib` that Unity adds to the Gradle build itself. There is nothing for you to update by
hand on either platform.

## Rename the attribution API

The native SDKs renamed their attribution APIs in 4.1, and the Unity SDK follows. There are no
deprecated aliases — deliberately: the old and new names would otherwise sit side by side in
autocomplete for a release cycle, and every existing call site is a small edit the compiler
finds for you anyway.

| v4.0 | v4.1 |
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
against the shared instances rather than string literals. On the wire nothing changed: the same
strings travel, and the data you were sending keeps working unchanged.

## Replace `FlowVersionId` with `HasViewConfiguration`

`AdaptyFlow.FlowVersionId` is no longer public. It named a renderer-internal version identifier
both 4.1 natives keep to themselves, and there was nothing an app could correctly do with the
value. If the compiler flags a use of it, that code was almost certainly asking a different
question — *can this flow be rendered?* — which is now a member of its own:

```csharp
if (flow.HasViewConfiguration)
{
    AdaptyUI.CreateFlowView(flow, (view, error) => { /* ... */ });
}
```

The flow still carries the identifier internally, so nothing changes on the wire or in what the
renderer receives.

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

The body above is the right default even if you have never set up a promoted purchase — they are
the ones you configure in App Store Connect to appear on your App Store product page. It does what
the native iOS SDK does when an app leaves the choice to it: completes the purchase the user
already started by tapping buy on that page. With this release the event *is* the purchase, so an
empty body silently drops something the user explicitly asked for — leave it empty only as a
deliberate decision to ignore promoted purchases. If a purchase must wait for your own flow — a
sign-in, a parental gate — hold on to the product and call `MakePromotedPurchase` when the flow
allows it. On Android the method is never called.

The same applies one level up: an app that never calls `Adapty.SetEventListener` has no handler
for the event, and a promoted purchase is dropped silently there too.

One thing worth knowing: this works because the pin is AdaptySDK-iOS 4.1.1. Native 4.1.0 completed
promoted purchases by itself, without telling anyone, so on that version the handler was never
called; 4.1.1 hands the purchase to the wrapper and expects `MakePromotedPurchase` to finish it. The
native dependency is pinned to exactly 4.1.1 — deliberately, so native behaviour never changes
underneath a wrapper that was not built for it — which is also why the handler was worth
implementing before there was a native that called it.

## Re-download your fallback files

This is the step the compiler cannot catch, and the symptom shows up at runtime looking like a
broken integration: right after upgrading, `Adapty.SetFallback` fails. On iOS it reports
`DecodingFailed` (`adapty_code: 2006`, *"The fallback paywalls version is not correct. Download a
new one from the Adapty Dashboard."*); on Android, `WrongParam` (`adapty_code: 3001`, *"The
fallback file version is not correct. Download a new one from the Adapty Dashboard."*). Your
integration is fine — the file is stale.

Both natives reworked how fallback placements are read in 4.1 and now expect **fallback file format
11**; the format 10 files you exported for v4.0 no longer pass. The fix is exactly what the error
says: download fresh fallback files for both platforms from the Adapty Dashboard and replace the
ones in `Assets/StreamingAssets/`.

## Turn Adapty Attribution on if you read installation details

The other step nothing warns you about, and the one that looks least like a migration: in 4.1 the
natives collect installation details only when you ask them to. In v4.0 they always did.

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

- `AdaptyConfiguration.Builder.SetAdaptyAttributionEnabled(true)` turns on the
  [Adapty Attribution](https://adapty.io/docs/attribution-integration) service. It is off by
  default and not even sent unless you set it — the one thing that hangs off it is the installation
  details covered above.
- That is the whole list. The v4.0 additions (`AdaptyUICreateFlowViewParameters.Locale`,
  `AdaptyUIFlowView.Locale`) are now officially part of the cross-platform contract, and the rest
  of the 4.1 wire-format changes — the nested offer identifier a purchase sends back, the
  `ui_schema` a flow carries for the renderer — happen inside the SDK, where you never see them.
