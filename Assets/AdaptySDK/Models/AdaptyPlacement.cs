//
//  AdaptyPlacement.cs
//  AdaptySDK
//
//  Created by Aleksei Goncharov on 09.09.2025.
//

using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPlacement
    {
        /// The identifier of the placement, configured in Adapty Dashboard.
        public readonly string Id;

        /// The name of the audience for the placement.
        public readonly string AudienceName;

        /// The current revision (version) of the placement.
        public readonly long Revision;

        /// Placement A/B test name
        public readonly string ABTestName;

        /// Placement audience version id
        public readonly string PlacementAudienceVersionId;

        public readonly bool? IsTrackingPurchases;

        public bool GetIsTrackingPurchases => IsTrackingPurchases ?? false;

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(AudienceName)}: {AudienceName}, "
            + $"{nameof(Revision)}: {Revision}, "
            + $"{nameof(ABTestName)}: {ABTestName}, "
            + $"{nameof(PlacementAudienceVersionId)}: {PlacementAudienceVersionId}, "
            + $"{nameof(IsTrackingPurchases)}: {IsTrackingPurchases}";
    }
}
