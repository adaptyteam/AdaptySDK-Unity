//
//  AndroidSubscriptionUpdateParameters.cs
//  Adapty
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public partial class AndroidSubscriptionUpdateParameters
        {
            /// The product id for current subscription to change.
            public string OldSubVendorProductId;
            public AndroidSubscriptionUpdateProrationMode ProrationMode;

            public AndroidSubscriptionUpdateParameters(string oldSubVendorProductId, AndroidSubscriptionUpdateProrationMode prorationMode)
            {
                OldSubVendorProductId = oldSubVendorProductId ?? throw new ArgumentNullException(nameof(oldSubVendorProductId)); //TODO
                ProrationMode = prorationMode;
            }

            public override string ToString()
            {
                return $"{nameof(OldSubVendorProductId)}: {OldSubVendorProductId}, " +
                       $"{nameof(ProrationMode)}: {ProrationMode}";
            }
        }
    }
}

