using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Reports every state in which Newtonsoft.Json is present but the SDK still will not build.
    /// The SDK assembly is gated on the package, so a copy from anywhere else silently drops it,
    /// as does no copy at all; two copies make its types ambiguous. Runs on Editor load, not at
    /// build time, since none of these should reach a build.
    /// </summary>
    [InitializeOnLoad]
    internal static class AdaptyNewtonsoftValidator
    {
        private const string AssemblyName = AdaptyDependencies.NewtonsoftAssembly;
        private const string PackageId = AdaptyDependencies.NewtonsoftId;

        static AdaptyNewtonsoftValidator()
        {
            var found = Loaded();

            if (found.Count == 0)
            {
                Debug.LogError(
                    $"[Adapty] {AssemblyName} is required by the Adapty SDK but is not loaded in this "
                        + $"project, so the SDK is not compiled. Run \"{AdaptyDependencies.MenuPath}\" "
                        + $"to install the \"{PackageId}\" package."
                );
                return;
            }

            if (found.Count > 1)
            {
                Debug.LogError(
                    $"[Adapty] {found.Count} copies of {AssemblyName} are loaded, so its types are "
                        + "ambiguous and compilation against the Adapty SDK may fail unpredictably. "
                        + $"Keep one - preferably the \"{PackageId}\" package - and remove the "
                        + "others:\n  " + string.Join("\n  ", Describe(found))
                );
                return;
            }

            if (AdaptyDependencies.PackageOf(found[0]) != PackageId)
            {
                Debug.LogError(AdaptyDependencies.StandaloneMessage(found[0]));
            }
        }

        private static List<Assembly> Loaded() =>
            AdaptyDependencies.Copies(AssemblyName).ToList();

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
    }
}
