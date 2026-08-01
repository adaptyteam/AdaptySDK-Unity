using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using AdaptySDK.Serialization;
using AdaptySDK.SimpleJSON;
using Newtonsoft.Json;
using NUnit.Framework;

namespace AdaptySDK.GoldenTests
{
    /// <summary>
    /// The Newtonsoft infrastructure of the next package, checked against the behaviour of the
    /// current layer. Confirmed to behave identically on an IL2CPP player — see tests/aot-probe.
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

        [Test]
        public void DatesKeepThePreviousBehaviour()
        {
            const string json = "{\"required_field\":\"r\",\"activated_at\":\"2026-07-30T10:00:00.000Z\"}";

            var viaNewtonsoft = AdaptyJson.Deserialize<Sample>(json).ActivatedAt.Value;
            var viaSimpleJson = JSONNode.Parse(json).AsObject.GetDateTime("activated_at");

            Assert.Multiple(() =>
            {
                Assert.That(viaNewtonsoft, Is.EqualTo(viaSimpleJson));
                Assert.That(viaNewtonsoft.Kind, Is.EqualTo(DateTimeKind.Local));
            });
        }

        [Test]
        public void DatesAreWrittenAsUtcWithMilliseconds()
        {
            var sample = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"activated_at\":\"2026-07-30T10:00:00.000Z\"}"
            );

            var written = AdaptyJson.Serialize(sample);
            var expected = new JSONObject();
            expected.Add("activated_at", sample.ActivatedAt.Value.ToJSONNode());

            Assert.That(written, Does.Contain("\"activated_at\":\"2026-07-30T10:00:00.000Z\""));
            Assert.That(expected.ToString(), Does.Contain("2026-07-30T10:00:00.000Z"));
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

        [Test]
        public void UnknownEnumValueFallsBackWhenTheEnumDeclaresUnknown()
        {
            var sample = AdaptyJson.Deserialize<Sample>(
                "{\"required_field\":\"r\",\"offer_type\":\"brand_new_from_native\"}"
            );

            Assert.That(sample.OfferType, Is.EqualTo(SampleOfferType.Unknown));
        }

        [Test]
        public void UnknownEnumValueStillThrowsWithoutAnUnknownMember() =>
            Assert.Throws<JsonSerializationException>(
                () => AdaptyJson.Deserialize<StrictHolder>("{\"value\":\"brand_new\"}")
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

            var viaNewtonsoft = AdaptyJson.Deserialize<Dictionary<string, object>>(json);
            var viaSimpleJson = JSONNode.Parse(json).GetDictionary();

            Assert.Multiple(() =>
            {
                Assert.That(viaNewtonsoft["n"], Is.EqualTo(viaSimpleJson["n"]).And.TypeOf<double>());
                Assert.That(viaNewtonsoft["flag"], Is.EqualTo(viaSimpleJson["flag"]));
                Assert.That(viaNewtonsoft["nested"], Is.TypeOf<Dictionary<string, object>>());
                Assert.That(viaNewtonsoft["list"], Is.TypeOf<List<object>>());
            });
        }

        /// <summary>
        /// Date-looking strings inside loose payloads must survive as strings.
        /// </summary>
        [Test]
        public void DateLikeStringsInLoosePayloadsStayStrings()
        {
            var parsed = AdaptyJson.Deserialize<Dictionary<string, object>>(
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
