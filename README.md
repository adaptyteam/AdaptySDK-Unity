<h1 align="center" style="border-bottom: none">
<b>
    <a href="https://adapty.io?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity">
        <img src="https://adapty-portal-media-production.s3.amazonaws.com/github/logo-adapty-new.svg">
    </a>
</b>
<br>Easy In-App Purchases Integration to
<br>Make Your Unity App Profitable
</h1>

<p align="center">
<a href="https://discord.com/invite/subscriptions-hub"><img src="https://img.shields.io/badge/Adapty-discord-purple"></a>
<a href="https://github.com/adaptyteam/AdaptySDK-Unity/blob/main/LICENSE"><img src="https://img.shields.io/badge/license-MIT-brightgreen.svg"></a>
</p>

<p align="center">
    <a href="https://adapty.io?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity"><b>Website</b></a> •
    <a href="https://discord.com/invite/subscriptions-hub"><b>Discord</b></a> •
    <a href="https://twitter.com/AdaptyTeam"><b>Twitter</b></a>
</p>

![Adapty: CRM for mobile apps with subscriptions](https://adapty-portal-media-production.s3.amazonaws.com/github/adapty-schema.png)

Adapty Unity SDK is a native wrapper around [Adapty iOS SDK](https://github.com/adaptyteam/AdaptySDK-iOS) and [Adapty Android SDK](https://github.com/adaptyteam/AdaptySDK-Android). Both SDKs are written in pure Swift/Kotlin, all wrapped into a C# lib.

Requires Unity 2022.3 or newer and Android API 21 or newer. iOS builds require **Xcode 26 or newer**
and a deployment target of 15.0 or newer: AdaptySDK-iOS 4.0 is a `swift-tools-version: 6.2` package,
and an older toolchain refuses to resolve it. Open the exported project as
**`Unity-iPhone.xcworkspace`** — building `Unity-iPhone.xcodeproj` directly fails at link time with
`ld: framework 'Pods_UnityFramework' not found`.

## Why Adapty?

- [No server code implementation](https://adapty.io/docs/sdk-installation-unity?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Integrate in-app purchases with server-side receipt validation in minutes — in your own paywall or one created in the no-code builder.
- [No-code paywall builder](https://adapty.io/docs/adapty-paywall-builder?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Create a beautiful, natively rendered paywall in the no-code editor and display it in your app to start getting paid instantly.
- [On-the-fly paywalls price testing](https://docs.adapty.io/v3.0/docs/ab-tests?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Test different prices, duration, offers, messages, and designs simultaneously, all without new app releases.
- [Beautiful onboardings](https://adapty.io/docs/onboardings?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Design onboardings in the no-code editor and guide users through their first app experience.
- [Full customer's payment history](https://docs.adapty.io/v3.0/docs/profiles-crm?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Explore the user's payment events from the trial start to subscription cancellation or billing issues.
- [3rd-party integrations](https://docs.adapty.io/v3.0/docs/events?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Send subscription events to 3rd-party analytics, attribution, and ad services with no coding, even if the user uninstalls the app.
- [Advanced analytics](https://docs.adapty.io/v3.0/docs/charts?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity). Analyze your app real-time metrics with advanced filters, such as Ad network, Ad campaign, country, A/B test, etc.

<h3 align="center" style="border-bottom: none; margin-top: -15px; margin-bottom: -15px; font-size: 150%">
<a href="https://adapty.io/schedule-demo?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity_schedule-demo">Talk to Us to Learn More</a>
</h3>

## Integrate IAPs within a few hours without server coding

**Adapty handles everything, from free trials to refunds, in a simple, developer-friendly SDK.**

- Free trials, upgrades, downgrades, crossgrades, family sharing, renewals, promo offers, intro offers, promo codes, and more – Adapty SDK handles them all through one API.
- Easy subscription management.
- One-time purchases and lifetime subscriptions supported.
- Sync subscribers' states across iOS, Android, and Web.

## Design paywalls in the no-code builder

![No-code builder](https://adapty.io/assets/uploads/2024/09/img-builder-and-templates@2x.webp)

With Adapty, you can create a complete, purchase-ready paywall in the no-code builder. 

Adapty automatically renders it and handles all the complex purchase flow, receipt validation, and subscription management behind the scenes.

## Test paywalls & prices on Unity without app releases

![Adapty: In-app subscriptions with paywall A/B testing](https://adapty-portal-media-production.s3.amazonaws.com/github/ab-test-new.png)

- Optimize in-app subscriptions with the paywall A/B testing. Conversions, trials, revenue, cancellations, and more — everything is calculated for you: each paywall and each A/B test.
- Change images, colors, layouts, and literally anything using the no-code builder or a custom JSON. Configure different prices, trial periods, promo offers, and more in Adapty without app releases.

## Real-time analytics for your Unity app

![Adapty: How Adapty works](https://adapty-portal-media-production.s3.amazonaws.com/github/analyticss.gif)

- Manage the subscription's state without managing transactions.
- 99.5% accuracy.
- View and analyze data by attributes, such as status, channels, campaigns, and more.
- Filter, group, and measure metrics by attribution, platform, custom users' segments, and more in a few clicks.

## Mobile app monetization's largest community

Ask questions, participate in discussions about Adapty-related topics, become a part of our community for app developers and marketers. Learn how to monetize your app, ask questions, post jobs, read industry news and analytics. Ad free.

<a href="https://discord.gg/subscriptions-hub"><img src="https://adapty-portal-media-production.s3.amazonaws.com/github/join-discord.svg" /></a>

## Get started

Follow our [quickstart guide](https://adapty.io/docs/unity-sdk-overview?utm_source=github&utm_medium=referral&utm_campaign=AdaptySDK-Unity#get-started) to install and configure Adapty SDK. Set up purchases in hours instead of weeks 🚀

**v4 works in flows.** Paywalls and onboardings are both fetched with `Adapty.GetFlow` and shown
with `AdaptyUI.CreateFlowView`, whether you built them in the Paywall Builder or the new Flow
Builder. The separate onboarding API of v3 still works but is deprecated and warns at compile time,
so start new integrations on flows.

**Installing with Package Manager:** *Add package from git URL*, with the path suffix — the package
does not sit at the repository root:

```
https://github.com/adaptyteam/AdaptySDK-Unity.git?path=/Packages/com.adapty.unity-sdk
```

The SDK depends on `com.unity.nuget.newtonsoft-json`, which Package Manager installs for you and
which every platform needs — the SDK assembly is gated on it. It also depends on External Dependency
Manager, but only for iOS, where it resolves the Swift package; Android never goes through it, since
its dependencies ship in a bundled `.androidlib` that Unity adds to the Gradle build itself. Neither
dependency can arrive with a `.unitypackage`, which carries assets only.

**Installing from a `.unitypackage`:** take the latest from
[Releases](https://github.com/adaptyteam/AdaptySDK-Unity/releases), and add
`com.unity.nuget.newtonsoft-json` **before** importing. Until it is there the SDK assembly is skipped
by a define constraint, so your calls into Adapty will not compile — and **Adapty SDK > Install
Dependencies**, the menu item that installs both dependencies, is unavailable for the same reason.
With Newtonsoft in place, that menu item adds whatever else is missing, including the OpenUPM scoped
registry External Dependency Manager is published on.

It also upgrades an External Dependency Manager older than the SDK needs — but only one installed as
a package. A copy imported from Google's own `.unitypackage` under `Assets/` has no version Package
Manager can read, so the menu item leaves it alone and warns instead; update that one yourself.

Already on 3.x? [MIGRATION-v3.17-to-v4.0.md](MIGRATION-v3.17-to-v4.0.md) covers the move to 4.0 — the renamed paywall API, the
new Newtonsoft.Json dependency, and the order to install things in.

Read the [release notes and known issues](Packages/com.adapty.unity-sdk/CHANGELOG.md) before you
integrate: they carry the limitations of the pinned native SDKs, which no amount of configuration on
your side will work around.

## Kids Mode on iOS

Apps in the App Store Kids Category must not link the advertising identifier. Add the
`ADAPTY_KIDS_MODE` scripting define to build such an app:

- the `KidsMode` trait is enabled on the AdaptySDK-iOS Swift package during the iOS build, so IDFA,
  AdSupport and AppTrackingTransparency are compiled out of the binary;
- `apple_idfa_collection_disabled` is forced in the runtime configuration.

Swift package traits need Xcode 26 or newer, which is already the floor for v4 on iOS — Kids Mode
adds no requirement of its own.

Set the define in **Player Settings > Other Settings > Scripting Define Symbols**. The build step
that enables the trait lives in an Editor assembly, and Player Settings is what Editor assemblies
are compiled with.

A build profile's scripting defines also work, but only once Unity has recompiled the Editor
assemblies for them. Switching build profiles and building in the same session — especially from a
script — can run the build against Editor assemblies compiled for the previous profile, in which
case the trait is silently not applied while the runtime still reports Kids Mode. The SDK fails the
iOS build when it detects that state, so it cannot ship. Setting the define in Player Settings and
letting the Editor recompile before building avoids the situation entirely.

`BuildPlayerOptions.extraScriptingDefines` never works for this define: it reaches the player
assemblies only, and it is invisible to every Editor API, so the mismatch cannot be detected either.

## Contributing

- Feel free to open an issue, we check all of them or drop us an email at [support@adapty.io](mailto:support@adapty.io) and tell us everything you want.
- Want to suggest a feature? Just contact us or open an issue in the repo.

## Like Adapty SDK?

So do we! Feel free to star the repo ⭐️⭐️⭐️ and make our developers happy!

## License

Adapty is available under the MIT license. [Click here](https://github.com/adaptyteam/AdaptySDK-Unity/blob/main/LICENSE) for details.

## Known issues

What is open in this release, and what closing it waits on. The
[changelog](Packages/com.adapty.unity-sdk/CHANGELOG.md) carries the full text of each under the
version it was found in.

- **Custom color and linear gradient assets are not rendered on iOS.** An asset built with
  `AdaptyCustomAsset.Color` or `AdaptyCustomAsset.LinearGradient` and passed through
  `AdaptyUICreateFlowViewParameters.SetCustomAssets` reaches the view as a transparent color and an
  empty gradient: the pinned AdaptySDK-iOS 4.0.2 substitutes those for whatever it receives, and so
  do 4.0.3 and 4.1.0, so there is no version to move the pin to. Custom image and video assets are
  unaffected; whether Android is affected has not been established. Waiting on a native iOS
  release, and on iOS acceptance after it.
