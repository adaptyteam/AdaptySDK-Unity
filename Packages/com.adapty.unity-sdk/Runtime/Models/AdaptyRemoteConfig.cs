using UnityEngine.Scripting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using AdaptySDK.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// The JSON configured against a flow in the Dashboard, for one localization.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyRemoteConfig
    {
        private AdaptyRemoteConfig() { }

        /// <summary>
        /// The localization this config belongs to.
        /// </summary>
        [DataMember(Name = "lang", IsRequired = true)]
        public readonly string Locale;
        /// <summary>
        /// The configured JSON, as the string it was written as. <see cref="Dictionary"/> parses it.
        /// </summary>
        [DataMember(Name = "data", IsRequired = true)]
        public readonly string Data;

        /// <summary>
        /// A custom dictionary configured in Adapty Dashboard for this paywall (same as `remoteConfigString`)
        /// </summary>
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
