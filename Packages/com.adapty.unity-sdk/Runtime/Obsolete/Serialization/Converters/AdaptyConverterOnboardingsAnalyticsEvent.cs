using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Onboarding analytics events, chosen by the <c>name</c> discriminator.
    /// </summary>
    /// <remarks>
    /// An event the SDK does not know becomes <see cref="AdaptyOnboardingsAnalyticsEventUnknown"/>
    /// carrying the raw name, so a newer native SDK can emit events without breaking the listener.
    /// </remarks>
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    internal sealed class AdaptyConverterOnboardingsAnalyticsEvent : JsonConverter
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
}
