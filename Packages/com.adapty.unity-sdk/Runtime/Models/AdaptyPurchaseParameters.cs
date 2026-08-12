using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// The optional extras of a purchase. Both are Android only — on iOS an instance changes
    /// nothing, and <see cref="Adapty.MakePurchase(AdaptyPaywallProduct, System.Action{AdaptyPurchaseResult, AdaptyError})"/>
    /// without one is the same call.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyPurchaseParameters
    {
        /// <summary>
        /// Android only. Makes the purchase replace a subscription the user already has. Null for
        /// an ordinary purchase.
        /// </summary>
        [DataMember(Name = "subscription_update_params")]
        public readonly AdaptySubscriptionUpdateParameters SubscriptionUpdateParams;

        /// <summary>
        /// Android only. Declares to Google Play that the price shown was personalised to this
        /// user, which some jurisdictions require disclosing. Null leaves the native default.
        /// </summary>
        [DataMember(Name = "is_offer_personalized")]
        public readonly bool? IsOfferPersonalized;

        /// <param name="subscriptionUpdateParams">
        /// Android only. The subscription this purchase replaces, or null.
        /// </param>
        /// <param name="isOfferPersonalized">
        /// Android only. Whether the price shown was personalised, or null.
        /// </param>
        /// <summary>
        /// Builds the extras for one purchase. Both are Android only.
        /// </summary>
        public AdaptyPurchaseParameters(
            AdaptySubscriptionUpdateParameters subscriptionUpdateParams = null,
            bool? isOfferPersonalized = null
        )
        {
            SubscriptionUpdateParams = subscriptionUpdateParams;
            IsOfferPersonalized = isOfferPersonalized;
        }

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(SubscriptionUpdateParams)}: {SubscriptionUpdateParams}, "
            + $"{nameof(IsOfferPersonalized)}: {IsOfferPersonalized}";
    }

    /// <summary>
    /// Assembles an <see cref="AdaptyPurchaseParameters"/>. Every setter returns the builder, so
    /// calls chain; the constructor takes both values directly if that reads better.
    /// </summary>
    [Preserve]
    public sealed class AdaptyPurchaseParametersBuilder
    {
        private AdaptyPurchaseParameters _parameters = new AdaptyPurchaseParameters();

        /// <summary>Sets <see cref="AdaptyPurchaseParameters.SubscriptionUpdateParams"/>. Android only.</summary>
        /// <param name="subscriptionUpdateParams">The subscription this purchase replaces.</param>
        public AdaptyPurchaseParametersBuilder SetSubscriptionUpdateParams(
            AdaptySubscriptionUpdateParameters subscriptionUpdateParams
        )
        {
            _parameters = new AdaptyPurchaseParameters(
                subscriptionUpdateParams,
                _parameters.IsOfferPersonalized
            );
            return this;
        }

        /// <summary>Sets <see cref="AdaptyPurchaseParameters.IsOfferPersonalized"/>. Android only.</summary>
        /// <param name="isOfferPersonalized">Whether the price shown was personalised to this user.</param>
        public AdaptyPurchaseParametersBuilder SetIsOfferPersonalized(bool? isOfferPersonalized)
        {
            _parameters = new AdaptyPurchaseParameters(
                _parameters.SubscriptionUpdateParams,
                isOfferPersonalized
            );
            return this;
        }

        /// <summary>
        /// The parameters described by this builder.
        /// </summary>
        public AdaptyPurchaseParameters Build()
        {
            return _parameters;
        }
    }
}
