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
        /// Builds the DOM for a payload arriving from native code.
        /// </summary>
        /// <remarks>
        /// Not <c>JToken.Parse</c>. Its reader defaults to <c>DateParseHandling.DateTime</c> with
        /// <c>RoundtripKind</c>, so every ISO string becomes a <see cref="DateTime"/> while the tree
        /// is being built - before a converter or a setting has any say. Typed dates would then
        /// reach the app as UTC rather than local, and a date-looking string in an untyped payload
        /// would come back reformatted instead of as it was sent.
        /// </remarks>
        internal static Newtonsoft.Json.Linq.JToken ParseDocument(string json)
        {
            using (var reader = new JsonTextReader(new System.IO.StringReader(json))
            {
                DateParseHandling = DateParseHandling.None,
                FloatParseHandling = FloatParseHandling.Double,
            })
            {
                var document = Newtonsoft.Json.Linq.JToken.Load(reader);

                // What JToken.Parse does after loading, and the reason a truncated payload cannot
                // pass for a whole one. Read to the end rather than once: a comment between two
                // documents would otherwise hide the second, and "{}/* c */{...}" would be taken
                // for "{}".
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.Comment)
                    {
                        throw new JsonReaderException(
                            "Additional text found after the JSON document."
                        );
                    }
                }

                return document;
            }
        }

        /// <summary>
        /// Reads a remote config's <c>data</c>, which is JSON the contract does not describe.
        /// </summary>
        internal static System.Collections.Generic.IDictionary<string, object>
            DeserializeRemoteConfigDictionary(string json)
        {
            var type = typeof(System.Collections.Generic.IDictionary<string, object>);

            using (var reader = new JsonTextReader(new System.IO.StringReader(json)))
            {
                return (System.Collections.Generic.IDictionary<string, object>)
                    CreateSerializerFor(type).Deserialize(reader, type);
            }
        }

        /// <summary>
        /// A serializer for reading one value of a known type, carrying the loose converter when
        /// that type is one the contract leaves untyped.
        /// </summary>
        /// <remarks>
        /// The loose converter is deliberately absent from the shared settings, so an ordinary
        /// <c>Dictionary&lt;string, object&gt;</c> keeps Newtonsoft's own shapes. What must not
        /// happen is a public payload the contract types as a bare object losing the CLR graph it
        /// gave in 3.x - so the decision is made here, once, from the type being asked for, rather
        /// than restated at each call site.
        /// </remarks>
        internal static JsonSerializer CreateSerializerFor(Type type)
        {
            var serializer = CreateSerializer();

            if (AdaptyConverterLooseJson.Instance.CanConvert(type))
            {
                serializer.Converters.Add(AdaptyConverterLooseJson.Instance);
            }

            return serializer;
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
                    new AdaptyConverterDateTime(),
                    new AdaptyConverterStringEnum(),
                    new AdaptyConverterOnboardingsStateUpdatedParams(),
                    new AdaptyConverterOnboardingsAnalyticsEvent(),
                    new AdaptyConverterSubscriptionOffer(),
                    new AdaptyConverterCustomAssets(),
                },
            };
    }
}
