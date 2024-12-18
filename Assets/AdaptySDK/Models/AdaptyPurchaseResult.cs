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

        public override string ToString() => 
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Profile)}: {Profile}";
    }
}