using UnityEngine.Scripting;
using System;

namespace AdaptySDK
{
    /// <summary>
    /// Everything <see cref="Adapty.Activate(AdaptyConfiguration, System.Action{AdaptyError})"/>
    /// needs. Build one with <see cref="Builder"/>.
    /// </summary>
    [Preserve]
    public sealed partial class AdaptyConfiguration
    {
        internal AdaptyConfiguration(Builder builder)
        {
            ApiKey = builder.ApiKey;
            CustomerUserId = builder.CustomerUserId;
            CustomerIdentity = builder.CustomerIdentity;
            ObserverMode = builder.ObserverMode;
            // The KidsMode trait compiles IDFA out of the binary; keep the request in sync.
            AppleIdfaCollectionDisabled =
#if ADAPTY_KIDS_MODE && UNITY_IOS
                true;
#else
                builder.AppleIdfaCollectionDisabled;
#endif
            GoogleAdvertisingIdCollectionDisabled = builder.GoogleAdvertisingIdCollectionDisabled;
            GoogleEnablePendingPrepaidPlans = builder.GoogleEnablePendingPrepaidPlans;
            GoogleLocalAccessLevelAllowed = builder.GoogleLocalAccessLevelAllowed;
            IpAddressCollectionDisabled = builder.IpAddressCollectionDisabled;
            AppleClearDataOnBackup = builder.AppleClearDataOnBackup;
            ServerCluster = builder.ServerCluster;
            BackendProxyHost = builder.BackendProxyHost;
            BackendProxyPort = builder.BackendProxyPort;
            LogLevel = builder.LogLevel;
            ActivateUI = builder.ActivateUI;
            AdaptyUIMediaCache = builder.AdaptyUIMediaCache;

            // Not sent when it carries neither value, as Identify does not send it either.
            if (CustomerIdentity != null && CustomerIdentity.IsEmpty)
            {
                CustomerIdentity = null;
            }
        }

        /// <summary>
        /// Assembles an <see cref="AdaptyConfiguration"/>. Every setter returns the builder, so
        /// calls chain, and each field can equally well be assigned directly.
        /// </summary>
        /// <remarks>
        /// Only the API key is required, but "not set" does not mean "not sent". The nullable
        /// members — <see cref="ObserverMode"/>, <see cref="GoogleLocalAccessLevelAllowed"/>,
        /// <see cref="AppleClearDataOnBackup"/>, <see cref="ServerCluster"/>,
        /// <see cref="CustomerUserId"/>, <see cref="CustomerIdentity"/>,
        /// <see cref="BackendProxyHost"/> and <see cref="AdaptyUIMediaCache"/> — are left out of
        /// the request when null, and the native SDK applies its own default. The rest are not
        /// nullable and always go, carrying whatever they hold: a configuration that touches none
        /// of them still sends <c>false</c> for both IDFA flags and the IP one,
        /// <c>activate_ui: false</c>, <c>log_level: "error"</c> and <c>backend_proxy_port: 0</c>.
        /// </remarks>
        public sealed class Builder
        {
            /// <summary>
            /// The public SDK key from the Adapty Dashboard. Required.
            /// </summary>
            public string ApiKey;

            /// <summary>
            /// The identifier of the user in your system, when you already know it at activation.
            /// Null to stay anonymous and call <see cref="Adapty.Identify(System.String, System.Action{AdaptySDK.AdaptyError})"/> later.
            /// </summary>
            public string CustomerUserId; // nullable

            /// <summary>
            /// The store account identifiers to attribute purchases with. Null, or an instance
            /// carrying neither value, is not sent.
            /// </summary>
            public AdaptyCustomerIdentity CustomerIdentity; // nullable

            /// <summary>
            /// Observer mode: your own code makes the purchases and Adapty only observes them.
            /// Null leaves the native default, which is off.
            /// </summary>
            public bool? ObserverMode;

            /// <summary>
            /// iOS only. Stops the SDK collecting the IDFA. Forced on, whatever this says, when
            /// the <c>ADAPTY_KIDS_MODE</c> scripting define is set, since the trait compiles IDFA
            /// out of the binary.
            /// </summary>
            public bool AppleIdfaCollectionDisabled;

            /// <summary>
            /// Android only. Stops the SDK collecting the Google Advertising ID.
            /// </summary>
            public bool GoogleAdvertisingIdCollectionDisabled;

            /// <summary>
            /// Android only. Reports pending transactions for
            /// <see href="https://developer.android.com/google/play/billing/subscriptions#prepaid-plans">prepaid plans</see>.
            /// </summary>
            public bool GoogleEnablePendingPrepaidPlans;

