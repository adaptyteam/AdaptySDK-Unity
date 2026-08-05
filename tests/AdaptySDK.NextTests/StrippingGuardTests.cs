using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Every type the serializer reaches by reflection has to carry <c>[Preserve]</c>. Managed
    /// stripping otherwise removes it, and the failure only shows on a device, the first time a
    /// response carries the type. Asked of the metadata here, so it runs without a Unity build.
    /// </summary>
    [TestFixture]
    public class StrippingGuardTests
    {
        private const string Preserve = "UnityEngine.Scripting.PreserveAttribute";
        private const string DataContract = "System.Runtime.Serialization.DataContractAttribute";
        private const string DataMember = "System.Runtime.Serialization.DataMemberAttribute";

        /// <summary>
        /// Bases whose subclasses a converter constructs by name. They carry no contract attribute
        /// of their own, so they would not be caught by the rules above.
        /// </summary>
        private static readonly string[] PolymorphicRoots =
        {
            "AdaptyCustomAsset",
            "AdaptyInstallationStatus",
            "AdaptyOnboardingsAnalyticsEvent",
            "AdaptyOnboardingsStateUpdatedParams",
            "AdaptyOnboardingsInput",
        };

        [Test]
        public void EveryReflectedTypeIsPreserved()
        {
            using var context = Open(out var package);

            var missing = Reflected(package)
                .Where(type => !IsPreserved(type))
                .Select(type => type.FullName)
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

            Assert.That(
                missing,
                Is.Empty,
                "these are created or read by reflection and would be stripped:\n  "
                    + string.Join("\n  ", missing)
            );
        }

        /// <summary>
        /// A type's <c>[Preserve]</c> does not extend to its methods, so every member the serializer
        /// reaches through one needs its own. Fields are not listed: they survive on the type
        /// attribute alone, as measured on a stripped player.
        /// </summary>
        [Test]
        public void EveryReflectedMemberIsPreserved()
        {
            using var context = Open(out var package);

            var missing = Reflected(package)
                .SelectMany(ReflectedMembers)
                .Where(member => !Has(member, Preserve))
                .Select(member => $"{member.DeclaringType.Name}.{member.Name}")
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

            Assert.That(
                missing,
                Is.Empty,
                "these are read by the serializer through a method and would be stripped:\n  "
                    + string.Join("\n  ", missing)
            );
        }

        /// <summary>
        /// The members a serializer invokes rather than reads directly: a contract property is read
        /// through its getter, and conditional emission is asked for by calling ShouldSerialize.
        /// </summary>
        private static IEnumerable<MemberInfo> ReflectedMembers(Type type)
        {
            const BindingFlags Declared =
                BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                | BindingFlags.DeclaredOnly;

            foreach (var property in type.GetProperties(Declared))
            {
                if (Has(property, DataMember))
                {
                    yield return property;
                }
            }

            foreach (var method in type.GetMethods(Declared))
            {
                if (method.Name.StartsWith("ShouldSerialize", StringComparison.Ordinal))
                {
                    yield return method;
                }
            }
        }

        /// <summary>
        /// A guard that stops recognising what it guards passes silently. If the rules below stop
        /// matching the package, this fails before the test above starts reporting success for the
        /// wrong reason.
        /// </summary>
        [Test]
        public void TheGuardStillRecognisesThePackage()
        {
            using var context = Open(out var package);

            var reflected = Reflected(package).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(reflected.Count, Is.GreaterThan(70), "far fewer types than the package has");

                Assert.That(
                    reflected.SelectMany(ReflectedMembers).Count(),
                    Is.GreaterThan(30),
                    "the member rules no longer match the contract properties they are meant to guard"
                );

                foreach (var name in new[]
                {
                    "AdaptyProfile",
                    "AdaptyFlow",
                    "AdaptyPaywallProduct",
                    "AdaptySubscriptionPeriod",
                    "AdaptyPaymentMode",
                    "AdaptyOnboarding",
                    "AdaptyCustomAssetLocalImageFile",
                    "AdaptyInstallationStatusDetermined",
                    "AdaptyOnboardingsSelectParams",
                    // A numeric-contract enum: it carries no [EnumMember], so an enum rule written
                    // in terms of that attribute would stop seeing it.
                    "AdaptyErrorCode",
                })
                {
                    Assert.That(
                        reflected.Any(type => type.Name == name),
                        Is.True,
                        $"{name} is no longer recognised as a reflection target"
                    );
                }
            });
        }

        /// <summary>
        /// A type the serializer creates from JSON, or whose members it reads.
        /// </summary>
        private static IEnumerable<Type> Reflected(Assembly package) =>
            package
                .GetTypes()
                .Where(type =>
                    Has(type, DataContract)
                    || type.IsEnum
                    || DerivesFromPolymorphicRoot(type)
                );

        private static bool DerivesFromPolymorphicRoot(Type type)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                if (PolymorphicRoots.Contains(current.Name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A nested type is covered by its declaring type's attribute — measured on a stripped
        /// player, where AdaptyOnboarding's private OnboardingBuilder survived on the outer
        /// attribute alone.
        /// </summary>
        private static bool IsPreserved(Type type)
        {
            for (var current = type; current != null; current = current.DeclaringType)
            {
                if (Has(current, Preserve))
                {
                    return true;
                }
            }
            return false;
        }

        // Attributes are matched by name: the assembly is read for metadata only, so the attribute
        // types themselves are never loaded and cannot be compared as Type.
        private static bool Has(MemberInfo member, string attribute) =>
            member.GetCustomAttributesData().Any(data => data.AttributeType.FullName == attribute);

        private static MetadataLoadContext Open(out Assembly package)
        {
            var directory = Path.Combine(
                Path.GetDirectoryName(SourcePath()),
                "..",
                "surface",
                "package",
                "bin",
                "Debug",
                "net8.0"
            );

            // The surface project is a library, so its dependencies are not copied next to it;
            // the test's own output directory has them.
            var assemblies = Directory
                .GetFiles(directory, "*.dll")
                .Concat(Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
                .Concat(
                    Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "*.dll")
                )
                .GroupBy(Path.GetFileName)
                .Select(group => group.First())
                .ToList();

            var context = new MetadataLoadContext(new PathAssemblyResolver(assemblies));
            package = context.LoadFromAssemblyPath(
                Path.Combine(directory, "AdaptySDK.Surface.dll")
            );
            return context;
        }

        private static string SourcePath([CallerFilePath] string path = null) => path;
    }
}
