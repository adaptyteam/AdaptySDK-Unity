//
//  AdaptyConfiguration+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 10.12.2024.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;
    public partial class AdaptyConfiguration
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("api_key", ApiKey);
            if (CustomerUserId != null) node.Add("customer_user_id", CustomerUserId);
            if (ObserverMode != null) node.Add("observer_mode", ObserverMode);
            if (IdfaCollectionDisabled != null) node.Add("idfa_collection_disabled", IdfaCollectionDisabled);
            if (IpAddressCollectionDisabled != null) node.Add("ip_address_collection_disabled", IpAddressCollectionDisabled);
            if (LogLevel != null) node.Add("log_level", LogLevel.Value.ToJSON());
            if (ServerCluster != null) node.Add("server_cluster", ServerCluster.Value.ToJSON());
            if (BackendBaseUrl != null) node.Add("backend_base_url", BackendBaseUrl);
            if (BackendFallbackBaseUrl != null) node.Add("backend_fallback_base_url", BackendFallbackBaseUrl);
            if (BackendConfigsBaseUrl != null) node.Add("backend_configs_base_url", BackendConfigsBaseUrl);
            if (BackendProxyHost != null) node.Add("backend_proxy_host", BackendProxyHost);
            if (BackendProxyPort != null) node.Add("backend_proxy_port", BackendProxyPort);

            node.Add("cross_platform_sdk_name", "unity");
            node.Add("cross_platform_sdk_version", Adapty.sdkVersion);
            return node;
        }
    }
}