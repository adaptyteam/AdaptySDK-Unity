//
//  OnboardingScreenParameters+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using AdaptySDK.SimpleJSON;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class OnboardingScreenParameters
        {
            internal JSONNode ToJSONNode()
            {
                var node = new JSONObject();
                if (Name is not null) node.Add("onboarding_name", Name);
                if (ScreenName is not null) node.Add("onboarding_screen_name", ScreenName);
                node.Add("onboarding_screen_order", ScreenOrder);
                return node;
            }
        }
    }
}
