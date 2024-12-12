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
            this.ApiKey = builder.ApiKey;
            this.CustomerUserId = builder.CustomerUserId;
            this.ObserverMode = builder.ObserverMode;
            this.IdfaCollectionDisabled = builder.IdfaCollectionDisabled;
            this.IpAddressCollectionDisabled = builder.IpAddressCollectionDisabled;
            this.BackendBaseUrl = builder.BackendBaseUrl;
            this.BackendFallbackBaseUrl = builder.BackendFallbackBaseUrl;
            this.BackendConfigsBaseUrl = builder.BackendConfigsBaseUrl;
            this.BackendProxyHost = builder.BackendProxyHost;
            this.BackendProxyPort = builder.BackendProxyPort;
            this.LogLevel = builder.LogLevel;
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
                this.ApiKey = apiKey;

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
                this.ApiKey = apiKey;
                return this;
            }

            public Builder SetCustomerUserId(string customerUserId)
            {
                this.CustomerUserId = customerUserId;
                return this;
            }

            public Builder SetObserverMode(bool observerMode)
            {
                this.ObserverMode = observerMode;
                return this;
            }

            public Builder SetIDFACollectionDisabled(bool idfaCollectionDisabled)
            {
                this.IdfaCollectionDisabled = idfaCollectionDisabled;
                return this;
            }

            public Builder SetIPAddressCollectionDisabled(bool ipAddressCollectionDisabled)
            {
                this.IpAddressCollectionDisabled = ipAddressCollectionDisabled;
                return this;
            }

            public Builder SetServerCluster(AdaptyServerCluster serverCluster)
            {
                this.ServerCluster = serverCluster;
                return this;
            }

            public Builder SetBackendBaseUrl(string backendBaseUrl)
            {
                this.BackendBaseUrl = backendBaseUrl;
                return this;
            }

            public Builder SetBackendFallbackBaseUrl(string backendFallbackBaseUrl)
            {
                this.BackendFallbackBaseUrl = backendFallbackBaseUrl;
                return this;
            }

            public Builder SetBackendConfigsBaseUrl(string backendConfigsBaseUrl)
            {
                this.BackendConfigsBaseUrl = backendConfigsBaseUrl;
                return this;
            }

            public Builder SetBackendProxy(string host, int port)
            {
                this.BackendProxyHost = host;
                this.BackendProxyPort = port;
                return this;
            }
        }
    }
}