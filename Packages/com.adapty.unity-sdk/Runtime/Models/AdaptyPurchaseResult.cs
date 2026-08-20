using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// How a purchase ended, and the updated profile when it succeeded.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyPurchaseResult
    {
        private AdaptyPurchaseResult() { }

        /// <summary>
        /// How the purchase ended.
        /// </summary>
        [DataMember(Name = "type", IsRequired = true)]
        public readonly AdaptyPurchaseResultType Type;
        /// <summary>
        /// The profile as it stands after the purchase. Only for
        /// <see cref="AdaptyPurchaseResultType.Success"/> — null otherwise.
        /// </summary>
        [DataMember(Name = "profile")]
        public readonly AdaptyProfile Profile;

        /// <summary>
        /// iOS only. The signed App Store transaction, for server-side verification of your own.
        /// Null off iOS, and when the store does not provide one.
        /// </summary>
        [DataMember(Name = "apple_jws_transaction")]
        public readonly string AppleJWSTransaction;

        /// <summary>
        /// Android only. The Google Play purchase token, for server-side verification of your own.
        /// Null off Android, and when the store does not provide one.
        /// </summary>
        [DataMember(Name = "google_purchase_token")]
        public readonly string GooglePurchaseToken;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Type)}: {Type}, "
            + $"{nameof(Profile)}: {(Profile == null ? "null" : Profile.ToString())}, "
            + $"{nameof(AppleJWSTransaction)}: {(string.IsNullOrEmpty(AppleJWSTransaction) ? "null or empty" : AppleJWSTransaction)}, "
            + $"{nameof(GooglePurchaseToken)}: {(string.IsNullOrEmpty(GooglePurchaseToken) ? "null or empty" : GooglePurchaseToken)}";
    }
}
