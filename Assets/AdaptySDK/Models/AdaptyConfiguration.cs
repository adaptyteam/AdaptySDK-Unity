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
        private readonly string CustomerUserId; //nullable
        private readonly bool? ObserverMode;
        private readonly bool? IdfaCollectionDisabled;
        private readonly bool? IpAddressCollectionDisabled;
        private readonly AdaptyServerCluster? ServerCluster;
        private readonly string BackendBaseUrl; //nullable
        private readonly string BackendFallbackBaseUrl; //nullable
        private readonly string BackendConfigsBaseUrl; //nullable
        private readonly string BackendProxyHost; //nullable
        private readonly int? BackendProxyPort; //nullable
        private readonly AdaptyLogLevel? LogLevel;
        private readonly bool? ActivateUI;  
        private AdaptyUIMediaCacheConfiguration AdaptyUIMediaCache;


        public override string ToString() =>
            $"{nameof(ApiKey)}: {ApiKey}, " +
            $"{nameof(CustomerUserId)}: {CustomerUserId}, " +
            $"{nameof(ObserverMode)}: {ObserverMode}, " +
            $"{nameof(IdfaCollectionDisabled)}: {IdfaCollectionDisabled}, " +
            $"{nameof(IpAddressCollectionDisabled)}: {IpAddressCollectionDisabled}, " +            
            $"{nameof(ServerCluster)}: {ServerCluster}, " +
            $"{nameof(BackendBaseUrl)}: {BackendBaseUrl}, " +
            $"{nameof(BackendFallbackBaseUrl)}: {BackendFallbackBaseUrl}, " +
            $"{nameof(BackendProxyHost)}: {BackendProxyHost}, " +
            $"{nameof(BackendProxyPort)}: {BackendProxyPort}, " +
            $"{nameof(ActivateUI)}: {ActivateUI}, " +
            $"{nameof(AdaptyUIMediaCache)}: {AdaptyUIMediaCache}, " +
            $"{nameof(LogLevel)}: {LogLevel}";
    }
}