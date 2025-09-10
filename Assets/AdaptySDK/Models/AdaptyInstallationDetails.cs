//
//  AdaptyInstallationDetails.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    public partial class AdaptyInstallationDetails
    {
        public readonly string InstallId; // nullable
        public readonly string InstallTime; // Date string, non-null
        public readonly int AppLaunchCount; // non-null
        public readonly string Payload; // nullable

        public override string ToString() =>
            $"{nameof(InstallId)}: {InstallId}, "
            + $"{nameof(InstallTime)}: {InstallTime}, "
            + $"{nameof(AppLaunchCount)}: {AppLaunchCount}, "
            + $"{nameof(Payload)}: {Payload}";
    }
}
