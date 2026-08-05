//
//  AdaptyContractResolver.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Reads the models' <c>System.Runtime.Serialization</c> attributes, with two corrections.
    /// </summary>
    /// <remarks>
    /// Newtonsoft maps <c>[DataMember(IsRequired = true)]</c> to <see cref="Required.AllowNull"/>,
    /// which accepts an explicit null. The SimpleJSON layer threw on it, and the native contract
    /// treats a required field as present and non-null, so it is raised to
    /// <see cref="Required.Always"/> here.
    ///
    /// <c>ShouldSerializeX</c> is also honoured when it is non-public or when X is a field.
    /// Newtonsoft looks the method up with public-only binding flags and only for properties, so
    /// on a model whose state is private readonly fields the convention would silently do nothing
    /// - the members would serialize unconditionally, which is how an empty customer identity or
    /// an empty remote config list would reach the native side.
    ///
    /// A collection declared as an interface is contracted as its concrete counterpart, so that
    /// nothing has to be built by reflection at runtime - see <see cref="Concrete"/>.
    /// </remarks>
    internal sealed class AdaptyContractResolver : DefaultContractResolver
    {
        internal static readonly AdaptyContractResolver Instance = new AdaptyContractResolver();

        private const string ShouldSerializePrefix = "ShouldSerialize";

        protected override JsonContract CreateContract(Type objectType) =>
            base.CreateContract(Concrete(objectType));

        /// <summary>
        /// The concrete collection to build for an interface-typed member.
        /// </summary>
        /// <remarks>
        /// The models expose their collections as <c>IList</c> and <c>IDictionary</c>, which
        /// implement neither non-generic <c>IList</c> nor non-generic <c>IDictionary</c>. Newtonsoft
        /// therefore populates them through a <c>CollectionWrapper</c> whose constructor it looks up
        /// by reflection - and on a stripped IL2CPP player that constructor is gone, so the lookup
        /// returns null and every profile fails to parse with "Value cannot be null. Parameter name:
        /// method". Measured on the simulator; it takes the whole SDK down, since the profile is
        /// what activation delivers first.
        ///
        /// Contracting the concrete type instead sidesteps the wrapper entirely: a List and a
        /// Dictionary are assignable to the interfaces the models declare, so nothing above this
        /// changes.
        /// </remarks>
        private static Type Concrete(Type objectType)
        {
            if (!objectType.IsInterface || !objectType.IsGenericType)
            {
                return objectType;
            }

            var definition = objectType.GetGenericTypeDefinition();
            var arguments = objectType.GetGenericArguments();

            if (definition == typeof(IList<>)
                || definition == typeof(ICollection<>)
                || definition == typeof(IEnumerable<>)
                || definition == typeof(IReadOnlyList<>)
                || definition == typeof(IReadOnlyCollection<>))
            {
                return typeof(List<>).MakeGenericType(arguments);
            }

            if (definition == typeof(IDictionary<,>) || definition == typeof(IReadOnlyDictionary<,>))
            {
                return typeof(Dictionary<,>).MakeGenericType(arguments);
            }

            return objectType;
        }

        protected override JsonProperty CreateProperty(
            MemberInfo member,
            MemberSerialization memberSerialization
        )
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.Required == Required.AllowNull)
            {
                property.Required = Required.Always;
            }

            property.ShouldSerialize ??= ShouldSerializeTest(member);

            return property;
        }

        private static Predicate<object> ShouldSerializeTest(MemberInfo member)
        {
            var method = member.DeclaringType.GetMethod(
                ShouldSerializePrefix + member.Name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null
            );

            if (method is null || method.ReturnType != typeof(bool))
            {
                return null;
            }

            return instance => (bool)method.Invoke(instance, null);
        }
    }
}
