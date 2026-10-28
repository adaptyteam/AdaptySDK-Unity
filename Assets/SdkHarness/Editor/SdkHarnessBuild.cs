using System;
using System.IO;
using Unity.Pipeline.Commands;
using Unity.Pipeline.Config;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEditor.UnityLinker;
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

        // Set for the duration of a harness build, so the post-processors below can tell a harness build
        // from the demo's and leave the demo's bundle alone.
        private static bool s_RigBuild;

        // What the harness reaches only by reflection. Response models and converters are left to the
        // linker: their decoding after stripping is what the harness checks. A link.xml under Assets/
        // would go into every build and keep the demo and StrippingBuild from being stripped too.
        private const string LinkXml = @"<linker>
  <assembly fullname=""com.adapty.unity-sdk"">
    <type fullname=""AdaptySDK.Adapty"" preserve=""methods"" />
    <type fullname=""AdaptySDK.AdaptyUI"" preserve=""methods"" />
    <type fullname=""AdaptySDK.Serialization.AdaptyJson"" preserve=""methods"" />
    <type fullname=""AdaptySDK.AdaptyConfiguration/Builder"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyProfileParameters/Builder"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyPlacementFetchPolicy"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyExternalAttributionProvider"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyUICreateFlowViewParameters"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyUIDialogConfiguration"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyPurchaseParameters"" preserve=""all"" />
    <type fullname=""AdaptySDK.AdaptyStoreMessageType"" preserve=""all"" />
  </assembly>
  <assembly fullname=""Assembly-CSharp"">
    <namespace fullname=""AdaptyExample.SdkHarness"" preserve=""all"" />
  </assembly>
</linker>";

        [CliCommand("harness_build", "Build the SDK harness scene for the iOS simulator into ios-harness-build (Development). Submit with --detach, then unity job wait.",
            MainThreadRequired = true, Tags = new[] { "adapty" })]
        public static object Build()
        {
            // AddLocalStoreKit is compiled for the platform the Editor is on, so from any other one
            // the build would come out without the local StoreKit and purchases would hit the sandbox.
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                throw new InvalidOperationException(
                    "harness_build needs the Editor switched to iOS (File > Build Profiles); it is on "
                        + EditorUserBuildSettings.activeBuildTarget
                );
            }

            var sdkVersion = PlayerSettings.iOS.sdkVersion;
            PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;

            try
            {
                return BuildHarness(BuildTarget.iOS, Output);
            }
            finally
            {
                PlayerSettings.iOS.sdkVersion = sdkVersion;
            }
        }

        /// <summary>
        /// Builds the harness scene with what a harness build needs switched on, and puts every
        /// setting it touched back afterwards.
        /// </summary>
        /// <remarks>
        /// Enable In Builds puts the Pipeline runtime server into any player built while it is on,
        /// release ones included, so the project keeps it off and only a harness build turns it on.
        /// The Pipeline build processor reads <c>EditorUserBuildSettings.development</c>, not the
        /// build options: with Enable In Builds on and the flag off it stops an interactive build on
        /// a release-build dialog.
        /// </remarks>
        private static object BuildHarness(BuildTarget target, string output)
        {
            var pipeline = RuntimePipelineConfig.Load()
                ?? throw new InvalidOperationException("No Pipeline runtime settings in ProjectSettings/Packages/com.unity.pipeline");
            var enableInBuilds = pipeline.enableInBuilds;
            var development = EditorUserBuildSettings.development;

            pipeline.enableInBuilds = true;
            pipeline.Save();
            EditorUserBuildSettings.development = true;
            s_RigBuild = true;

            BuildReport report;

            try
            {
                report = BuildPipeline.BuildPlayer(
                    new BuildPlayerOptions
                    {
                        scenes = new[] { Scene },
                        locationPathName = output,
                        target = target,
                        options = BuildOptions.Development,
                    }
                );
            }
            finally
            {
                s_RigBuild = false;
                EditorUserBuildSettings.development = development;
                pipeline.enableInBuilds = enableInBuilds;
                pipeline.Save();
                UnityEngine.Object.DestroyImmediate(pipeline);
            }

            return new
            {
                result = report.summary.result.ToString(),
                errors = report.summary.totalErrors,
                output = report.summary.outputPath,
                seconds = (int)report.summary.totalTime.TotalSeconds,
            };
        }

        private sealed class HarnessLinkXml : IUnityLinkerProcessor
        {
            public int callbackOrder => 0;

            public string GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
            {
                var path = Path.GetFullPath("Temp/SdkHarness.link.xml");
                File.WriteAllText(path, s_RigBuild ? LinkXml : "<linker />");
                return path;
            }
        }

        /// <summary>
        /// The same scene for an Android device: a Development build, so the player is debuggable
        /// and <c>run-as</c> can read the runtime descriptor <c>AndroidHarnessServer</c> writes.
        /// </summary>
        [CliCommand("harness_build_android", "Build the SDK harness scene for an Android device into android-harness-build.apk (Development, ARM64). Submit with --detach, then unity job wait.",
            MainThreadRequired = true, Tags = new[] { "adapty" })]
        public static object BuildAndroid()
        {
            var architectures = PlayerSettings.Android.targetArchitectures;
            var identifier = PlayerSettings.GetApplicationIdentifier(NamedBuildTarget.Android);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.adaptytest");

            try
            {
                return BuildHarness(BuildTarget.Android, "android-harness-build.apk");
            }
            finally
            {
                PlayerSettings.Android.targetArchitectures = architectures;
                PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, identifier);
            }
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
