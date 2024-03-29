﻿//
//  PaywallFetchPolicy+JSON.cs
//  Adapty
//
//  Created by Aleksei Valiano on 26.12.2023.
//

namespace AdaptySDK
{
    using System;
    using AdaptySDK.SimpleJSON;

    public static partial class Adapty
    {
        public partial class PaywallFetchPolicy
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
}