//
//  AdaptyConfiguration.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 10.12.2024.
//

namespace AdaptySDK
{
    public partial class AdaptyConfiguration
    {
        private readonly string ApiKey;
        private readonly string CustomerUserId; // nullable
        private readonly AdaptyCustomerIdentity CustomerIdentity; // nullable
        private readonly bool? ObserverMode;
        private readonly bool? AppleIdfaCollectionDisabled;
        private readonly bool? GoogleAdvertisingIdCollectionDisabled;
        private readonly bool? GoogleEnablePendingPrepaidPlans;
        private readonly bool? GoogleLocalAccessLevelAllowed;
        private readonly bool? IpAddressCollectionDisabled;
        private readonly bool? AppleClearDataOnBackup;
        private readonly AdaptyServerCluster? ServerCluster;
        private readonly string BackendProxyHost; // nullable
        private readonly int? BackendProxyPort; // nullable
        private readonly AdaptyLogLevel? LogLevel;
        private readonly bool? ActivateUI;
        private AdaptyUIMediaCacheConfiguration AdaptyUIMediaCache;

        public override string ToString() =>
            $"{nameof(ApiKey)}: {ApiKey}, "
            + $"{nameof(CustomerUserId)}: {CustomerUserId}, "
            + $"{nameof(CustomerIdentity)}: {CustomerIdentity}, "
            + $"{nameof(ObserverMode)}: {ObserverMode}, "
            + $"{nameof(AppleIdfaCollectionDisabled)}: {AppleIdfaCollectionDisabled}, "
            + $"{nameof(GoogleAdvertisingIdCollectionDisabled)}: {GoogleAdvertisingIdCollectionDisabled}, "
            + $"{nameof(GoogleEnablePendingPrepaidPlans)}: {GoogleEnablePendingPrepaidPlans}, "
            + $"{nameof(GoogleLocalAccessLevelAllowed)}: {GoogleLocalAccessLevelAllowed}, "
            + $"{nameof(IpAddressCollectionDisabled)}: {IpAddressCollectionDisabled}, "
            + $"{nameof(AppleClearDataOnBackup)}: {AppleClearDataOnBackup}, "
            + $"{nameof(ServerCluster)}: {ServerCluster}, "
            + $"{nameof(BackendProxyHost)}: {BackendProxyHost}, "
            + $"{nameof(BackendProxyPort)}: {BackendProxyPort}, "
            + $"{nameof(ActivateUI)}: {ActivateUI}, "
            + $"{nameof(AdaptyUIMediaCache)}: {AdaptyUIMediaCache}, "
            + $"{nameof(LogLevel)}: {LogLevel}";
    }
}
