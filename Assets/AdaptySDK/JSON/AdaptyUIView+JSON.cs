//
//  AdaptyUIView+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIView
    {
        internal AdaptyUIView(JSONObject jsonNode)
        {
            Id = jsonNode.GetString("id");
            PlacementId = jsonNode.GetString("placement_id");
            PaywallVariationId = jsonNode.GetString("paywall_variation_id");
        }
    }
}


namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyUIView GetAdaptyUIView(this JSONNode node) => 
            new AdaptyUIView(GetObject(node));

        internal static AdaptyUIView GetAdaptyUIView(this JSONNode node, string aKey) => 
            new AdaptyUIView(GetObject(node, aKey));
    }
}