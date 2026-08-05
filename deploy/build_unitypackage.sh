#!/usr/bin/env bash

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

DEPLOY_PATH="$SCRIPT_DIR/output"
SOURCE_PATH="$PROJECT_ROOT/Packages/com.adapty.unity-sdk/Runtime"
EDITOR_SOURCE_PATH="$PROJECT_ROOT/Packages/com.adapty.unity-sdk/Editor"
PACKAGE_JSON="$PROJECT_ROOT/Packages/com.adapty.unity-sdk/package.json"
STAGED_SDK_PATH="Assets/AdaptySDK"
UNITY_PATH="${UNITY_PATH:-}"
PACKAGE_NAME="${PACKAGE_NAME:-}"
PACKAGE_VERSION="$(sed -n 's/^[[:space:]]*"version"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' "$PACKAGE_JSON" 2>/dev/null | head -n 1 || true)"
DEFAULT_PACKAGE_NAME="adapty-unity-plugin-${PACKAGE_VERSION:-unknown}.unitypackage"

PRODUCTION=0
KEEP_STAGING=0

usage() {
  cat <<EOF
Usage: $0 [options]

Options:
  -p, --production      Move the exported .unitypackage to the repository root.
  --unity-path PATH     Use a specific Unity executable.
  --keep-staging        Keep the temporary staging project for inspection.
  -h, --help            Show this help message.

Environment:
  UNITY_PATH            Unity executable to use if --unity-path is not passed.
  PACKAGE_NAME          Output package name. Defaults to $DEFAULT_PACKAGE_NAME.
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    -p|--production)
      PRODUCTION=1
      ;;
    --unity-path)
      if [[ $# -lt 2 ]]; then
        echo "Missing value for --unity-path" >&2
        exit 1
      fi
      UNITY_PATH="$2"
      shift
      ;;
    --keep-staging)
      KEEP_STAGING=1
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Unknown option: $1" >&2
      usage
      exit 1
      ;;
  esac
  shift
done

resolve_unity_path() {
  if [[ -n "$UNITY_PATH" ]]; then
    return
  fi

  local project_version
  project_version="$(awk -F': ' '/m_EditorVersion:/{print $2; exit}' "$PROJECT_ROOT/ProjectSettings/ProjectVersion.txt")"

  local project_unity="/Applications/Unity/Hub/Editor/$project_version/Unity.app/Contents/MacOS/Unity"
  if [[ -x "$project_unity" ]]; then
    UNITY_PATH="$project_unity"
    return
  fi

  local candidate
  candidate="$(find /Applications/Unity/Hub/Editor -path '*/Unity.app/Contents/MacOS/Unity' -type f 2>/dev/null | sort | tail -n 1 || true)"
  if [[ -n "$candidate" && -x "$candidate" ]]; then
    UNITY_PATH="$candidate"
    return
  fi

  echo "Unity executable not found. Pass --unity-path or set UNITY_PATH." >&2
  exit 1
}

if [[ ! -d "$SOURCE_PATH" ]]; then
  echo "SDK source not found: $SOURCE_PATH" >&2
  exit 1
fi

if [[ ! -d "$EDITOR_SOURCE_PATH" ]]; then
  echo "SDK editor sources not found: $EDITOR_SOURCE_PATH" >&2
  exit 1
fi

if [[ ! -f "$PACKAGE_JSON" ]]; then
  echo "Package manifest not found: $PACKAGE_JSON" >&2
  exit 1
fi

if [[ -z "$PACKAGE_VERSION" ]]; then
  echo "Package version not found in $PACKAGE_JSON" >&2
  exit 1
fi

if [[ -z "$PACKAGE_NAME" ]]; then
  PACKAGE_NAME="$DEFAULT_PACKAGE_NAME"
fi

resolve_unity_path

if [[ ! -x "$UNITY_PATH" ]]; then
  echo "Unity executable is not runnable: $UNITY_PATH" >&2
  exit 1
fi

mkdir -p "$DEPLOY_PATH"

STAGING_PROJECT="$(mktemp -d "${TMPDIR:-/tmp}/adapty-unitypackage.XXXXXX")"
cleanup() {
  if [[ "$KEEP_STAGING" -eq 0 ]]; then
    rm -rf "$STAGING_PROJECT"
  else
    echo "Staging project kept at $STAGING_PROJECT"
  fi
}
trap cleanup EXIT

echo "Preparing staging project at $STAGING_PROJECT"

mkdir -p "$STAGING_PROJECT/Assets"
mkdir -p "$STAGING_PROJECT/Packages"
mkdir -p "$STAGING_PROJECT/ProjectSettings"

# The staged sources are compiled by the Editor during export, so the staging project has to
# resolve what they import. Newtonsoft arrived with the JSON layer migration; without it the
# export builds against a project that cannot compile the SDK.
printf '{ "dependencies": { "com.unity.nuget.newtonsoft-json": "3.2.2" } }\n' > "$STAGING_PROJECT/Packages/manifest.json"
cp "$PROJECT_ROOT/ProjectSettings/ProjectVersion.txt" "$STAGING_PROJECT/ProjectSettings/ProjectVersion.txt"

cp -R "$SOURCE_PATH" "$STAGING_PROJECT/$STAGED_SDK_PATH"
cp "$SOURCE_PATH.meta" "$STAGING_PROJECT/$STAGED_SDK_PATH.meta"

# The package keeps its editor-only code outside Runtime, so it needs a second copy. It merges into
# the Editor folder Runtime already contributes, which carries AdaptySDKDependencies.xml.
cp -R "$EDITOR_SOURCE_PATH/." "$STAGING_PROJECT/$STAGED_SDK_PATH/Editor/"

EXPORT_PATH="$DEPLOY_PATH/$PACKAGE_NAME"
LOG_PATH="$DEPLOY_PATH/build_unitypackage.log"

rm -f "$EXPORT_PATH" "$LOG_PATH"

echo "Start Build for $PACKAGE_NAME"
echo "Unity: $UNITY_PATH"
echo "Export path inside package: $STAGED_SDK_PATH"

"$UNITY_PATH" \
  -gvh_disable \
  -batchmode \
  -nographics \
  -logFile "$LOG_PATH" \
  -projectPath "$STAGING_PROJECT" \
  -exportPackage "$STAGED_SDK_PATH" "$EXPORT_PATH" \
  -quit

echo "The package exported to $EXPORT_PATH"

if [[ "$PRODUCTION" -eq 1 ]]; then
  ROOT_EXPORT_PATH="$PROJECT_ROOT/$PACKAGE_NAME"

  echo "Moving $EXPORT_PATH to $ROOT_EXPORT_PATH"
  mv "$EXPORT_PATH" "$ROOT_EXPORT_PATH"

  echo "Removing $DEPLOY_PATH"
  rm -rf "$DEPLOY_PATH"
else
  echo "Dev mode. No files removed. Run with -p or --production to move the package to the repository root."
fi
