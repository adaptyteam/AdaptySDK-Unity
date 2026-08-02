//
//  AdaptyContractResolver.cs
//  AdaptySDK
//

using System;
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
    /// </remarks>
    internal sealed class AdaptyContractResolver : DefaultContractResolver
    {
        internal static readonly AdaptyContractResolver Instance = new AdaptyContractResolver();

        private const string ShouldSerializePrefix = "ShouldSerialize";

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
