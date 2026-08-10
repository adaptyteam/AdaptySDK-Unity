using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Flattens the contract's nested <c>offer_identifier</c> onto the model's Identifier/Type
    /// pair.
    /// </summary>
    /// <remarks>
    /// Kept out of the model so it needs no Newtonsoft attribute: the models carry
    /// <c>System.Runtime.Serialization</c> annotations only, which is what makes a later move to
    /// another serializer a matter of replacing converters.
    /// </remarks>
    internal sealed class AdaptyConverterSubscriptionOffer : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(AdaptySubscriptionOffer);

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
            var identity = JsonRequire.Object(node, "offer_identifier");

            // The key is not even looked at off Android: an Android-only value of an unexpected
            // shape must not be able to fail a read that never uses it.
#if UNITY_ANDROID
            var offerTags = node["offer_tags"]?.ToObject<IList<string>>(serializer);
#else
            IList<string> offerTags = null;
#endif

            // Read before the id: the contract requires the id in two of the type's branches only,
            // so which rule applies is not known until the type is.
            var type = JsonRequire
                .Token(identity, "type")
                .ToObject<AdaptySubscriptionOfferType>(serializer);

            var idIsRequired =
                type == AdaptySubscriptionOfferType.Promotional
                || type == AdaptySubscriptionOfferType.WinBack;
#if UNITY_ANDROID
            // The contract marks the id required on the introductory branch for Android as well.
            idIsRequired = idIsRequired || type == AdaptySubscriptionOfferType.Introductory;
#endif

            var identifier = idIsRequired
                ? JsonRequire.String(identity, "id")
                : identity.Value<string>("id");

            return new AdaptySubscriptionOffer(
                identifier,
                type,
                JsonRequire
                    .Array(node, "phases")
                    .ToObject<IList<AdaptySubscriptionPhase>>(serializer),
                offerTags
            );
        }
    }
}
