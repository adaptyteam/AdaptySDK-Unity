//
//  AdaptyConfiguration.Builder.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 10.12.2024.
//

using System;

namespace AdaptySDK
{
    public partial class AdaptyConfiguration
    {
        internal AdaptyConfiguration(Builder builder)
        {
            ApiKey = builder.ApiKey;
            CustomerUserId = builder.CustomerUserId;
            CustomerIdentity = builder.CustomerIdentity;
            ObserverMode = builder.ObserverMode;
            AppleIdfaCollectionDisabled = builder.AppleIdfaCollectionDisabled;
            GoogleAdvertisingIdCollectionDisabled = builder.GoogleAdvertisingIdCollectionDisabled;
            GoogleEnablePendingPrepaidPlans = builder.GoogleEnablePendingPrepaidPlans;
            IpAddressCollectionDisabled = builder.IpAddressCollectionDisabled;
            BackendProxyHost = builder.BackendProxyHost;
            BackendProxyPort = builder.BackendProxyPort;
            LogLevel = builder.LogLevel;
            ActivateUI = builder.ActivateUI;
            AdaptyUIMediaCache = builder.AdaptyUIMediaCache;
        }

        public class Builder
        {
            public string ApiKey;
            public string CustomerUserId; //nullable
            public AdaptyCustomerIdentity CustomerIdentity; // nullable
            public bool? ObserverMode;

            [Obsolete(
                "IdfaCollectionDisabled is deprecated, please use AppleIdfaCollectionDisabled instead."
            )]
            public bool IdfaCollectionDisabled
            {
                get { return AppleIdfaCollectionDisabled; }
                set { AppleIdfaCollectionDisabled = value; }
            }
            public bool AppleIdfaCollectionDisabled;
            public bool GoogleAdvertisingIdCollectionDisabled;
            public bool GoogleEnablePendingPrepaidPlans;
            public bool IpAddressCollectionDisabled;
            public AdaptyServerCluster ServerCluster;
            public string BackendProxyHost; //nullable
            public int BackendProxyPort;
            public AdaptyLogLevel LogLevel;
            public bool ActivateUI;
            public AdaptyUIMediaCacheConfiguration AdaptyUIMediaCache; //nullable

            public Builder(string apiKey) => ApiKey = apiKey;

            public AdaptyConfiguration Build() => new AdaptyConfiguration(this);

            public override string ToString() =>
                $"{nameof(ApiKey)}: {ApiKey}, "
                + $"{nameof(CustomerUserId)}: {CustomerUserId}, "
                + $"{nameof(CustomerIdentity)}: {CustomerIdentity}, "
                + $"{nameof(ObserverMode)}: {ObserverMode}, "
                + $"{nameof(AppleIdfaCollectionDisabled)}: {AppleIdfaCollectionDisabled}, "
                + $"{nameof(GoogleAdvertisingIdCollectionDisabled)}: {GoogleAdvertisingIdCollectionDisabled}, "
                + $"{nameof(IpAddressCollectionDisabled)}: {IpAddressCollectionDisabled}, "
                + $"{nameof(ServerCluster)}: {ServerCluster}, "
                + $"{nameof(BackendProxyHost)}: {BackendProxyHost}, "
                + $"{nameof(BackendProxyPort)}: {BackendProxyPort}, "
                + $"{nameof(ActivateUI)}: {ActivateUI}, "
                + $"{nameof(AdaptyUIMediaCache)}: {AdaptyUIMediaCache}, "
                + $"{nameof(LogLevel)}: {LogLevel}";

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

            public Builder SetCustomerUserId(
                string customerUserId,
                Guid iosAppAccountToken, // nullable
                string androidObfuscatedAccountId // nullable
            )
            {
                CustomerUserId = customerUserId;
                CustomerIdentity = new AdaptyCustomerIdentity(
                    iosAppAccountToken,
                    androidObfuscatedAccountId
                );
                return this;
            }

            public Builder SetObserverMode(bool observerMode)
            {
                ObserverMode = observerMode;
                return this;
            }

            [Obsolete(
                "SetIDFACollectionDisabled is deprecated, please use SetAppleIDFACollectionDisabled instead."
            )]
            public Builder SetIDFACollectionDisabled(bool appleIdfaCollectionDisabled)
            {
                return SetAppleIDFACollectionDisabled(appleIdfaCollectionDisabled);
            }

            public Builder SetAppleIDFACollectionDisabled(bool appleIdfaCollectionDisabled)
            {
                AppleIdfaCollectionDisabled = appleIdfaCollectionDisabled;
                return this;
            }

            public Builder SetGoogleAdvertisingIdCollectionDisabled(
                bool googleAdvertisingIdCollectionDisabled
            )
            {
                GoogleAdvertisingIdCollectionDisabled = googleAdvertisingIdCollectionDisabled;
                return this;
            }

            public Builder SetGoogleEnablePendingPrepaidPlans(bool googleEnablePendingPrepaidPlans)
            {
                GoogleEnablePendingPrepaidPlans = googleEnablePendingPrepaidPlans;
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

            public Builder SetBackendProxy(string host, int port)
            {
                BackendProxyHost = host;
                BackendProxyPort = port;
                return this;
            }

            public Builder SetActivateUI(bool activate)
            {
                ActivateUI = activate;
                return this;
            }

            public Builder SetAdaptyUIMediaCache(
                int? memoryStorageTotalCostLimit,
                int? memoryStorageCountLimit,
                int? diskStorageSizeLimit
            )
            {
                AdaptyUIMediaCache = new AdaptyUIMediaCacheConfiguration(
                    memoryStorageTotalCostLimit,
                    memoryStorageCountLimit,
                    diskStorageSizeLimit
                );
                return this;
            }
        }
    }
}
