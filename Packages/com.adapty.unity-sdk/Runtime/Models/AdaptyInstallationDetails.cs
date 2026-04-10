//
//  AdaptyInstallationDetails.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using System;

namespace AdaptySDK
{
    public partial class AdaptyInstallationDetails
    {
        public readonly string InstallId; // nullable
        public readonly DateTime InstallTime; // Date string, non-null
        public readonly int AppLaunchCount; // non-null
        public readonly string Payload; // nullable

        public override string ToString() =>
            $"(installId: {InstallId}, "
            + $"installTime: {InstallTime}, "
            + $"appLaunchCount: {AppLaunchCount}, "
            + $"payload: {Payload})";
    }
}
