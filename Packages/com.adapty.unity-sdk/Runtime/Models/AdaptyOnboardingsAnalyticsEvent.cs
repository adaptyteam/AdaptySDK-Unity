//
//  AdaptyOnboardingsAnalyticsEvent.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

using UnityEngine.Scripting;

namespace AdaptySDK
{
    [Preserve]
    public abstract class AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventOnboardingStarted
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventSecondScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventRegistrationScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventProductsScreenPresented
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventUserEmailCollected
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
    public sealed class AdaptyOnboardingsAnalyticsEventOnboardingCompleted
        : AdaptyOnboardingsAnalyticsEvent { }

    [Preserve]
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
    public sealed class AdaptyOnboardingsAnalyticsEventUnknown : AdaptyOnboardingsAnalyticsEvent
    {
        public readonly string Name;

        public AdaptyOnboardingsAnalyticsEventUnknown(string name)
        {
            Name = name;
        }
    }
}
