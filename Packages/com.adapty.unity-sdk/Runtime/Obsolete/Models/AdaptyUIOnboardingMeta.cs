using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
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

        public override string ToString() =>
            $"{nameof(OnboardingId)}: {OnboardingId}, "
            + $"{nameof(ScreenClientId)}: {ScreenClientId}, "
            + $"{nameof(ScreenIndex)}: {ScreenIndex}, "
            + $"{nameof(ScreensTotal)}: {ScreensTotal}";
    }
}
