using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// A span of time, as the stores express it — a number and a unit, not a duration.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscriptionPeriod
    {
        private AdaptySubscriptionPeriod() { }

        /// <summary>
        /// The unit the period is counted in.
        /// </summary>
        [DataMember(Name = "unit", IsRequired = true)]
        public readonly AdaptySubscriptionPeriodUnit Unit;

        /// <summary>
        /// How many of that unit — three months is <c>Month</c> and 3.
        /// </summary>
        [DataMember(Name = "number_of_units", IsRequired = true)]
        public readonly long NumberOfUnits;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Unit)}: {Unit}, " +
            $"{nameof(NumberOfUnits)}: {NumberOfUnits}";
    }
}
