//
//  AdaptyPurchaseResult.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public partial class AdaptyPurchaseResult
    {
        private AdaptyPurchaseResult() { }

        [DataMember(Name = "type", IsRequired = true)]
        public readonly AdaptyPurchaseResultType Type;
        [DataMember(Name = "profile")]
        public readonly AdaptyProfile Profile;

        [DataMember(Name = "apple_jws_transaction")]
        public readonly string AppleJWSTransaction; // nullable, iOS Only

        [DataMember(Name = "google_purchase_token")]
        public readonly string GooglePurchaseToken; // nullable, Android Only

        public override string ToString() =>
            $"{nameof(Type)}: {Type}, "
            + $"{nameof(Profile)}: {(Profile == null ? "null" : Profile.ToString())}, "
            + $"{nameof(AppleJWSTransaction)}: {(string.IsNullOrEmpty(AppleJWSTransaction) ? "null or empty" : AppleJWSTransaction)}, "
            + $"{nameof(GooglePurchaseToken)}: {(string.IsNullOrEmpty(GooglePurchaseToken) ? "null or empty" : GooglePurchaseToken)}";
    }
}
