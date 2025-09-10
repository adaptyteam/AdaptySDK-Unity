//
//  AdaptyInstallationStatus+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyInstallationStatus
    {
        internal AdaptyInstallationStatus(JSONObject jsonNode)
        {
            var statusString = jsonNode.GetString("status");
            switch (statusString)
            {
                case "determined":
                    Status = AdaptyInstallationStatusType.Determined;
                    break;
                case "not_available":
                    Status = AdaptyInstallationStatusType.NotAvailable;
                    break;
                case "not_determined":
                    Status = AdaptyInstallationStatusType.NotDetermined;
                    break;
                default:
                    throw new Exception(
                        $"Unknown AdaptyInstallationStatus status: '{statusString}'"
                    );
            }
            var detailsObj = JSONNodeExtensions.GetObjectIfPresent(jsonNode, "details");
            Details = detailsObj is null ? null : new AdaptyInstallationDetails(detailsObj);
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyInstallationStatus GetInstallationStatus(this JSONNode node) =>
            new AdaptyInstallationStatus(JSONNodeExtensions.GetObject(node));

        internal static AdaptyInstallationStatus GetInstallationStatus(
            this JSONNode node,
            string aKey
        ) => new AdaptyInstallationStatus(JSONNodeExtensions.GetObject(node, aKey));
    }
}
