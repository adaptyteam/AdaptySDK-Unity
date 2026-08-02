//
//  AdaptyEnumConverter.cs
//  AdaptySDK
//

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Maps enums to the strings of the cross-platform contract via <see cref="EnumMemberAttribute"/>.
    /// </summary>
    /// <remarks>
    /// A value the SDK does not know becomes <c>Unknown</c> when the enum declares such a member,
    /// so a new value from a newer native SDK does not fail the whole response. Enums without an
    /// <c>Unknown</c> member still throw.
    ///
    /// Reading and writing are not symmetric. A member without <see cref="EnumMemberAttribute"/>
    /// has no name in the cross-platform contract, so it can be produced by a read but never sent:
    /// <c>AdaptyWebPresentation</c> is read from a user action and also passed to OpenUrl and the
    /// onboarding view, and sending <c>"Unknown"</c> there would be a value the native side has
    /// never heard of. The SimpleJSON writer threw for exactly this case.
    ///
    /// Stateless with a concurrent cache: serialization can run off the main thread, on the
    /// permission round-trip.
    /// </remarks>
    internal sealed class AdaptyEnumConverter : JsonConverter
    {
        private static readonly ConcurrentDictionary<Type, EnumMapping> Cache =
            new ConcurrentDictionary<Type, EnumMapping>();

        public override bool CanConvert(Type objectType)
        {
            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;

            // Enums without EnumMember names are numeric in the contract - AdaptyErrorCode carries
            // the native error code, AppTrackingTransparencyStatus the ATT raw value - and are left
            // to the default numeric handling.
            return type.IsEnum && MappingFor(type).HasContractNames;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var mapping = MappingFor(value.GetType());
            if (!mapping.ToName.TryGetValue(value, out var name))
            {
                throw new JsonSerializationException(
                    $"{value.GetType().Name} cannot be sent as: {value}"
                );
            }

            writer.WriteValue(name);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var mapping = MappingFor(type);
            var text = reader.Value as string;

            if (text != null && mapping.FromName.TryGetValue(text, out var known))
            {
                return known;
            }

            if (mapping.Unknown != null)
            {
                return mapping.Unknown;
            }

            throw new JsonSerializationException(
                $"{type.Name} unknown value: {text ?? reader.Value?.ToString() ?? "null"}"
            );
        }

        private static EnumMapping MappingFor(Type type) => Cache.GetOrAdd(type, Build);

        private static EnumMapping Build(Type type)
        {
            var hasContractNames = false;
            var toName = new Dictionary<object, string>();
            var fromName = new Dictionary<string, object>(StringComparer.Ordinal);
            object unknown = null;

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var value = field.GetValue(null);
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();

                if (attribute != null)
                {
                    hasContractNames = true;
                    toName[value] = attribute.Value;
                    fromName[attribute.Value] = value;
                }

                if (field.Name == "Unknown")
                {
                    unknown = value;
                }
            }

            return new EnumMapping(toName, fromName, unknown, hasContractNames);
        }

        private sealed class EnumMapping
        {
            internal EnumMapping(
                Dictionary<object, string> toName,
                Dictionary<string, object> fromName,
                object unknown,
                bool hasContractNames
            )
            {
                ToName = toName;
                FromName = fromName;
                Unknown = unknown;
                HasContractNames = hasContractNames;
            }

            internal Dictionary<object, string> ToName { get; }

            internal Dictionary<string, object> FromName { get; }

            internal object Unknown { get; }

            internal bool HasContractNames { get; }
        }
    }
}
