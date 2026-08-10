using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Customer identity parameters for iOS and Android platforms.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyCustomerIdentity
    {
        /// <summary>
        /// The UUID that you generate to associate a customer's In-App Purchase with its resulting App Store transaction. (iOS Only). Nullable.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://developer.apple.com/documentation/appstoreserverapi/appaccounttoken">Apple Documentation</see>
        /// </remarks>
        public readonly Guid IosAppAccountToken;

        /// <summary>
        /// The obfuscated account identifier (Android Only). Nullable.
        /// </summary>
        /// <remarks>
        /// Read more at <see href="https://developer.android.com/google/play/billing/developer-payload#attribute">Android Documentation</see>
        /// </remarks>
        public readonly string AndroidObfuscatedAccountId;

        /// <summary>
        /// Initializes a new instance of the AdaptyCustomerIdentity class.
        /// </summary>
        /// <param name="appAccountToken">The UUID for iOS App Store transactions (iOS Only). Nullable.</param>
        /// <param name="obfuscatedAccountId">The obfuscated account identifier (Android Only). Nullable.</param>
        public AdaptyCustomerIdentity(Guid iosAppAccountToken, string androidObfuscatedAccountId)
        {
            IosAppAccountToken = iosAppAccountToken;
            AndroidObfuscatedAccountId = androidObfuscatedAccountId;
        }

        /// <summary>
        /// Gets a value indicating whether neither AppAccountToken nor ObfuscatedAccountId carries a value.
        /// </summary>
        public bool IsEmpty =>
            IosAppAccountToken == Guid.Empty && string.IsNullOrEmpty(AndroidObfuscatedAccountId);

        // Emitted through members of their own: the contract omits an unset token or account id
        // rather than sending an empty value, and NullValueHandling then drops them.
        [DataMember(Name = "app_account_token")]
        [Preserve]
        private Guid? IosAppAccountTokenForRequest =>
            IosAppAccountToken == Guid.Empty ? (Guid?)null : IosAppAccountToken;

        [DataMember(Name = "obfuscated_account_id")]
        [Preserve]
        private string AndroidObfuscatedAccountIdForRequest =>
            string.IsNullOrEmpty(AndroidObfuscatedAccountId) ? null : AndroidObfuscatedAccountId;

        public override string ToString() =>
            $"{nameof(IosAppAccountToken)}: {IosAppAccountToken}, "
            + $"{nameof(AndroidObfuscatedAccountId)}: {AndroidObfuscatedAccountId}";
    }
}
