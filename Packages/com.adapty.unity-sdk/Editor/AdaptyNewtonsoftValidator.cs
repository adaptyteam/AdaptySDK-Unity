//
//  AdaptyNewtonsoftValidator.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AdaptySDK.Editor
{
    /// <summary>
    /// Reports a missing or duplicated Newtonsoft.Json, since either one otherwise surfaces as a
    /// compile error that does not name the cause. Runs on Editor load, not at build time, because
    /// both break compilation and a build hook would never run.
    /// </summary>
    [InitializeOnLoad]
    internal static class AdaptyNewtonsoftValidator
    {
        private const string AssemblyName = "Newtonsoft.Json";
        private const string PackageId = "com.unity.nuget.newtonsoft-json";

        static AdaptyNewtonsoftValidator()
        {
            var found = Loaded();

            if (found.Count == 0)
            {
                Debug.LogError(
                    $"[Adapty] {AssemblyName} is required by the Adapty SDK but is not loaded in this "
                        + $"project. Add the \"{PackageId}\" package to restore it."
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
            }
        }

        private static List<Assembly> Loaded() =>
            AppDomain
                .CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().Name == AssemblyName)
                .ToList();

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
