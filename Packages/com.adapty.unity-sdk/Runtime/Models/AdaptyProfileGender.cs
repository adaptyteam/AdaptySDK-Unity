using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyProfileGender
    {
        [EnumMember(Value = "f")]
        Female = 0,
        [EnumMember(Value = "m")]
        Male = 1,
        [EnumMember(Value = "o")]
        Other = 2,
    }
}