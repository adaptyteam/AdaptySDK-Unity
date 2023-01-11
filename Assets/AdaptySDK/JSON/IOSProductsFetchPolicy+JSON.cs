//
//  IOSProductsFetchPolicy+JSON.cs
//  Adapty
//
//  Created by Aleksei Goncharov on 10.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.IOSProductsFetchPolicy value)
            => value switch
            {
                Adapty.IOSProductsFetchPolicy.Default => "default",
                Adapty.IOSProductsFetchPolicy.WaitForReceiptValidation => "wait_for_receipt_validation",
                _ => throw new Exception($"IOSProductsFetchPolicy unknown value: {value}"),
            };
    }
}