            /// <summary>
            /// Android only.
            /// <see href="https://adapty.io/docs/local-access-levels">Local access levels</see>:
            /// when Adapty's servers cannot be reached after a purchase, the SDK verifies it
            /// against the store instead and grants the access level on the device. Null leaves
            /// the native default, which is off.
            /// </summary>
            public bool? GoogleLocalAccessLevelAllowed;

            /// <summary>
            /// Stops the SDK collecting the device's IP address.
            /// </summary>
            public bool IpAddressCollectionDisabled;

            /// <summary>
            /// iOS only. Clears the SDK's stored data when the app is restored from an iCloud
            /// backup, so a restored device does not carry the previous one's profile. Null
            /// leaves the native default, which is off.
            /// </summary>
            public bool? AppleClearDataOnBackup;

            /// <summary>
            /// Which Adapty server region to talk to. Null uses the default cluster.
            /// </summary>
            public AdaptyServerCluster? ServerCluster;

            /// <summary>
            /// The host of a proxy to route Adapty's backend calls through. Null for none.
            /// </summary>
            public string BackendProxyHost; // nullable

            /// <summary>
            /// The port of the proxy named by <see cref="BackendProxyHost"/>. Ignored without it.
            /// </summary>
            public int BackendProxyPort;

            /// <summary>
            /// How much the native SDK logs. Also settable at any time with
            /// <see cref="Adapty.SetLogLevel(AdaptySDK.AdaptyLogLevel, System.Action{AdaptySDK.AdaptyError})"/>.
            /// </summary>
            public AdaptyLogLevel LogLevel;

            /// <summary>
            /// Activates the flow rendering module along with the SDK. Required before
            /// <see cref="AdaptyUI.CreateFlowView(AdaptySDK.AdaptyFlow, AdaptySDK.AdaptyUICreateFlowViewParameters, System.Action{AdaptySDK.AdaptyUIFlowView, AdaptySDK.AdaptyError})"/> can build a view.
            /// </summary>
            public bool ActivateUI;

            /// <summary>
            /// Limits for the cache the flow renderer keeps for images and video. Null leaves the
            /// native defaults.
            /// </summary>
            public AdaptyUIMediaCacheConfiguration AdaptyUIMediaCache; // nullable

            /// <summary>
            /// Starts a configuration for the given API key.
            /// </summary>
            /// <param name="apiKey">The public SDK key from the Adapty Dashboard.</param>
            public Builder(string apiKey) => ApiKey = apiKey;

            /// <summary>
            /// The configuration described by this builder.
            /// </summary>
            public AdaptyConfiguration Build() => new AdaptyConfiguration(this);

            /// <summary>
            /// A description for logs and the debugger. The format is not part of the contract —
            /// read the members rather than parsing it.
            /// </summary>
            public override string ToString() =>
                $"{nameof(ApiKey)}: {ApiKey}, "
                + $"{nameof(CustomerUserId)}: {CustomerUserId}, "
                + $"{nameof(CustomerIdentity)}: {CustomerIdentity}, "
                + $"{nameof(ObserverMode)}: {ObserverMode}, "
                + $"{nameof(AppleIdfaCollectionDisabled)}: {AppleIdfaCollectionDisabled}, "
                + $"{nameof(GoogleAdvertisingIdCollectionDisabled)}: {GoogleAdvertisingIdCollectionDisabled}, "
                + $"{nameof(GoogleLocalAccessLevelAllowed)}: {GoogleLocalAccessLevelAllowed}, "
                + $"{nameof(IpAddressCollectionDisabled)}: {IpAddressCollectionDisabled}, "
                + $"{nameof(AppleClearDataOnBackup)}: {AppleClearDataOnBackup}, "
                + $"{nameof(ServerCluster)}: {ServerCluster}, "
                + $"{nameof(BackendProxyHost)}: {BackendProxyHost}, "
                + $"{nameof(BackendProxyPort)}: {BackendProxyPort}, "
                + $"{nameof(ActivateUI)}: {ActivateUI}, "
                + $"{nameof(AdaptyUIMediaCache)}: {AdaptyUIMediaCache}, "
                + $"{nameof(LogLevel)}: {LogLevel}";

            /// <summary>Replaces the API key the builder was created with.</summary>
            /// <param name="apiKey">The public SDK key from the Adapty Dashboard.</param>
            public Builder SetAPIKey(string apiKey)
            {
                ApiKey = apiKey;
                return this;
            }

            /// <summary>Sets <see cref="CustomerUserId"/>.</summary>
            /// <param name="customerUserId">The identifier of the user in your system.</param>
            public Builder SetCustomerUserId(string customerUserId)
            {
                CustomerUserId = customerUserId;
                return this;
            }

