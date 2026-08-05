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
    /// Reads the models' <c>System.Runtime.Serialization</c> attributes, with three corrections:
    /// <c>IsRequired</c> means present and non-null, not <see cref="Required.AllowNull"/>;
    /// <c>ShouldSerializeX</c> is honoured for non-public members and for fields, which Newtonsoft
    /// skips; and interface-typed collections are contracted as concrete ones, see
    /// <see cref="Concrete"/>.
    /// </summary>
    internal sealed class AdaptyContractResolver : DefaultContractResolver
    {
        internal static readonly AdaptyContractResolver Instance = new AdaptyContractResolver();

        private const string ShouldSerializePrefix = "ShouldSerialize";

        protected override JsonContract CreateContract(Type objectType) =>
            base.CreateContract(Concrete(objectType));

        /// <summary>
        /// The concrete collection to build for an interface-typed member. Newtonsoft would populate
        /// the interface through a <c>CollectionWrapper</c> whose constructor it finds by reflection,
        /// and a stripped IL2CPP player no longer has it - which fails every parse.
        /// </summary>
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
