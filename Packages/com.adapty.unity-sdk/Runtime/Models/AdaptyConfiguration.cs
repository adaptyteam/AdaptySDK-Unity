using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    public sealed partial class AdaptyConfiguration
    {
        [DataMember(Name = "api_key", IsRequired = true)]
        private readonly string ApiKey;
        [DataMember(Name = "customer_user_id")]
        private readonly string CustomerUserId; // nullable
        [DataMember(Name = "customer_identity_parameters")]
        private readonly AdaptyCustomerIdentity CustomerIdentity; // nullable
        [DataMember(Name = "observer_mode")]
        private readonly bool? ObserverMode;
        private readonly bool? AppleIdfaCollectionDisabled;
        [DataMember(Name = "google_adid_collection_disabled")]
        private readonly bool? GoogleAdvertisingIdCollectionDisabled;
        [DataMember(Name = "google_enable_pending_prepaid_plans")]
        private readonly bool? GoogleEnablePendingPrepaidPlans;
        [DataMember(Name = "google_local_access_level_allowed")]
        private readonly bool? GoogleLocalAccessLevelAllowed;
        [DataMember(Name = "ip_address_collection_disabled")]
        private readonly bool? IpAddressCollectionDisabled;
        [DataMember(Name = "clear_data_on_backup")]
        private readonly bool? AppleClearDataOnBackup;
        [DataMember(Name = "server_cluster")]
        private readonly AdaptyServerCluster? ServerCluster;
        [DataMember(Name = "backend_proxy_host")]
        private readonly string BackendProxyHost; // nullable
        [DataMember(Name = "backend_proxy_port")]
        private readonly int? BackendProxyPort; // nullable
        [DataMember(Name = "log_level")]
        private readonly AdaptyLogLevel? LogLevel;
        [DataMember(Name = "activate_ui")]
        private readonly bool? ActivateUI;
        [DataMember(Name = "media_cache")]
        private AdaptyUIMediaCacheConfiguration AdaptyUIMediaCache;


        /// <remarks>
        /// The KidsMode trait compiles IDFA out of the binary; keep the request in sync.
        /// </remarks>
        [DataMember(Name = "apple_idfa_collection_disabled")]
        [Preserve]
        private bool? AppleIdfaCollectionDisabledForRequest =>
#if ADAPTY_KIDS_MODE && UNITY_IOS
            true;
#else
            AppleIdfaCollectionDisabled;
#endif

        [DataMember(Name = "cross_platform_sdk_name")]
        [Preserve]
        private string CrossPlatformSdkName => "unity";

        [DataMember(Name = "cross_platform_sdk_version")]
        [Preserve]
        private string CrossPlatformSdkVersion => Adapty.SDKVersion;

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
