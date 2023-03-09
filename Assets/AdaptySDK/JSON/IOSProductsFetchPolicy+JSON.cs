//
//  IOSProductsFetchPolicy+JSON.cs
//  Adapty
//
//  Created by Aleksei Goncharov on 10.12.2022.
//

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static string ToJSON(this Adapty.IOSProductsFetchPolicy value)
        {
            switch (value)
            {
                case Adapty.IOSProductsFetchPolicy.WaitForReceiptValidation: return "wait_for_receipt_validation";
                default: return "default";
            }
        }
    }
}