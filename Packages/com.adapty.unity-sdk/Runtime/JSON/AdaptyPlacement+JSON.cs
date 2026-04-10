//
//  AdaptyPaywall+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Goncharov on 09.09.2025.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPlacement
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("developer_id", Id);
            node.Add("audience_name", AudienceName);
            node.Add("revision", Revision);
            node.Add("ab_test_name", ABTestName);
            node.Add("placement_audience_version_id", PlacementAudienceVersionId);

            if (IsTrackingPurchases != null)
            {
                node.Add("is_tracking_purchases", IsTrackingPurchases);
            }
            return node;
        }

        internal AdaptyPlacement(JSONObject jsonNode)
        {
            Id = jsonNode.GetString("developer_id");
            AudienceName = jsonNode.GetString("audience_name");
            Revision = jsonNode.GetLong("revision");
            ABTestName = jsonNode.GetString("ab_test_name");
            PlacementAudienceVersionId = jsonNode.GetString("placement_audience_version_id");
            IsTrackingPurchases = jsonNode.GetBooleanIfPresent("is_tracking_purchases") ?? false;
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyPlacement GetPlacement(this JSONNode node) =>
            new AdaptyPlacement(GetObject(node));

        internal static AdaptyPlacement GetPlacement(this JSONNode node, string aKey) =>
            new AdaptyPlacement(GetObject(node, aKey));
    }
}
