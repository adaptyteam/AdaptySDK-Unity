//
//  AdaptyUIOnboardingMeta.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    public sealed class AdaptyUIOnboardingMeta
    {
        private AdaptyUIOnboardingMeta() { }

        [DataMember(Name = "onboarding_id", IsRequired = true)]
        public readonly string OnboardingId;
        [DataMember(Name = "screen_cid", IsRequired = true)]
        public readonly string ScreenClientId;
        [DataMember(Name = "screen_index", IsRequired = true)]
        public readonly int ScreenIndex;
        [DataMember(Name = "total_screens", IsRequired = true)]
        public readonly int ScreensTotal;

        internal AdaptyUIOnboardingMeta(
            string onboardingId,
            string screenClientId,
            int screenIndex,
            int screensTotal
        )
        {
            OnboardingId = onboardingId;
            ScreenClientId = screenClientId;
            ScreenIndex = screenIndex;
            ScreensTotal = screensTotal;
        }

        public override string ToString() =>
            $"{nameof(OnboardingId)}: {OnboardingId}, "
            + $"{nameof(ScreenClientId)}: {ScreenClientId}, "
            + $"{nameof(ScreenIndex)}: {ScreenIndex}, "
            + $"{nameof(ScreensTotal)}: {ScreensTotal}";
    }
}
