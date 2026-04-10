//
//  AdaptyOnboardingsAnalyticsEvent+JSON.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptySDK.AdaptyOnboardingsAnalyticsEvent GetOnboardingsAnalyticsEvent(
            this JSONNode node,
            string aKey
        )
        {
            var obj = JSONNodeExtensions.GetObject(node, aKey);
            var name = obj.GetString("name");
            switch (name)
            {
                case "onboarding_started":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventOnboardingStarted();
                case "screen_presented":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventScreenPresented();
                case "screen_completed":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventScreenCompleted(
                        obj.GetStringIfPresent("element_id"),
                        obj.GetStringIfPresent("reply")
                    );
                case "second_screen_presented":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventSecondScreenPresented();
                case "registration_screen_presented":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventRegistrationScreenPresented();
                case "products_screen_presented":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventProductsScreenPresented();
                case "user_email_collected":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventUserEmailCollected();
                case "onboarding_completed":
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventOnboardingCompleted();
                default:
                    return new AdaptySDK.AdaptyOnboardingsAnalyticsEventUnknown(name);
            }
        }
    }
}
