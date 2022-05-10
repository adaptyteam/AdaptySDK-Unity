using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

using System.IO;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public static class AdaptySDKPostProcess {

	[PostProcessBuild]
	public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
		if (buildTarget == BuildTarget.iOS) {
			OnPostProcessBuildIOS(buildTarget, buildPath);
		}
	}


	static void OnPostProcessBuildIOS(BuildTarget buildTarget, string buildPath) {
#if UNITY_IOS
        var projPath = buildPath + "/Unity-Iphone.xcodeproj/project.pbxproj";
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);

        var testTargetGuid = proj.TargetGuidByName(PBXProject.GetUnityTestTargetName());
        var mainTargetGuid = proj.GetUnityMainTargetGuid();

        UpdateBuildProperties(proj, testTargetGuid);
        CopyAdaptyInfoPlist(buildPath, proj, mainTargetGuid);

        proj.WriteToFile(projPath);
#endif
	}

#if UNITY_IOS
    static void UpdateBuildProperties(PBXProject project, string targetGuid) {
        project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        project.SetBuildProperty(targetGuid, "SWIFT_OBJC_BRIDGING_HEADER", "Libraries/AdaptySDK/Plugins/iOS/Source/AdaptyUnityPlugin-Bridging-Header.h");
        project.SetBuildProperty(targetGuid, "SWIFT_OBJC_INTERFACE_HEADER_NAME", "AdaptyUnityPlugin-Swift.h");


        project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks $(PROJECT_DIR)/lib/$(CONFIGURATION) $(inherited)");
        project.AddBuildProperty(targetGuid, "FRAMERWORK_SEARCH_PATHS",
            "$(inherited) $(PROJECT_DIR) $(PROJECT_DIR)/Frameworks");
        project.AddBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
        project.AddBuildProperty(targetGuid, "DYLIB_INSTALL_NAME_BASE", "@rpath");
        project.AddBuildProperty(targetGuid, "LD_DYLIB_INSTALL_NAME",
            "@executable_path/../Frameworks/$(EXECUTABLE_PATH)");
        project.AddBuildProperty(targetGuid, "DEFINES_MODULE", "YES");
        project.AddBuildProperty(targetGuid, "SWIFT_VERSION", "4.0");
        project.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");
    }

    static void CopyAdaptyInfoPlist(string buildPath, PBXProject project, string targetGuid) {
        var fileName = "Adapty-Info.plist";
        var plistPath = Path.Combine(Application.dataPath, fileName);
        var destinationPath = Path.Combine(buildPath, fileName);

        File.Copy(plistPath, destinationPath, true);

        var fileGuid = project.AddFile(fileName, fileName);
        project.AddFileToBuild(targetGuid, fileGuid);
    }
#endif
}
