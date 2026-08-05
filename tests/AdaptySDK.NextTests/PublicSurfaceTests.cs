using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AdaptySDK.TestSupport;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Pins the package's public API, member by member, against an approved snapshot: the behaviour
    /// snapshots only cover what the tests happen to call. Read for metadata only, since this
    /// project compiles the same type names into itself.
    /// </summary>
    [TestFixture]
    public class PublicSurfaceTests
    {
        [Test]
        public void ThePublicSurfaceIsUnchanged()
        {
            using var context = Open(out var package);

            var surface = string.Join(
                Environment.NewLine,
                Describe(package, "AdaptySDK").OrderBy(member => member, StringComparer.Ordinal)
            );

            Snapshots.Matches("public-surface", surface);
        }

        /// <summary>
        /// Renders every public member as one line, so the comparison is a set difference and the
        /// snapshot reads as a diff.
        /// </summary>
        private static HashSet<string> Describe(Assembly assembly, string root)
        {
            var members = new HashSet<string>(StringComparer.Ordinal);

            foreach (var type in assembly.GetTypes().OrderBy(t => t.FullName, StringComparer.Ordinal))
            {
                if (!IsVisible(type) || !type.Namespace.StartsWith(root, StringComparison.Ordinal))
                {
                    continue;
                }

                members.Add(
                    $"{Access(type)}{Abstractness(type)}{Kind(type)} {type.FullName}{Bases(type)}"
                );

                const BindingFlags flags =
                    BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.DeclaredOnly;

                foreach (var member in type.GetMembers(flags))
                {
                    var line = DescribeMember(type, member);
                    if (line != null)
                    {
                        members.Add(line);
                    }
                }
            }

            return members;
        }

        private static string DescribeMember(Type type, MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo field when IsVisible(field):
                    // An enum's members are its API, and so are their numeric values - a
                    // renumbering is a silent breaking change for anything that persisted one.
                    // The backing value__ field is not a literal and carries nothing.
                    var value = field.IsLiteral ? $" = {field.GetRawConstantValue()}" : string.Empty;
                    return $"{Access(field)}{Static(field.IsStatic)}{Mutability(field)}"
                        + $"{type.FullName}.{field.Name} : {Name(field.FieldType)}{value}";

                case PropertyInfo property when IsVisible(property.GetMethod)
                    || IsVisible(property.SetMethod):
                    return $"{Access(property)}{Static(IsStatic(property))}"
                        + $"{type.FullName}.{property.Name} : {Name(property.PropertyType)}"
                        + $" {{{Accessor("get", property.GetMethod)}{Accessor("set", property.SetMethod)} }}";

                case MethodInfo method when IsVisible(method) && !IsAccessor(type, method):
                    return $"{Access(method)}{Static(method.IsStatic)}{Inheritance(method)}"
                        + $"{type.FullName}.{method.Name}{Arity(method)}({Parameters(method)})"
                        + $" : {Name(method.ReturnType)}";

                case ConstructorInfo constructor when IsVisible(constructor):
                    return $"{Access(constructor)}{type.FullName}.ctor({Parameters(constructor)})";

                case EventInfo declared when IsVisible(declared.AddMethod):
                    return $"{Access(declared.AddMethod)}{Static(declared.AddMethod.IsStatic)}"
                        + $"{type.FullName}.{declared.Name} (event) : {Name(declared.EventHandlerType)}";

                default:
                    return null;
            }
        }

        private static string Access(Type type) =>
            type.IsPublic || type.IsNestedPublic ? "public "
            : type.IsNestedFamORAssem ? "protected internal "
            : "protected ";

        private static string Access(FieldInfo field) =>
            field.IsPublic ? "public "
            : field.IsFamilyOrAssembly ? "protected internal "
            : "protected ";

        private static string Access(MethodBase method) =>
            method.IsPublic ? "public "
            : method.IsFamilyOrAssembly ? "protected internal "
            : "protected ";

        /// <summary>
        /// A property's own accessibility is the widest of its accessors.
        /// </summary>
        private static string Access(PropertyInfo property)
        {
            var accessors = new[] { property.GetMethod, property.SetMethod }
                .Where(a => a != null)
                .ToList();

            return accessors.Any(a => a.IsPublic) ? "public "
                : accessors.Any(a => a.IsFamilyOrAssembly) ? "protected internal "
                : "protected ";
        }

        private static bool IsVisible(FieldInfo field) =>
            field.IsPublic || field.IsFamily || field.IsFamilyOrAssembly;

        private static bool IsStatic(PropertyInfo property) =>
            (property.GetMethod ?? property.SetMethod).IsStatic;

        private static string Static(bool isStatic) => isStatic ? "static " : string.Empty;

        private static string Mutability(FieldInfo field) =>
            field.IsLiteral ? "const "
            : field.IsInitOnly ? "readonly "
            : string.Empty;

        /// <summary>
        /// Losing a setter, or narrowing one to protected, breaks callers as surely as losing the
        /// property.
        /// </summary>
        private static string Accessor(string name, MethodInfo accessor)
        {
            if (!IsVisible(accessor))
            {
                return string.Empty;
            }

            return accessor.IsPublic ? $" {name};"
                : accessor.IsFamilyOrAssembly ? $" protected internal {name};"
                : $" protected {name};";
        }

        private static string Inheritance(MethodInfo method)
        {
            if (method.IsAbstract)
            {
                return "abstract ";
            }

            if (!method.IsVirtual)
            {
                return string.Empty;
            }

            // GetBaseDefinition is unavailable under a MetadataLoadContext, so the slot is read
            // from the metadata flag instead: NewSlot means the method introduces one.
            var introduces = method.Attributes.HasFlag(MethodAttributes.NewSlot);
            return introduces ? "virtual "
                : method.IsFinal ? "sealed override "
                : "override ";
        }

        private static string Arity(MethodInfo method) =>
            method.IsGenericMethodDefinition
                ? "<" + string.Join(", ", method.GetGenericArguments().Select(Constraint)) + ">"
                : string.Empty;

        private static string Constraint(Type parameter)
        {
            var constraints = parameter
                .GetGenericParameterConstraints()
                .Select(Name)
                .ToList();

            var attributes = parameter.GenericParameterAttributes;
            if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
            {
                constraints.Insert(0, "class");
            }
            if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
            {
                constraints.Insert(0, "struct");
            }
            if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
            {
                constraints.Add("new()");
            }

            return constraints.Count == 0
                ? parameter.Name
                : $"{parameter.Name} : {string.Join(", ", constraints)}";
        }

        private static bool IsAccessor(Type type, MethodInfo method) =>
            type.GetProperties(
                    BindingFlags.Public
                        | BindingFlags.NonPublic
                        | BindingFlags.Instance
                        | BindingFlags.Static
                        | BindingFlags.DeclaredOnly
                )
                .Any(p => p.GetMethod == method || p.SetMethod == method)
            || type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Any(e => e.AddMethod == method || e.RemoveMethod == method);

        // protected internal is externally visible too: a type in another assembly can derive and
        // reach it, so narrowing one is a breaking change like any other.
        private static bool IsVisible(Type type) =>
            type.IsPublic || type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamORAssem;

        private static bool IsVisible(MethodBase method) =>
            method != null && (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly);

        /// <summary>
        /// A model that stops being abstract, or loses a base type an app switched on, is a
        /// breaking change the member lists alone would not show.
        /// </summary>
        private static string Abstractness(Type type)
        {
            if (type.IsEnum || type.IsInterface || type.IsValueType)
            {
                return string.Empty;
            }

            return type.IsAbstract && type.IsSealed ? "static "
                : type.IsAbstract ? "abstract "
                : type.IsSealed ? "sealed "
                : string.Empty;
        }

        private static string Bases(Type type)
        {
            var bases = new List<string>();

            if (type.BaseType != null && type.BaseType.Name != "Object" && !type.IsEnum)
            {
                bases.Add(Name(type.BaseType));
            }

            bases.AddRange(type.GetInterfaces().Select(Name).OrderBy(x => x, StringComparer.Ordinal));

            return bases.Count == 0 ? string.Empty : " : " + string.Join(", ", bases);
        }

        private static string Kind(Type type) =>
            type.IsEnum ? "enum"
            : type.IsInterface ? "interface"
            : type.IsValueType ? "struct"
            : "class";

        private static string Parameters(MethodBase method) =>
            string.Join(", ", method.GetParameters().Select(Parameter));

        /// <summary>
        /// Direction and default matter: adding a default is source-compatible but removing one is
        /// not, and neither is turning a value parameter into a ref.
        /// </summary>
        private static string Parameter(ParameterInfo parameter)
        {
            var direction =
                parameter.IsOut ? "out "
                : parameter.ParameterType.IsByRef ? (parameter.IsIn ? "in " : "ref ")
                : string.Empty;

            var type = parameter.ParameterType.IsByRef
                ? Name(parameter.ParameterType.GetElementType())
                : Name(parameter.ParameterType);

            var optional = parameter.HasDefaultValue
                ? " = " + (parameter.RawDefaultValue?.ToString() ?? "null")
                : string.Empty;

            return $"{direction}{type} {parameter.Name}{optional}";
        }

        /// <summary>
        /// Type names without assembly identity: the package is read from a second, metadata-only
        /// assembly, so its types would never compare equal to this project's by qualified name.
        /// </summary>
        private static string Name(Type type)
        {
            // A type parameter is named by itself: it belongs to the signature, not to a namespace.
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            // A nested generic's own name carries no arity marker, and a constructed type built
            // from an outer type's parameters has none of its own.
            if (type.IsGenericType && type.Name.IndexOf('`') >= 0)
            {
                var name = type.Name.Substring(0, type.Name.IndexOf('`'));
                var arguments = string.Join(", ", type.GetGenericArguments().Select(Name));
                return $"{Namespace(type)}{name}<{arguments}>";
            }

            if (type.IsArray)
            {
                return Name(type.GetElementType()) + "[]";
            }

            return Namespace(type) + type.Name;
        }

        private static string Namespace(Type type) =>
            string.IsNullOrEmpty(type.Namespace) ? string.Empty : type.Namespace + ".";

        private static string Built(string project) =>
            Path.Combine(ProjectDirectory(), "..", "surface", project, "bin", "Debug", "net8.0");

        private static MetadataLoadContext Open(out Assembly package)
        {
            var built = Built("package");

            var assemblies = Directory
                .GetFiles(built, "*.dll")
                .Concat(
                    Directory.GetFiles(Path.GetDirectoryName(typeof(object).Assembly.Location), "*.dll")
                )
                .GroupBy(Path.GetFileName)
                .Select(group => group.First())
                .ToList();

            var context = new MetadataLoadContext(new PathAssemblyResolver(assemblies));
            package = context.LoadFromAssemblyPath(Path.Combine(built, "AdaptySDK.Surface.dll"));
            return context;
        }

        private static string ProjectDirectory([CallerFilePath] string callerPath = null) =>
            Path.GetDirectoryName(callerPath);
    }
}
