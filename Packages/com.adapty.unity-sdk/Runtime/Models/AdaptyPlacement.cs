//
//  AdaptyPlacement.cs
//  AdaptySDK
//
//  Created by Aleksei Goncharov on 09.09.2025.
//

using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdaptySDK
{

    [DataContract]
    [Preserve]
    public sealed class AdaptyPlacement
    {
        private AdaptyPlacement() { }

        /// The identifier of the placement, configured in Adapty Dashboard.
        [DataMember(Name = "developer_id", IsRequired = true)]
        public readonly string Id;

        /// The name of the audience for the placement.
        [DataMember(Name = "audience_name", IsRequired = true)]
        public readonly string AudienceName;

        /// The current revision (version) of the placement.
        [DataMember(Name = "revision", IsRequired = true)]
        public readonly long Revision;

        /// Placement A/B test name
        [DataMember(Name = "ab_test_name", IsRequired = true)]
        public readonly string ABTestName;

        /// Placement audience version id
        [DataMember(Name = "placement_audience_version_id", IsRequired = true)]
        public readonly string PlacementAudienceVersionId;

        [DataMember(Name = "is_tracking_purchases")]
        public readonly bool? IsTrackingPurchases = false;

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(AudienceName)}: {AudienceName}, "
            + $"{nameof(Revision)}: {Revision}, "
            + $"{nameof(ABTestName)}: {ABTestName}, "
            + $"{nameof(PlacementAudienceVersionId)}: {PlacementAudienceVersionId}, "
            + $"{nameof(IsTrackingPurchases)}: {IsTrackingPurchases}";
    }
}
