using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdaptySDK.TestSupport
{
    /// <summary>
    /// The objects the write tests serialize. Built through the package's own public API, so the
    /// file stops compiling if that API changes shape.
    /// </summary>
    public static class Samples
    {
        public static AdaptyConfiguration Configuration() =>
            new AdaptyConfiguration.Builder("public_live_key")
                .SetCustomerUserId("user-1", Guid.Empty, "obfuscated-1")
                .SetObserverMode(true)
                .SetIPAddressCollectionDisabled(true)
                .SetServerCluster(AdaptyServerCluster.EU)
                .SetBackendProxy("proxy.example.com", 8080)
                .SetAdaptyUIMediaCache(100, 200, 300)
                .Build();

        /// <summary>
        /// The default cluster is an explicit contract value, not the absence of one - a member
        /// without its own contract name would serialize as the CLR name instead.
        /// </summary>
        public static AdaptyConfiguration ConfigurationWithDefaultCluster() =>
            new AdaptyConfiguration.Builder("public_live_key")
                .SetServerCluster(AdaptyServerCluster.Default)
                .Build();

        /// <summary>
        /// An identity with neither of its platform tokens set is dropped, not sent empty.
        /// </summary>
        public static AdaptyConfiguration ConfigurationWithEmptyIdentity() =>
            new AdaptyConfiguration.Builder("public_live_key")
                .SetCustomerUserId("user-1", Guid.Empty, null)
                .Build();

        public static AdaptyProfileParameters ProfileParameters()
        {
            var parameters = new AdaptyProfileParameters.Builder()
                .SetFirstName("Ada")
                .SetLastName("Lovelace")
                .SetGender(AdaptyProfileGender.Female)
                .SetBirthday(new DateTime(1815, 12, 10))
                .SetEmail("ada@example.com")
                .SetPhoneNumber("+15550100")
                .SetAnalyticsDisabled(false)
                .Build();
            parameters.SetCustomStringAttribute("plan", "gold");
            parameters.SetCustomDoubleAttribute("score", 12.5);
            return parameters;
        }

        public static AdaptyProductIdentifier ProductIdentifier() =>
            new AdaptyProductIdentifier(
                "com.adapty.sample.monthly",
                "adapty-product-1",
                "monthly-base-plan"
            );

        /// <summary>
        /// No base plan: an App Store product never has one, and the key is left out rather than
        /// sent empty.
        /// </summary>
        public static AdaptyProductIdentifier ProductIdentifierWithoutBasePlan() =>
            new AdaptyProductIdentifier("com.adapty.sample.lifetime", "adapty-product-2", null);

        /// <summary>
        /// The same identifier built with an empty base plan rather than none. It has to reach the
        /// wire as the one above: null is what NullValueHandling drops, and an app that read the id
        /// out of a text field has an empty string, not a null.
        /// </summary>
        public static AdaptyProductIdentifier ProductIdentifierWithEmptyBasePlan() =>
            new AdaptyProductIdentifier("com.adapty.sample.lifetime", "adapty-product-2", "");

        public static AdaptyUIDialogConfiguration DialogConfiguration() =>
            new AdaptyUIDialogConfiguration()
                .SetTitle("Cancel subscription?")
                .SetContent("You keep access until the end of the period.")
                .SetDefaultActionTitle("Keep")
                .SetSecondaryActionTitle("Cancel");

        /// <summary>
        /// Only the required action title - the rest of the dialog is optional.
        /// </summary>
        public static AdaptyUIDialogConfiguration DialogConfigurationMinimal() =>
            new AdaptyUIDialogConfiguration().SetDefaultActionTitle("OK");

        public static AdaptyPlacementFetchPolicy FetchPolicyDefault() =>
            AdaptyPlacementFetchPolicy.Default;

        public static AdaptyPlacementFetchPolicy FetchPolicyWithMaxAge() =>
            AdaptyPlacementFetchPolicy.ReturnCacheDataIfNotExpiredElseLoad(
                TimeSpan.FromMinutes(1.5)
            );

        /// <summary>
        /// One asset of every kind, so the platform-dependent paths and both colour forms are all
        /// exercised in a single payload.
        /// </summary>
        public static Dictionary<string, AdaptyCustomAsset> CustomAssets()
        {
            var gradient = new Gradient
            {
                colorKeys = new[]
                {
                    new GradientColorKey(new Color(1f, 0f, 0f, 1f), 0f),
                    new GradientColorKey(new Color(0f, 0f, 1f, 1f), 1f),
                },
                alphaKeys = new[]
                {
                    new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.5f, 0.25f),
                    new GradientAlphaKey(0f, 1f),
                },
            };

            return new Dictionary<string, AdaptyCustomAsset>
            {
                { "hero_data", AdaptyCustomAsset.LocalImageData(new byte[] { 1, 2, 3, 250 }) },
                { "hero_asset", AdaptyCustomAsset.LocalImageAsset("hero-asset-id") },
                { "hero_file", AdaptyCustomAsset.LocalImageFile("images/hero.png") },
                { "clip_asset", AdaptyCustomAsset.LocalVideoAsset("clip-asset-id") },
                { "clip_file", AdaptyCustomAsset.LocalVideoFile("videos/clip.mp4") },
                { "accent", AdaptyCustomAsset.Color(new Color(0.2f, 0.4f, 0.6f, 1f)) },
                { "backdrop", AdaptyCustomAsset.LinearGradient(gradient) },
            };
        }
    }
}
