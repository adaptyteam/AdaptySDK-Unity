//
//  AdaptyInstallationStatus.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

namespace AdaptySDK
{
    public abstract class AdaptyInstallationStatus
    {
        internal AdaptyInstallationStatus() { }
    }

    public sealed class AdaptyInstallationStatusNotAvailable : AdaptyInstallationStatus
    {
        public AdaptyInstallationStatusNotAvailable() { }

        public override string ToString() => nameof(AdaptyInstallationStatusNotAvailable);
    }

    public sealed class AdaptyInstallationStatusNotDetermined : AdaptyInstallationStatus
    {
        public AdaptyInstallationStatusNotDetermined() { }

        public override string ToString() => nameof(AdaptyInstallationStatusNotDetermined);
    }

    public sealed class AdaptyInstallationStatusDetermined : AdaptyInstallationStatus
    {
        public readonly AdaptyInstallationDetails Details;

        public AdaptyInstallationStatusDetermined(AdaptyInstallationDetails details)
        {
            Details = details;
        }

        public override string ToString() =>
            $"{nameof(AdaptyInstallationStatusDetermined)}({Details})";
    }
}
