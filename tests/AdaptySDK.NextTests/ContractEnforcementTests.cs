using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using AdaptySDK.TestSupport;
using AdaptySDK.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using UnityEngine;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// The negative half of the contract: a payload that omits a required key has to fail, and so
    /// does one carrying a value the contract does not list.
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

        /// <summary>
        /// The one key the contract requires on a branch rather than always, which no attribute can
        /// state. A deserialization callback enforces it, and Newtonsoft invokes one through
        /// <c>MethodInfo.Invoke</c> — so the model's complaint arrives wrapped. Both boundaries a
        /// reply crosses catch <c>Exception</c> and print it, inner exception included.
        /// </summary>
        [Test]
        public void DeterminedInstallationStatusRequiresDetails() =>
            Assert.That(
                () => AdaptyJson.Deserialize<AdaptyInstallationStatus>("{\"status\":\"determined\"}"),
                Throws
                    .InstanceOf<System.Reflection.TargetInvocationException>()
                    .With.InnerException.InstanceOf<JsonSerializationException>()
                    .And.InnerException.Message.Contains("'details'")
            );

        /// <summary>
        /// The other half of the same invariant: details belong to the determined branch, so a stray
        /// one does not reach the app. The branch-per-subclass model this replaced never carried it
        /// either.
        /// </summary>
        [TestCase("not_available")]
        [TestCase("not_determined")]
        public void InstallationDetailsOutsideTheDeterminedBranchAreDropped(string status)
        {
            var parsed = AdaptyJson.Deserialize<AdaptyInstallationStatus>(
                "{\"status\":\""
                    + status
                    + "\",\"details\":{\"install_time\":\"2026-07-30T10:00:00.000Z\","
                    + "\"app_launch_count\":1}}"
            );

            Assert.That(parsed.Details, Is.Null);
        }

        [TestCase("{\"phases\":[]}", "offer_identifier")]
        [TestCase("{\"offer_identifier\":{\"id\":\"x\"}}", "type")]
        [TestCase("{\"offer_identifier\":{\"id\":\"x\",\"type\":\"promotional\"}}", "phases")]
        [TestCase("{\"offer_identifier\":{\"type\":\"promotional\"},\"phases\":[]}", "id")]
        [TestCase("{\"offer_identifier\":{\"type\":\"win_back\"},\"phases\":[]}", "id")]
#if UNITY_ANDROID
        // The contract marks the id required on the introductory branch for Android as well.
        [TestCase("{\"offer_identifier\":{\"type\":\"introductory\"},\"phases\":[]}", "id")]
