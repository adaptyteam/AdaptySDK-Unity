//
//  AdaptyConverterCustomAssets.cs
//  AdaptySDK
//

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Custom assets travel as an array, not as a map: the key the app used becomes the element's
    /// <c>id</c>.
    /// </summary>
    internal sealed class AdaptyConverterCustomAssets : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(Dictionary<string, AdaptyCustomAsset>);

        public override bool CanRead => false;

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        ) => throw new NotSupportedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var assets = (Dictionary<string, AdaptyCustomAsset>)value;

            writer.WriteStartArray();
            foreach (var entry in assets)
            {
                var node = JObject.FromObject(entry.Value, serializer);
                node["id"] = entry.Key;
                node.WriteTo(writer);
            }
            writer.WriteEndArray();
        }
    }
}
