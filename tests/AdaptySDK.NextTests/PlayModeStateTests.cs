#if !UNITY_IOS && !UNITY_ANDROID

using System.Linq;
using System.Reflection;
using AdaptySDK.TestSupport;
using NUnit.Framework;
using UnityEngine;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// With Domain Reload disabled — the default for fast iteration — statics survive leaving Play
    /// Mode. Anything the SDK holds that a developer registered has to be gone before the next run,
    /// or that run receives the previous one's callbacks.
    /// </summary>
    /// <remarks>
    /// The reset is what Unity calls; the suites call it directly, which is the same thing minus
    /// the Editor. What cannot be checked here is that Unity calls it at all — that is the two
    /// consecutive Play Mode runs in the acceptance pass.
    /// </remarks>
    [TestFixture]
    public class PlayModeStateTests
    {
        [TearDown]
        public void TearDown() => Adapty.SetEventListener(null);

        [Test]
        public void AListenerDoesNotSurviveIntoTheNextRun()
        {
            Adapty.SetEventListener(new Listener());

            Adapty.ResetListeners();

            Adapty.OnMessage(
                "did_load_latest_profile",
                "{\"profile\":" + Snapshots.LoadResponse("profile-minimal") + "}"
            );

            Assert.That(Listener.Calls, Is.Zero, "a listener from the previous run was still called");
        }

        [Test]
        public void TheNoopHandlerDoesNotSurviveEither()
        {
            AdaptySDK.Noop.AdaptyNoop.Handler = (method, request) => "{}";

            AdaptySDK.Noop.AdaptyNoop.ResetHandler();

            Assert.That(AdaptySDK.Noop.AdaptyNoop.Handler, Is.Null);
        }

        /// <summary>
        /// Every reset has to be one Unity actually calls, and it only calls the ones carrying the
        /// attribute. A method renamed or added without it would leave its state behind silently.
        /// </summary>
        [Test]
        public void EveryResetIsRegisteredWithUnity()
        {
            var resets = typeof(Adapty)
                .Assembly.GetTypes()
                .SelectMany(type =>
                    type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                )
                .Where(method => method.Name.StartsWith("Reset"))
                .ToList();

            var unregistered = resets
                .Where(method =>
                    method.GetCustomAttribute<RuntimeInitializeOnLoadMethodAttribute>() is null
                )
                .Select(method => $"{method.DeclaringType.Name}.{method.Name}")
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(resets, Is.Not.Empty, "no reset methods found - the rule matches nothing");
                Assert.That(
                    unregistered,
                    Is.Empty,
                    "these reset static state but Unity never calls them:\n  "
                        + string.Join("\n  ", unregistered)
                );
            });
        }

        private sealed class Listener : IAdaptyEventListener
        {
            internal static int Calls;

            public Listener() => Calls = 0;

            public void OnLoadLatestProfile(AdaptyProfile profile) => Calls += 1;

            public void OnInstallationDetailsSuccess(AdaptyInstallationDetails details) { }

            public void OnInstallationDetailsFail(AdaptyError error) { }
        }
    }
}

#endif
