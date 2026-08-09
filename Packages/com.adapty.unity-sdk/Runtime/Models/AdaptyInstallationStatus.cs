//
//  AdaptyInstallationStatus.cs
//  AdaptySDK
//
//  Created by Alexey Goncharov on 10.09.2025.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public sealed class AdaptyInstallationStatus
    {
        private AdaptyInstallationStatus() { }

        [DataMember(Name = "status", IsRequired = true)]
        public readonly AdaptyInstallationStatusType Status;

        /// <summary>
        /// The installation, present when <see cref="Status"/> is
        /// <see cref="AdaptyInstallationStatusType.Determined"/> and null otherwise.
        /// </summary>
        [DataMember(Name = "details")]
        [Preserve]
        public AdaptyInstallationDetails Details { get; private set; } // nullable

        // The contract carries details on the determined branch only, which no attribute can say.
        // Dropping it elsewhere rather than failing is what the branch-per-subclass model did.
        // [Preserve] because a type's attribute does not cover its methods.
        [Preserve]
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (Status != AdaptyInstallationStatusType.Determined)
            {
                Details = null;
            }
            else if (Details is null)
            {
                throw Serialization.JsonRequire.Missing("details");
            }
        }

        public override string ToString() =>
            $"{nameof(Status)}: {Status}, "
            + $"{nameof(Details)}: {(Details == null ? "null" : Details.ToString())}";
    }
}
