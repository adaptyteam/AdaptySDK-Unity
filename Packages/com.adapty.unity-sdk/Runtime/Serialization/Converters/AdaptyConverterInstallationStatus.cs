//
//  AdaptyConverterInstallationStatus.cs
//  AdaptySDK
//

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Installation status, chosen by the <c>status</c> discriminator.
    /// </summary>
    internal sealed class AdaptyConverterInstallationStatus : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(AdaptyInstallationStatus);

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotSupportedException();

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

            var node = JObject.Load(reader);
            var status = JsonRequire.String(node, "status");

            switch (status)
            {
                case "not_available":
                    return new AdaptyInstallationStatusNotAvailable();

                case "not_determined":
                    return new AdaptyInstallationStatusNotDetermined();

                case "determined":
                    return new AdaptyInstallationStatusDetermined(
                        JsonRequire
                            .Object(node, "details")
                            .ToObject<AdaptyInstallationDetails>(serializer)
                    );

                default:
                    throw new JsonSerializationException(
                        $"AdaptyInstallationStatus unknown value: {status}"
                    );
            }
        }
    }
}
