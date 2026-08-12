using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// What the SDK knows about this installation of the app.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyInstallationDetails
    {
        private AdaptyInstallationDetails() { }

        /// <summary>
        /// Adapty's identifier for this installation, from the registration it performs. Null when it
        /// has not been established.
        /// </summary>
        [DataMember(Name = "install_id")]
        public readonly string InstallId;
        /// <summary>
        /// When the app was installed, on the machine's clock — the wire carries UTC and the SDK converts.
        /// </summary>
        [DataMember(Name = "install_time", IsRequired = true)]
        public readonly DateTime InstallTime; // Date string, non-null
        /// <summary>
        /// How many times the app has been launched, counted by the SDK.
        /// </summary>
        [DataMember(Name = "app_launch_count", IsRequired = true)]
        public readonly int AppLaunchCount; // non-null
        /// <summary>
        /// The install payload the attribution provider passed through, as its own string. Null when there
        /// was none.
        /// </summary>
        [DataMember(Name = "payload")]
        public readonly string Payload;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"(installId: {InstallId}, "
            + $"installTime: {InstallTime}, "
            + $"appLaunchCount: {AppLaunchCount}, "
            + $"payload: {Payload})";
    }
}
