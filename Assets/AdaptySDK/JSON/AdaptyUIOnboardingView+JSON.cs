//
//  AdaptyUIOnboardingView+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIOnboardingView
    {
        internal AdaptyUIOnboardingView(JSONObject jsonNode)
        {
            Id = jsonNode.GetString("id");
            PlacementId = jsonNode.GetString("placement_id");
            PaywallVariationId = jsonNode.GetString("variation_id");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyUIOnboardingView GetAdaptyUIOnboardingView(this JSONNode node) =>
            new AdaptyUIOnboardingView(GetObject(node));

        internal static AdaptyUIOnboardingView GetAdaptyUIOnboardingView(
            this JSONNode node,
            string aKey
        ) => new AdaptyUIOnboardingView(GetObject(node, aKey));
    }
}