#endif
        public void SubscriptionOfferRequiresKey(string json, string missing) =>
            Assert.That(
                () => AdaptyJson.Deserialize<AdaptySubscriptionOffer>(json),
                Throws
                    .InstanceOf<JsonSerializationException>()
                    .With.Message.Contains($"'{missing}'")
            );

        /// <summary>
        /// A string the contract does not list has to fail the read.
        /// </summary>
        /// <remarks>
        /// The SDK ships with the native SDKs it is pinned to, so an unlisted value is a broken
        /// payload rather than one from the future. Where the contract does want an open set it
        /// says so - a flow permission and an onboarding event name are strings, not enums - and
        /// where it lists "unknown" itself the value is a member like any other, see
        /// <see cref="ContractsOwnUnknownIsRead"/>.
        /// </remarks>
        [TestCase(
            "{\"offer_identifier\":{\"id\":\"x\",\"type\":\"loyalty_reward\"},\"phases\":[]}",
            typeof(AdaptySubscriptionOffer)
        )]
        [TestCase("{\"type\":\"teleport\"}", typeof(AdaptyUIUserAction))]
        [TestCase("{\"type\":\"close\",\"open_in\":\"holodeck\"}", typeof(AdaptyUIUserAction))]
        [TestCase("{\"type\":\"deferred_to_the_afterlife\"}", typeof(AdaptyPurchaseResult))]
        // A near miss is not a value either: the C# member name, another casing of the contract
        // value, and the value with whitespace around it are all outside the contract.
        [TestCase("{\"type\":\"UserCancelled\"}", typeof(AdaptyPurchaseResult))]
        [TestCase("{\"type\":\"Close\"}", typeof(AdaptyUIUserAction))]
        [TestCase("{\"type\":\"USER_CANCELLED\"}", typeof(AdaptyPurchaseResult))]
        [TestCase("{\"type\":\" user_cancelled \"}", typeof(AdaptyPurchaseResult))]
        [TestCase("{\"type\":\"SystemBack\"}", typeof(AdaptyUIUserAction))]
        public void UnknownEnumValueIsRejected(string json, System.Type type) =>
            Assert.That(
                () => JsonConvert.DeserializeObject(json, type, Settings()),
                Throws.InstanceOf<JsonSerializationException>()
            );

        /// <summary>
        /// The two enums whose contract lists "unknown" among its values keep reading it.
        /// </summary>
        [Test]
        public void ContractsOwnUnknownIsRead()
        {
            Assert.That(
                AdaptyJson.Deserialize<AdaptyPaymentMode>("\"unknown\""),
                Is.EqualTo(AdaptyPaymentMode.Unknown)
            );
            Assert.That(
                AdaptyJson.Deserialize<AdaptySubscriptionPeriodUnit>("\"unknown\""),
                Is.EqualTo(AdaptySubscriptionPeriodUnit.Unknown)
            );
        }

        /// <summary>
        /// The other half of the same rule: outside the branches that require an offer id, one has
        /// to keep reading without it. The introductory branch is in this half everywhere except
        /// Android, where the contract requires the id too.
        /// </summary>
        [TestCase("code")]
#if !UNITY_ANDROID
        [TestCase("introductory")]
