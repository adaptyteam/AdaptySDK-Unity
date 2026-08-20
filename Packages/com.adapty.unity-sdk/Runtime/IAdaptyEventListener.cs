namespace AdaptySDK
{
    /// <summary>
    /// Interface for listening to Adapty SDK events.
    /// </summary>
    /// <remarks>
    /// Implement this interface to receive notifications about profile updates and installation details.
    /// Use <see cref="Adapty.SetEventListener(IAdaptyEventListener)"/> to register your listener.
    /// </remarks>
    public interface IAdaptyEventListener
    {
        /// <summary>
        /// Called when the latest profile is loaded.
        /// </summary>
        /// <param name="profile">The updated <see cref="AdaptyProfile"/> object.</param>
        void OnLoadLatestProfile(AdaptyProfile profile);

        /// <summary>
        /// Called when the user starts a promoted in-app purchase from the App Store product page. iOS only.
        /// </summary>
        /// <remarks>
        /// Complete the purchase by passing the product to
        /// <see cref="Adapty.MakePromotedPurchase(AdaptyPromotedProduct, System.Action{AdaptyPurchaseResult, AdaptyError})"/>.
        /// </remarks>
        /// <param name="product">The <see cref="AdaptyPromotedProduct"/> the user chose.</param>
        void OnReceivePromotedPurchase(AdaptyPromotedProduct product);

        /// <summary>
        /// Called when installation details are successfully retrieved.
        /// </summary>
        /// <param name="details">The <see cref="AdaptyInstallationDetails"/> object containing installation information.</param>
        void OnInstallationDetailsSuccess(AdaptyInstallationDetails details);

        /// <summary>
        /// Called when installation details retrieval fails.
        /// </summary>
        /// <param name="error">The <see cref="AdaptyError"/> object describing the error.</param>
        void OnInstallationDetailsFail(AdaptyError error);
    }
}
