using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    using AdaptySDK.Serialization;

    [DataContract]
    [Preserve]
    public sealed class AdaptyRemoteConfig
    {
        private AdaptyRemoteConfig() { }

        [DataMember(Name = "lang", IsRequired = true)]
        public readonly string Locale;
        [DataMember(Name = "data", IsRequired = true)]
        public readonly string Data;

        /// A custom dictionary configured in Adapty Dashboard for this paywall (same as `remoteConfigString`)
        public IReadOnlyDictionary<string, object> Dictionary
        {
            get
            {
                if (string.IsNullOrEmpty(Data))
                {
                    return null;
                }

                return new ReadOnlyDictionary<string, object>(
                    AdaptyJson.DeserializeRemoteConfigDictionary(Data)
                );
            }
        }
    }
}
