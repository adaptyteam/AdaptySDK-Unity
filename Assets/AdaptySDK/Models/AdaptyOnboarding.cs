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

    public partial class AdaptyOnboarding
    {
        public readonly AdaptyPlacement Placement;

        public readonly string OnboardingId;

        public readonly string Name;

        public readonly string VariationId;

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
