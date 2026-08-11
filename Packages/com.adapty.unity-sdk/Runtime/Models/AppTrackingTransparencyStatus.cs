using UnityEngine.Scripting;

namespace AdaptySDK {

    /// <summary>
    /// iOS only. What the user answered to the App Tracking Transparency prompt, as <c>ATTrackingManager.AuthorizationStatus</c> reports it.
    /// </summary>
    /// <remarks>
    /// The numbers are Apple's own, so a value read from <c>ATTrackingManager</c> can be cast across directly.
    /// </remarks>
    [Preserve]
    public enum AppTrackingTransparencyStatus {
        /// <summary>
        /// The prompt has not been shown yet.
        /// </summary>
        NotDetermined = 0,
        /// <summary>
        /// Tracking is not permitted on this device — a restriction outside the user's control, such as a managed device.
        /// </summary>
        Restricted = 1,
        /// <summary>
        /// The user was asked and declined.
        /// </summary>
        Denied = 2,
        /// <summary>
        /// The user was asked and agreed.
        /// </summary>
        Authorized = 3
    }

}
