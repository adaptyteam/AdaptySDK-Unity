using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How a subscription product is billed: the plan, each payment, and the total the user
    /// commits to. iOS only.
    /// </summary>
    /// <remarks>
    /// Every subscription product on iOS carries terms — <see cref="AdaptySubscriptionBillingPlan.UpFront"/>
    /// when no other plan applies, and then the payment and the commitment are the same.
    /// </remarks>
    [DataContract]
    [Preserve]
    public sealed class AdaptySubscriptionPricingTerms
    {
        private AdaptySubscriptionPricingTerms() { }

        [DataMember(Name = "billing_plan_id", IsRequired = true)]
        private readonly string _BillingPlan;

        /// <summary>
        /// The plan these terms are for. It goes back with the purchase, so the product is bought
        /// on this plan.
        /// </summary>
        public AdaptySubscriptionBillingPlan BillingPlan => new(_BillingPlan);

        /// <summary>
        /// The price of each payment.
        /// </summary>
        [DataMember(Name = "billing_price", IsRequired = true)]
        public readonly AdaptyPrice BillingPrice;

        /// <summary>
        /// The time between payments.
        /// </summary>
        [DataMember(Name = "billing_period", IsRequired = true)]
        public readonly AdaptySubscriptionPeriod BillingPeriod;

        /// <summary>
        /// The total price and period the user commits to.
        /// </summary>
        [DataMember(Name = "commitment_info", IsRequired = true)]
        public readonly AdaptySubscriptionCommitmentInfo CommitmentInfo;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(BillingPlan)}: {_BillingPlan}, " +
            $"{nameof(BillingPrice)}: {BillingPrice}, " +
            $"{nameof(BillingPeriod)}: {BillingPeriod}, " +
            $"{nameof(CommitmentInfo)}: {CommitmentInfo}";
    }
}
