using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How much the SDK knows about this installation.
    /// </summary>
    [Preserve]
    public enum AdaptyInstallationStatusType
    {
        /// <summary>
        /// The details are not available. Reported when the platform has nothing to report, and on iOS
        /// also when the install time or the launch count could not be obtained.
        /// </summary>
        [EnumMember(Value = "not_available")]
        NotAvailable = 0,

        /// <summary>
        /// Not established yet. Ask again later.
        /// </summary>
        [EnumMember(Value = "not_determined")]
        NotDetermined = 1,

        /// <summary>
        /// Established — the details are on <see cref="AdaptyInstallationStatus.Details"/>.
        /// </summary>
        [EnumMember(Value = "determined")]
        Determined = 2,
    }
}
