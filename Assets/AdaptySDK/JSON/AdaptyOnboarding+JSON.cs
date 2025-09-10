//
//  AdaptyOnboarding+JSON.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using System;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyOnboarding
    {
        internal JSONNode ToJSONNode()
        {
            var node = new JSONObject();
            node.Add("placement", Placement.ToJSONNode());
            node.Add("onboarding_id", OnboardingId);
            node.Add("onboarding_name", Name);
            node.Add("variation_id", VariationId);
            if (RemoteConfig != null)
                node.Add("remote_config", RemoteConfig.ToJSONNode());

            var builder = new JSONObject();
            builder.Add("config_url", _Builder.ConfigUrl);
            node.Add("onboarding_builder", builder);

            if (_PayloadData != null)
                node.Add("payload_data", _PayloadData);
            node.Add("response_created_at", _ResponseCreatedAt);
            node.Add("request_locale", _RequestLocale);
            return node;
        }

        internal AdaptyOnboarding(JSONObject jsonNode)
        {
            Placement = jsonNode.GetPlacement("placement");
            OnboardingId = jsonNode.GetString("onboarding_id");
            Name = jsonNode.GetString("onboarding_name");
            VariationId = jsonNode.GetString("variation_id");
            RemoteConfig = jsonNode.GetRemoteConfigIfPresent("remote_config");

            var builder = JSONNodeExtensions.GetObject(jsonNode, "onboarding_builder");
            _Builder = new OnboardingBuilder(builder.GetString("config_url"));

            _PayloadData = jsonNode.GetStringIfPresent("payload_data");
            _ResponseCreatedAt = jsonNode.GetInteger("response_created_at");
            _RequestLocale = jsonNode.GetString("request_locale");
        }
    }
}

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static AdaptyOnboarding GetOnboarding(this JSONNode node) =>
            new AdaptyOnboarding(JSONNodeExtensions.GetObject(node));

        internal static AdaptyOnboarding GetOnboarding(this JSONNode node, string aKey) =>
            new AdaptyOnboarding(JSONNodeExtensions.GetObject(node, aKey));
    }
}
