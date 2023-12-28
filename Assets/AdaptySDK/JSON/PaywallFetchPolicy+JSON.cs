//
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

                Int64? maxAgeInMilliseconds = _MaxAge.HasValue ? (Int64)_MaxAge.Value.TotalMilliseconds : null;

                var node = new JSONObject();
                node.Add("type", _Type);
                if (maxAgeInMilliseconds != null) node.Add("max_age", maxAgeInMilliseconds);
                return node;
            }
        }
    }
}