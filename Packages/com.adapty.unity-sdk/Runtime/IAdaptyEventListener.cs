//
//  IAdaptyEventListener.cs
//  AdaptySDK
//

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
