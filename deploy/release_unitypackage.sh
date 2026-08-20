#!/usr/bin/env bash
#
# Publishes the .unitypackage: builds it, commits it under Releases/, tags, pushes, and creates the
# GitHub release. The route around it is feature branch -> dev -> a release/x.y branch cut from dev;
# main gets a merge only for a release. 4.0.0-beta.1 confirms that much of it - its lightweight tag
# sits on a merge into origin/release/4.0.0 and is on neither main nor dev - but not this script's
# own flow: that tag names a merge rather than an "add unitypackage" commit, and no beta.1 artifact
# is tracked under Releases/ at all.
#
# Three things here decide what actually ships, none of them obvious from the flags:
#
#   - It REBUILDS the package unless --skip-build, so a run without that flag puts different bytes
#     under the tag than whatever was accepted. Pass --skip-build to release the file you tested.
#   - It creates its own commit, "add unitypackage <version>", and tags THAT. On a clean build run
#     here, that commit's parent is the source it was built from, so the lineage is not lost. What
#     the parent does not establish is the actual build inputs: nothing checks that the tree was
#     clean, and with --skip-build the package may have been built from another commit entirely.
#     Record the source SHA and build the artifact from a clean tree if that has to be provable.
#   - It pushes HEAD and the tag without checking which branch it is on or whether the tree is
#     clean. Standing on the wrong branch releases that branch.
#
# The tag is lightweight, which is why --notes-file is required to create a release: with
# --notes-from-tag GitHub falls back to the commit message and the notes read "add unitypackage
# <version>". --prerelease is off by default and has to be passed explicitly - to this script, and
# to any `gh release create` run by hand after --skip-github-release.

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

PACKAGE_JSON="$PROJECT_ROOT/Packages/com.adapty.unity-sdk/package.json"
BUILD_SCRIPT="$SCRIPT_DIR/build_unitypackage.sh"
RELEASES_DIR="$PROJECT_ROOT/Releases"

VERSION="$(sed -n 's/^[[:space:]]*"version"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' "$PACKAGE_JSON" 2>/dev/null | head -n 1 || true)"
REMOTE="${REMOTE:-origin}"
TAG="${TAG:-$VERSION}"
PACKAGE_NAME="adapty-unity-plugin-$VERSION.unitypackage"
ROOT_PACKAGE_PATH="$PROJECT_ROOT/$PACKAGE_NAME"
RELEASE_PACKAGE_PATH="$RELEASES_DIR/$PACKAGE_NAME"

fail() {
  echo "$1" >&2
  exit 1
}

DRY_RUN=0
SKIP_BUILD=0
SKIP_COMMIT=0
SKIP_PUSH=0
SKIP_GITHUB_RELEASE=0
FORCE=0
DRAFT=0
PRERELEASE=0
NOTES_FILE=""

usage() {
  cat <<EOF
Usage: $0 [options]

Build and publish the Adapty Unity SDK .unitypackage release.

Options:
  --dry-run                 Print commands without changing files or publishing.
  --skip-build              Use an existing root package instead of building it.
  --skip-commit             Do not create the release package commit.
  --skip-push               Do not push the commit/tag.
  --skip-github-release     Do not create or update the GitHub Release.
  --force                   Replace an existing Releases/$PACKAGE_NAME file.
  --draft                   Create the GitHub Release as a draft.
  --prerelease              Mark the GitHub Release as a prerelease.
  --notes-file <path>       Release notes. Required unless --skip-github-release: the tag this
                            script writes is lightweight, so GitHub would otherwise fall back
                            to the commit message.
  -h, --help                Show this help message.

Environment:
  REMOTE                    Git remote to push to. Defaults to origin.
  TAG                       Git tag / GitHub Release tag. Defaults to $VERSION.
  UNITY_PATH                Passed through to build_unitypackage.sh.

Release package:
  $RELEASE_PACKAGE_PATH
EOF
}

