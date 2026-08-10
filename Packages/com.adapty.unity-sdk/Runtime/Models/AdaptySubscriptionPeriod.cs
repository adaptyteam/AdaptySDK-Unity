//
//  AdaptySubscriptionPeriod.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public class AdaptySubscriptionPeriod
    {
        private AdaptySubscriptionPeriod() { }

        [DataMember(Name = "unit", IsRequired = true)]
        public readonly AdaptySubscriptionPeriodUnit Unit;

        [DataMember(Name = "number_of_units", IsRequired = true)]
        public readonly long NumberOfUnits;

        public override string ToString() =>
            $"{nameof(Unit)}: {Unit}, " +
            $"{nameof(NumberOfUnits)}: {NumberOfUnits}";
    }
}