            /// <summary>
            /// Sets <see cref="CustomerUserId"/> together with the store account identifiers to
            /// attribute purchases with. An identity carrying neither of them is not sent.
            /// </summary>
            /// <param name="customerUserId">The identifier of the user in your system.</param>
            /// <param name="iosAppAccountToken">
            /// iOS only. The UUID tying a purchase to its App Store transaction;
            /// <see cref="System.Guid.Empty"/> for none.
            /// </param>
            /// <param name="androidObfuscatedAccountId">
            /// Android only. The obfuscated account identifier Google Play records; null for none.
            /// </param>
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

            /// <summary>Sets <see cref="ObserverMode"/>.</summary>
            /// <param name="observerMode">True when your own code makes the purchases and Adapty only observes them.</param>
            public Builder SetObserverMode(bool observerMode)
            {
                ObserverMode = observerMode;
                return this;
            }

            /// <summary>Sets <see cref="AppleIdfaCollectionDisabled"/>. iOS only.</summary>
            /// <param name="appleIdfaCollectionDisabled">True to stop the SDK collecting the IDFA.</param>
            public Builder SetAppleIDFACollectionDisabled(bool appleIdfaCollectionDisabled)
            {
                AppleIdfaCollectionDisabled = appleIdfaCollectionDisabled;
                return this;
            }

            /// <summary>Sets <see cref="GoogleAdvertisingIdCollectionDisabled"/>. Android only.</summary>
            /// <param name="googleAdvertisingIdCollectionDisabled">True to stop the SDK collecting the Google Advertising ID.</param>
            public Builder SetGoogleAdvertisingIdCollectionDisabled(
                bool googleAdvertisingIdCollectionDisabled
            )
            {
                GoogleAdvertisingIdCollectionDisabled = googleAdvertisingIdCollectionDisabled;
                return this;
            }

            /// <summary>Sets <see cref="GoogleEnablePendingPrepaidPlans"/>. Android only.</summary>
            /// <param name="googleEnablePendingPrepaidPlans">True to report pending transactions for prepaid plans.</param>
            public Builder SetGoogleEnablePendingPrepaidPlans(bool googleEnablePendingPrepaidPlans)
            {
                GoogleEnablePendingPrepaidPlans = googleEnablePendingPrepaidPlans;
                return this;
            }

            /// <summary>Sets <see cref="GoogleLocalAccessLevelAllowed"/>. Android only.</summary>
            /// <param name="googleLocalAccessLevelAllowed">True to grant access levels on the device when Adapty cannot be reached.</param>
            public Builder SetGoogleLocalAccessLevelAllowed(bool googleLocalAccessLevelAllowed)
            {
                GoogleLocalAccessLevelAllowed = googleLocalAccessLevelAllowed;
                return this;
            }

            /// <summary>Sets <see cref="IpAddressCollectionDisabled"/>.</summary>
            /// <param name="ipAddressCollectionDisabled">True to stop the SDK collecting the device's IP address.</param>
            public Builder SetIPAddressCollectionDisabled(bool ipAddressCollectionDisabled)
            {
                IpAddressCollectionDisabled = ipAddressCollectionDisabled;
                return this;
            }

            /// <summary>Sets <see cref="AppleClearDataOnBackup"/>. iOS only.</summary>
            /// <param name="appleClearDataOnBackup">True to clear stored data when the app is restored from an iCloud backup.</param>
            public Builder SetAppleClearDataOnBackup(bool appleClearDataOnBackup)
            {
                AppleClearDataOnBackup = appleClearDataOnBackup;
                return this;
            }

            /// <summary>Sets <see cref="ServerCluster"/>.</summary>
            /// <param name="serverCluster">The Adapty server region to talk to.</param>
            public Builder SetServerCluster(AdaptyServerCluster serverCluster)
            {
                ServerCluster = serverCluster;
                return this;
            }

            /// <summary>Sets <see cref="BackendProxyHost"/> and <see cref="BackendProxyPort"/>.</summary>
            /// <param name="host">The proxy host to route Adapty's backend calls through.</param>
            /// <param name="port">The port on that host.</param>
            public Builder SetBackendProxy(string host, int port)
            {
                BackendProxyHost = host;
                BackendProxyPort = port;
                return this;
            }

            /// <summary>Sets <see cref="ActivateUI"/>.</summary>
            /// <param name="activate">True to activate the flow rendering module along with the SDK.</param>
            public Builder SetActivateUI(bool activate)
            {
                ActivateUI = activate;
                return this;
            }

            /// <summary>Sets <see cref="AdaptyUIMediaCache"/>. Null leaves a native default.</summary>
            /// <param name="memoryStorageTotalCostLimit">In-memory cache limit, in bytes.</param>
            /// <param name="memoryStorageCountLimit">How many items the in-memory cache holds.</param>
            /// <param name="diskStorageSizeLimit">On-disk cache limit, in bytes.</param>
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
