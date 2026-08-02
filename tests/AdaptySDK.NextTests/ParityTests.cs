using System.Collections.Generic;
using AdaptySDK;
using AdaptySDK.GoldenTests;
using AdaptySDK.Serialization;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Parses the golden fixtures through Newtonsoft and compares the model state with the
    /// snapshots the current package produced. Failures here are migration work still to do.
    /// </summary>
    [TestFixture]
    public class ParityTests
    {
        [TestCase("profile-full")]
        [TestCase("profile-minimal")]
        public void Profile(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(AdaptyJson.Deserialize<AdaptyProfile>(Snapshots.LoadResponse(fixture)))
            );

        [TestCase("flow-full")]
        [TestCase("flow-minimal")]
        public void Flow(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(AdaptyJson.Deserialize<AdaptyFlow>(Snapshots.LoadResponse(fixture)))
            );

        [TestCase("onboarding-full")]
        [TestCase("onboarding-minimal")]
        public void Onboarding(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(
                    AdaptyJson.Deserialize<AdaptyOnboarding>(Snapshots.LoadResponse(fixture))
                )
            );

        [TestCase("onboarding-analytics-started")]
        [TestCase("onboarding-analytics-screen-completed")]
        [TestCase("onboarding-analytics-screen-completed-bare")]
        [TestCase("onboarding-analytics-unknown")]
        public void OnboardingAnalyticsEvent(string fixture)
        {
            var payload = Newtonsoft.Json.Linq.JObject.Parse(Snapshots.LoadResponse(fixture));
            var analyticsEvent = payload["event"].ToObject<AdaptyOnboardingsAnalyticsEvent>(
                AdaptyJson.CreateSerializer()
            );
            Snapshots.Matches(fixture, ModelSnapshot.Render(analyticsEvent));
        }

        [TestCase("installation-determined")]
        [TestCase("installation-determined-minimal")]
        [TestCase("installation-not-determined")]
        [TestCase("installation-not-available")]
        public void InstallationStatus(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(
                    AdaptyJson.Deserialize<AdaptyInstallationStatus>(Snapshots.LoadResponse(fixture))
                )
            );

        [TestCase("error-full")]
        [TestCase("error-minimal")]
        public void Error(string fixture)
        {
            var payload = Newtonsoft.Json.Linq.JObject.Parse(Snapshots.LoadResponse(fixture));
            var error = payload["error"].ToObject<AdaptyError>(AdaptyJson.CreateSerializer());
            Snapshots.Matches(fixture, ModelSnapshot.Render(error));
        }

        [TestCase("user-action-full")]
        [TestCase("user-action-minimal")]
        public void UserAction(string fixture)
        {
            var payload = Newtonsoft.Json.Linq.JObject.Parse(Snapshots.LoadResponse(fixture));
            var action = payload["action"].ToObject<AdaptyUIUserAction>(AdaptyJson.CreateSerializer());
            Snapshots.Matches(fixture, ModelSnapshot.Render(action));
        }

        [TestCase("flow-full")]
        public void RemoteConfigDictionary(string fixture)
        {
            var flow = AdaptyJson.Deserialize<AdaptyFlow>(Snapshots.LoadResponse(fixture));
            Snapshots.Matches(
                fixture + "-remote-config-dictionary",
                ModelSnapshot.Render(flow.RemoteConfig.Dictionary)
            );
        }

        [TestCase("products-full")]
        public void PaywallProducts(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(
                    AdaptyJson.Deserialize<IList<AdaptyPaywallProduct>>(Snapshots.LoadResponse(fixture))
                )
            );

        [TestCase("purchase-result-success")]
        [TestCase("purchase-result-pending")]
        [TestCase("purchase-result-cancelled")]
        public void PurchaseResult(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(
                    AdaptyJson.Deserialize<AdaptyPurchaseResult>(Snapshots.LoadResponse(fixture))
                )
            );

        [TestCase("onboarding-date-picker-full")]
        [TestCase("onboarding-date-picker-partial")]
        [TestCase("onboarding-select")]
        public void OnboardingStateUpdated(string fixture)
        {
            var payload = Newtonsoft.Json.Linq.JObject.Parse(Snapshots.LoadResponse(fixture));
            var parameters = payload["action"].ToObject<AdaptyOnboardingsStateUpdatedParams>(
                AdaptyJson.CreateSerializer()
            );
            Snapshots.Matches(fixture, ModelSnapshot.Render(parameters));
        }
    }
}