#endif
        public void OfferIdIsOptionalOutsideItsRequiredBranches(string type)
        {
            var offer = AdaptyJson.Deserialize<AdaptySubscriptionOffer>(
                "{\"offer_identifier\":{\"type\":\"" + type + "\"},\"phases\":[]}"
            );

            Assert.That(offer.Identifier, Is.Null);
        }

        /// <summary>
        /// Every member of a string enum carries exactly one contract name, and no two members share
        /// it.
        /// </summary>
        /// <remarks>
        /// Asked of the metadata, because neither half of the converter can report it. Writing is
        /// stock <c>StringEnumConverter</c>, which falls back to the C# member name instead of
        /// failing, so a member that lost its <c>[EnumMember]</c> would quietly send "Unknown".
        /// Reading is the ordinal map built from the same attributes, where a duplicate name means
        /// one of the two members can never be read and which one is decided by field order.
        /// </remarks>
        [Test]
        public void EveryMemberOfAContractNamedEnumHasItsName()
        {
            var broken = new List<string>();

            foreach (var type in typeof(AdaptyFlow).Assembly.GetTypes())
            {
                if (!type.IsEnum || type.Namespace != "AdaptySDK")
                {
                    continue;
                }

                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static);

                // No name anywhere means the contract spells this one as a number.
                if (!System.Array.Exists(fields, HasContractName))
                {
                    continue;
                }

                var seen = new Dictionary<string, string>();

                foreach (var field in fields)
                {
                    var name = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;

                    if (string.IsNullOrEmpty(name))
                    {
                        broken.Add($"{type.Name}.{field.Name} - no contract name");
                        continue;
                    }

                    if (seen.TryGetValue(name, out var owner))
                    {
                        broken.Add($"{type.Name}.{field.Name} - shares \"{name}\" with {owner}");
                        continue;
                    }

                    seen[name] = field.Name;
                }
            }

            Assert.That(
                broken,
                Is.Empty,
                "these break the mapping to the contract:\n  " + string.Join("\n  ", broken)
            );
        }

        /// <summary>
        /// The other half of the same rule: an enum the contract spells as a number must not be
        /// caught by the string converter. Stock <c>StringEnumConverter</c> claims every enum, so
        /// what keeps error codes numeric is the gate in front of it.
        /// </summary>
        [Test]
        public void NumericEnumsStayNumeric()
        {
            Assert.That(
                AdaptyJson.Serialize(AdaptyErrorCode.NoPurchasesToRestore),
                Is.EqualTo("1004")
            );
            Assert.That(
                AdaptyJson.Deserialize<AdaptyErrorCode>("1004"),
                Is.EqualTo(AdaptyErrorCode.NoPurchasesToRestore)
            );
            Assert.That(
                AdaptyJson.Serialize(AppTrackingTransparencyStatus.Authorized),
                Is.EqualTo("3")
            );
            Assert.That(
                AdaptyJson.Deserialize<AppTrackingTransparencyStatus>("3"),
                Is.EqualTo(AppTrackingTransparencyStatus.Authorized)
            );
        }

        private static bool HasContractName(FieldInfo field) =>
            field.GetCustomAttribute<EnumMemberAttribute>() != null;

        /// <summary>
        /// Every public enum member states its number. An inserted member otherwise renumbers every
        /// member below it, and the numbers are public API even where the wire format is a string.
        /// </summary>
        /// <remarks>
        /// Read from the sources, because metadata cannot tell an explicit value from one the
        /// compiler counted out. The approved public surface is what catches a number that moves;
        /// this is what catches a number that was never written down.
        /// </remarks>
        [Test]
        public void EveryPublicEnumMemberStatesItsValue()
        {
            var models = System.IO.Path.Combine(
                ProjectDirectory(),
                "..",
                "..",
                "Packages",
                "com.adapty.unity-sdk",
                "Runtime",
                "Models"
            );

            var implicitly_ = new List<string>();

            foreach (var file in System.IO.Directory.GetFiles(models, "*.cs"))
            {
                string enumeration = null;
                var depth = 0;

                foreach (var line in System.IO.File.ReadAllLines(file))
                {
                    var declaration = Regex.Match(line, @"public enum (\w+)");
                    if (declaration.Success)
                    {
                        enumeration = declaration.Groups[1].Value;
                        depth = 0;
                    }

                    if (enumeration is null)
                    {
                        continue;
                    }

                    depth += Count(line, '{') - Count(line, '}');

                    var member = Regex.Match(line, @"^[ \t]+([A-Za-z_]\w*)[ \t]*(,?)[ \t]*(//.*)?$");
                    if (member.Success)
                    {
                        implicitly_.Add($"{enumeration}.{member.Groups[1].Value}");
                    }

                    if (depth == 0 && line.Contains("}"))
                    {
                        enumeration = null;
                    }
                }
            }

            Assert.That(
                implicitly_,
                Is.Empty,
                "these take their number from the member above them:\n  "
                    + string.Join("\n  ", implicitly_)
            );
        }

        private static int Count(string line, char character)
        {
            var found = 0;
            foreach (var c in line)
            {
                if (c == character)
                {
                    found += 1;
                }
            }
            return found;
        }

        private static string ProjectDirectory(
            [System.Runtime.CompilerServices.CallerFilePath] string callerPath = null
        ) => System.IO.Path.GetDirectoryName(callerPath);

        /// <summary>
        /// A response model hands out views, not its own storage. Declaring the member as a
        /// read-only interface would not be enough on its own: <c>ReadOnlyCollection</c> and
        /// <c>ReadOnlyDictionary</c> do implement the mutable interfaces, so the cast back compiles
        /// and succeeds — what it yields is the wrapper, which refuses to write, rather than the
        /// dictionary behind it.
        /// </summary>
        [Test]
        public void AResponseModelCannotBeMutatedThroughItsCollections()
        {
            var profile = AdaptyJson.Deserialize<AdaptyProfile>(
                Snapshots.LoadResponse("profile-full")
            );

            Assert.Multiple(() =>
            {
                Assert.That(
                    () => ((IDictionary<string, AdaptyProfile.AccessLevel>)profile.AccessLevels).Clear(),
                    Throws.InstanceOf<NotSupportedException>()
                );
                Assert.That(
                    () => ((IDictionary<string, AdaptyProfile.Subscription>)profile.Subscriptions).Clear(),
                    Throws.InstanceOf<NotSupportedException>()
                );
                Assert.That(
                    () => ((IDictionary<string, object>)profile.CustomAttributes)["x"] = 1,
                    Throws.InstanceOf<NotSupportedException>()
                );
                Assert.That(
                    () => ((IList<string>)profile.AppliedAttributionSources).Add("x"),
                    Throws.InstanceOf<NotSupportedException>()
                );

                // Both levels: the values of the outer dictionary are views too.
                foreach (var purchases in profile.NonSubscriptions.Values)
                {
                    Assert.That(
                        () => ((IList<AdaptyProfile.NonSubscription>)purchases).Clear(),
                        Throws.InstanceOf<NotSupportedException>()
                    );
                }
            });

            Assert.That(profile.NonSubscriptions.Values, Is.Not.Empty, "the fixture stopped covering the nested case");

            Assert.That(
                () =>
                    ((IDictionary<string, IReadOnlyList<AdaptyProfile.NonSubscription>>)profile.NonSubscriptions).Clear(),
                Throws.InstanceOf<NotSupportedException>(),
                "the outer dictionary is writable"
            );
        }

        /// <summary>
        /// The mirror on the way in: what the SDK will send has to be decided when the setter is
        /// called, not whenever the request happens to be serialized. Nothing else would catch this
        /// — the public surface and the happy-path snapshots look the same either way.
        /// </summary>
        [Test]
        public void AParameterObjectDoesNotKeepTheCallersDictionary()
        {
            var tags = new Dictionary<string, string> { ["greeting"] = "hello" };
            var parameters = new AdaptyUICreateFlowViewParameters().SetCustomTags(tags);

            tags["greeting"] = "goodbye";
            tags["added_later"] = "x";

            Assert.Multiple(() =>
            {
                Assert.That(parameters.CustomTags["greeting"], Is.EqualTo("hello"));
                Assert.That(parameters.CustomTags.ContainsKey("added_later"), Is.False);
                Assert.That(AdaptyJson.Serialize(parameters), Does.Not.Contain("added_later"));
            });
        }

        /// <summary>
        /// The same ownership question for the one asset built from a caller's buffer.
        /// </summary>
        [Test]
        public void ACustomAssetDoesNotKeepTheCallersBuffer()
        {
            var pixels = new byte[] { 1, 2, 3 };
            var asset = (AdaptyCustomAssetLocalImageData)AdaptyCustomAsset.LocalImageData(pixels);

            pixels[0] = 99;
            asset.Data[1] = 99;

            Assert.That(AdaptyJson.Serialize(asset), Does.Contain(Convert.ToBase64String(new byte[] { 1, 2, 3 })));
        }

        /// <summary>
        /// A <c>Gradient</c> is the family's other mutable argument, and the payload is read from it
        /// lazily, so the window a caller can change it in runs until the request goes out.
        /// </summary>
        [Test]
        public void ACustomAssetDoesNotKeepTheCallersGradient()
        {
            var gradient = new Gradient
            {
                colorKeys = new[] { new GradientColorKey(new Color(1f, 0f, 0f, 1f), 0f) },
                alphaKeys = new[] { new GradientAlphaKey(1f, 0f) },
            };
            var asset = (AdaptyCustomAssetLinearGradient)AdaptyCustomAsset.LinearGradient(gradient);

            gradient.colorKeys = new[] { new GradientColorKey(new Color(0f, 1f, 0f, 1f), 0f) };

            Assert.That(AdaptyJson.Serialize(asset), Does.Contain("#FF0000FF"));
        }

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

            if (node is not JObject map)
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

            if (resolved is not Newtonsoft.Json.Serialization.JsonObjectContract contract)
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
