//
//  AdaptyConverterDateTime.cs
//  AdaptySDK
//

using System;
using System.Globalization;
using Newtonsoft.Json;

namespace AdaptySDK.Serialization
{
    /// <summary>
    /// Dates in the format the native SDKs use, reproducing the previous behaviour exactly:
    /// written as UTC with milliseconds, read back as local time.
    /// </summary>
    /// <remarks>
    /// Both directions live here rather than in <c>DateTimeZoneHandling</c>, which would also
    /// affect writing and would send local offsets to the native side.
    /// </remarks>
    internal sealed class AdaptyConverterDateTime : JsonConverter
    {
        private const string Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

        public override bool CanConvert(Type objectType) =>
            objectType == typeof(DateTime) || objectType == typeof(DateTime?);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            var moment = (DateTime)value;
            if (moment.Kind != DateTimeKind.Utc)
            {
                moment = moment.ToUniversalTime();
            }

            writer.WriteValue(moment.ToString(Format, CultureInfo.InvariantCulture));
        }

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

            if (reader.Value is DateTime alreadyParsed)
            {
                return alreadyParsed;
            }

            var text = reader.Value as string;
            if (text is null)
            {
                throw new JsonSerializationException(
                    $"Expected a date string, got {reader.TokenType}"
                );
            }

            try
            {
                return DateTime.Parse(text, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new JsonSerializationException($"Failed decoding DateTime from \"{text}\"", e);
            }
        }
    }
}
