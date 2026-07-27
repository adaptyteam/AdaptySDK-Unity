//
//  AdaptyIOSKidsModePostprocessor.cs
//  AdaptySDK
//

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
    /// Compiled in only when the ADAPTY_KIDS_MODE scripting define is set (the same define that
    /// forces apple_idfa_collection_disabled in the runtime configuration), so the two can never
    /// disagree. Enables the KidsMode trait on the AdaptySDK-iOS Swift package reference in the
    /// generated Xcode project, so IDFA / AdSupport / AppTrackingTransparency code is compiled out
    /// (App Store Kids Category / COPPA compliance).
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
            File.WriteAllText(pbxPath, EnableKidsModeTrait(File.ReadAllText(pbxPath)));
            Debug.Log("Adapty: enabled the KidsMode trait on the AdaptySDK-iOS Swift package.");
        }

        internal static string EnableKidsModeTrait(string project)
        {
            var urlIndex = project.IndexOf("adaptyteam/AdaptySDK-iOS", StringComparison.Ordinal);
            if (urlIndex < 0)
            {
                throw new BuildFailedException(
                    "Adapty: ADAPTY_KIDS_MODE is set, but the generated Xcode project contains no "
                        + "AdaptySDK-iOS Swift package reference. Make sure External Dependency Manager "
                        + "resolved the iOS dependencies declared in AdaptySDKDependencies.xml."
                );
            }

            var openIndex = project.LastIndexOf('{', urlIndex);
            var closeIndex = FindMatchingBrace(project, openIndex);
            var reference = project.Substring(openIndex, closeIndex - openIndex);

            if (reference.Contains("KidsMode"))
            {
                return project;
            }

            if (reference.Contains("traits"))
            {
                throw new BuildFailedException(
                    "Adapty: the AdaptySDK-iOS package reference already declares a traits block "
                        + "without KidsMode; Adapty will not merge into it automatically. Add KidsMode "
                        + "to that block manually, or report this so the postprocessor can be updated."
                );
            }

            return project.Insert(closeIndex, "\ttraits = (\n\t\t\t\tKidsMode,\n\t\t\t);\n\t\t");
        }

        private static int FindMatchingBrace(string project, int openIndex)
        {
            var depth = 0;
            var inString = false;
            for (var i = openIndex; i < project.Length; i += 1)
            {
                var c = project[i];
                if (inString)
                {
                    if (c == '\\')
                    {
                        i += 1;
                    }
                    else if (c == '"')
                    {
                        inString = false;
                    }
                    continue;
                }

                switch (c)
                {
                    case '"':
                        inString = true;
                        break;
                    case '{':
                        depth += 1;
                        break;
                    case '}':
                        depth -= 1;
                        if (depth == 0)
                        {
                            return i;
                        }
                        break;
                }
            }

            throw new BuildFailedException(
                "Adapty: failed to locate the end of the AdaptySDK-iOS package reference in project.pbxproj."
            );
        }
    }
}
#endif
