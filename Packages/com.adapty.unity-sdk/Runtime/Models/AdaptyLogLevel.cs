//
//  AdaptyLogLevel.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyLogLevel
    {
        [EnumMember(Value = "error")]
        Error = 0,
        [EnumMember(Value = "warn")]
        Warn = 1,
        [EnumMember(Value = "info")]
        Info = 2,
        [EnumMember(Value = "verbose")]
        Verbose = 3,
        [EnumMember(Value = "debug")]
        Debug = 4
    }
}