//
//  Error.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;
using AdaptySDK.SimpleJSON;
using UnityEngine;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class Error
        {
            public readonly ErrorCode Code;
            public readonly string Message;
            public readonly string Detail;

            public override string ToString()
            {
                return $"{nameof(Code)}: {Code}, " +
                       $"{nameof(Message)}: {Message}, " +
                       $"{nameof(Detail)}: {Detail}";
            }
        }
    }
}

