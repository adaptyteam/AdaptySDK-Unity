//
//  AdaptyPolymorphicConverters.cs
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
    internal sealed class AdaptyInstallationStatusConverter : JsonConverter
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

    /// <summary>
    /// Onboarding state updates: <c>element_type</c> selects the shape, and for an input element a
    /// nested <c>type</c> selects the value kind.
    /// </summary>
    /// <remarks>
    /// Unknown element types return null, as the previous parser did — an onboarding built with a
    /// newer element must not fail the event.
    /// </remarks>
    internal sealed class AdaptyOnboardingsStateUpdatedParamsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(AdaptyOnboardingsStateUpdatedParams);

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
            var elementType = JsonRequire.String(node, "element_type");

            switch (elementType)
            {
                case "select":
                    return ReadSelect(JsonRequire.Object(node, "value"));

                case "multi_select":
                    var items = new System.Collections.Generic.List<AdaptyOnboardingsSelectParams>();
                    foreach (var item in JsonRequire.Array(node, "value"))
                    {
                        items.Add(ReadSelect(item));
                    }
                    return new AdaptyOnboardingsMultiSelectParams(items);

                case "input":
                    return ReadInput(JsonRequire.Object(node, "value"));

                case "date_picker":
                    var picker = JsonRequire.Object(node, "value");
                    return new AdaptyOnboardingsDatePickerParams(
                        picker.Value<int?>("day"),
                        picker.Value<int?>("month"),
                        picker.Value<int?>("year")
                    );

                default:
                    return null;
            }
        }

        private static AdaptyOnboardingsSelectParams ReadSelect(JToken value) =>
            new AdaptyOnboardingsSelectParams(
                JsonRequire.String(value, "id"),
                JsonRequire.String(value, "value"),
                JsonRequire.String(value, "label")
            );

        private static AdaptyOnboardingsStateUpdatedParams ReadInput(JToken value)
        {
            var type = JsonRequire.String(value, "type");
            switch (type)
            {
                case "text":
                    return new AdaptyOnboardingsInputParams(
                        new AdaptyOnboardingsTextInput(JsonRequire.String(value, "value"))
                    );

                case "email":
                    return new AdaptyOnboardingsInputParams(
                        new AdaptyOnboardingsEmailInput(JsonRequire.String(value, "value"))
                    );

                case "number":
                    return new AdaptyOnboardingsInputParams(
                        new AdaptyOnboardingsNumberInput(JsonRequire.Double(value, "value"))
                    );

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// Onboarding analytics events, chosen by the <c>name</c> discriminator.
    /// </summary>
    /// <remarks>
    /// An event the SDK does not know becomes <see cref="AdaptyOnboardingsAnalyticsEventUnknown"/>
    /// carrying the raw name, so a newer native SDK can emit events without breaking the listener.
    /// </remarks>
    internal sealed class AdaptyOnboardingsAnalyticsEventConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(AdaptyOnboardingsAnalyticsEvent);

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
            var name = JsonRequire.String(node, "name");

            switch (name)
            {
                case "onboarding_started":
                    return new AdaptyOnboardingsAnalyticsEventOnboardingStarted();

                case "screen_presented":
                    return new AdaptyOnboardingsAnalyticsEventScreenPresented();

                case "screen_completed":
                    return new AdaptyOnboardingsAnalyticsEventScreenCompleted(
                        node.Value<string>("element_id"),
                        node.Value<string>("reply")
                    );

                case "second_screen_presented":
                    return new AdaptyOnboardingsAnalyticsEventSecondScreenPresented();

                case "registration_screen_presented":
                    return new AdaptyOnboardingsAnalyticsEventRegistrationScreenPresented();

                case "products_screen_presented":
                    return new AdaptyOnboardingsAnalyticsEventProductsScreenPresented();

                case "user_email_collected":
                    return new AdaptyOnboardingsAnalyticsEventUserEmailCollected();

                case "onboarding_completed":
                    return new AdaptyOnboardingsAnalyticsEventOnboardingCompleted();

                default:
                    return new AdaptyOnboardingsAnalyticsEventUnknown(name);
            }
        }
    }

    /// <summary>
    /// Custom assets travel as an array, not as a map: the key the app used becomes the element's
    /// <c>id</c>.
    /// </summary>
    internal sealed class AdaptyCustomAssetsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(System.Collections.Generic.Dictionary<string, AdaptyCustomAsset>);

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

            var assets = (System.Collections.Generic.Dictionary<string, AdaptyCustomAsset>)value;

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
