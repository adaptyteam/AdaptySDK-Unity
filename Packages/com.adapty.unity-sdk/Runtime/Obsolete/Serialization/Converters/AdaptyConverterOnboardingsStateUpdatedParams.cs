using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Onboarding state updates: <c>element_type</c> selects the shape, and for an input element a
    /// nested <c>type</c> selects the value kind.
    /// </summary>
    /// <remarks>
    /// Unknown element types return null, as the previous parser did — an onboarding built with a
    /// newer element must not fail the event.
    /// </remarks>
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    internal sealed class AdaptyConverterOnboardingsStateUpdatedParams : JsonConverter
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
                    var items = new List<AdaptyOnboardingsSelectParams>();
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
}
