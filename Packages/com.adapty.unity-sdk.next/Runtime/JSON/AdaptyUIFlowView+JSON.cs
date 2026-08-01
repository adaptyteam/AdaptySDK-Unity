//
//  AdaptyUIFlowView+JSON.cs
//  AdaptySDK
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIFlowView
    {
        internal AdaptyUIFlowView(JSONObject jsonNode)
        {
            Id = jsonNode.GetString("id");
            PlacementId = jsonNode.GetString("placement_id");
            VariationId = jsonNode.GetString("variation_id");
            Locale = jsonNode.GetStringIfPresent("locale");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyUIFlowView GetAdaptyUIFlowView(this JSONNode node) =>
            new AdaptyUIFlowView(GetObject(node));

        internal static AdaptyUIFlowView GetAdaptyUIFlowView(this JSONNode node, string aKey) =>
            new AdaptyUIFlowView(GetObject(node, aKey));
    }
}
