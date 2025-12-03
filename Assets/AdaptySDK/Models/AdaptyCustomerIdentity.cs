//
//  AdaptyCustomerIdentity.cs
//  AdaptySDK
//
//  Created by AI Assistant on 14.01.2025.
//

using System;

namespace AdaptySDK
{
    /// <summary>
    /// Customer identity parameters for iOS and Android platforms.
    /// </summary>
    public partial class AdaptyCustomerIdentity
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
        /// Gets a value indicating whether both AppAccountToken and ObfuscatedAccountId are null.
        /// </summary>
        public bool IsEmpty => IosAppAccountToken == null && AndroidObfuscatedAccountId == null;

        public override string ToString() =>
            $"{nameof(IosAppAccountToken)}: {IosAppAccountToken}, "
            + $"{nameof(AndroidObfuscatedAccountId)}: {AndroidObfuscatedAccountId}";
    }
}
