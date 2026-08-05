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
        Error,
        [EnumMember(Value = "warn")]
        Warn,
        [EnumMember(Value = "info")]
        Info,
        [EnumMember(Value = "verbose")]
        Verbose,
        [EnumMember(Value = "debug")]
        Debug
    }
}