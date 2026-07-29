//
//  AdaptyIOSBuildValidator.cs
//  AdaptySDK
//

using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Validates that the iOS deployment target satisfies the minimum required by the native Adapty SDK.
    /// </summary>
    internal sealed class AdaptyIOSBuildValidator : IPreprocessBuildWithReport
    {
        private static readonly Version MinIOSVersion = new Version(15, 0);

        public int callbackOrder => 0;

        private const string KidsModeDefine = "ADAPTY_KIDS_MODE";

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
            {
                return;
            }

            ValidateKidsMode();

            var targetString = PlayerSettings.iOS.targetOSVersionString;
            if (string.IsNullOrEmpty(targetString))
            {
                return;
            }

            if (!targetString.Contains("."))
            {
                targetString += ".0";
            }

            if (!Version.TryParse(targetString, out var target))
            {
                UnityEngine.Debug.LogWarning(
                    $"[Adapty] Could not parse the iOS deployment target '{PlayerSettings.iOS.targetOSVersionString}', "
                        + $"skipping the minimum version check. Adapty SDK requires iOS {MinIOSVersion} or newer."
                );
                return;
            }

            if (target < MinIOSVersion)
            {
                throw new BuildFailedException(
                    $"Adapty SDK requires iOS deployment target {MinIOSVersion} or newer, "
                        + $"but Player Settings specify {PlayerSettings.iOS.targetOSVersionString}. "
                        + "Increase 'Target minimum iOS Version' in Player Settings > Other Settings."
                );
            }
        }

        /// <summary>
        /// The player assemblies see ADAPTY_KIDS_MODE from a build profile too, but the Editor assemblies —
        /// and with them the postprocessor that enables the KidsMode Swift package trait — only ever see
        /// Player Settings. Left alone, that ships a binary that reports Kids Mode at runtime while still
        /// linking IDFA / AdSupport / AppTrackingTransparency, which is exactly what the App Store Kids
        /// Category forbids. This assembly is always compiled, so the absence of the postprocessor type is
        /// a reliable signal that the define never reached the Editor side.
        /// </summary>
        private static void ValidateKidsMode()
        {
            var defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.iOS);
            var kidsModeRequested = Array.IndexOf(
                defines.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries),
                KidsModeDefine
            ) >= 0;

            if (!kidsModeRequested)
            {
                return;
            }

            var postprocessor = typeof(AdaptyIOSBuildValidator).Assembly.GetType(
                "AdaptySDK.Editor.AdaptyIOSKidsModePostprocessor"
            );

            if (postprocessor != null)
            {
                return;
            }

            throw new BuildFailedException(
                $"Adapty: {KidsModeDefine} is set for the player, but not for the Editor assemblies, so the "
                    + "KidsMode Swift package trait would not be enabled and the build would still link IDFA "
                    + "while reporting Kids Mode at runtime. This happens when the define comes from a build "
                    + $"profile. Add {KidsModeDefine} to Player Settings > Other Settings > Scripting Define "
                    + "Symbols instead."
            );
        }
    }
}
