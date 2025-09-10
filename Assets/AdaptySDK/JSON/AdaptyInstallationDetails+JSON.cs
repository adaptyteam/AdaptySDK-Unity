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
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            if (InstallId != null)
                node.Add("install_id", InstallId);
            node.Add("install_time", InstallTime);
            node.Add("app_launch_count", AppLaunchCount);
            if (Payload != null)
                node.Add("payload", Payload);
            return node;
        }

        internal AdaptyInstallationDetails(JSONObject jsonNode)
        {
            InstallId = jsonNode.GetStringIfPresent("install_id");
            InstallTime = jsonNode.GetString("install_time");
            AppLaunchCount = jsonNode.GetInteger("app_launch_count");
            Payload = jsonNode.GetStringIfPresent("payload");
        }
    }
}
