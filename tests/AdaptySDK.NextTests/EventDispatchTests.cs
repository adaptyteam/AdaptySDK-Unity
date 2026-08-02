// The bridge is chosen by the same #if the SDK uses: off the editor it is a real P/Invoke or
// AndroidJavaClass with nothing behind it on a desktop test host. These fixtures drive the
// transport end to end, so they need the no-op bridge; device coverage is a separate stage.
// The platform-dependent payloads themselves - the custom asset paths - are pinned per platform
// by the request snapshots.
#if !UNITY_IOS && !UNITY_ANDROID

using System;
using System.Collections.Generic;
using AdaptySDK.GoldenTests;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Events arrive from native code. On iOS that is a reverse-P/Invoke callback with nothing
    /// behind it, so the dispatcher's first duty is to never let anything escape - and its second
    /// is to hand the listener a fully built model.
    /// </summary>
    [TestFixture]
    public class EventDispatchTests
    {
        private Listener _listener;

        [SetUp]
        public void Setup()
        {
            _listener = new Listener();
            Adapty.SetEventListener(_listener);
        }

        [TearDown]
        public void TearDown() => Adapty.SetEventListener(null);

        [Test]
        public void ProfileUpdatesReachTheListener()
        {
            Adapty.OnMessage(
                "did_load_latest_profile",
                "{\"profile\":" + Snapshots.LoadResponse("profile-minimal") + "}"
            );

            Assert.That(_listener.Profile, Is.Not.Null);
            Assert.That(_listener.Profile.ProfileId, Is.Not.Null);
        }

        /// <summary>
        /// Every way a payload can be wrong, none of which may reach the caller as an exception.
        /// </summary>
        [TestCase("not json at all", TestName = "malformed json")]
        [TestCase("[]", TestName = "not an object")]
        [TestCase("{}", TestName = "missing payload")]
        [TestCase("{\"profile\":null}", TestName = "null payload")]
        [TestCase("{\"profile\":{}}", TestName = "payload missing required fields")]
        [TestCase("{\"profile\":\"a string\"}", TestName = "payload of the wrong shape")]
        public void BrokenPayloadsAreContained(string json)
        {
            Assert.That(() => Adapty.OnMessage("did_load_latest_profile", json), Throws.Nothing);
            Assert.That(_listener.Profile, Is.Null, "a broken payload reached the listener");
        }

        /// <summary>
        /// The listener is the app's code, and it throwing is the app's bug - but it happens on the
        /// same callback, so it cannot be allowed to take the process down either.
        /// </summary>
        [Test]
        public void AThrowingListenerIsContained()
        {
            _listener.Throw = true;

            Assert.That(
                () =>
                    Adapty.OnMessage(
                        "did_load_latest_profile",
                        "{\"profile\":" + Snapshots.LoadResponse("profile-minimal") + "}"
                    ),
                Throws.Nothing
            );
        }

        [Test]
        public void UnknownEventIdsAreIgnored() =>
            Assert.That(
                () => Adapty.OnMessage("something_from_a_newer_native_sdk", "{}"),
                Throws.Nothing
            );

        [TestCase(null)]
        [TestCase("")]
        public void EmptyPayloadsAreIgnored(string json) =>
            Assert.That(() => Adapty.OnMessage("did_load_latest_profile", json), Throws.Nothing);

        private sealed class Listener : IAdaptyEventListener
        {
            internal AdaptyProfile Profile;
            internal bool Throw;

            public void OnLoadLatestProfile(AdaptyProfile profile)
            {
                if (Throw)
                {
                    throw new InvalidOperationException("the app's own bug");
                }

                Profile = profile;
            }

            public void OnInstallationDetailsSuccess(AdaptyInstallationDetails details) { }

            public void OnInstallationDetailsFail(AdaptyError error) { }
        }
    }
}

#endif
