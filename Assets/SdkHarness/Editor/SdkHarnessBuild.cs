using System.IO;
using Unity.Pipeline.Commands;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace AdaptyExample.SdkHarness.Editor
{
    /// <summary>
    /// Builds the harness scene — and only it — into <c>ios-harness-build</c>.
    /// </summary>
    /// <remarks>
    /// The Pipeline package has a build command of its own, but a single <c>--scenes</c> value
    /// reaches it as a string, binds to an empty array, and it silently builds
    /// <c>EditorBuildSettings</c> instead — the demo scene, with no harness in it. This one takes no
    /// scene argument, so there is nothing to lose on the way.
    ///
    /// A player build holds the main thread for minutes, past any request budget: submit it with
    /// <c>--detach</c> and collect the result with <c>unity job wait</c>.
    /// </remarks>
    public static class SdkHarnessBuild
    {
        public const string Scene = "Assets/SdkHarness/SdkHarness.unity";
        public const string Output = "ios-harness-build";
        public const string StoreKitConfiguration = "Assets/SdkHarness/AdaptyDemo.storekit";

        // Set for the duration of a harness build, so the post-processor below can tell a harness build
        // from the demo's and leave the demo's bundle alone.
        private static bool s_RigBuild;

        [CliCommand("harness_build", "Build the SDK harness scene for the iOS simulator into ios-harness-build (Development). Submit with --detach, then unity job wait.",
            MainThreadRequired = true, Tags = new[] { "adapty" })]
        public static object Build()
        {
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
            s_RigBuild = true;

            UnityEditor.Build.Reporting.BuildReport report;

            try
            {
                report = BuildPipeline.BuildPlayer(
                    new BuildPlayerOptions
                    {
                        scenes = new[] { Scene },
                        locationPathName = Output,
                        target = BuildTarget.iOS,
                        options = BuildOptions.Development,
                    }
                );
            }
            finally
            {
                s_RigBuild = false;
            }

            return new
            {
                result = report.summary.result.ToString(),
                errors = report.summary.totalErrors,
                output = report.summary.outputPath,
                seconds = (int)report.summary.totalTime.TotalSeconds,
            };
        }

        /// <summary>
        /// Gives the harness build Xcode's local StoreKit: the configuration is filed in the project
        /// and selected in the scheme, so a run from Xcode syncs it into the simulator and
        /// purchases never reach the App Store sandbox.
        /// </summary>
        /// <remarks>
        /// Only Xcode does that sync (through its Instruments service inside the simulator):
        /// <c>xcodebuild</c> ignores the scheme option, and <c>SKTestSession</c> from the app is
        /// refused by <c>storekitd</c> as not entitled. So the harness is launched from Xcode — see
        /// AGENTS.md. Xcode offers a <c>.storekit</c> file as a configuration only when it is filed
        /// as <c>text</c> with no target membership; Unity's <c>AddFile</c> files it as
        /// <c>file</c>, hence the rewrite.
        /// </remarks>
        [PostProcessBuild]
        private static void AddLocalStoreKit(BuildTarget target, string path)
        {
#if UNITY_IOS
            if (!s_RigBuild || target != BuildTarget.iOS)
            {
                return;
            }

            var configuration = Path.GetFileName(StoreKitConfiguration);
            File.Copy(StoreKitConfiguration, Path.Combine(path, configuration), overwrite: true);

            var projectPath = PBXProject.GetPBXProjectPath(path);
            var project = new PBXProject();
            project.ReadFromFile(projectPath);
            project.AddFile(configuration, configuration);
            project.WriteToFile(projectPath);
            File.WriteAllText(projectPath, File.ReadAllText(projectPath).Replace(
                "lastKnownFileType = file; path = " + configuration,
                "lastKnownFileType = text; path = " + configuration));

            // The identifier is relative to the .xcodeproj directory.
            var scheme = Path.Combine(path, "Unity-iPhone.xcodeproj/xcshareddata/xcschemes/Unity-iPhone.xcscheme");
            File.WriteAllText(scheme, File.ReadAllText(scheme).Replace(
                "   </LaunchAction>",
                "      <StoreKitConfigurationFileReference identifier = \"../" + configuration + "\">\n"
                + "      </StoreKitConfigurationFileReference>\n"
                + "   </LaunchAction>"));
#endif
        }
    }
}
