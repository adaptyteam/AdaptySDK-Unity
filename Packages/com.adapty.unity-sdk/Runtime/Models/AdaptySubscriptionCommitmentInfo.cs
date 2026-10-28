using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// The total a subscription's billing plan commits the user to. iOS only.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscriptionCommitmentInfo
    {
        private AdaptySubscriptionCommitmentInfo() { }

        /// <summary>
        /// The total price over the whole commitment.
        /// </summary>
        [DataMember(Name = "price", IsRequired = true)]
        public readonly AdaptyPrice Price;

        /// <summary>
        /// How long the commitment lasts.
        /// </summary>
        [DataMember(Name = "period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod Period;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Price)}: {Price}, " +
            $"{nameof(Period)}: {Period}";
    }
}
