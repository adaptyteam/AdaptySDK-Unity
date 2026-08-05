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
    /// The single entry point to JSON for the SDK. The settings are immutable and the converters
    /// hold no state, so serialization can run off the main thread.
    /// </summary>
    internal static class AdaptyJson
    {
        private static readonly JsonSerializerSettings Settings = CreateSettings();

        internal static string Serialize(object value) =>
            JsonConvert.SerializeObject(value, Settings);

        internal static T Deserialize<T>(string json) =>
            JsonConvert.DeserializeObject<T>(json, Settings);

        /// <summary>
        /// Serializes one value into the DOM, for a request assembled key by key.
        /// </summary>
        internal static Newtonsoft.Json.Linq.JToken ToNode(object value) =>
            value is null
                ? Newtonsoft.Json.Linq.JValue.CreateNull()
                : Newtonsoft.Json.Linq.JToken.FromObject(value, CreateSerializer());

        /// <summary>
        /// Serializes a request and stamps the method name into it, as a sibling of the parameters
        /// rather than a wrapper around them.
        /// </summary>
        internal static string SerializeRequest(string method, object request)
        {
            var node =
                request is null ? new Newtonsoft.Json.Linq.JObject()
                : request is Newtonsoft.Json.Linq.JObject given ? (Newtonsoft.Json.Linq.JObject)given.DeepClone()
                : Newtonsoft.Json.Linq.JObject.FromObject(request, CreateSerializer());

            node["method"] = method;
            return node.ToString(Formatting.None);
        }

        /// <summary>
        /// A serializer for call sites that read a sub-token. Created per call, since
        /// <see cref="JsonSerializer"/> is not documented as thread-safe; the settings and the
        /// resolver's contract cache behind it are shared, so it stays cheap.
        /// </summary>
        internal static JsonSerializer CreateSerializer() => JsonSerializer.Create(Settings);

        private static JsonSerializerSettings CreateSettings() =>
            new JsonSerializerSettings
            {
                // The manual layer could not emit an explicit null, so neither does this one.
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Keeps date-looking strings in payload_data and custom attributes round-tripping
                // as written instead of becoming DateTime.
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Double,

                Culture = CultureInfo.InvariantCulture,
                TypeNameHandling = TypeNameHandling.None,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,

                // Models are built from native responses only, through a private constructor.
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

                // Omitted collections keep their empty field initializer, so callers iterate without
                // null checks; Replace, so a present one is not appended to it.
                ObjectCreationHandling = ObjectCreationHandling.Replace,

                ContractResolver = AdaptyContractResolver.Instance,
                Converters = new JsonConverter[]
                {
                    new AdaptyDateTimeConverter(),
                    new AdaptyEnumConverter(),
                    new AdaptyObjectConverter(),
                    new AdaptyInstallationStatusConverter(),
                    new AdaptyOnboardingsStateUpdatedParamsConverter(),
                    new AdaptyOnboardingsAnalyticsEventConverter(),
                    new AdaptySubscriptionOfferConverter(),
                    new AdaptyCustomAssetsConverter(),
                },
            };
    }
}
