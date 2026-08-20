using System.Collections.Generic;
using AdaptySDK;
using AdaptySDK.TestSupport;
using AdaptySDK.Serialization;
using NUnit.Framework;

namespace AdaptySDK.NextTests
{
    /// <summary>
    /// Parses the fixtures and compares the full state of each model with its approved snapshot.
    /// The snapshots were taken from the manual layer, so a diff is a change in what the SDK reads.
    /// </summary>
    [TestFixture]
    public class ParityTests
    {
        /// <summary>
        /// The contract puts <c>profile</c> in the success branch only, so the other two results
        /// carry a null one. ToString has to survive that: an app logging a cancelled purchase is
        /// not on an error path.
        /// </summary>
        [TestCase("purchase-result-success")]
        [TestCase("purchase-result-pending")]
        [TestCase("purchase-result-cancelled")]
        public void PurchaseResultDescribesEveryVariant(string fixture) =>
            Assert.That(
                () =>
                    AdaptyJson
                        .Deserialize<AdaptyPurchaseResult>(Snapshots.LoadResponse(fixture))
                        .ToString(),
                Throws.Nothing
            );

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

        [TestCase("promoted-product-full")]
        [TestCase("promoted-product-minimal")]
        public void PromotedProduct(string fixture) =>
            Snapshots.Matches(
                fixture,
                ModelSnapshot.Render(
                    AdaptyJson.Deserialize<AdaptyPromotedProduct>(Snapshots.LoadResponse(fixture))
                )
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
