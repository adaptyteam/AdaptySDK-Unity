//
//  AdaptyInstallationStatus.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    public enum AdaptyInstallationStatusType
    {
        NotAvailable,
        NotDetermined,
        Determined,
    }

    public partial class AdaptyInstallationStatus
    {
        public readonly AdaptyInstallationStatusType Status;
        public readonly AdaptyInstallationDetails Details; // nullable unless status == determined

        public override string ToString() =>
            $"{nameof(Status)}: {Status}, " + $"{nameof(Details)}: {Details}";
    }
}
