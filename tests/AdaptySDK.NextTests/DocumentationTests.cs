using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Every public type and member of the SDK has to be documented, because IntelliSense is where
    /// a caller reads the contract — not the converter, the YAML or the native SDK.
    /// </summary>
    /// <remarks>
    /// The compiler already knows: `CS1591` names every undocumented public member, and the
    /// surface project turns the documentation file on. It is a warning rather than an error
    /// because two things share that assembly and are not the SDK's surface — the Unity stubs the
    /// suites link in, and the deprecated tree, which by policy is maintained rather than brought
    /// in line. So the check reads the generated file and applies the same exclusions.
    /// </remarks>
    [TestFixture]
    public class DocumentationTests
    {
        [Test]
        public void EveryPublicMemberIsDocumented()
        {
            var summarised = Summarised();

            using var context = Open(out var package);

            var missing = new List<string>();

            foreach (var type in package.GetTypes().Where(IsSurface))
            {
                var name = type.FullName.Replace('+', '.');

                if (!summarised.Contains("T:" + name))
                {
                    missing.Add(name);
                }

                foreach (var member in Members(type))
                {
                    if (!IsSummarised(summarised, name, member))
                    {
                        missing.Add($"{name}.{member.Name}");
                    }
                }
            }

            Assert.That(
                missing.OrderBy(name => name, StringComparer.Ordinal).ToList(),
                Is.Empty,
                "these are public and carry no XML documentation:\n  " + string.Join("\n  ", missing)
            );
        }

        /// <summary>
        /// A check that stops finding the surface would pass whatever the sources said.
        /// </summary>
        [Test]
        public void TheCheckStillSeesTheSurface()
        {
            using var context = Open(out var package);

            var types = package.GetTypes().Where(IsSurface).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(types.Count, Is.GreaterThan(40), "far fewer public types than the SDK has");
                Assert.That(
                    types.SelectMany(Members).Count(),
                    Is.GreaterThan(150),
                    "the member rule no longer matches what it is meant to cover"
                );
                Assert.That(Summarised().Count, Is.GreaterThan(250), "the documentation file is not being read");
            });
        }

        /// <summary>
        /// The SDK's own public types. The Unity stubs stand in for types Unity ships, and the
        /// deprecated tree is exempt by the policy in CLAUDE.md.
        /// </summary>
        private static bool IsSurface(Type type) =>
            (type.IsPublic || (type.IsNestedPublic && IsSurface(type.DeclaringType)))
            && type.Namespace != null
            && type.Namespace.StartsWith("AdaptySDK", StringComparison.Ordinal)
            && !type.GetCustomAttributesData().Any(data =>
                data.AttributeType.FullName == "System.ObsoleteAttribute"
            );

        private static IEnumerable<MemberInfo> Members(Type type)
        {
            const BindingFlags Declared =
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

            foreach (var member in type.GetMembers(Declared))
            {
                if (member is MethodInfo method && (method.IsSpecialName || method.DeclaringType == typeof(object)))
                {
                    continue;
                }

                // A nested type is walked as a type of its own, and the file names it `T:`.
                if (member is TypeInfo)
                {
                    continue;
                }

                if (member is FieldInfo field && field.Name == "value__")
                {
                    continue;
                }

                // A parameterless constructor is usually the implicit one, which has no
                // declaration to document and which CS1591 does not report either. What such a
                // type needs said is said on the type.
                if (member is ConstructorInfo constructor && constructor.GetParameters().Length == 0)
                {
                    continue;
                }

                if (member.GetCustomAttributesData().Any(data =>
                        data.AttributeType.FullName == "System.ObsoleteAttribute"))
                {
                    continue;
                }

                yield return member;
            }
        }

        /// <summary>
        /// Every member the file gives a <c>summary</c> for. An entry is not enough on its own —
        /// a lone <c>param</c> or <c>remarks</c> produces one, and CS1591 is satisfied by it too,
        /// so neither would notice a member that says nothing about itself.
        /// </summary>
        private static HashSet<string> Summarised()
        {
            var entries = Entries();

            var duplicated = entries
                .Where(entry => entry.Elements("summary").Count() > 1)
                .Select(entry => entry.Attribute("name").Value)
                .ToList();

            Assert.That(
                duplicated,
                Is.Empty,
                "these carry more than one summary, so only the first is shown:\n  "
                    + string.Join("\n  ", duplicated)
            );

            return new HashSet<string>(
                entries
                    .Where(entry => entry.Elements("summary").Any(summary =>
                        !string.IsNullOrWhiteSpace(summary.Value)
                    ))
                    .Select(entry => entry.Attribute("name").Value),
                StringComparer.Ordinal
            );
        }

        private static List<XElement> Entries()
        {
            var file = Path.Combine(Output(), "AdaptySDK.Surface.xml");

            Assert.That(
                File.Exists(file),
                Is.True,
                "the surface project stopped emitting its documentation file, so this check would pass on nothing"
            );

            return XDocument.Load(file).Descendants("member").ToList();
        }

        /// <summary>
        /// Whether the file gives this member a summary. A field, property or event is named
        /// exactly; a method carries its parameter list, so it is matched by its name and the
        /// bracket that follows — which is what stops one member standing in for another whose
        /// name merely begins the same way, the failure a plain prefix match allows.
        /// </summary>
        /// <remarks>
        /// Overloads of one name are still covered together: telling them apart means rendering
        /// the parameter types the way the compiler writes them, and what this is here to catch is
        /// a member with nothing said about it at all.
        /// </remarks>
        private static bool IsSummarised(HashSet<string> summarised, string type, MemberInfo member)
        {
            var name = member.Name == ".ctor" ? "#ctor" : member.Name;
            var stem = $"{type}.{name}";

            if (member is MethodBase)
            {
                return summarised.Contains("M:" + stem)
                    || summarised.Any(entry => entry.StartsWith("M:" + stem + "(", StringComparison.Ordinal));
            }

            return summarised.Contains("F:" + stem)
                || summarised.Contains("P:" + stem)
                || summarised.Contains("E:" + stem);
        }

        private static MetadataLoadContext Open(out Assembly package)
        {
            var assemblies = Directory
                .GetFiles(Output(), "*.dll")
                .Concat(Directory.GetFiles(AppContext.BaseDirectory, "*.dll"))
                .Concat(Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "*.dll"))
                .GroupBy(Path.GetFileName)
                .Select(group => group.First())
                .ToList();

            var context = new MetadataLoadContext(new PathAssemblyResolver(assemblies));
            package = context.LoadFromAssemblyPath(Path.Combine(Output(), "AdaptySDK.Surface.dll"));
            return context;
        }

        private static string Output() =>
            Path.Combine(
                Path.GetDirectoryName(SourcePath()),
                "..",
                "surface",
                "package",
                "bin",
                "Debug",
                "net8.0"
            );

        private static string SourcePath([CallerFilePath] string path = null) => path;
    }
}
