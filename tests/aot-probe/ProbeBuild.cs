using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class ProbeBuild
{
    public static void BuildIOSSimulator()
    {
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.iOS, ManagedStrippingLevel.High);
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        // Apple Silicon simulators are arm64; Unity defaults the simulator player to x86_64.
        PlayerSettings.SetPropertyInt("iOSSimulatorArchitecture", 1, BuildTargetGroup.iOS);
        PlayerSettings.iOS.targetOSVersionString = "15.0";
        PlayerSettings.applicationIdentifier = "io.adapty.aotprobe";
        PlayerSettings.productName = "AotProbe";

        var scenePath = "Assets/Probe.unity";
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            UnityEditor.SceneManagement.NewSceneMode.Single
        );
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);

        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { scenePath },
            locationPathName = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "ios-build"),
            target = BuildTarget.iOS,
            targetGroup = BuildTargetGroup.iOS,
            options = BuildOptions.None,
        });

        Debug.Log("ProbeBuild: " + report.summary.result + ", errors: " + report.summary.totalErrors);
        EditorApplication.Exit(report.summary.result == BuildResult.Succeeded ? 0 : 1);
    }
}
