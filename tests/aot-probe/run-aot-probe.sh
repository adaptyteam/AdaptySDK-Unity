#!/usr/bin/env bash
# Rebuilds the IL2CPP probe for the arm64 simulator and prints its output.
#
# Unity generates the simulator player as x86_64 regardless of the architecture setting when
# driven from the command line, so the arm64 runtime and baselib are swapped in afterwards.

set -euo pipefail

SP="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT="$SP/AotProbe"
BUILD="$PROJECT/ios-build"
UNITY=/Applications/Unity/Hub/Editor/6000.4.5f1/Unity.app/Contents/MacOS/Unity
TRAMPOLINE=/Applications/Unity/Hub/Editor/6000.4.5f1/PlaybackEngines/iOSSupport/Trampoline
BUNDLE_ID=io.adapty.aotprobe

echo "== unity player =="
rm -rf "$BUILD"
"$UNITY" -batchmode -quit -projectPath "$PROJECT" \
  -executeMethod ProbeBuild.BuildIOSSimulator -logFile "$SP/aotprobe-build.log" >/dev/null 2>&1
grep -E "ProbeBuild:" "$SP/aotprobe-build.log" | tail -1

echo "== arm64 swap =="
rm -rf "$BUILD/Frameworks/UnityRuntime.framework"
cp -R "$TRAMPOLINE/Frameworks/UnityRuntime-sim-arm64/UnityRuntime.framework" "$BUILD/Frameworks/"
cp "$TRAMPOLINE/Libraries/baselib-sim-arm64.a" "$BUILD/Libraries/baselib.a"

echo "== xcodebuild =="
cd "$BUILD"
rm -rf dd
xcodebuild -project Unity-iPhone.xcodeproj -scheme Unity-iPhone -configuration Debug \
  -sdk iphonesimulator -destination 'generic/platform=iOS Simulator' \
  -derivedDataPath ./dd CODE_SIGNING_ALLOWED=NO ARCHS=arm64 ONLY_ACTIVE_ARCH=NO build \
  > "$BUILD/xcodebuild.log" 2>&1
grep -E "BUILD SUCCEEDED|BUILD FAILED" "$BUILD/xcodebuild.log" | tail -1

echo "== run =="
APP="$BUILD/dd/Build/Products/Debug-iphonesimulator/AotProbe.app"
xcrun simctl terminate booted "$BUNDLE_ID" >/dev/null 2>&1 || true
xcrun simctl uninstall booted "$BUNDLE_ID" >/dev/null 2>&1 || true
xcrun simctl install booted "$APP"
(xcrun simctl launch --console-pty booted "$BUNDLE_ID" > /tmp/aot-console.txt 2>&1 &)
sleep 15
pkill -f "simctl launch" >/dev/null 2>&1 || true
xcrun simctl terminate booted "$BUNDLE_ID" >/dev/null 2>&1 || true

echo "== probe output =="
grep -o "\[AOT-PROBE\].*" /tmp/aot-console.txt || echo "(no probe output captured)"
