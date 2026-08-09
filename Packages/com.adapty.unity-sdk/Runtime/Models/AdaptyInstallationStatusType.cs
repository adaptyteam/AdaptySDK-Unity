//
//  AdaptyInstallationStatusType.cs
//  AdaptySDK
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyInstallationStatusType
    {
        [EnumMember(Value = "not_available")]
        NotAvailable = 0,
        [EnumMember(Value = "not_determined")]
        NotDetermined = 1,
        [EnumMember(Value = "determined")]
        Determined = 2,
    }
}
