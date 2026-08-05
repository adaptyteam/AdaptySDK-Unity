//
//  AdaptyInstallationStatus.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using UnityEngine.Scripting;

namespace AdaptySDK
{
    [Preserve]
    public abstract class AdaptyInstallationStatus
    {
        internal AdaptyInstallationStatus() { }
    }

    [Preserve]
    public sealed class AdaptyInstallationStatusNotAvailable : AdaptyInstallationStatus
    {
        public AdaptyInstallationStatusNotAvailable() { }

        public override string ToString() => nameof(AdaptyInstallationStatusNotAvailable);
    }

    [Preserve]
    public sealed class AdaptyInstallationStatusNotDetermined : AdaptyInstallationStatus
    {
        public AdaptyInstallationStatusNotDetermined() { }

        public override string ToString() => nameof(AdaptyInstallationStatusNotDetermined);
    }

    [Preserve]
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
