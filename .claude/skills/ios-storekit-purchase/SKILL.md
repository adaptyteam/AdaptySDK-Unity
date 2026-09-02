---
name: ios-storekit-purchase
description: Use when a run on the iOS simulator has to get through a StoreKit purchase — the local StoreKit configuration, the purchase sheet and the alerts after it, which log line means which window is up and where its button is — for any app; the SDK harness is the usual one
---

# StoreKit purchases on the iOS simulator

A purchase on the simulator is local only while Xcode's StoreKit configuration is active for the
app: Xcode syncs the `.storekit` file selected in the scheme's Run options into the simulator when
it launches the app, and `storekitd` then serves the products itself. Nothing else performs that
sync — `xcodebuild` ignores the scheme option, and an `SKTestSession` opened from the app is
refused as not entitled — so the app has to be launched by Xcode. Without the configuration every
purchase goes to the App Store sandbox and asks for an Apple Account. (`Assets/SdkHarness/AGENTS.md`
is how this repository's harness gets the configuration and the launch.)

With it active, every purchase goes through three system windows, each needing a tap. They always
sit at the same place, and each one writes a line to the simulator's log, so nothing has to be
looked at: start the purchase, wait for the line, tap, repeat. The taps come from the simulator
control tool; the waits are shell.

## The three windows and their log lines

Everything is read with `xcrun simctl spawn <udid> log show --start <t0> --predicate …`. One
`t0` per purchase, taken before the purchase is sent: a wait that takes its own start time after a
tap misses the line the tap produced a second earlier.

| window | log line that means it is on screen | tap |
|---|---|---|
| purchase sheet "Xcode · <product> · Subscribe" | `process == "PassbookUIService" AND subsystem == "com.apple.passkit" AND category == "Analytics" AND eventMessage CONTAINS "inApp"` — the sheet is hosted by `PassbookUIService`, and this line is written once the product is rendered | Subscribe |
| alert "You’re all set." | `process == "SpringBoard" AND category == "AlertItems" AND eventMessage CONTAINS "Received request to activate alertItem"`, `title:` in the same line | OK (full width) |
| alert "You’re currently subscribed to this." | same line, different title — the product is already active, no sheet is shown | OK (right of Manage) |
| alert closed | same category, `eventMessage CONTAINS "Deactivated alertItem"` | — |

The purchase completes only after the alert's OK: `storekitd` logs `AMSPurchaseTask … ===
Finished ===` at that tap, not at Subscribe. So whatever started the purchase has to keep waiting
with a long budget, and its result is read after the third window.

## Where the buttons are

All three windows are system UI, laid out by iOS from the screen size in points (W×H) and the
safe area, so the taps follow from the size rather than from a table. W and H are the screenshot's
pixel size over 3 (every iPhone simulator is 3×); C = (H + 25) / 2 is the middle of the safe area
on a Dynamic Island phone (59 pt above, 34 below):

| button | x | y | why |
|---|---|---|---|
| Subscribe | W / 2 | H − 55 | pinned to the bottom of the sheet |
| OK on "You’re all set." | W / 2 | C + 57 | the alert is centred in the safe area, its text never changes, its width is fixed |
| OK on "You’re currently subscribed to this." | W / 2 + 74 | C + 99 | longer text, two buttons side by side; Manage is at W / 2 − 74 |

Verified on iPhone 17 Pro (402×874: 201×819, 201×506, 275×548) and iPhone 17 Pro Max (440×956:
220×901, 220×547, 294×589) — every measured button within 1 pt of the formula. On iPhone 17e
(390×844) only Subscribe (195×789) was measured; its alerts were not, so the 25 in C is a
Dynamic-Island figure that the 17e has not confirmed. iPad is out: the sheet and the alerts are
laid out differently.

## Running it

```bash
UDID=<the simulator Xcode launched the app on>
T0=$(date '+%Y-%m-%d %H:%M:%S')                                # one t0 for the whole purchase
# start the purchase in the background with a long budget — on the harness:
#   unity command harness_call --runtime-path "$APP" --method MakePurchase --args '{"product":"$GetPaywallProducts[0]"}' --timeout_ms 90000 --timeout 120 > /tmp/purchase.json &
# wait for a window: the first line after T0 matching the sheet or an alert (title in the line if it is an alert)
until L=$(xcrun simctl spawn $UDID log show --start "$T0" --predicate '(process == "PassbookUIService" AND subsystem == "com.apple.passkit" AND category == "Analytics" AND eventMessage CONTAINS "inApp") OR (process == "SpringBoard" AND category == "AlertItems" AND eventMessage CONTAINS "Received request to activate alertItem")' --style compact | grep -v '^Timestamp' | head -1) && [[ -n "$L" ]]; do sleep 1; done; echo "$L" | grep -o 'title: [^;]*' || echo sheet
# tap Subscribe (or the button the title calls for), then wait the same way for the alert line, tap its OK,
# then wait for `eventMessage CONTAINS "Deactivated alertItem"`; only after that does the purchase return.
```

A whole purchase is about 25 s wall-clock, nearly all of it the turns between commands and taps;
the sheet itself is logged in the same second the purchase is sent.

## What the store remembers

Transactions live in the simulator's `storekitd`, not in the app: reinstalling the app keeps
them, so a second purchase of an active subscription gets the "currently subscribed" alert (and
still returns success), and a different product in the same group is queued to start when the
current one expires — the profile keeps showing the old product, which is StoreKit doing its job.
Clearing them takes Xcode's Debug ▸ StoreKit ▸ Manage Transactions, or erasing the simulator.

## When a sign-in window appears

"Sign in to Apple Account" (fields, Cancel/OK) is also an `SBUserNotificationAlert`, so the wait
reports it by title. Tap Cancel (W / 2 − 74, above the keyboard) and stop: never type anything
into it. It means the purchase went to the App Store sandbox, i.e. the local configuration is not
active — the app was launched by something other than Xcode (`xcodebuild`, `simctl launch`), or
the scheme lost its `StoreKitConfigurationFileReference`. Fix the launch; do not press on.
