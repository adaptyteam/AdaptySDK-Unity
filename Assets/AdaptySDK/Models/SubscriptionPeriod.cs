//
//  SubscriptionPeriod.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class SubscriptionPeriod
        {
            public readonly PeriodUnit Unit;

            public readonly long NumberOfUnits;

            public override string ToString() => $"{nameof(Unit)}: {Unit}, " +
                       $"{nameof(NumberOfUnits)}: {NumberOfUnits}";
        }
    }
}
