using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Rules about how the runtime sources are arranged. Metadata cannot answer either of these:
    /// <c>partial</c> does not survive compilation, and a file's directory is not a property of the
    /// types in it — so both are asked of the sources themselves.
    /// </summary>
    [TestFixture]
    public class SourceConventionTests
    {
        private static readonly Regex Declaration = new Regex(
            @"^(?<indent>[ \t]*)(?:(?:public|internal|private|protected|static|sealed|abstract)[ \t]+)*partial[ \t]+(?:class|struct|interface)[ \t]+(?<name>\w+)"
        );

        /// <summary>
        /// <c>partial</c> on a type that lives in one file promises a second part that does not
        /// exist, and sends the reader looking for it. The six that keep it are split for a reason:
        /// a nested builder, a nested model, or - for <c>Adapty</c> and <c>AdaptyUI</c> - the
        /// deprecated half held apart under <c>Obsolete/</c>.
        /// </summary>
        [Test]
        public void EveryPartialTypeIsActuallySplit()
        {
            var parts = new Dictionary<string, List<string>>();

            foreach (var file in Sources())
            {
                var nesting = new List<KeyValuePair<int, string>>();

                foreach (var line in File.ReadAllLines(file))
                {
                    var match = Declaration.Match(line);
                    if (!match.Success)
                    {
                        continue;
                    }

                    var indent = match.Groups["indent"].Value.Length;
                    while (nesting.Count > 0 && nesting[nesting.Count - 1].Key >= indent)
                    {
                        nesting.RemoveAt(nesting.Count - 1);
                    }

                    var name = match.Groups["name"].Value;
                    var identity = string.Join(
                        ".",
                        nesting.Select(level => level.Value).Concat(new[] { name })
                    );
                    nesting.Add(new KeyValuePair<int, string>(indent, name));

                    if (!parts.TryGetValue(identity, out var files))
                    {
                        parts[identity] = files = new List<string>();
                    }
                    files.Add(Path.GetFileName(file));
                }
            }

            var alone = parts
                .Where(entry => entry.Value.Count == 1)
                .Select(entry => $"{entry.Key} ({entry.Value[0]})")
                .OrderBy(name => name, System.StringComparer.Ordinal)
                .ToList();

            Assert.That(
                alone,
                Is.Empty,
                "these are partial but declared once, so the modifier points at nothing:\n  "
                    + string.Join("\n  ", alone)
            );
        }

        /// <summary>
        /// Removing the deprecated API should be a directory deletion plus whatever then fails to
        /// compile. That only holds while the attribute and the folder travel together.
        /// </summary>
        [Test]
        public void EveryObsoleteMemberLivesUnderObsolete()
        {
            var outside = Sources()
                .Where(file => !file.Replace('\\', '/').Contains("/Obsolete/"))
                .Where(file => Regex.IsMatch(File.ReadAllText(file), @"\[(System\.)?Obsolete"))
                .Select(Path.GetFileName)
                .OrderBy(name => name, System.StringComparer.Ordinal)
                .ToList();

            Assert.That(
                outside,
                Is.Empty,
                "these carry [Obsolete] outside Runtime/Obsolete:\n  " + string.Join("\n  ", outside)
            );
        }

        /// <summary>
        /// The conventions swept in one pass, kept in one test because each is a single line and
        /// they fail the same way: a file that reintroduces the habit.
        /// </summary>
        /// <remarks>
        /// Deliberately not a rule about <c>== null</c>. That reads as the same tidiness, and it is
        /// not: <c>UnityEngine.Object</c> overloads the operator to answer true for a destroyed
        /// native object, and <c>is null</c> bypasses the overload - so a blanket ban would push
        /// correct Unity code into being wrong.
        /// </summary>
        [Test]
        public void TheSourcesKeepTheirShape()
        {
            var headers = new List<string>();
            var boms = new List<string>();
            var scopedUsings = new List<string>();
            var negatedPatterns = new List<string>();

            foreach (var file in Sources())
            {
                var name = Path.GetFileName(file);
                var text = File.ReadAllText(file);

                // Read as bytes: File.ReadAllText strips the mark while decoding, and
                // StartsWith("\uFEFF") is culture-sensitive, where a zero-weight character matches
                // the start of every string - both would report the opposite of the truth.
                var head = new byte[3];
                using (var stream = File.OpenRead(file))
                {
                    if (stream.Read(head, 0, 3) == 3
                        && head[0] == 0xEF && head[1] == 0xBB && head[2] == 0xBF)
                    {
                        boms.Add(name);
                    }
                }

                if (Regex.IsMatch(text, @"^//\s*\r?\n//\s+[\w.]+\.cs\r?\n"))
                {
                    headers.Add(name);
                }

                foreach (var line in Code(file))
                {
                    if (Regex.IsMatch(line, @"^\s+using [\w.= ]+;\s*$"))
                    {
                        scopedUsings.Add($"{name}: {line.Trim()}");
                    }

                    if (Regex.IsMatch(line, @"!\([^()]*\bis\b"))
                    {
                        negatedPatterns.Add($"{name}: {line.Trim()}");
                    }
                }
            }

            Assert.Multiple(() =>
            {
                Assert.That(headers, Is.Empty, "these carry the old file header block");
                Assert.That(boms, Is.Empty, "these start with a byte order mark");
                Assert.That(scopedUsings, Is.Empty, "these declare a using inside the namespace");
                Assert.That(
                    negatedPatterns,
                    Is.Empty,
                    "these negate a type pattern with ! instead of writing `is not`"
                );
            });
        }

        /// <summary>
        /// A sweep that stops finding what it sweeps passes silently.
        /// </summary>
        [Test]
        public void TheSweepStillSeesTheRuntime()
        {
            var files = Sources().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(files.Count, Is.GreaterThan(50), "far fewer sources than the package has");
                Assert.That(
                    files.Count(file => file.Replace('\\', '/').Contains("/Obsolete/")),
                    Is.GreaterThan(5),
                    "the deprecated tree is no longer being found"
                );
                Assert.That(
                    files.Count(file => File.ReadAllLines(file).Any(Declaration.IsMatch)),
                    Is.GreaterThan(5),
                    "the partial rule no longer matches any declaration"
                );
            });
        }

        private static IEnumerable<string> Sources() =>
            Directory.EnumerateFiles(Package(), "*.cs", SearchOption.AllDirectories);

        /// <summary>
        /// Lines that are not a comment, so a rule about code does not trip over prose.
        /// </summary>
        private static IEnumerable<string> Code(string file) =>
            File.ReadAllLines(file).Where(line => !line.TrimStart().StartsWith("//"));

        private static string Package() =>
            Path.Combine(ProjectDirectory(), "..", "..", "Packages", "com.adapty.unity-sdk");

        private static string ProjectDirectory([CallerFilePath] string callerPath = null) =>
            Path.GetDirectoryName(callerPath);
    }
}
