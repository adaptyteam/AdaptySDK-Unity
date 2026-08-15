using System;
using System.IO;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace AdaptySDK.TestSupport
{
    /// <summary>
    /// Fixture loading and approval-style assertions. An approved snapshot is the layer's reference
    /// behaviour, so a diff is either a regression or a change worth recording deliberately.
    /// Set ADAPTY_UPDATE_SNAPSHOTS=1 to rewrite the approved files instead of failing.
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
        /// Sorts keys so snapshots compare what was sent, not the order it was written in, and folds
        /// whole-numbered floats to integers - a difference no reader can observe. Fractional values
        /// keep their digits, so real precision drift still shows.
        /// </summary>
        /// <remarks>
        /// Not <c>JToken.Parse</c>, for the reason <c>AdaptyJson.ParseDocument</c> avoids it too: its
        /// reader defaults to <c>DateParseHandling.DateTime</c>, so every ISO string would be parsed
        /// to a <see cref="DateTime"/> and printed back in Newtonsoft's own form. The snapshot would
        /// then record that form rather than the one the SDK writes, and the format
        /// <c>AdaptyConverterDateTime</c> emits - milliseconds and the <c>Z</c> designator included -
        /// could change without moving a single approved file.
        /// </remarks>
        public static string Canonical(string json)
        {
            using (var reader = new Newtonsoft.Json.JsonTextReader(new StringReader(json))
            {
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Double,
            })
            {
                var token = Newtonsoft.Json.Linq.JToken.Load(reader);
                return Sort(token).ToString(Newtonsoft.Json.Formatting.Indented) + "\n";
            }
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

        /// <summary>
        /// Hides the SDK version, so a version bump does not fail unrelated snapshots. That the
        /// field is there is worth pinning; today's number is not.
        /// </summary>
        private static string Normalize(string snapshot) =>
            snapshot.Replace(Adapty.SDKVersion, "<sdk-version>");

        public static void Matches(string name, string actual)
        {
            actual = Normalize(actual);

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
