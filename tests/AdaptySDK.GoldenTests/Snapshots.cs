using System;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace AdaptySDK.GoldenTests
{
    /// <summary>
    /// Fixture loading and approval-style assertions.
    ///
    /// An approved snapshot is the reference behaviour of the current SimpleJSON layer. The new
    /// Newtonsoft package is expected to reproduce it, so a diff here is either a regression or a
    /// deliberate change that has to be recorded in the migration plan.
    ///
    /// Set ADAPTY_UPDATE_SNAPSHOTS=1 to (re)write approved files instead of failing.
    /// </summary>
    public static class Snapshots
    {
#if UNITY_IOS
        public const string Platform = "ios";
#elif UNITY_ANDROID
        public const string Platform = "android";
#else
        public const string Platform = "editor";
#endif

        private static readonly string ProjectDirectory = ResolveProjectDirectory();

        private static string FixturesDirectory => Path.Combine(ProjectDirectory, "Fixtures");

        public static string LoadResponse(string name) =>
            File.ReadAllText(Path.Combine(FixturesDirectory, "responses", name + ".json"));

        /// <summary>
        /// Rewrites a request payload with its keys sorted, so snapshots compare what was sent
        /// rather than the order it happened to be written in: the manual layer emits keys in the
        /// order of the Add(..) calls, Newtonsoft in the order of the members.
        ///
        /// A whole-numbered float is also folded to an integer. JSON has a single number type, and
        /// the two writers render the same value differently - SimpleJSON writes 0, Newtonsoft
        /// writes 0.0 - which is not a difference any reader can observe. Fractional values keep
        /// their digits, so a real precision drift still shows up.
        /// </summary>
        public static string Canonical(string json)
        {
            var token = Newtonsoft.Json.Linq.JToken.Parse(json);
            return Sort(token).ToString(Newtonsoft.Json.Formatting.Indented) + "\n";
        }

        private static Newtonsoft.Json.Linq.JToken Sort(Newtonsoft.Json.Linq.JToken token)
        {
            if (token is Newtonsoft.Json.Linq.JObject map)
            {
                var sorted = new Newtonsoft.Json.Linq.JObject();
                var keys = new System.Collections.Generic.List<string>();
                foreach (var property in map.Properties())
                {
                    keys.Add(property.Name);
                }
                keys.Sort(System.StringComparer.Ordinal);
                foreach (var key in keys)
                {
                    sorted.Add(key, Sort(map[key]));
                }
                return sorted;
            }

            if (token is Newtonsoft.Json.Linq.JArray array)
            {
                var sorted = new Newtonsoft.Json.Linq.JArray();
                foreach (var item in array)
                {
                    sorted.Add(Sort(item));
                }
                return sorted;
            }

            if (token.Type == Newtonsoft.Json.Linq.JTokenType.Float)
            {
                var number = ((Newtonsoft.Json.Linq.JValue)token).ToObject<double>();
                if (number == Math.Floor(number) && !double.IsInfinity(number))
                {
                    return new Newtonsoft.Json.Linq.JValue((long)number);
                }
            }

            return token;
        }

        public static void Matches(string name, string actual)
        {
            var approvedDirectory = Path.Combine(FixturesDirectory, "approved");
            Directory.CreateDirectory(approvedDirectory);

            var approvedPath = Path.Combine(approvedDirectory, name + "." + Platform + ".approved.txt");
            var updating = Environment.GetEnvironmentVariable("ADAPTY_UPDATE_SNAPSHOTS") == "1";

            if (updating || !File.Exists(approvedPath))
            {
                File.WriteAllText(approvedPath, actual);

                if (!updating)
                {
                    Assert.Fail(
                        $"No approved snapshot for '{name}'. One was written to {approvedPath}; "
                            + "review it and re-run."
                    );
                }

                return;
            }

            var approved = File.ReadAllText(approvedPath);
            if (approved == actual)
            {
                return;
            }

            var receivedPath = Path.Combine(approvedDirectory, name + "." + Platform + ".received.txt");
            File.WriteAllText(receivedPath, actual);

            Assert.Fail(
                $"Snapshot '{name}' differs from the approved one.\n"
                    + $"  approved: {approvedPath}\n"
                    + $"  received: {receivedPath}\n\n"
                    + Diff(approved, actual)
            );
        }

        private static string Diff(string approved, string actual)
        {
            var approvedLines = approved.Replace("\r\n", "\n").Split('\n');
            var actualLines = actual.Replace("\r\n", "\n").Split('\n');
            var lines = Math.Max(approvedLines.Length, actualLines.Length);
            var report = new System.Text.StringBuilder();
            var shown = 0;

            for (var i = 0; i < lines && shown < 20; i++)
            {
                var left = i < approvedLines.Length ? approvedLines[i] : "<missing>";
                var right = i < actualLines.Length ? actualLines[i] : "<missing>";
                if (left == right)
                {
                    continue;
                }

                report.AppendLine($"  line {i + 1}:");
                report.AppendLine($"    - {left}");
                report.AppendLine($"    + {right}");
                shown++;
            }

            if (shown == 0)
            {
                report.AppendLine("  (only trailing whitespace differs)");
            }

            return report.ToString();
        }

        private static string ResolveProjectDirectory([CallerFilePath] string callerPath = null) =>
            Path.GetDirectoryName(callerPath);
    }
}
