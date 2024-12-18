//
//  AdaptyConfiguration.Builder.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 10.12.2024.
//

namespace AdaptySDK
{
    public partial class AdaptyConfiguration
    {
        internal AdaptyConfiguration(Builder builder)
        {
            ApiKey = builder.ApiKey;
            CustomerUserId = builder.CustomerUserId;
            ObserverMode = builder.ObserverMode;
            IdfaCollectionDisabled = builder.IdfaCollectionDisabled;
            IpAddressCollectionDisabled = builder.IpAddressCollectionDisabled;
            BackendBaseUrl = builder.BackendBaseUrl;
            BackendFallbackBaseUrl = builder.BackendFallbackBaseUrl;
            BackendConfigsBaseUrl = builder.BackendConfigsBaseUrl;
            BackendProxyHost = builder.BackendProxyHost;
            BackendProxyPort = builder.BackendProxyPort;
            LogLevel = builder.LogLevel;
        }

        public class Builder
        {
            public string ApiKey;
            public string CustomerUserId; //nullable
            public bool? ObserverMode;
            public bool? IdfaCollectionDisabled;
            public bool? IpAddressCollectionDisabled;
            public AdaptyServerCluster? ServerCluster;
            public string BackendBaseUrl; //nullable
            public string BackendFallbackBaseUrl; //nullable
            public string BackendConfigsBaseUrl; //nullable
            public string BackendProxyHost; //nullable
            public int? BackendProxyPort; //nullable
            public AdaptyLogLevel? LogLevel;


            public Builder(string apiKey) =>
                ApiKey = apiKey;

            public AdaptyConfiguration Build() =>
                new AdaptyConfiguration(this);


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
                $"{nameof(LogLevel)}: {LogLevel}";

            public Builder SetAPIKey(string apiKey)
            {
                ApiKey = apiKey;
                return this;
            }

            public Builder SetCustomerUserId(string customerUserId)
            {
                CustomerUserId = customerUserId;
                return this;
            }

            public Builder SetObserverMode(bool observerMode)
            {
                ObserverMode = observerMode;
                return this;
            }

            public Builder SetIDFACollectionDisabled(bool idfaCollectionDisabled)
            {
                IdfaCollectionDisabled = idfaCollectionDisabled;
                return this;
            }

            public Builder SetIPAddressCollectionDisabled(bool ipAddressCollectionDisabled)
            {
                IpAddressCollectionDisabled = ipAddressCollectionDisabled;
                return this;
            }

            public Builder SetServerCluster(AdaptyServerCluster serverCluster)
            {
                ServerCluster = serverCluster;
                return this;
            }

            public Builder SetBackendBaseUrl(string backendBaseUrl)
            {
                BackendBaseUrl = backendBaseUrl;
                return this;
            }

            public Builder SetBackendFallbackBaseUrl(string backendFallbackBaseUrl)
            {
                BackendFallbackBaseUrl = backendFallbackBaseUrl;
                return this;
            }

            public Builder SetBackendConfigsBaseUrl(string backendConfigsBaseUrl)
            {
                BackendConfigsBaseUrl = backendConfigsBaseUrl;
                return this;
            }

            public Builder SetBackendProxy(string host, int port)
            {
                BackendProxyHost = host;
                BackendProxyPort = port;
                return this;
            }
        }
    }
}