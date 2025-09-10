// AdaptyRemoteConfig.cs
// AdaptySDK
//
// Created by Aleksei Goncharov on 09.09.2025.

using System.Collections.Generic;

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public partial class AdaptyRemoteConfig
    {
        public readonly string Locale;
        public readonly string Data;

        /// A custom dictionary configured in Adapty Dashboard for this paywall (same as `remoteConfigString`)
        public IDictionary<string, dynamic> Dictionary
        {
            get
            {
                if (string.IsNullOrEmpty(Data))
                {
                    return null;
                }

                return JSONNode.Parse(Data).GetDictionary();
            }
        }
    }
}
