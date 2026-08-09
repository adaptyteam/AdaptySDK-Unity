//
//  AdaptyConverterLooseJson.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Reads the JSON the contract does not type into plain
    /// <see cref="Dictionary{TKey,TValue}"/> and <see cref="List{T}"/> values, with every number as
    /// <see cref="double"/>.
    /// </summary>
    /// <remarks>
    /// Newtonsoft would hand back <c>JObject</c> / <c>JArray</c> and <c>long</c>, and the three
    /// payloads this serves are public API that gave a CLR graph of doubles in 3.x. Each reaches it
    /// a different way, because each arrives a different way:
    /// <list type="bullet">
    /// <item><c>AdaptyProfile.CustomAttributes</c> is a member, so
    /// <c>AdaptyContractResolver</c> attaches this converter to it;</item>
    /// <item><c>AdaptyRemoteConfig.Dictionary</c> is a string parsed on demand, through
    /// <c>AdaptyJson.DeserializeRemoteConfigDictionary</c>;</item>
    /// <item>the analytic event's <c>params</c> is a sub-token the dispatcher reads, through
    /// <c>AdaptyJson.CreateSerializerFor</c>, which covers every <c>Required</c> and
    /// <c>Optional</c> rather than that one event.</item>
    /// </list>
    /// It is not in the shared settings, so any other bare <c>object</c> keeps Newtonsoft's own
    /// shapes. All three ask <see cref="CanConvert"/> what counts as loose - it is the only place
    /// the type list is written down. Verified to behave the same way on IL2CPP.
    /// </remarks>
    internal sealed class AdaptyConverterLooseJson : JsonConverter
    {
        /// <summary>
        /// Shared because the three routes to it all have to agree on what "loose" means, and the
        /// converter holds no state.
        /// </summary>
        internal static readonly AdaptyConverterLooseJson Instance = new AdaptyConverterLooseJson();

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
