//
//  AdaptyManifest.cs
//  AdaptySDK
//

using System;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Edits Packages/manifest.json as text. A scoped registry has no public Package Manager API,
    /// and no JSON library can be assumed present - Newtonsoft is exactly what may be missing.
    /// Free of Unity types so that the tests can run it.
    /// </summary>
    internal static class AdaptyManifest
    {
        internal const string RegistryName = "package.openupm.com";
        internal const string RegistryUrl = "https://package.openupm.com";
        internal const string RegistryScope = "com.google";

        private const string Registries = "\"scopedRegistries\"";
        private const string Scopes = "\"scopes\"";

        private const string Entry =
            "    {\n"
            + "      \"name\": \"" + RegistryName + "\",\n"
            + "      \"url\": \"" + RegistryUrl + "\",\n"
            + "      \"scopes\": [\n"
            + "        \"" + RegistryScope + "\"\n"
            + "      ]\n"
            + "    }";

        /// <summary>
        /// Returns the manifest with the registry and the scope in place, the manifest unchanged
        /// when both are already there, and null when it is shaped in a way this will not edit -
        /// the caller writes nothing in that case.
        /// </summary>
        internal static string AddRegistry(string manifest)
        {
            if (string.IsNullOrEmpty(manifest))
            {
                return null;
            }

            var url = manifest.IndexOf(RegistryUrl, StringComparison.Ordinal);
            if (url >= 0)
            {
                return AddScope(manifest, url);
            }

            var registries = manifest.IndexOf(Registries, StringComparison.Ordinal);
            return registries >= 0 ? AddEntry(manifest, registries) : AddSection(manifest);
        }

        private static string AddEntry(string manifest, int registries)
        {
            var open = manifest.IndexOf('[', registries);
            if (open < 0)
            {
                return null;
            }

            var close = manifest.IndexOf(']', open);
            if (close < 0)
            {
                return null;
            }

            return manifest.Insert(
                open + 1,
                Blank(manifest, open + 1, close) ? $"\n{Entry}\n  " : $"\n{Entry},"
            );
        }

        private static string AddSection(string manifest)
        {
            var open = manifest.IndexOf('{');
            var close = manifest.LastIndexOf('}');
            if (open < 0 || close < open)
            {
                return null;
            }

            var section = $"\n  {Registries}: [\n{Entry}\n  ]";
            return manifest.Insert(
                open + 1,
                Blank(manifest, open + 1, close) ? $"{section}\n" : $"{section},"
            );
        }

        private static string AddScope(string manifest, int url)
        {
            var open = manifest.LastIndexOf('{', url);
            var close = manifest.IndexOf('}', url);
            if (open < 0 || close < 0)
            {
                return null;
            }

            var scopes = manifest.IndexOf(Scopes, open, StringComparison.Ordinal);
            if (scopes < 0 || scopes > close)
            {
                return null;
            }

            var arrayOpen = manifest.IndexOf('[', scopes);
            if (arrayOpen < 0 || arrayOpen > close)
            {
                return null;
            }

            var arrayClose = manifest.IndexOf(']', arrayOpen);
            if (arrayClose < 0)
            {
                return null;
            }

            var body = manifest.Substring(arrayOpen + 1, arrayClose - arrayOpen - 1);
            if (body.Contains($"\"{RegistryScope}\""))
            {
                return manifest;
            }

            return manifest.Insert(
                arrayOpen + 1,
                body.Trim().Length == 0
                    ? $"\n        \"{RegistryScope}\"\n      "
                    : $"\n        \"{RegistryScope}\","
            );
        }

        /// <summary>
        /// Whether a container is empty, which decides the separating comma. Getting this wrong
        /// writes a trailing comma and breaks Package Manager for the whole project.
        /// </summary>
        private static bool Blank(string text, int start, int end)
        {
            for (var index = start; index < end; index++)
            {
                if (!char.IsWhiteSpace(text[index]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
