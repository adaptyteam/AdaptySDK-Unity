//
//  AdaptySubscriptionPeriod.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public partial class AdaptySubscriptionPeriod
    {
        public readonly AdaptySubscriptionPeriodUnit Unit;

        public readonly long NumberOfUnits;

        public override string ToString() =>
            $"{nameof(Unit)}: {Unit}, " +
            $"{nameof(NumberOfUnits)}: {NumberOfUnits}";
    }
}
