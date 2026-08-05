using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using AdaptySDK.Serialization;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// No contract may need a collection wrapper, because the wrapper cannot be built on a
    /// stripped IL2CPP player.
    /// </summary>
    /// <remarks>
    /// When a member's declared type implements neither non-generic <c>IList</c> nor non-generic
    /// <c>IDictionary</c> - which is every <c>IList&lt;T&gt;</c> and <c>IDictionary&lt;K,V&gt;</c>
    /// the models expose - Newtonsoft populates it through a <c>CollectionWrapper</c>, and gets at
    /// that wrapper's constructor by reflection. Managed stripping removes the constructor, the
    /// lookup returns null, and deserialization dies on "Value cannot be null. Parameter name:
    /// method".
    ///
    /// Measured on the simulator, where it took out the profile - so activation, the access levels
    /// and every purchase result with it. Desktop cannot reproduce it: nothing is stripped there
    /// and the wrapper builds happily. So the property under test is the one that holds on both,
    /// which is that no wrapper is ever asked for.
    /// </remarks>
    [TestFixture]
    public class AotContractTests
    {
        [Test]
        public void NoModelCollectionNeedsAWrapper()
        {
            var wrapped = CollectionTypes()
                .Where(NeedsWrapper)
                .Select(type => type.ToString())
                .Distinct()
                .OrderBy(name => name, StringComparer.Ordinal)
                .ToList();

            Assert.That(
                wrapped,
                Is.Empty,
                "these resolve to a wrapper Newtonsoft builds by reflection, "
                    + "which a stripped player cannot do:\n  " + string.Join("\n  ", wrapped)
            );
        }

        /// <summary>
        /// A guard that stops finding collections would pass no matter what the resolver did.
        /// </summary>
        [Test]
        public void TheGuardStillFindsTheModelCollections()
        {
            var found = CollectionTypes().Distinct().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(found.Count, Is.GreaterThan(5), "far fewer collections than the models declare");

                Assert.That(
                    found,
                    Has.Some.EqualTo(typeof(IList<string>)),
                    "AdaptyProfile.AppliedAttributionSources is no longer seen"
                );
                Assert.That(
                    found,
                    Has.Some.EqualTo(typeof(IDictionary<string, IList<AdaptyProfile.NonSubscription>>)),
                    "AdaptyProfile.NonSubscriptions is no longer seen"
                );
            });
        }

        private static bool NeedsWrapper(Type type)
        {
            var contract = AdaptyContractResolver.Instance.ResolveContract(type);
            if (contract is not JsonArrayContract && contract is not JsonDictionaryContract)
            {
                return false;
            }

            // Newtonsoft keeps the flag internal; read it rather than restate the rule, so the test
            // tracks what the serializer will actually do.
            var flag = contract
                .GetType()
                .GetProperty(
                    "ShouldCreateWrapper",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                );

            Assert.That(flag, Is.Not.Null, $"Newtonsoft no longer exposes ShouldCreateWrapper on {contract.GetType().Name}");

            return (bool)flag.GetValue(contract);
        }

        /// <summary>
        /// Every collection the serializer contracts: a member of a model, or anything nested in
        /// one. Only contract types are walked - the converters' own caches are dictionaries too,
        /// and no serializer ever sees them.
        /// </summary>
        private static IEnumerable<Type> CollectionTypes()
        {
            const BindingFlags Members =
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            var models = typeof(Adapty)
                .Assembly.GetTypes()
                .Where(type => type.GetCustomAttribute<DataContractAttribute>() != null);

            foreach (var type in models)
            {
                var declared = type.GetFields(Members)
                    .Select(field => field.FieldType)
                    .Concat(type.GetProperties(Members).Select(property => property.PropertyType));

                foreach (var member in declared)
                {
                    foreach (var collection in Collections(member))
                    {
                        yield return collection;
                    }
                }
            }
        }

        /// <summary>
        /// The type itself and its generic arguments, which the serializer contracts in turn: a
        /// dictionary of lists asks for a wrapper twice.
        /// </summary>
        private static IEnumerable<Type> Collections(Type type)
        {
            if (type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type))
            {
                yield return type;
            }

            if (!type.IsGenericType)
            {
                yield break;
            }

            foreach (var argument in type.GetGenericArguments())
            {
                foreach (var nested in Collections(argument))
                {
                    yield return nested;
                }
            }
        }
    }
}
