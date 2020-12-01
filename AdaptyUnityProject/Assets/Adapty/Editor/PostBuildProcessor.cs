using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class AdaptyPostBuildProcessor {

	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
		if (buildTarget == BuildTarget.iOS) {
			UpdateXcodeProject(buildTarget, buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj");
		}
	}

	private static void UpdateXcodeProject(BuildTarget buildTarget, string projectPath) {
		PBXProject project = new PBXProject();
		project.ReadFromString(File.ReadAllText(projectPath));

		string target = project.GetUnityFrameworkTargetGuid();
		project.AddBuildProperty(target, "OTHER_CPLUSPLUSFLAGS", "$(OTHER_CFLAGS) -fcxx-modules");

		File.WriteAllText(projectPath, project.WriteToString());
	}
}
