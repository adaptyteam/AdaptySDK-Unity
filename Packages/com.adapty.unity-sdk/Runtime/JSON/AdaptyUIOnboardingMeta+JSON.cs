//
//  AdaptyUIOnboardingMeta+JSON.cs
//  AdaptySDK
//
//  Created by GPT-5 on 17.09.2025.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public sealed partial class AdaptyUIOnboardingMetaExtensions { }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyUIOnboardingMeta GetAdaptyUIOnboardingMeta(
            this JSONNode node,
            string aKey
        )
        {
            var obj = JSONNodeExtensions.GetObject(node, aKey);
            // cross_platform.yaml uses keys: onboarding_id, screen_cid, screen_index, total_screens
            var onboardingId = obj.GetString("onboarding_id");
            var screenCid = obj.GetString("screen_cid");
            var screenIndex = obj.GetInteger("screen_index");
            var totalScreens = obj.GetInteger("total_screens");
            return new AdaptySDK.AdaptyUIOnboardingMeta(
                onboardingId,
                screenCid,
                screenIndex,
                totalScreens
            );
        }
    }
}
