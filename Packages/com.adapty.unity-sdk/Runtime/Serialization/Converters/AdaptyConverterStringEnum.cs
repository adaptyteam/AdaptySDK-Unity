using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Writes enums with stock <see cref="StringEnumConverter"/> and reads them back exactly:
    /// a value is one of the contract's own strings or it is not a value at all.
    /// </summary>
    /// <remarks>
    /// Two departures from stock, and both are load-bearing.
    /// <see cref="StringEnumConverter"/> claims every enum, while the ones without
    /// <see cref="EnumMemberAttribute"/> names are numeric in the contract -
    /// <see cref="AdaptyErrorCode"/> carries the native error code and
    /// <see cref="AppTrackingTransparencyStatus"/> the ATT raw value - so
    /// <see cref="CanConvert"/> leaves those to the default numeric handling. And stock reading is
    /// lenient in three ways the contract does not allow: it matches the C# member name as well as
    /// the <see cref="EnumMemberAttribute"/> one, ignores case, and trims the value. Reading is
    /// therefore an ordinal lookup here rather than a call to the base.
    /// </remarks>
    internal sealed class AdaptyConverterStringEnum : StringEnumConverter
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, object>> Cache =
            new ConcurrentDictionary<Type, Dictionary<string, object>>();

        internal AdaptyConverterStringEnum()
        {
            AllowIntegerValues = false;
        }

        public override bool CanConvert(Type objectType)
        {
            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;

            return type.IsEnum && ContractNames(type).Count > 0;
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var type = Nullable.GetUnderlyingType(objectType) ?? objectType;

            if (reader.TokenType == JsonToken.String
                && ContractNames(type).TryGetValue((string)reader.Value, out var known))
            {
                return known;
            }

            throw new JsonSerializationException(
                $"{type.Name} unknown value: {reader.Value ?? "null"}"
            );
        }

        private static Dictionary<string, object> ContractNames(Type type) =>
            Cache.GetOrAdd(type, Build);

        private static Dictionary<string, object> Build(Type type)
        {
            var names = new Dictionary<string, object>(StringComparer.Ordinal);

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null)
                {
                    names[attribute.Value] = field.GetValue(null);
                }
            }

            return names;
        }
    }
}
