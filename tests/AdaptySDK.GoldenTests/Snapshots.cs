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
