using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Required-value accessors for the hand-written converters.
    /// </summary>
    /// <remarks>
    /// A converter reads its payload itself, so it never goes through
    /// <see cref="AdaptyContractResolver"/> and gets no <c>Required.Always</c> checking. Plain
    /// <c>JToken.Value&lt;T&gt;(key)</c> would turn a missing contract-required key into null or 0
    /// and hand the listener a half-built model; these throw the way the SimpleJSON layer did.
    ///
    /// This is separate from the unknown-discriminator fallback: a value the SDK does not know yet
    /// is forward compatibility, a missing required value is a malformed payload.
    /// </remarks>
    internal static class AdaptyJsonRequire
    {
        internal static JObject Object(JToken node, string key)
        {
            if (node?[key] is not JObject value)
            {
                throw Missing(key);
            }
            return value;
        }

        internal static JArray Array(JToken node, string key)
        {
            if (node?[key] is not JArray value)
            {
                throw Missing(key);
            }
            return value;
        }

        internal static JToken Token(JToken node, string key)
        {
            var value = node?[key];
            if (value is null || value.Type == JTokenType.Null)
            {
                throw Missing(key);
            }
            return value;
        }

        internal static string String(JToken node, string key)
        {
            var value = node?[key];
            if (value is null || value.Type == JTokenType.Null)
            {
                throw Missing(key);
            }
            return value.Value<string>();
        }

        internal static double Double(JToken node, string key)
        {
            var value = node?[key];
            if (value is null || value.Type == JTokenType.Null)
            {
                throw Missing(key);
            }
            return value.Value<double>();
        }

        /// <summary>
        /// The same failure for a key a model can only require conditionally, on the branch its
        /// discriminator selects.
        /// </summary>
        internal static JsonSerializationException Missing(string key) =>
            new JsonSerializationException($"Required property '{key}' not found in JSON.");
    }
}
