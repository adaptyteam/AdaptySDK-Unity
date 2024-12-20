using UnityEditor;
using UnityEditor.Callbacks;

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
        var frameworkTargetGuid = proj.GetUnityFrameworkTargetGuid();

        UpdateBitcodeProperty(proj, mainTargetGuid);
        UpdateBitcodeProperty(proj, frameworkTargetGuid);

        UpdateBuildProperties(proj, testTargetGuid);

        proj.WriteToFile(projPath);
#endif
	}

#if UNITY_IOS
    static void UpdateBitcodeProperty(PBXProject project, string targetGuid)
    {
        project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
    }

    static void UpdateBuildProperties(PBXProject project, string targetGuid) {
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
        project.AddBuildProperty(targetGuid, "SWIFT_VERSION", "5.9");
        project.AddBuildProperty(targetGuid, "COREML_CODEGEN_LANGUAGE", "Swift");
    }
#endif
}
