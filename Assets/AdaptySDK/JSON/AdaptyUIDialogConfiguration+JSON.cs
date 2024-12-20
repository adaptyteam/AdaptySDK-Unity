//
//  AdaptyUIDialogConfiguration+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyUIDialogConfiguration
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            if (Title != null) node.Add("title", Title);
            if (Content != null) node.Add("content", Content);
            node.Add("default_action_title", DefaultActionTitle);
            if (SecondaryActionTitle != null) node.Add("secondary_action_title", SecondaryActionTitle);
            return node;
        }
    }
}

