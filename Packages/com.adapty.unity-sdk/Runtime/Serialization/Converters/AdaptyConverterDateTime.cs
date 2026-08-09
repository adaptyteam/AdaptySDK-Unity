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
    /// Translates between the two time zones the SDK speaks: the wire is UTC, the public API is
    /// local.
    /// </summary>
    /// <remarks>
    /// This is a decision, not inherited behaviour, and the two directions are mirrors of each
    /// other:
    /// <list type="bullet">
    /// <item>reading turns the contract's UTC string into the same instant as local time, because
    /// the dates the SDK hands back - a subscription's expiry, an access level's activation - are
    /// shown to end users, and <c>expiresAt &gt; DateTime.Now</c> is what an app naturally
    /// writes;</item>
    /// <item>writing takes it back, and reads a <c>DateTimeKind.Unspecified</c> value as local for
    /// the same reason: an app that builds a countdown from <c>new DateTime(2026, 7, 30, 22, 0, 0)</c>
    /// means 22:00 on the user's clock. <c>DateTime.ToUniversalTime</c> resolves it the same
    /// way.</item>
    /// </list>
    /// Neither half can move to <c>DateTimeZoneHandling</c>. Set to <c>Utc</c> it reads correctly
    /// but relabels an unspecified value on write instead of converting it, which would shift every
    /// custom timer by the user's offset.
    /// <para>
    /// What the convention leaves to the app: a local time inside a daylight saving transition is
    /// ambiguous, and a device that changes time zone shows different digits for the same
    /// subscription. Call <see cref="DateTime.ToUniversalTime"/> where an instant is what matters.
    /// </para>
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

            // A reader that recognised the date itself. AdaptyJson.ParseDocument keeps that from
            // happening on the SDK's own paths, and this is the belt: the contract decides the
            // kind, not whichever reader got there first. An unspecified one came from a string the
            // contract writes as UTC.
            if (reader.Value is DateTime alreadyParsed)
            {
                return alreadyParsed.Kind == DateTimeKind.Local
                    ? alreadyParsed
                    : DateTime.SpecifyKind(alreadyParsed, DateTimeKind.Utc).ToLocalTime();
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
