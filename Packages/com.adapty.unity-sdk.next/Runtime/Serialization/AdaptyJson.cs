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
        /// Serializes a request and stamps the method name into it.
        /// </summary>
        /// <remarks>
        /// The method travels as a sibling of the parameters, not as a wrapper around them, so the
        /// request object is flattened into the same object rather than nested under a key.
        /// </remarks>
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
        /// A serializer for the call sites that read a sub-token, such as
        /// <c>JToken.ToObject&lt;T&gt;(serializer)</c>.
        /// </summary>
        /// <remarks>
        /// Created per call rather than shared: a <see cref="JsonSerializer"/> is not documented as
        /// thread-safe, and events can arrive off the main thread. The settings and the contract
        /// resolver behind it are shared, so this stays cheap - the resolver's contract cache is
        /// what actually costs.
        /// </remarks>
        internal static JsonSerializer CreateSerializer() => JsonSerializer.Create(Settings);

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

                // Models expose no public constructor: they are only ever built from a native
                // response, and a private parameterless one keeps that from becoming public API.
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,

                // Collections that the contract omits keep the empty instance from their field
                // initializer, so callers can iterate a profile without null checks - as before.
                // Replace rather than Auto, so a present collection is not appended to that
                // initial instance.
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
