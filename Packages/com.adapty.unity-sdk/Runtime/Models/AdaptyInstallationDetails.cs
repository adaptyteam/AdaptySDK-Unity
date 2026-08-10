//
//  AdaptyInstallationDetails.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public sealed class AdaptyInstallationDetails
    {
        private AdaptyInstallationDetails() { }

        [DataMember(Name = "install_id")]
        public readonly string InstallId; // nullable
        [DataMember(Name = "install_time", IsRequired = true)]
        public readonly DateTime InstallTime; // Date string, non-null
        [DataMember(Name = "app_launch_count", IsRequired = true)]
        public readonly int AppLaunchCount; // non-null
        [DataMember(Name = "payload")]
        public readonly string Payload; // nullable

        public override string ToString() =>
            $"(installId: {InstallId}, "
            + $"installTime: {InstallTime}, "
            + $"appLaunchCount: {AppLaunchCount}, "
            + $"payload: {Payload})";
    }
}
