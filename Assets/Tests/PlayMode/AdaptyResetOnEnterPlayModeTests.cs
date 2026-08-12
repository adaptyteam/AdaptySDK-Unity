using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AdaptySDK;
using NUnit.Framework;
using UnityEngine;

namespace AdaptyExample.PlayModeTests
{
    /// <summary>
    /// That Unity really calls the SDK's Play Mode resets. The desktop suite calls them directly,
    /// which proves what they do and not that anything invokes them — and an attribute that is
    /// never honoured is exactly the failure this guards: the Editor-side callback next to these
    /// once had the wrong signature and was dead code that compiled.
    /// </summary>
    /// <remarks>
    /// Only meaningful with Domain Reload disabled, which is why the fixture asserts that first:
    /// with it on, the statics are gone because the domain was rebuilt, and the test would pass
    /// without the reset existing at all.
    /// </remarks>
    [TestFixture]
    public class AdaptyResetOnEnterPlayModeTests
    {
        // Written on the Editor side, by assemblies this one cannot reference. AdaptyPlayModeSeed
        // leaves "<entry>:<UTC ticks>:<1 if Domain Reload is off>"; AdaptyPlayModeEntryStamp names
        // the entry now under way, and wipes the seed's marker on the way into each one.
        private const string EntryKey = "adapty.playmode.entry";
        private const string SeedKey = "adapty.playmode.seed";

        /// <summary>
        /// How recent the seed's timestamp has to be, once it has already named this entry.
        /// </summary>
        /// <remarks>
        /// A second bound rather than the check itself: it is what remains if the entry id ever
        /// stops rotating, which would make a marker from an earlier entry match again.
        /// </remarks>
        private static readonly TimeSpan MaxAge = TimeSpan.FromMinutes(5);

        private static readonly string[] Listeners =
        {
            "m_Listener",
            "m_FlowsEventsListener",
            "m_SystemRequestsHandler",
            "m_ObserverModeResolver",
        };

        private string m_Entry;
        private string m_Marker;

        /// <summary>
        /// Reads the seed's marker and deletes it in the same breath. The entry id is left alone —
        /// it belongs to the Editor side, which rotates it per entry.
        /// </summary>
        [OneTimeSetUp]
        public void ConsumeTheMarker()
        {
            m_Entry = PlayerPrefs.GetString(EntryKey, string.Empty);
            m_Marker = PlayerPrefs.GetString(SeedKey, string.Empty);

            PlayerPrefs.DeleteKey(SeedKey);
            PlayerPrefs.Save();
        }

        [Test]
        public void TheSeedRanForThisRun()
        {
            var age =
                DateTime.UtcNow
                - new DateTime(
                    long.Parse(Fields()[1], CultureInfo.InvariantCulture),
                    DateTimeKind.Utc
                );

            Assert.That(
                age,
                Is.LessThan(MaxAge),
                $"the seed marker is {age.TotalMinutes:F1} minutes old, so the entry id it names is "
                    + "no longer being rotated and it belongs to an earlier entry"
            );
        }

        [Test]
        public void DomainReloadIsDisabledForThisRun()
        {
            Assert.That(
                Fields()[2],
                Is.EqualTo("1"),
                "Domain Reload is on for this run, so the statics were cleared by the domain being "
                    + "rebuilt and the reset below is not what this measured. Enable Enter Play "
                    + "Mode Options with Disable Domain Reload."
            );
        }

        [Test]
        public void UnityClearsEveryListenerOnEnteringPlayMode()
        {
            var survivors = Listeners
                .Where(name => Field(name).GetValue(null) != null)
                .ToList();

            Assert.That(
                survivors,
                Is.Empty,
                "these listeners survived into this run, so the SubsystemRegistration reset did "
                    + "not run: " + string.Join(", ", survivors)
            );
        }

        /// <summary>
        /// The marker, once it has been shown to belong to this entry. Every test that reads it
        /// goes through here, so none of them can report on one another entry left behind.
        /// </summary>
        private string[] Fields()
        {
            Assert.That(
                m_Entry,
                Is.Not.Empty,
                "no entry id, so AdaptyPlayModeEntryStamp did not run and nothing here can tell "
                    + "which entry the seed marker belongs to"
            );

            Assert.That(
                m_Marker,
                Is.Not.Empty,
                "AdaptyPlayModeSeed did not run, so the reset had nothing to clear and the "
                    + "assertions here would hold whatever the SDK does"
            );

            var parts = m_Marker.Split(':');

            Assert.That(
                parts.Length,
                Is.EqualTo(3),
                $"the seed marker is not <entry>:<ticks>:<flag>: \"{m_Marker}\""
            );

            Assert.That(
                parts[0],
                Is.EqualTo(m_Entry),
                "the seed marker names another entry into Play Mode, so it was left behind by that "
                    + "one and the seed did not run for this"
            );

            return parts;
        }

        private static FieldInfo Field(string name)
        {
            var field = typeof(Adapty).GetField(
                name,
                BindingFlags.NonPublic | BindingFlags.Static
            );

            Assert.That(field, Is.Not.Null, $"Adapty.{name} is gone - this test is looking at nothing");
            return field;
        }
    }
}
