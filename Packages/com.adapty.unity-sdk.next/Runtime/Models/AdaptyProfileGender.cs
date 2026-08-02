//
//  AdaptyProfileGender.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System.Runtime.Serialization;

namespace AdaptySDK
{
    public enum AdaptyProfileGender
    {
        [EnumMember(Value = "f")]
        Female,
        [EnumMember(Value = "m")]
        Male,
        [EnumMember(Value = "o")]
        Other,
    }
}