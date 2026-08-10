using UnityEngine.Scripting;

namespace AdaptySDK
{
    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public abstract class AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventOnboardingStarted
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventSecondScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventRegistrationScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventProductsScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventUserEmailCollected
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventOnboardingCompleted
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventScreenCompleted
        : AdaptyOnboardingsAnalyticsEvent
    {
        public readonly string ElementId; // nullable
        public readonly string Reply; // nullable

        public AdaptyOnboardingsAnalyticsEventScreenCompleted(string elementId, string reply)
        {
            ElementId = elementId;
            Reply = reply;
        }
    }

    [Preserve]
    [System.Obsolete("The legacy onboarding API is deprecated in favor of Flows.")]
    public sealed class AdaptyOnboardingsAnalyticsEventUnknown : AdaptyOnboardingsAnalyticsEvent
    {
        public readonly string Name;

        public AdaptyOnboardingsAnalyticsEventUnknown(string name)
        {
            Name = name;
        }
    }
}
