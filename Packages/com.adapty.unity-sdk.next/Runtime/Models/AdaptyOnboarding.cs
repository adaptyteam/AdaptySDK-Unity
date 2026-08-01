//
//  AdaptyOnboarding.cs
//  AdaptySDK
//
//  Created by Aleksei Goncharov on 09.09.2025.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    /// <summary>
    /// Represents an onboarding configuration in Adapty.
    /// </summary>
    /// <remarks>
    /// An onboarding is a set of screens that can be displayed to users during their first app experience.
    /// Read more at <see href="https://adapty.io/docs/onboardings">Adapty Documentation</see>
    /// </remarks>
    public partial class AdaptyOnboarding
    {
        /// <summary>
        /// An <see cref="AdaptyPlacement"/> object that contains information about the placement of the onboarding.
        /// </summary>
        public readonly AdaptyPlacement Placement;

        /// <summary>
        /// The unique identifier of the onboarding.
        /// </summary>
        public readonly string OnboardingId;

        /// <summary>
        /// The onboarding name configured in the Adapty Dashboard.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The identifier of the variation, used to attribute analytics to the onboarding.
        /// </summary>
        public readonly string VariationId;

        /// <summary>
        /// The custom JSON formatted data configured in the Adapty Dashboard.
        /// </summary>
        /// <remarks>
        /// This can be null if no remote config is configured for the onboarding.
        /// </remarks>
        public readonly AdaptyRemoteConfig RemoteConfig; // nullable

        private readonly OnboardingBuilder _Builder;

        private readonly long _ResponseCreatedAt;
        private readonly string _PayloadData; // nullable
        private readonly string _RequestLocale;

        public override string ToString() =>
            $"{nameof(Placement)}: {Placement}, "
            + $"{nameof(OnboardingId)}: {OnboardingId}, "
            + $"{nameof(Name)}: {Name}, "
            + $"{nameof(VariationId)}: {VariationId}, "
            + $"{nameof(RemoteConfig)}: {RemoteConfig}, "
            + $"{nameof(_Builder)}: {_Builder}, "
            + $"{nameof(_ResponseCreatedAt)}: {_ResponseCreatedAt}, "
            + $"{nameof(_PayloadData)}: {_PayloadData}, "
            + $"{nameof(_RequestLocale)}: {_RequestLocale}";

        private sealed class OnboardingBuilder
        {
            public readonly string ConfigUrl;

            internal OnboardingBuilder(string configUrl)
            {
                ConfigUrl = configUrl;
            }

            public override string ToString() => $"{nameof(ConfigUrl)}: {ConfigUrl}";
        }
    }
}
