// AdaptyRemoteConfig.cs
// AdaptySDK
//
// Created by Aleksei Goncharov on 09.09.2025.

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    using AdaptySDK.Serialization;

    [DataContract]
    public partial class AdaptyRemoteConfig
    {
        private AdaptyRemoteConfig() { }

        [DataMember(Name = "lang", IsRequired = true)]
        public readonly string Locale;
        [DataMember(Name = "data", IsRequired = true)]
        public readonly string Data;

        /// A custom dictionary configured in Adapty Dashboard for this paywall (same as `remoteConfigString`)
        public IDictionary<string, object> Dictionary
        {
            get
            {
                if (string.IsNullOrEmpty(Data))
                {
                    return null;
                }

                return AdaptyJson.Deserialize<IDictionary<string, object>>(Data);
            }
        }
    }
}
