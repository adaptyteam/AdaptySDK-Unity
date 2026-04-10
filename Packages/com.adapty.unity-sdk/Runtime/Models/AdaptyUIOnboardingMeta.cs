//
//  AdaptyUIOnboardingMeta.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

namespace AdaptySDK
{
    public sealed class AdaptyUIOnboardingMeta
    {
        public readonly string OnboardingId;
        public readonly string ScreenClientId;
        public readonly int ScreenIndex;
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
