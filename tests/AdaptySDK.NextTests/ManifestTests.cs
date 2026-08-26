using System.Text.Json;
using AdaptySDK.Editor;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The OpenUPM registry is written into the user's Packages/manifest.json by hand, since
    /// scoped registries have no public Package Manager API. A malformed edit breaks Package
    /// Manager for the whole project, so every shape is parsed back with a strict parser -
    /// System.Text.Json rather than Newtonsoft, which accepts trailing commas.
    /// </summary>
    public class ManifestTests
    {
        private const string Url = "https://package.openupm.com";
        private const string Scope = "com.google";

        [Test]
        public void AddsTheSectionWhenThereIsNone()
        {
            var result = AdaptyManifest.AddRegistry(
                "{\n  \"dependencies\": {\n    \"com.unity.ugui\": \"2.0.0\"\n  }\n}\n"
            );

            var root = Parsed(result);
            Assert.That(Registries(root).GetArrayLength(), Is.EqualTo(1));
            Assert.That(Scopes(root, 0), Does.Contain(Scope));
            Assert.That(root.TryGetProperty("dependencies", out _), Is.True);
        }

        [Test]
        public void AddsTheSectionToAnEmptyManifest()
        {
            var root = Parsed(AdaptyManifest.AddRegistry("{}"));

            Assert.That(Registries(root).GetArrayLength(), Is.EqualTo(1));
            Assert.That(Scopes(root, 0), Does.Contain(Scope));
        }

        [Test]
        public void AddsTheEntryToAnEmptyRegistryArray()
        {
            var root = Parsed(
                AdaptyManifest.AddRegistry(
                    "{\n  \"scopedRegistries\": [],\n  \"dependencies\": {}\n}\n"
                )
            );

            Assert.That(Registries(root).GetArrayLength(), Is.EqualTo(1));
            Assert.That(Scopes(root, 0), Does.Contain(Scope));
        }

        [Test]
        public void KeepsRegistriesThatAreAlreadyThere()
        {
            var root = Parsed(
                AdaptyManifest.AddRegistry(
                    "{\n  \"scopedRegistries\": [\n    {\n      \"name\": \"other\",\n"
                        + "      \"url\": \"https://other.example\",\n      \"scopes\": [\n"
                        + "        \"com.other\"\n      ]\n    }\n  ],\n  \"dependencies\": {}\n}\n"
                )
            );

            Assert.That(Registries(root).GetArrayLength(), Is.EqualTo(2));
            Assert.That(
                Registries(root)[1].GetProperty("url").GetString(),
                Is.EqualTo("https://other.example")
            );
        }

        [Test]
        public void AddsTheScopeToAnEmptyScopeArray()
        {
            var root = Parsed(AdaptyManifest.AddRegistry(Registry("[]")));

            Assert.That(Scopes(root, 0), Is.EqualTo(new[] { Scope }));
        }

        [Test]
        public void KeepsScopesThatAreAlreadyThere()
        {
            var root = Parsed(
                AdaptyManifest.AddRegistry(Registry("[\n        \"com.cysharp\"\n      ]"))
            );

            Assert.That(Scopes(root, 0), Is.EquivalentTo(new[] { Scope, "com.cysharp" }));
        }

        [Test]
        public void LeavesAManifestThatAlreadyHasEverything()
        {
            var manifest = Registry($"[\n        \"{Scope}\"\n      ]");

            Assert.That(AdaptyManifest.AddRegistry(manifest), Is.EqualTo(manifest));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("not json at all")]
        [TestCase("{\n  \"scopedRegistries\": \"not an array\"\n}")]
        public void RefusesToEditWhatItCannotParse(string manifest)
        {
            Assert.That(AdaptyManifest.AddRegistry(manifest), Is.Null);
        }

        private static string Registry(string scopes) =>
            "{\n  \"scopedRegistries\": [\n    {\n      \"name\": \"package.openupm.com\",\n"
            + $"      \"url\": \"{Url}\",\n      \"scopes\": {scopes}\n    }}\n  ],\n"
            + "  \"dependencies\": {}\n}\n";

        private static JsonElement Parsed(string manifest)
        {
            Assert.That(manifest, Is.Not.Null, "the edit was refused");
            return JsonDocument.Parse(manifest).RootElement;
        }

        private static JsonElement Registries(JsonElement root) =>
            root.GetProperty("scopedRegistries");

        private static string[] Scopes(JsonElement root, int index)
        {
            var scopes = Registries(root)[index].GetProperty("scopes");
            var result = new string[scopes.GetArrayLength()];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = scopes[i].GetString();
            }

            return result;
        }
    }
}
