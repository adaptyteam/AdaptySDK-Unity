using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    public sealed partial class AdaptyProfile
    {
        /// <summary>
        /// Where a subscription bought on a commitment plan stands within that commitment.
        /// </summary>
        [DataContract]
        public sealed class SubscriptionCommitmentInfo
        {
            private SubscriptionCommitmentInfo() { }

            /// <summary>
            /// The billing period the commitment is in now, counted from the start of the
            /// commitment.
            /// </summary>
            [DataMember(Name = "billing_period_number", IsRequired = true)]
            public readonly int BillingPeriodNumber;

            /// <summary>
            /// How many billing periods the commitment has in total.
            /// </summary>
            [DataMember(Name = "total_billing_periods", IsRequired = true)]
            public readonly int TotalBillingPeriods;

            /// <summary>
            /// When the commitment ends — later than the expiry of the access it grants, which
            /// ends with the period paid for.
            /// </summary>
            [DataMember(Name = "expires_at", IsRequired = true)]
            public readonly DateTime ExpiresAt;

            /// <summary>
            /// A description for logs and the debugger. The format is not part of the contract —
            /// read the members rather than parsing it.
            /// </summary>
            public override string ToString() =>
                $"{nameof(BillingPeriodNumber)}: {BillingPeriodNumber}, " +
                $"{nameof(TotalBillingPeriods)}: {TotalBillingPeriods}, " +
                $"{nameof(ExpiresAt)}: {ExpiresAt}";
        }
    }
}
