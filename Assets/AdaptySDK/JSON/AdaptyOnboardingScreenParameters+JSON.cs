//
//  AdaptyOnboardingScreenParameters+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;
    public partial class AdaptyOnboardingScreenParameters
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            if (Name != null) node.Add("onboarding_name", Name);
            if (ScreenName != null) node.Add("onboarding_screen_name", ScreenName);
            node.Add("onboarding_screen_order", ScreenOrder);
            return node;
        }
    }
}