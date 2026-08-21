using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Installs the packages the SDK needs from Package Manager. A .unitypackage carries assets
    /// only and cannot touch the project manifest, so nothing else can bring them in. Installs
    /// whichever ones are missing, and upgrades an External Dependency Manager older than the SDK
    /// needs; a copy Package Manager does not describe is reported rather than replaced.
    /// </summary>
    internal static class AdaptyDependencies
    {
        internal const string MenuPath = "Adapty SDK/Install Dependencies";

        internal const string NewtonsoftAssembly = "Newtonsoft.Json";
        private const string EdmAssembly = "Google.VersionHandler";

        private static AddAndRemoveRequest m_Request;

        // Whether the run behind m_Request had to write the registry into the manifest first -
        // the one failure mode where trying again is itself the fix.
        private static bool m_RegistryJustWritten;

        /// <summary>
        /// The Editor assembly is out of reach of the runtime resets, and with Domain Reload
        /// disabled a subscription outlives entering Play Mode.
        /// </summary>
        /// <remarks>
        /// Unity passes <see cref="EnterPlayModeOptions"/> to a callback with this attribute, so
        /// the signature is not optional — a parameterless one is simply never called.
        /// </remarks>
        /// <param name="options">What this run of Play Mode is reloading, if anything.</param>
        [InitializeOnEnterPlayMode]
        private static void ResetInstallState(EnterPlayModeOptions options)
        {
            // A request in flight is an Editor operation, not something belonging to the session
            // being entered: `m_Request` is also the only guard against starting a second
            // `AddAndRemove` over the first, so dropping it here would let the menu item do
            // exactly that. `Poll` unsubscribes itself once the request completes.
            if (m_Request != null && !m_Request.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= Poll;
            m_Request = null;
        }

        [MenuItem(MenuPath)]
        private static void Install()
        {
            if (m_Request != null && !m_Request.IsCompleted)
            {
                Debug.Log("[Adapty] Already installing, waiting for Package Manager.");
                return;
            }

            // Materialized once: the order of GetAssemblies() is not specified, so asking twice -
            // or looking only at the first copy - decides the same project differently from run to
            // run. Every state the validator calls an error has to stop the install too, or the
            // menu item reports success over a project that will not compile.
            var newtonsoft = Copies(NewtonsoftAssembly).ToList();

            if (newtonsoft.Count > 1)
            {
                Debug.LogError(DuplicateMessage(newtonsoft, "Nothing was installed."));
                return;
            }

            if (newtonsoft.Count == 1 && PackageOf(newtonsoft[0]) != AdaptyDependencyPlan.NewtonsoftId)
            {
                // Adding the package on top would leave two copies of it, which is its own failure.
                // Nothing else is installed either: the project is in a state the user has to fix
                // first, and saying anything about the other dependencies here would contradict it.
                Debug.LogError(StandaloneMessage(newtonsoft[0]));
                return;
            }

            var edm = EdmInstalled(out var edmVersion);
            var missing = AdaptyDependencyPlan.Missing(newtonsoft.Count > 0, edm, edmVersion).ToArray();

            // Said whether or not anything is installed: a copy whose version cannot be read is
            // the one case where "everything is installed" would be a guess rather than a fact.
            var caution = AdaptyDependencyPlan.EdmCaution(edm, edmVersion);
            if (caution != null)
            {
                Debug.LogWarning(caution);
            }

            if (missing.Length == 0)
            {
                if (caution == null)
                {
                    Debug.Log("[Adapty] Every dependency is already installed.");
                }

                return;
            }

            // EDM is published on OpenUPM, and a scoped registry has no public API - the project
            // manifest is the only way in.
            m_RegistryJustWritten = false;
            if (missing.Any(package =>
                    package.StartsWith(AdaptyDependencyPlan.EdmId, StringComparison.Ordinal)
                )
                && !EnsureRegistry(out m_RegistryJustWritten))
            {
                return;
            }

            Debug.Log($"[Adapty] Installing {string.Join(", ", missing)}...");

            m_Request = Client.AddAndRemove(missing, null);
            EditorApplication.update += Poll;
        }

        /// <summary>
        /// Which copy of External Dependency Manager the project has, and the version when that is
        /// something Package Manager can answer.
        /// </summary>
        /// <remarks>
        /// More than one copy counts as unmanaged: no single version answers for the project, and
        /// the order <c>GetAssemblies()</c> returns is not specified, so picking one would decide
        /// the same project differently from run to run.
        /// </remarks>
        private static AdaptyEdmSource EdmInstalled(out string version)
        {
            version = null;

            var copies = Copies(EdmAssembly).ToList();

            if (copies.Count == 0)
            {
                return AdaptyEdmSource.None;
            }

            if (copies.Count > 1)
            {
                return AdaptyEdmSource.Unmanaged;
            }

            var info = PackageInfo.FindForAssembly(copies[0]);

            if (info?.name != AdaptyDependencyPlan.EdmId)
            {
                return AdaptyEdmSource.Unmanaged;
            }

            version = info.version;
            return AdaptyEdmSource.Package;
        }

        internal static IEnumerable<Assembly> Copies(string assemblyName) =>
            AppDomain
                .CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name == assemblyName);

        internal static string PackageOf(Assembly assembly) =>
            PackageInfo.FindForAssembly(assembly)?.name;

        /// <summary>
        /// The one wording for two copies, shared by the validator that reports the state and the
        /// installer that refuses to add to it.
        /// </summary>
        /// <param name="copies">Every loaded copy, listed so the user can tell them apart.</param>
        /// <param name="andThen">
        /// What the caller did about it, placed before the list rather than after it - the list
        /// ends in a file path, and a sentence trailing that is unreadable.
        /// </param>
        internal static string DuplicateMessage(IReadOnlyList<Assembly> copies, string andThen = null) =>
            $"[Adapty] {copies.Count} copies of {NewtonsoftAssembly} are loaded, so its types are "
            + "ambiguous and compilation against the Adapty SDK may fail unpredictably. "
            + (andThen is null ? "" : andThen + " ")
            + $"Keep one - preferably the \"{AdaptyDependencyPlan.NewtonsoftId}\" package - and remove the others:\n  "
            + string.Join("\n  ", Describe(copies));

        /// <summary>
        /// Where each copy came from, since the name alone does not distinguish them.
        /// </summary>
        private static IEnumerable<string> Describe(IEnumerable<Assembly> assemblies) =>
            assemblies.Select(assembly =>
            {
                var name = assembly.GetName();
                string location;
                try
                {
                    location = string.IsNullOrEmpty(assembly.Location)
                        ? "(no file on disk)"
                        : assembly.Location;
                }
                catch (NotSupportedException)
                {
                    location = "(location unavailable)";
                }

                return $"{name.Name} {name.Version} - {location}";
            });

        internal static string StandaloneMessage(Assembly assembly) =>
            $"[Adapty] {NewtonsoftAssembly} is in this project, but not as the \"{AdaptyDependencyPlan.NewtonsoftId}\" "
            + "package, and the Adapty SDK only compiles against that package. Nothing was "
            + $"installed. Remove the copy at {Where(assembly)}, then run \"{MenuPath}\" again.";

        private static string Where(Assembly assembly)
        {
            try
            {
                return string.IsNullOrEmpty(assembly.Location)
                    ? "(no file on disk)"
                    : assembly.Location;
            }
            catch (NotSupportedException)
            {
                return "(location unavailable)";
            }
        }

        private static void Poll()
        {
            if (m_Request == null || !m_Request.IsCompleted)
            {
                return;
            }

            EditorApplication.update -= Poll;

            var request = m_Request;
            m_Request = null;

            if (request.Status == StatusCode.Success)
            {
                Debug.Log("[Adapty] Dependencies installed.");
            }
            else
            {
                Debug.LogError(
                    $"[Adapty] Package Manager failed: {request.Error?.message}"
                        + (m_RegistryJustWritten
                            ? $" The \"{AdaptyManifest.RegistryName}\" registry was only just "
                                + $"added to the project manifest, so run \"{MenuPath}\" again - "
                                + "on the second run it is already there."
                            : "")
                );
            }
        }

        /// <summary>
        /// Writes the OpenUPM registry into the project manifest. No Resolve afterwards: Package
        /// Manager operations have to run one at a time, and on Unity 6000.4 the AddAndRemove
        /// that follows was observed to read the just-written manifest. An observation, not a
        /// guarantee - which is why a failure right after a write asks for a second run.
        /// </summary>
        /// <param name="wrote">Whether this call changed the manifest on disk.</param>
        private static bool EnsureRegistry(out bool wrote)
        {
            wrote = false;
            var path = Path.GetFullPath(
                Path.Combine(Application.dataPath, "..", "Packages", "manifest.json")
            );

            string manifest;
            try
            {
                manifest = File.ReadAllText(path);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                Debug.LogError($"[Adapty] Could not read {path}: {error.Message}");
                return false;
            }

            var updated = AdaptyManifest.AddRegistry(manifest);

            if (updated == null)
            {
                Debug.LogError(
                    $"[Adapty] Could not add the \"{AdaptyManifest.RegistryName}\" registry to "
                        + $"{path}. Add it by hand, with \"{AdaptyManifest.RegistryScope}\" among "
                        + $"its scopes, and run \"{MenuPath}\" again."
                );
                return false;
            }

            if (updated == manifest)
            {
                return true;
            }

            try
            {
                File.WriteAllText(path, updated);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                Debug.LogError($"[Adapty] Could not write {path}: {error.Message}");
                return false;
            }

            wrote = true;
            Debug.Log(
                $"[Adapty] Added the \"{AdaptyManifest.RegistryName}\" registry to the project "
                    + "manifest."
            );
            return true;
        }
    }
}
