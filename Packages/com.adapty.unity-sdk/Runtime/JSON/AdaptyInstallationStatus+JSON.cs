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

    internal static partial class AdaptyInstallationStatusFactory
    {
        internal static AdaptyInstallationStatus CreateFromJSON(JSONObject jsonNode)
        {
            var statusString = jsonNode.GetString("status");
            switch (statusString)
            {
                case "determined":
                    var detailsObj = JSONNodeExtensions.GetObjectIfPresent(jsonNode, "details");
                    if (detailsObj == null)
                    {
                        throw new Exception(
                            "AdaptyInstallationStatus 'determined' requires 'details' field"
                        );
                    }
                    return new AdaptyInstallationStatusDetermined(
                        new AdaptyInstallationDetails(detailsObj)
                    );
                case "not_available":
                    return new AdaptyInstallationStatusNotAvailable();
                case "not_determined":
                    return new AdaptyInstallationStatusNotDetermined();
                default:
                    throw new Exception(
                        $"Unknown AdaptyInstallationStatus status: '{statusString}'"
                    );
            }
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyInstallationStatus GetInstallationStatus(this JSONNode node) =>
            AdaptyInstallationStatusFactory.CreateFromJSON(JSONNodeExtensions.GetObject(node));

        internal static AdaptyInstallationStatus GetInstallationStatus(
            this JSONNode node,
            string aKey
        ) => AdaptyInstallationStatusFactory.CreateFromJSON(JSONNodeExtensions.GetObject(node, aKey));
    }
}
