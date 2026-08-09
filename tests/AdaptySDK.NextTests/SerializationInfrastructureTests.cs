using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using AdaptySDK.Serialization;
using AdaptySDK.TestSupport;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The Newtonsoft infrastructure: the resolver, the converters and the settings behind
    /// AdaptyJson. Confirmed to behave identically on an IL2CPP player — see tests/aot-probe.
    /// </summary>
    [TestFixture]
    public class SerializationInfrastructureTests
    {
        [DataContract]
        private class Sample
        {
            [DataMember(Name = "required_field", IsRequired = true)]
            public readonly string RequiredField;

            [DataMember(Name = "optional_field")]
            public readonly string OptionalField;

            [DataMember(Name = "activated_at")]
            public readonly DateTime? ActivatedAt;

            [DataMember(Name = "offer_type")]
            public readonly SampleOfferType OfferType;

            [DataMember(Name = "index")]
            public readonly int Index;
        }

        private enum SampleOfferType
        {
            [EnumMember(Value = "unknown")]
            Unknown = 0,

            [EnumMember(Value = "win_back")]
            WinBack,
        }

        private enum StrictEnum
        {
            [EnumMember(Value = "first")]
            First,
        }

        [DataContract]
        private class StrictHolder
        {
            [DataMember(Name = "value")]
            public readonly StrictEnum Value;
        }

        /// <summary>
        /// Stands in for <c>AdaptyUICreateFlowViewParameters.CustomTimers</c>, the one place an app
        /// hands the SDK a date of its own.
        /// </summary>
        [DataContract]
        private class Timers
        {
            [DataMember(Name = "at")]
            public DateTime At;
        }

        [Test]
        public void ReadonlyFieldsAreAssigned()
        {
            var sample = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"optional_field\":\"o\",\"index\":7}"
            );

            Assert.Multiple(() =>
            {
                Assert.That(sample.RequiredField, Is.EqualTo("r"));
                Assert.That(sample.OptionalField, Is.EqualTo("o"));
                Assert.That(sample.Index, Is.EqualTo(7));
            });
        }

        [Test]
        public void MissingRequiredFieldThrows() =>
            Assert.Throws<JsonSerializationException>(
                () => AdaptyJson.Deserialize<Sample>("{\"optional_field\":\"o\"}")
            );

        /// <summary>
        /// DataMember.IsRequired alone maps to Required.AllowNull, which would let an explicit null
        /// through; AdaptyContractResolver raises it to Required.Always.
        /// </summary>
        [Test]
        public void NullInRequiredFieldThrows() =>
            Assert.Throws<JsonSerializationException>(
                () => AdaptyJson.Deserialize<Sample>("{\"required_field\":null}")
            );

        /// <summary>
        /// The wire is UTC and the public API is local: a date read from a response is the same
        /// instant, expressed on the machine's clock, so an app compares it against
        /// <c>DateTime.Now</c>.
        /// </summary>
        [Test]
        public void DatesReachTheAppAsLocalTime()
        {
            var parsed = AdaptyJson
                .Deserialize<Sample>(
                    "{\"required_field\":\"r\",\"activated_at\":\"2026-07-30T10:00:00.000Z\"}"
                )
                .ActivatedAt.Value;

            Assert.Multiple(() =>
            {
                Assert.That(parsed.Kind, Is.EqualTo(DateTimeKind.Local));
                Assert.That(
                    parsed.ToUniversalTime(),
                    Is.EqualTo(new DateTime(2026, 7, 30, 10, 0, 0, DateTimeKind.Utc))
                );
            });
        }

        /// <summary>
        /// The same invariant on the path production actually uses. A reply from native code is a
        /// document parsed before anything typed is built, and a reader that parses dates while
        /// building the tree would settle their kind before any converter is consulted.
        /// </summary>
        [Test]
        public void DatesReachTheAppAsLocalTimeThroughAResponse()
        {
            var reply = AdaptyResponse.Parse<AdaptyProfile>(
                "{\"success\":" + Snapshots.LoadResponse("profile-full") + "}"
            );

            Assert.That(reply.Error, Is.Null);

            var premium = reply.Value.AccessLevels["premium"];

            Assert.Multiple(() =>
            {
                Assert.That(
                    premium.ActivatedAt.Kind,
                    Is.EqualTo(DateTimeKind.Local),
                    "a date came out of the reply envelope in the wrong zone"
                );
                Assert.That(
                    premium.ActivatedAt.ToUniversalTime(),
                    Is.EqualTo(new DateTime(2026, 1, 15, 9, 30, 0, DateTimeKind.Utc)),
                    "the zone was right and the instant was not"
                );
            });
        }

        /// <summary>
        /// A payload with anything after it is not a payload. The reply from native code is checked
        /// here and nowhere else, so a second document must not be able to hide behind a comment
        /// and leave the first one standing in for the whole thing.
        /// </summary>
        [TestCase("{} {\"a\":1}", TestName = "a second document")]
        [TestCase("{}/* c */{\"a\":1}", TestName = "a second document behind a comment")]
        [TestCase("{} nonsense", TestName = "trailing text")]
        public void ADocumentWithAnythingAfterItIsRejected(string json) =>
            Assert.That(
                () => AdaptyJson.ParseDocument(json),
                Throws.InstanceOf<JsonReaderException>()
            );

        [TestCase("{\"a\":1}", TestName = "well formed")]
        [TestCase("{}/* c */", TestName = "a trailing comment and nothing else")]
        public void ADocumentWithNothingAfterItIsAccepted(string json) =>
            Assert.That(() => AdaptyJson.ParseDocument(json), Throws.Nothing);

        /// <summary>
        /// The converter's belt, tested on its own because nothing else can reach it.
        /// </summary>
        /// <remarks>
        /// <c>AdaptyJson.ParseDocument</c> keeps a reader from settling a date's kind before the
        /// converter sees it, so on the SDK's own paths this branch never runs - which also means
        /// the two tests above stay green whether or not the belt is there, and this one is what
        /// holds it. <c>JToken.Parse</c> is used deliberately: its reader is the one that parses
        /// dates while building the tree.
        /// </remarks>
        [Test]
        public void APreParsedDateIsBroughtBackToTheContract()
        {
            var preParsed = Newtonsoft.Json.Linq.JToken.Parse(
                "{\"required_field\":\"r\",\"activated_at\":\"2026-07-30T10:00:00.000Z\"}"
            );

            Assert.That(
                preParsed["activated_at"].Type,
                Is.EqualTo(Newtonsoft.Json.Linq.JTokenType.Date),
                "sanity: this reader is supposed to have parsed the date already"
            );

            var parsed = preParsed.ToObject<Sample>(AdaptyJson.CreateSerializer()).ActivatedAt.Value;

            Assert.Multiple(() =>
            {
                Assert.That(parsed.Kind, Is.EqualTo(DateTimeKind.Local));
                Assert.That(
                    parsed.ToUniversalTime(),
                    Is.EqualTo(new DateTime(2026, 7, 30, 10, 0, 0, DateTimeKind.Utc))
                );
            });
        }

        /// <summary>
        /// The other half of the same convention: whatever the app hands over goes out as UTC.
        /// </summary>
        /// <remarks>
        /// The unspecified case is the one that matters and the one no other test reaches - it is
        /// how an app writes a custom timer, <c>new DateTime(2026, 7, 30, 22, 0, 0)</c>, and it has
        /// to mean 22:00 on the user's clock rather than 22:00 UTC. A value the SDK itself read is
        /// already local, so it cannot show the difference.
        /// </remarks>
        [Test]
        public void DatesTheAppSuppliesAreWrittenAsUtc()
        {
            var wall = new DateTime(2026, 7, 30, 22, 0, 0);
            var offset = TimeZoneInfo.Local.GetUtcOffset(wall);

            if (offset == TimeSpan.Zero)
            {
                Assert.Ignore(
                    "the host is on UTC, where converting and relabelling agree - "
                        + "run with TZ set to a zone with an offset, as CI does"
                );
            }

            // Not through ToUniversalTime: that is the very call under test, and comparing a value
            // against itself would pass either way.
            var expected = (wall - offset).ToString(
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                CultureInfo.InvariantCulture
            );

            Assert.Multiple(() =>
            {
                Assert.That(wall.Kind, Is.EqualTo(DateTimeKind.Unspecified), "sanity");
                Assert.That(
                    AdaptyJson.Serialize(new Timers { At = wall }),
                    Does.Contain("\"at\":\"" + expected + "\""),
                    "an unspecified date was not read as the user's local clock"
                );
            });
        }

        [Test]
        public void DatesAreWrittenAsUtcWithMilliseconds()
        {
            var sample = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"activated_at\":\"2026-07-30T10:00:00.000Z\"}"
            );

            Assert.That(
                AdaptyJson.Serialize(sample),
                Does.Contain("\"activated_at\":\"2026-07-30T10:00:00.000Z\"")
            );
        }

        [Test]
        public void EnumsUseContractNames()
        {
            var sample = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"offer_type\":\"win_back\"}"
            );

            Assert.That(sample.OfferType, Is.EqualTo(SampleOfferType.WinBack));
            Assert.That(AdaptyJson.Serialize(sample), Does.Contain("\"offer_type\":\"win_back\""));
        }

        /// <summary>
        /// A declared "unknown" catches nothing: where the contract lists the string it is a value
        /// like any other, and a value outside the contract throws whether or not the enum has one.
        /// </summary>
        [Test]
        public void UnknownEnumValueThrowsWhateverTheEnumDeclares()
        {
            var declared = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"offer_type\":\"unknown\"}"
            );
            Assert.That(declared.OfferType, Is.EqualTo(SampleOfferType.Unknown));

            Assert.Throws<JsonSerializationException>(
                () =>
                    AdaptyJson.Deserialize<Sample>(
                        "{\"required_field\":\"r\",\"offer_type\":\"brand_new_from_native\"}"
                    )
            );

            Assert.Throws<JsonSerializationException>(
                () => AdaptyJson.Deserialize<StrictHolder>("{\"value\":\"brand_new\"}")
            );
        }

        /// <summary>
        /// A number is not a value of a string enum, whatever it would map to.
        /// </summary>
        [Test]
        public void NumberInAStringEnumThrows() =>
            Assert.Throws<JsonSerializationException>(
                () => AdaptyJson.Deserialize<StrictHolder>("{\"value\":0}")
            );

        [Test]
        public void MissingEnumFieldBecomesTheZeroMember()
        {
            var sample = AdaptyJson.Deserialize<Sample>("{\"required_field\":\"r\"}");

            Assert.That(sample.OfferType, Is.EqualTo(SampleOfferType.Unknown));
        }

        /// <summary>
        /// Loose JSON has to keep yielding double and nested dictionaries, as SimpleJSON did:
        /// AdaptyRemoteConfig.Dictionary is public API.
        /// </summary>
        [Test]
        public void LooseObjectsMatchTheCurrentShape()
        {
            const string json = "{\"n\":42,\"s\":\"x\",\"flag\":true,\"nested\":{\"k\":1},\"list\":[1,2]}";

            var parsed = AdaptyJson.DeserializeRemoteConfigDictionary(json);

            Assert.Multiple(() =>
            {
                Assert.That(parsed["n"], Is.EqualTo(42d).And.TypeOf<double>());
                Assert.That(parsed["s"], Is.EqualTo("x"));
                Assert.That(parsed["flag"], Is.EqualTo(true));
                Assert.That(parsed["nested"], Is.TypeOf<Dictionary<string, object>>());
                Assert.That(parsed["list"], Is.TypeOf<List<object>>());
            });
        }

        /// <summary>
        /// The other side of that border. The loose converter is not in the shared settings, so a
        /// dictionary read through the ordinary path gets Newtonsoft's own shapes.
        /// </summary>
        [Test]
        public void LooseObjectsOutsideTheirMembersKeepNewtonsoftShapes()
        {
            var parsed = AdaptyJson.Deserialize<Dictionary<string, object>>(
                "{\"n\":42,\"nested\":{\"k\":1},\"list\":[1,2]}"
            );

            Assert.Multiple(() =>
            {
                Assert.That(parsed["n"], Is.TypeOf<long>());
                Assert.That(parsed["nested"], Is.TypeOf<Newtonsoft.Json.Linq.JObject>());
                Assert.That(parsed["list"], Is.TypeOf<Newtonsoft.Json.Linq.JArray>());
            });
        }

        /// <summary>
        /// The second member the converter serves, and the one no fixture can cover: the only
        /// number among the profile fixture's custom attributes is 12.5, which is a double whether
        /// or not the converter runs, and the snapshot prints an integral double and a long alike.
        /// </summary>
        [Test]
        public void ProfileCustomAttributesKeepTheirDoubles()
        {
            var payload = Newtonsoft.Json.Linq.JObject.Parse(
                Snapshots.LoadResponse("profile-minimal")
            );
            payload["custom_attributes"] = Newtonsoft.Json.Linq.JObject.Parse(
                "{\"score\":12,\"name\":\"x\"}"
            );

            var profile = AdaptyJson.Deserialize<AdaptyProfile>(payload.ToString());

            Assert.Multiple(() =>
            {
                Assert.That(profile.CustomAttributes["score"], Is.EqualTo(12d).And.TypeOf<double>());
                Assert.That(profile.CustomAttributes["name"], Is.EqualTo("x"));
            });
        }

        /// <summary>
        /// Date-looking strings inside loose payloads must survive as strings.
        /// </summary>
        [Test]
        public void DateLikeStringsInLoosePayloadsStayStrings()
        {
            var parsed = AdaptyJson.DeserializeRemoteConfigDictionary(
                "{\"released_at\":\"2026-07-30T10:00:00.000Z\"}"
            );

            Assert.That(parsed["released_at"], Is.TypeOf<string>());
        }

        [Test]
        public void NullValuesAreOmittedOnWrite()
        {
            var sample = AdaptyJson.Deserialize<Sample>("{\"required_field\":\"r\"}");

            Assert.That(AdaptyJson.Serialize(sample), Does.Not.Contain("optional_field"));
        }

        /// <summary>
        /// Zero and false must still be written: EmitDefaultValue = false would drop them, which is
        /// why the models do not use it.
        /// </summary>
        [Test]
        public void ZeroIsStillWritten()
        {
            var sample = AdaptyJson.Deserialize<Sample>("{\"required_field\":\"r\",\"index\":0}");

            Assert.That(AdaptyJson.Serialize(sample), Does.Contain("\"index\":0"));
        }

        [Test]
        public void UnknownFieldsFromNewerNativeSdkAreIgnored()
        {
            var sample = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"field_from_the_future\":123}"
            );

            Assert.That(sample.RequiredField, Is.EqualTo("r"));
        }
    }
}
