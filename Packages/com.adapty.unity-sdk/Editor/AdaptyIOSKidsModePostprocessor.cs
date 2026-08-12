#if UNITY_IOS && ADAPTY_KIDS_MODE
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Enables the KidsMode trait on the AdaptySDK-iOS Swift package reference in the generated
    /// Xcode project, so IDFA / AdSupport / AppTrackingTransparency code is compiled out of the
    /// binary (App Store Kids Category / COPPA compliance). The edit itself is in
    /// AdaptyKidsModeTrait, which is free of Unity types so that the tests can run it.
    ///
    /// Compiled in only when the ADAPTY_KIDS_MODE scripting define is set — the same define that
    /// forces apple_idfa_collection_disabled in the runtime configuration. Prefer setting it in Player
    /// Settings: a build profile's defines reach the Editor assemblies only after Unity recompiles them,
    /// so switching profiles and building in the same session can run this build against stale Editor
    /// assemblies and skip the trait while the runtime still reports Kids Mode. AdaptyIOSBuildValidator
    /// fails the build on that state. BuildPlayerOptions.extraScriptingDefines never reaches Editor
    /// assemblies at all and is invisible to every Editor API, so it cannot be detected.
    /// </summary>
    internal static class AdaptyIOSKidsModePostprocessor
    {
        // Must run after External Dependency Manager adds the Swift package references at order 35.
        [PostProcessBuild(100)]
        private static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS)
            {
                return;
            }

            var pbxPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);

            try
            {
                File.WriteAllText(pbxPath, AdaptyKidsModeTrait.Enable(File.ReadAllText(pbxPath)));
            }
            catch (InvalidOperationException e)
            {
                throw new BuildFailedException(e.Message);
            }

            Debug.Log("Adapty: enabled the KidsMode trait on the AdaptySDK-iOS Swift package.");
        }
    }
}
#endif
