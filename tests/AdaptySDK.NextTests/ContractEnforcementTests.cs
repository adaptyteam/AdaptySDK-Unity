using System.Collections.Generic;
using AdaptySDK.TestSupport;
using AdaptySDK.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The negative half of the contract: a payload that omits a required key has to fail, and a
    /// value the SDK does not know yet has to degrade instead of failing.
    /// </summary>
    /// <remarks>
    /// A green snapshot matrix cannot show either of these - both need a payload no fixture
    /// contains. The required-key cases are generated from the fixtures themselves, so a field
    /// that gains <c>IsRequired</c> later is covered without editing this file.
    /// </remarks>
    [TestFixture]
    public class ContractEnforcementTests
    {
        /// <summary>
        /// Drops one required key at a time from a fixture and expects each removal to throw.
        /// </summary>
        [TestCase("profile-full", typeof(AdaptyProfile))]
        [TestCase("profile-minimal", typeof(AdaptyProfile))]
        [TestCase("flow-full", typeof(AdaptyFlow))]
        [TestCase("flow-minimal", typeof(AdaptyFlow))]
        [TestCase("onboarding-full", typeof(AdaptyOnboarding))]
        [TestCase("onboarding-minimal", typeof(AdaptyOnboarding))]
        [TestCase("purchase-result-success", typeof(AdaptyPurchaseResult))]
        public void RequiredKeysAreEnforced(string fixture, System.Type type)
        {
            var json = Snapshots.LoadResponse(fixture);
            var required = RequiredKeyPaths(JToken.Parse(json), type);

            Assert.That(required, Is.Not.Empty, "no required keys found - the walk is broken");

            foreach (var path in required)
            {
                var mutilated = JToken.Parse(json);
                mutilated.SelectToken(path).Parent.Remove();

                Assert.That(
                    () => JsonConvert.DeserializeObject(mutilated.ToString(), type, Settings()),
                    Throws.InstanceOf<JsonSerializationException>(),
                    $"removing '{path}' was accepted"
                );
            }
        }

        [TestCase("products-full")]
        public void RequiredKeysAreEnforcedInProducts(string fixture)
        {
            var json = Snapshots.LoadResponse(fixture);
            var required = RequiredKeyPaths(JToken.Parse(json)[0], typeof(AdaptyPaywallProduct));

            Assert.That(required, Is.Not.Empty);

            foreach (var path in required)
            {
                var mutilated = JToken.Parse(json);
                mutilated.SelectToken(path).Parent.Remove();

                Assert.That(
                    () =>
                        JsonConvert.DeserializeObject<IList<AdaptyPaywallProduct>>(
                            mutilated.ToString(),
                            Settings()
                        ),
                    Throws.InstanceOf<JsonSerializationException>(),
                    $"removing '{path}' was accepted"
                );
            }
        }

        /// <summary>
        /// Values handled by a hand-written converter never reach the contract resolver, so their
        /// required keys have to be listed rather than derived.
        /// </summary>
        [TestCase("{}", "element_type")]
        [TestCase("{\"element_type\":\"select\"}", "value")]
        [TestCase("{\"element_type\":\"select\",\"value\":{\"value\":\"f\",\"label\":\"F\"}}", "id")]
        [TestCase("{\"element_type\":\"select\",\"value\":{\"id\":\"g\",\"label\":\"F\"}}", "value")]
        [TestCase("{\"element_type\":\"select\",\"value\":{\"id\":\"g\",\"value\":\"f\"}}", "label")]
        [TestCase("{\"element_type\":\"multi_select\",\"value\":{}}", "value")]
        [TestCase("{\"element_type\":\"input\",\"value\":{}}", "type")]
        [TestCase("{\"element_type\":\"input\",\"value\":{\"type\":\"text\"}}", "value")]
        [TestCase("{\"element_type\":\"input\",\"value\":{\"type\":\"number\"}}", "value")]
        [TestCase("{\"element_type\":\"date_picker\"}", "value")]
        public void OnboardingStateRequiresKey(string json, string missing) =>
            Assert.That(
                () => AdaptyJson.Deserialize<AdaptyOnboardingsStateUpdatedParams>(json),
                Throws
                    .InstanceOf<JsonSerializationException>()
                    .With.Message.Contains($"'{missing}'")
            );

        [TestCase("{}", "name")]
        public void AnalyticsEventRequiresKey(string json, string missing) =>
            Assert.That(
                () => AdaptyJson.Deserialize<AdaptyOnboardingsAnalyticsEvent>(json),
                Throws
                    .InstanceOf<JsonSerializationException>()
                    .With.Message.Contains($"'{missing}'")
            );

        [TestCase("{}", "status")]
        [TestCase("{\"status\":\"determined\"}", "details")]
        [TestCase("{\"status\":\"determined\",\"details\":{\"app_launch_count\":1}}", "install_time")]
        [TestCase(
            "{\"status\":\"determined\",\"details\":{\"install_time\":\"2026-07-30T10:00:00.000Z\"}}",
            "app_launch_count"
        )]
        public void InstallationStatusRequiresKey(string json, string missing) =>
            Assert.That(
                () => AdaptyJson.Deserialize<AdaptyInstallationStatus>(json),
                Throws
                    .InstanceOf<JsonSerializationException>()
                    .With.Message.Contains($"'{missing}'")
            );

        [TestCase("{\"phases\":[]}", "offer_identifier")]
        [TestCase("{\"offer_identifier\":{\"id\":\"x\"}}", "type")]
        public void SubscriptionOfferRequiresKey(string json, string missing) =>
            Assert.That(
                () => AdaptyJson.Deserialize<AdaptySubscriptionOffer>(json),
                Throws
                    .InstanceOf<JsonSerializationException>()
                    .With.Message.Contains($"'{missing}'")
            );

        /// <summary>
        /// A newer native SDK sending a value this one has never heard of must degrade to Unknown,
        /// not fail the response around it.
        /// </summary>
        [Test]
        public void UnknownEnumValueDegrades()
        {
            var offer = AdaptyJson.Deserialize<AdaptySubscriptionOffer>(
                "{\"offer_identifier\":{\"id\":\"x\",\"type\":\"loyalty_reward\"}}"
            );
            Assert.That(offer.Type, Is.EqualTo(AdaptySubscriptionOfferType.Unknown));

            var action = AdaptyJson.Deserialize<AdaptyUIUserAction>(
                "{\"type\":\"teleport\",\"open_in\":\"holodeck\"}"
            );
            Assert.That(action.Type, Is.EqualTo(AdaptyUIUserActionType.Unknown));
            Assert.That(action.OpenIn, Is.EqualTo(AdaptyWebPresentation.Unknown));

            var result = AdaptyJson.Deserialize<AdaptyPurchaseResult>(
                "{\"type\":\"deferred_to_the_afterlife\"}"
            );
            Assert.That(result.Type, Is.EqualTo(AdaptyPurchaseResultType.Unknown));
        }

        /// <summary>
        /// A member the contract has no name for can come out of a read but must never go into a
        /// request: the native side would receive a value it has never heard of, and the
        /// SimpleJSON writer threw rather than send one.
        /// </summary>
        [Test]
        public void ReadFallbackEnumsCannotBeSent()
        {
            Assert.That(
                () =>
                    AdaptyJson.Serialize(
                        new AdaptyUIDialogConfiguration().SetDefaultActionTitle("OK")
                    ),
                Throws.Nothing,
                "sanity: a payload with no fallback member still serializes"
            );

            foreach (var value in new object[]
            {
                AdaptyWebPresentation.Unknown,
                AdaptyUIUserActionType.Unknown,
                AdaptyPurchaseResultType.Unknown,
                AdaptySubscriptionRenewalType.Unknown,
                AdaptyUIDialogActionType.Unknown,
            })
            {
                Assert.That(
                    () => AdaptyJson.Serialize(value),
                    Throws.InstanceOf<JsonSerializationException>(),
                    $"{value.GetType().Name}.Unknown was accepted on the write path"
                );
            }
        }

        /// <summary>
        /// The one fallback member that is writable: the SimpleJSON writer mapped anything it did
        /// not recognise to "unknown", and a product carries its offer type back to the native side.
        /// </summary>
        [Test]
        public void OfferTypeFallbackKeepsItsContractName() =>
            Assert.That(
                AdaptyJson.Serialize(AdaptySubscriptionOfferType.Unknown),
                Is.EqualTo("\"unknown\"")
            );

        /// <summary>
        /// An unknown discriminator is forward compatibility; a missing one is a broken payload.
        /// The two must not be confused.
        /// </summary>
        [Test]
        public void UnknownDiscriminatorIsNotAMissingOne()
        {
            Assert.That(
                AdaptyJson.Deserialize<AdaptyOnboardingsStateUpdatedParams>(
                    "{\"element_type\":\"slider\",\"value\":{}}"
                ),
                Is.Null
            );

            var analytics = AdaptyJson.Deserialize<AdaptyOnboardingsAnalyticsEvent>(
                "{\"name\":\"screen_dismissed\"}"
            );
            Assert.That(analytics, Is.TypeOf<AdaptyOnboardingsAnalyticsEventUnknown>());
        }

        private static JsonSerializerSettings Settings()
        {
            var settings = new JsonSerializerSettings();
            var serializer = AdaptyJson.CreateSerializer();

            settings.ContractResolver = serializer.ContractResolver;
            settings.NullValueHandling = serializer.NullValueHandling;
            settings.MissingMemberHandling = serializer.MissingMemberHandling;
            settings.DateParseHandling = serializer.DateParseHandling;
            settings.FloatParseHandling = serializer.FloatParseHandling;
            settings.Culture = serializer.Culture;
            settings.ConstructorHandling = serializer.ConstructorHandling;
            settings.ObjectCreationHandling = serializer.ObjectCreationHandling;
            settings.MetadataPropertyHandling = serializer.MetadataPropertyHandling;
            foreach (var converter in serializer.Converters)
            {
                settings.Converters.Add(converter);
            }
            return settings;
        }

        /// <summary>
        /// Walks the contract of <paramref name="type"/> alongside the fixture and returns the
        /// JSONPath of every value the contract marks required.
        /// </summary>
        /// <remarks>
        /// The paths come from <see cref="JToken.Path"/> rather than being assembled here:
        /// subscriptions are keyed by vendor product id, and a hand-built path would read the dots
        /// in "com.adapty.sample.monthly" as separators.
        /// </remarks>
        private static List<string> RequiredKeyPaths(JToken node, System.Type type)
        {
            var paths = new List<string>();
            Walk(node, type, paths, 0);
            return paths;
        }

        private static void Walk(JToken node, System.Type type, List<string> paths, int depth)
        {
            if (depth > 8)
            {
                return;
            }

            if (node is JArray array)
            {
                var element = ElementType(type);
                if (element is null)
                {
                    return;
                }

                foreach (var item in array)
                {
                    Walk(item, element, paths, depth + 1);
                }
                return;
            }

            if (!(node is JObject map))
            {
                return;
            }

            // A converter reads its payload by hand, so the resolver knows nothing about what is
            // inside it and the walk would stop at the converter's type. Cross the boundary
            // explicitly, so the annotated models nested under one stay covered.
            if (type == typeof(AdaptySubscriptionOffer))
            {
                if (map["offer_identifier"] is JToken identity)
                {
                    paths.Add(identity.Path);
                    if (identity["type"] != null)
                    {
                        paths.Add(identity["type"].Path);
                    }
                }

                Walk(map["phases"], typeof(IList<AdaptySubscriptionPhase>), paths, depth + 1);
                return;
            }

            var resolved = AdaptyContractResolver.Instance.ResolveContract(type);

            // Access levels and subscriptions arrive keyed by identifier, so the models inside
            // them are only reachable through the dictionary's value type.
            if (resolved is Newtonsoft.Json.Serialization.JsonDictionaryContract dictionary)
            {
                foreach (var entry in map)
                {
                    Walk(entry.Value, dictionary.DictionaryValueType, paths, depth + 1);
                }
                return;
            }

            if (!(resolved is Newtonsoft.Json.Serialization.JsonObjectContract contract))
            {
                return;
            }

            foreach (var property in contract.Properties)
            {
                var child = map[property.PropertyName];
                if (child is null)
                {
                    continue;
                }

                if (property.Required == Required.Always)
                {
                    paths.Add(child.Path);
                }

                Walk(child, property.PropertyType, paths, depth + 1);
            }
        }

        private static System.Type ElementType(System.Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            return type.IsGenericType && type.GetGenericArguments().Length == 1
                ? type.GetGenericArguments()[0]
                : null;
        }
    }
}
