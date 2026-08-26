using System;
using System.Collections.Generic;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Where the External Dependency Manager in this project came from, which is what decides
    /// whether its version can be read at all.
    /// </summary>
    internal enum AdaptyEdmSource
    {
        /// <summary>No copy in the project.</summary>
        None,

        /// <summary>The Package Manager package, whose version is exact.</summary>
        Package,

        /// <summary>
        /// A copy Package Manager does not describe - the one Google ships as its own
        /// .unitypackage under Assets/ - or more than one copy, where no single version answers
        /// for the project.
        /// </summary>
        Unmanaged,
    }

    /// <summary>
    /// What Package Manager has to install, decided from what the project already has. Kept apart
    /// from the Editor code that gathers those facts, so the decision can be tested without one.
    /// </summary>
    internal static class AdaptyDependencyPlan
    {
        internal const string NewtonsoftId = "com.unity.nuget.newtonsoft-json";
        internal const string NewtonsoftVersion = "3.2.2";
        internal const string EdmId = "com.google.external-dependency-manager";
        internal const string EdmVersion = "1.2.188";

        private static readonly Version Required = Version.Parse(EdmVersion);

        /// <summary>
        /// An EDM older than the SDK needs is in the list too: to Package Manager that request is
        /// an upgrade, so the one call covers both installing and moving it forward.
        /// </summary>
        /// <param name="newtonsoftPresent">Whether the Newtonsoft package is in the project.</param>
        /// <param name="edm">Which copy of External Dependency Manager the project has.</param>
        /// <param name="edmVersion">
        /// The version Package Manager reports for it, when it is the one describing it.
        /// </param>
        internal static IEnumerable<string> Missing(
            bool newtonsoftPresent,
            AdaptyEdmSource edm,
            string edmVersion
        )
        {
            // The SDK assembly is gated on the package rather than on the assembly, so a copy that
            // came from anywhere else does not make the SDK compile and is reported separately.
            if (!newtonsoftPresent)
            {
                yield return $"{NewtonsoftId}@{NewtonsoftVersion}";
            }

            if (edm == AdaptyEdmSource.None || (edm == AdaptyEdmSource.Package && IsOlder(edmVersion)))
            {
                yield return $"{EdmId}@{EdmVersion}";
            }
        }

        /// <summary>
        /// What to say about an EDM the SDK cannot establish the version of, and <c>null</c> when
        /// there is nothing to say.
        /// </summary>
        /// <remarks>
        /// Reported rather than installed over: adding the package on top of a copy under
        /// <c>Assets/</c> would leave two, which is a failure of its own. The version is not in the
        /// assembly either - every 1.2.x build of <c>Google.VersionHandler</c> carries the same
        /// <c>1.2.0.0</c>, so Package Manager is the only thing that can tell 1.2.187 from 1.2.188.
        /// </remarks>
        internal static string EdmCaution(AdaptyEdmSource edm, string edmVersion)
        {
            if (edm == AdaptyEdmSource.Unmanaged)
            {
                return "[Adapty] External Dependency Manager is in this project, but not as the "
                    + $"\"{EdmId}\" package, so its version cannot be read. The Adapty SDK needs "
                    + $"{EdmVersion} or newer: older versions get the Xcode project path wrong for "
                    + "the Swift project type, and the iOS build fails without naming the cause. "
                    + "Check the copy this project has, and update it if it is older.";
            }

            if (edm == AdaptyEdmSource.Package && !Version.TryParse(edmVersion, out _))
            {
                return "[Adapty] Package Manager reports External Dependency Manager as "
                    + $"\"{edmVersion}\", which cannot be compared as a version. The Adapty SDK "
                    + $"needs {EdmVersion} or newer to resolve the iOS dependencies.";
            }

            return null;
        }

        private static bool IsOlder(string version) =>
            Version.TryParse(version, out var installed) && installed < Required;
    }
}
