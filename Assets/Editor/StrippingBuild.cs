using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

/// <summary>
/// Builds the demo with managed stripping set to High.
/// </summary>
/// <remarks>
/// High is the setting the serialization layer has to survive: it is what removes the constructors
/// and members a reflection-based serializer reaches for, and the reason the package ships a
/// link.xml. The project's own setting is Low, so it is raised here rather than in ProjectSettings
/// - the migration has to prove the strict case, not change what the demo ships with.
/// </remarks>
public static class StrippingBuild
{
    public static void IOS() => Build(BuildTarget.iOS, NamedBuildTarget.iOS, "ios-stripped-build");

    /// <summary>
    /// The simulator player, still at stripping High: the scenario run has to exercise the same
    /// configuration the device build does.
    /// </summary>
    public static void IOSSimulator()
    {
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        Build(BuildTarget.iOS, NamedBuildTarget.iOS, "ios-sim-build");
    }

    /// <summary>
    /// The Android player, at stripping High and built for ARM64.
    /// </summary>
    /// <remarks>
    /// The project targets ARMv7, which no arm64-only emulator will install
    /// (INSTALL_FAILED_NO_MATCHING_ABIS). Set here rather than in ProjectSettings, for the same
    /// reason the stripping level is: the run has to prove the strict case without changing what
    /// the demo ships with.
    /// </remarks>
    public static void Android()
    {
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

        // ProjectSettings already says com.adaptytest, but a batchmode build does not pick it up -
        // it falls back to com.Company.Product. The wrong id is not a build error: the app installs,
        // activates and reports success, and only the flow comes back empty, because the backend does
        // not recognise it. Set it explicitly so the run measures the real app.
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.adaptytest");

        Build(BuildTarget.Android, NamedBuildTarget.Android, "android-stripped-build.apk");
    }

    /// <summary>
    /// <see cref="Android"/> as a development build: a release player's logs do not reach logcat on
    /// every device. Stripping stays High.
    /// </summary>
    public static void AndroidDevelopment()
    {
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.adaptytest");

        Build(
            BuildTarget.Android,
            NamedBuildTarget.Android,
            "android-dev-build.apk",
            BuildOptions.Development
        );
    }

    private static void Build(
        BuildTarget target,
        NamedBuildTarget named,
        string output,
        BuildOptions options = BuildOptions.None
    )
    {
        PlayerSettings.SetManagedStrippingLevel(named, ManagedStrippingLevel.High);
        PlayerSettings.SetScriptingBackend(named, ScriptingImplementation.IL2CPP);

        Debug.Log(
            $"StrippingBuild: {named.TargetName} stripping="
                + PlayerSettings.GetManagedStrippingLevel(named)
                + " backend="
                + PlayerSettings.GetScriptingBackend(named)
        );

        var scenes = EditorBuildSettings
            .scenes.Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        var report = BuildPipeline.BuildPlayer(
            new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = target,
                options = options,
            }
        );

        Debug.Log(
            $"StrippingBuild: {report.summary.result}, errors: {report.summary.totalErrors}"
                + $", size: {report.summary.totalSize}"
        );

        EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
