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

            var defaultAction = new JSONObject();
            defaultAction.Add("title", DefaultActionTitle);
            node.Add("default_action", defaultAction);

            if (SecondaryActionTitle != null)
            {
                var action = new JSONObject();
                action.Add("title", SecondaryActionTitle);
                node.Add("secondary_action", action);
            }
            return node;
        }
    }
}

