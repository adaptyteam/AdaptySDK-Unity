//
//  AdaptyConverterObject.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Reads loosely typed JSON into plain <see cref="Dictionary{TKey,TValue}"/> and
    /// <see cref="List{T}"/> values, with every number as <see cref="double"/>.
    /// </summary>
    /// <remarks>
    /// Without it Newtonsoft hands back <c>JObject</c> / <c>JArray</c> and <c>long</c>, which would
    /// change what <c>AdaptyRemoteConfig.Dictionary</c> and profile custom attributes give the
    /// caller. Verified to behave the same way on IL2CPP.
    /// </remarks>
    internal sealed class AdaptyConverterObject : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(object)
            || objectType == typeof(IDictionary<string, object>)
            || objectType == typeof(Dictionary<string, object>)
            || objectType == typeof(IList<object>)
            || objectType == typeof(List<object>);

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotSupportedException();

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        ) => Convert(JToken.Load(reader));

        private static object Convert(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    var map = new Dictionary<string, object>();
                    foreach (var property in (JObject)token)
                    {
                        map[property.Key] = Convert(property.Value);
                    }
                    return map;

                case JTokenType.Array:
                    var list = new List<object>();
                    foreach (var item in (JArray)token)
                    {
                        list.Add(Convert(item));
                    }
                    return list;

                case JTokenType.Integer:
                case JTokenType.Float:
                    return token.Value<double>();

                case JTokenType.Boolean:
                    return token.Value<bool>();

                case JTokenType.Null:
                case JTokenType.Undefined:
                    return null;

                default:
                    return token.Value<string>();
            }
        }
    }
}
