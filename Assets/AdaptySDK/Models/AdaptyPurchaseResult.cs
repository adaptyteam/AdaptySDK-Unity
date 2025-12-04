//
//  AdaptyPurchaseResult.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK
{
    public partial class AdaptyPurchaseResult
    {
        public readonly AdaptyPurchaseResultType Type;
        public readonly AdaptyProfile Profile;

        public readonly string AppleJWSTransaction; // nullable, iOS Only

        public readonly string GooglePurchaseToken; // nullable, Android Only

        public override string ToString() =>
            $"{nameof(Type)}: {Type}, "
            + $"{nameof(Profile)}: {Profile.ToString()}, "
            + $"{nameof(AppleJWSTransaction)}: {(string.IsNullOrEmpty(AppleJWSTransaction) ? "null or empty" : AppleJWSTransaction)}, "
            + $"{nameof(GooglePurchaseToken)}: {(string.IsNullOrEmpty(GooglePurchaseToken) ? "null or empty" : GooglePurchaseToken)}";
    }
}
