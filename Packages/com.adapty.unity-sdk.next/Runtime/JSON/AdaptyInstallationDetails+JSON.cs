//
//  AdaptyInstallationDetails+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyInstallationDetails
    {
        internal AdaptyInstallationDetails(JSONObject jsonNode)
        {
            InstallId = jsonNode.GetStringIfPresent("install_id");
            InstallTime = jsonNode.GetDateTime("install_time");
            AppLaunchCount = jsonNode.GetInteger("app_launch_count");
            Payload = jsonNode.GetStringIfPresent("payload");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyInstallationDetails GetAdaptyInstallationDetails(
            this JSONNode node
        ) => new AdaptyInstallationDetails(JSONNodeExtensions.GetObject(node));

        internal static AdaptyInstallationDetails GetAdaptyInstallationDetails(
            this JSONNode node,
            string aKey
        ) => new AdaptyInstallationDetails(JSONNodeExtensions.GetObject(node, aKey));
    }
}
