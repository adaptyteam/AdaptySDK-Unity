//
//  AdaptyUIUserAction+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

using System;
using System.Collections.Generic;

namespace AdaptySDK {
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIUserAction {
        internal AdaptyUIUserAction(JSONObject jsonNode) {
            Type = jsonNode.GetAdaptyUIUserActionType("type");
            Value = jsonNode.GetStringIfPresent("value");
        }
    }
}

namespace AdaptySDK.SimpleJSON {
    internal static partial class JSONNodeExtensions {
        internal static AdaptyUIUserAction GetAdaptyUIUserAction(this JSONNode node, string aKey) =>
            new AdaptyUIUserAction(GetObject(node, aKey));

        internal static AdaptyUIUserAction GetAdaptyUIUserActionIfPresent(
            this JSONNode node,
            string aKey
        ) {
            var obj = GetObjectIfPresent(node, aKey);
            if (obj is null) {
                return null;
            }

            return new AdaptyUIUserAction(obj);
        }
    }
}
