//
//  AdaptyOnboardingsAnalyticsEvent.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

namespace AdaptySDK
{
    public abstract class AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventOnboardingStarted
        : AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventSecondScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventRegistrationScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventProductsScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventUserEmailCollected
        : AdaptyOnboardingsAnalyticsEvent { }

    public sealed class AdaptyOnboardingsAnalyticsEventOnboardingCompleted
        : AdaptyOnboardingsAnalyticsEvent { }

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

    public sealed class AdaptyOnboardingsAnalyticsEventUnknown : AdaptyOnboardingsAnalyticsEvent
    {
        public readonly string Name;

        public AdaptyOnboardingsAnalyticsEventUnknown(string name)
        {
            Name = name;
        }
    }
}
