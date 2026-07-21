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

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.iOS)
            {
                return;
            }

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
    }
}
