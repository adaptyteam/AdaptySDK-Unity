using System;
using System.Collections.Generic;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Enables the KidsMode trait on the AdaptySDK-iOS package reference in a generated
    /// project.pbxproj. Free of Unity types so that the tests can run it; the build step that calls
    /// it is AdaptyIOSKidsModePostprocessor.
    /// </summary>
    /// <remarks>
    /// Text surgery because nothing models this: Unity's PBXProject has no API for the traits of a
    /// remote Swift package reference. Every shape this does not recognise throws, since a Kids
    /// Category build that quietly keeps IDFA is worse than one that fails.
    /// </remarks>
    internal static class AdaptyKidsModeTrait
    {
        internal const string PackageUrl = "adaptyteam/AdaptySDK-iOS";
        internal const string Trait = "KidsMode";

        private const string ReferenceKind = "isa = XCRemoteSwiftPackageReference";

        /// <summary>
        /// The project with the trait in place, and unchanged when it is already there.
        /// </summary>
        internal static string Enable(string project)
        {
            var (open, close) = Reference(project);
            var body = project.Substring(open, close - open);

            if (body.IndexOf(Trait, StringComparison.Ordinal) >= 0)
            {
                return project;
            }

            if (body.IndexOf("traits", StringComparison.Ordinal) >= 0)
            {
                throw Failed(
                    "the AdaptySDK-iOS package reference already declares a traits block without "
                        + Trait
                        + "; Adapty will not merge into it automatically. Add "
                        + Trait
                        + " to that block manually, or report this so the postprocessor can be updated."
                );
            }

            return project.Insert(close, "\ttraits = (\n\t\t\t\t" + Trait + ",\n\t\t\t);\n\t\t");
        }

        /// <summary>
        /// Where the AdaptySDK-iOS package reference begins and ends.
        /// </summary>
        /// <remarks>
        /// The object is identified by its <c>isa</c> and not by the brace nearest the URL alone.
        /// Walking back from the URL is what picks the object, and if it ever picked the wrong one
        /// the trait would be written into a neighbour — a build that reports Kids Mode and still
        /// links IDFA, with nothing to notice it. Requiring the kind, and exactly one match, is
        /// what makes that failure loud.
        /// </remarks>
        private static (int Open, int Close) Reference(string project)
        {
            var found = new List<(int Open, int Close)>();

            for (
                var url = project.IndexOf(PackageUrl, StringComparison.Ordinal);
                url >= 0;
                url = project.IndexOf(PackageUrl, url + 1, StringComparison.Ordinal)
            )
            {
                var open = project.LastIndexOf('{', url);
                if (open < 0)
                {
                    continue;
                }

                var close = MatchingBrace(project, open);
                if (close > url && project.IndexOf(ReferenceKind, open, close - open, StringComparison.Ordinal) >= 0)
                {
                    found.Add((open, close));
                }
            }

            if (found.Count == 1)
            {
                return found[0];
            }

            throw Failed(
                found.Count == 0
                    ? "the generated Xcode project contains no AdaptySDK-iOS Swift package "
                        + "reference. Make sure External Dependency Manager resolved the iOS "
                        + "dependencies declared in AdaptySDKDependencies.xml."
                    : "the generated Xcode project contains "
                        + found.Count
                        + " AdaptySDK-iOS Swift package references, so there is no single one to "
                        + "enable the trait on. Report this so the postprocessor can be updated."
            );
        }

        private static int MatchingBrace(string project, int open)
        {
            var depth = 0;
            var inString = false;

            for (var index = open; index < project.Length; index += 1)
            {
                var character = project[index];

                if (inString)
                {
                    if (character == '\\')
                    {
                        index += 1;
                    }
                    else if (character == '"')
                    {
                        inString = false;
                    }
                    continue;
                }

                switch (character)
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
                            return index;
                        }
                        break;
                }
            }

            throw Failed(
                "failed to locate the end of the AdaptySDK-iOS package reference in project.pbxproj."
            );
        }

        /// <summary>
        /// The one exception type the build step turns into a BuildFailedException, so its own bugs
        /// are not dressed up as a diagnosis of the project.
        /// </summary>
        internal static InvalidOperationException Failed(string message) =>
            new InvalidOperationException("Adapty: " + message);
    }
}
