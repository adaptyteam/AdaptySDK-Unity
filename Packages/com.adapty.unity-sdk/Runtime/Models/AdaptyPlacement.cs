using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdaptySDK
{

    /// <summary>
    /// The placement a flow was fetched for, with the A/B test and audience it resolved to.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyPlacement
    {
        private AdaptyPlacement() { }

        /// The identifier of the placement, configured in Adapty Dashboard.
        /// <summary>
        /// The placement identifier from the Dashboard.
        /// </summary>
        [DataMember(Name = "developer_id", IsRequired = true)]
        public readonly string Id;

        /// The name of the audience for the placement.
        /// <summary>
        /// The audience the profile fell into.
        /// </summary>
        [DataMember(Name = "audience_name", IsRequired = true)]
        public readonly string AudienceName;

        /// The current revision (version) of the placement.
        /// <summary>
        /// Which revision of the placement this is — it goes up on every change in the Dashboard.
        /// </summary>
        [DataMember(Name = "revision", IsRequired = true)]
        public readonly long Revision;

        /// Placement A/B test name
        /// <summary>
        /// The A/B test the placement is running, when it is running one.
        /// </summary>
        [DataMember(Name = "ab_test_name", IsRequired = true)]
        public readonly string ABTestName;

        /// Placement audience version id
        /// <summary>
        /// The exact placement-and-audience version this was resolved from.
        /// </summary>
        [DataMember(Name = "placement_audience_version_id", IsRequired = true)]
        public readonly string PlacementAudienceVersionId;

        /// <summary>
        /// Whether purchases in this placement count towards its analytics. Never arrives null — a
        /// missing key leaves the declared false.
        /// </summary>
        [DataMember(Name = "is_tracking_purchases")]
        public readonly bool? IsTrackingPurchases = false;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(AudienceName)}: {AudienceName}, "
            + $"{nameof(Revision)}: {Revision}, "
            + $"{nameof(ABTestName)}: {ABTestName}, "
            + $"{nameof(PlacementAudienceVersionId)}: {PlacementAudienceVersionId}, "
            + $"{nameof(IsTrackingPurchases)}: {IsTrackingPurchases}";
    }
}
