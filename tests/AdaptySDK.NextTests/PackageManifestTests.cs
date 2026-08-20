using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// <c>package.json</c> is what a release is cut from — the tag, the artifact name and what
    /// Package Manager resolves all come from it. Two other places restate parts of it: the
    /// version the SDK reports at runtime, and the dependency versions the installer asks Package
    /// Manager for on behalf of <c>.unitypackage</c> users.
    /// </summary>
    /// <remarks>
    /// Neither restatement is reachable from the other, so a bump that misses one produces a
    /// release that builds, ships and is wrong: a tag and a filename carrying the new version
    /// around an SDK that reports the old one. The release script derives everything from the
    /// manifest and so cannot notice.
    /// </remarks>
    [TestFixture]
    public class PackageManifestTests
    {
        [Test]
        public void TheVersionTheSdkReportsIsTheVersionItShipsAs() =>
            Assert.That(Adapty.SDKVersion, Is.EqualTo((string)Manifest()["version"]));

        /// <summary>
        /// The installer is the only route to these packages for a <c>.unitypackage</c> project,
        /// which has no manifest of its own for Package Manager to read.
        /// </summary>
        [Test]
        public void TheInstallerAsksForEveryDependencyTheManifestDeclares()
        {
            var declared = Declared();

            Assert.That(declared, Is.Not.Empty, "no dependencies read from package.json");
            Assert.That(Installed(), Is.EquivalentTo(declared));
        }

        private static Dictionary<string, string> Declared()
        {
            var manifest = Manifest();

            return new[] { "dependencies", "peerDependencies" }
                .Select(section => manifest[section])
                .Where(section => section != null)
                .SelectMany(section => ((JObject)section).Properties())
                .ToDictionary(property => property.Name, property => (string)property.Value);
        }

        /// <summary>
        /// The installer names each package in a pair of constants — <c>&lt;name&gt;Id</c> and
        /// <c>&lt;name&gt;Version</c> — so the pairs are read back by that shared prefix rather
        /// than by a list kept here, which would be a third copy of the same thing.
        /// </summary>
        private static Dictionary<string, string> Installed()
        {
            var source = File.ReadAllText(PackageFile("Editor/AdaptyDependencyPlan.cs"));

            var constants = Regex
                .Matches(source, @"const\s+string\s+(?<name>\w+?)(?<kind>Id|Version)\s*=\s*""(?<value>[^""]*)""")
                .Cast<Match>()
                .ToLookup(match => match.Groups["name"].Value);

            return constants
                .Where(pair => pair.Count() == 2)
                .ToDictionary(
                    pair => pair.Single(match => match.Groups["kind"].Value == "Id").Groups["value"].Value,
                    pair => pair.Single(match => match.Groups["kind"].Value == "Version").Groups["value"].Value
                );
        }

        private static JObject Manifest() => JObject.Parse(File.ReadAllText(PackageFile("package.json")));

        private static string PackageFile(string path) =>
            Path.Combine(
                Path.GetDirectoryName(SourcePath()),
                "..",
                "..",
                "Packages",
                "com.adapty.unity-sdk",
                path
            );

        private static string SourcePath([CallerFilePath] string path = null) => path;
    }
}
