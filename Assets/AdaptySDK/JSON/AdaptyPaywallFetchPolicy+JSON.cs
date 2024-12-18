//
//  AdaptyPaywallFetchPolicy+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 26.12.2023.
//

namespace AdaptySDK
{
    using System;
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyPaywallFetchPolicy
    {
        internal JSONNode ToJSONNode()
        {
            double? maxAgeInSeconds = _MaxAge.HasValue ? _MaxAge.Value.TotalSeconds : null;

            var node = new JSONObject();
            node.Add("type", _Type);
            if (maxAgeInSeconds != null) node.Add("max_age", maxAgeInSeconds);
            return node;
        }
    }
}