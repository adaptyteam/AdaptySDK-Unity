using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Reads the models' <c>System.Runtime.Serialization</c> attributes, with two corrections:
    /// <c>IsRequired</c> means present and non-null on both sides of the wire, not
    /// <see cref="Required.AllowNull"/>; and interface-typed collections are contracted as concrete
    /// ones, see <see cref="Concrete"/>.
    /// </summary>
    /// <remarks>
    /// Both are contract rules that cannot be stated per member without repeating them across the
    /// 128 required members the models declare, or without a backing collection per interface-typed
    /// one. Anything a model can say about itself belongs in the model — this is not the place for a
    /// convention of the SDK's own.
    /// </remarks>
    internal sealed class AdaptyContractResolver : DefaultContractResolver
    {
        internal static readonly AdaptyContractResolver Instance = new AdaptyContractResolver();

        protected override JsonContract CreateContract(Type objectType) => base.CreateContract(Concrete(objectType));

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
                property.NullValueHandling = NullValueHandling.Include;
            }

            return property;
        }
    }
}