while [[ $# -gt 0 ]]; do
  case "$1" in
    --dry-run)
      DRY_RUN=1
      ;;
    --skip-build)
      SKIP_BUILD=1
      ;;
    --skip-commit)
      SKIP_COMMIT=1
      ;;
    --skip-push)
      SKIP_PUSH=1
      ;;
    --skip-github-release)
      SKIP_GITHUB_RELEASE=1
      ;;
    --force)
      FORCE=1
      ;;
    --draft)
      DRAFT=1
      ;;
    --prerelease)
      PRERELEASE=1
      ;;
    --notes-file)
      [[ $# -ge 2 ]] || fail "--notes-file needs a path"
      NOTES_FILE="$2"
      shift
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

run() {
  if [[ "$DRY_RUN" -eq 1 ]]; then
    printf '+'
    printf ' %q' "$@"
    printf '\n'
  else
    "$@"
  fi
}

if [[ -z "$VERSION" ]]; then
  fail "Package version not found in $PACKAGE_JSON"
fi

if [[ ! -x "$BUILD_SCRIPT" ]]; then
  fail "Build script is not executable: $BUILD_SCRIPT"
fi

if git -C "$PROJECT_ROOT" rev-parse "$TAG" >/dev/null 2>&1; then
  fail "Tag already exists locally: $TAG"
fi

if git -C "$PROJECT_ROOT" ls-remote --exit-code --tags "$REMOTE" "refs/tags/$TAG" >/dev/null 2>&1; then
  fail "Tag already exists on $REMOTE: $TAG"
fi

if [[ "$DRY_RUN" -eq 0 && "$SKIP_GITHUB_RELEASE" -eq 0 ]] && ! command -v gh >/dev/null 2>&1; then
  fail "GitHub CLI is required. Install/authenticate gh or pass --skip-github-release."
fi

if [[ "$SKIP_GITHUB_RELEASE" -eq 0 ]]; then
  # The tag written below is lightweight, so --notes-from-tag would make the release notes read
  # "add unitypackage <version>". Refuse to publish rather than publish that.
  [[ -n "$NOTES_FILE" ]] || fail "--notes-file is required to create a release. Pass it, or --skip-github-release to publish the release yourself."
  [[ -f "$NOTES_FILE" ]] || fail "Notes file not found: $NOTES_FILE"
fi

if [[ -e "$RELEASE_PACKAGE_PATH" && "$FORCE" -eq 0 ]]; then
  fail "Release package already exists: $RELEASE_PACKAGE_PATH. Pass --force to replace it."
fi

if [[ "$SKIP_BUILD" -eq 0 ]]; then
  run env "PACKAGE_NAME=$PACKAGE_NAME" "$BUILD_SCRIPT" --production
else
  [[ -f "$ROOT_PACKAGE_PATH" ]] || fail "Root package not found: $ROOT_PACKAGE_PATH"
fi

run mkdir -p "$RELEASES_DIR"
run mv "$ROOT_PACKAGE_PATH" "$RELEASE_PACKAGE_PATH"

if [[ "$SKIP_COMMIT" -eq 0 ]]; then
  run git -C "$PROJECT_ROOT" add "$RELEASE_PACKAGE_PATH"
  run git -C "$PROJECT_ROOT" commit -m "add unitypackage $VERSION"
else
  echo "Skipping commit. Remember to commit $RELEASE_PACKAGE_PATH before tagging."
fi

run git -C "$PROJECT_ROOT" tag "$TAG"

if [[ "$SKIP_PUSH" -eq 0 ]]; then
  run git -C "$PROJECT_ROOT" push "$REMOTE" HEAD
  run git -C "$PROJECT_ROOT" push "$REMOTE" "$TAG"
else
  echo "Skipping push. Remember to push the commit and tag manually."
fi

if [[ "$SKIP_GITHUB_RELEASE" -eq 0 ]]; then
  GH_ARGS=(release create "$TAG" "$RELEASE_PACKAGE_PATH" --title "$TAG" --notes-file "$NOTES_FILE")

  if [[ "$DRAFT" -eq 1 ]]; then
    GH_ARGS+=(--draft)
  fi

  if [[ "$PRERELEASE" -eq 1 ]]; then
    GH_ARGS+=(--prerelease)
  fi

  if [[ "$DRY_RUN" -eq 1 ]]; then
    run gh "${GH_ARGS[@]}"
  elif gh release view "$TAG" >/dev/null 2>&1; then
    run gh release upload "$TAG" "$RELEASE_PACKAGE_PATH" --clobber
  else
    run gh "${GH_ARGS[@]}"
  fi
else
  MANUAL="gh release create \"$TAG\" \"$RELEASE_PACKAGE_PATH\" --title \"$TAG\" --notes-file <notes.md>"
  [[ "$DRAFT" -eq 1 ]] && MANUAL="$MANUAL --draft"
  [[ "$PRERELEASE" -eq 1 ]] && MANUAL="$MANUAL --prerelease"
  echo "Skipping GitHub Release. Upload manually with:"
  echo "$MANUAL"
  echo "Not --notes-from-tag: this tag is lightweight, so the notes become the commit message."
fi
