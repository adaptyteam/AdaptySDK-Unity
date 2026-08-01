//
//  AdaptyContractResolver.cs
//  AdaptySDK
//

using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Reads the models' <c>System.Runtime.Serialization</c> attributes, with one correction.
    /// </summary>
    /// <remarks>
    /// Newtonsoft maps <c>[DataMember(IsRequired = true)]</c> to <see cref="Required.AllowNull"/>,
    /// which accepts an explicit null. The SimpleJSON layer threw on it, and the native contract
    /// treats a required field as present and non-null, so it is raised to
    /// <see cref="Required.Always"/> here.
    /// </remarks>
    internal sealed class AdaptyContractResolver : DefaultContractResolver
    {
        internal static readonly AdaptyContractResolver Instance = new AdaptyContractResolver();

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

            return property;
        }
    }
}
