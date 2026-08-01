//
//  AdaptyJson.cs
//  AdaptySDK
//

using System;
using System.Globalization;
using Newtonsoft.Json;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// The single entry point to JSON for the SDK.
    /// </summary>
    /// <remarks>
    /// Settings are built once and never mutated, and the converters hold no state: serialization
    /// can run off the main thread — the permission round-trip answers from an OS callback.
    /// </remarks>
    internal static class AdaptyJson
    {
        private static readonly JsonSerializerSettings Settings = CreateSettings();

        private static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

        internal static string Serialize(object value) =>
            JsonConvert.SerializeObject(value, Settings);

        internal static T Deserialize<T>(string json) =>
            JsonConvert.DeserializeObject<T>(json, Settings);

        internal static JsonSerializer SharedSerializer => Serializer;

        private static JsonSerializerSettings CreateSettings() =>
            new JsonSerializerSettings
            {
                // The manual layer emitted a key only when the value was non-null; Add(key, null)
                // threw, so nothing could reach the native side as an explicit null.
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Without this, date-looking strings inside payload_data and custom attributes
                // would be turned into DateTime and no longer round-trip as written.
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Double,

                Culture = CultureInfo.InvariantCulture,
                TypeNameHandling = TypeNameHandling.None,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,

                ContractResolver = AdaptyContractResolver.Instance,
                Converters = new JsonConverter[]
                {
                    new AdaptyDateTimeConverter(),
                    new AdaptyEnumConverter(),
                    new AdaptyObjectConverter(),
                },
            };
    }
}
