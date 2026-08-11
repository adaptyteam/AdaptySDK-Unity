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
    /// whichever ones are missing and leaves the rest alone.
    /// </summary>
    internal static class AdaptyDependencies
    {
        internal const string MenuPath = "Adapty SDK/Install Dependencies";

        // Keep in sync with dependencies and peerDependencies in package.json.
        internal const string NewtonsoftId = "com.unity.nuget.newtonsoft-json";
        private const string NewtonsoftVersion = "3.2.2";
        private const string EdmId = "com.google.external-dependency-manager";
        private const string EdmVersion = "1.2.188";

        internal const string NewtonsoftAssembly = "Newtonsoft.Json";
        private const string EdmAssembly = "Google.VersionHandler";

        private static AddAndRemoveRequest m_Request;

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

            var newtonsoft = Copies(NewtonsoftAssembly).FirstOrDefault();
            if (newtonsoft != null && PackageOf(newtonsoft) != NewtonsoftId)
            {
                // Adding the package on top would leave two copies of it, which is its own failure.
                // Nothing else is installed either: the project is in a state the user has to fix
                // first, and saying anything about the other dependencies here would contradict it.
                Debug.LogError(StandaloneMessage(newtonsoft));
                return;
            }

            var missing = Missing().ToArray();
            if (missing.Length == 0)
            {
                Debug.Log("[Adapty] Every dependency is already installed.");
                return;
            }

            // EDM is published on OpenUPM, and a scoped registry has no public API - the project
            // manifest is the only way in.
            if (missing.Any(package => package.StartsWith(EdmId, StringComparison.Ordinal))
                && !EnsureRegistry())
            {
                return;
            }

            Debug.Log($"[Adapty] Installing {string.Join(", ", missing)}...");

            m_Request = Client.AddAndRemove(missing, null);
            EditorApplication.update += Poll;
        }

        private static IEnumerable<string> Missing()
        {
            // The SDK assembly is gated on the package rather than on the assembly, so a copy that
            // came from anywhere else does not make the SDK compile and is reported separately.
            if (!Copies(NewtonsoftAssembly).Any())
            {
                yield return $"{NewtonsoftId}@{NewtonsoftVersion}";
            }

            // EDM has no define constraint, so any copy will do - including the one Google ships as
            // its own .unitypackage under Assets/.
            if (!Copies(EdmAssembly).Any())
            {
                yield return $"{EdmId}@{EdmVersion}";
            }
        }

        internal static IEnumerable<Assembly> Copies(string assemblyName) =>
            AppDomain
                .CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name == assemblyName);

        internal static string PackageOf(Assembly assembly) =>
            PackageInfo.FindForAssembly(assembly)?.name;

        internal static string StandaloneMessage(Assembly assembly) =>
            $"[Adapty] {NewtonsoftAssembly} is in this project, but not as the \"{NewtonsoftId}\" "
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
                Debug.LogError($"[Adapty] Package Manager failed: {request.Error?.message}");
            }
        }

        /// <summary>
        /// Writes the OpenUPM registry into the project manifest. No Resolve afterwards: Package
        /// Manager operations have to run one at a time, and the AddAndRemove that follows reads
        /// the manifest itself.
        /// </summary>
        private static bool EnsureRegistry()
        {
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

            Debug.Log(
                $"[Adapty] Added the \"{AdaptyManifest.RegistryName}\" registry to the project "
                    + "manifest."
            );
            return true;
        }
    }
}
