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

            public AndroidSubscriptionUpdateReplacementMode ReplacementMode;

            public AndroidSubscriptionUpdateParameters(string oldSubVendorProductId, AndroidSubscriptionUpdateReplacementMode replacementMode)
            {
                OldSubVendorProductId = oldSubVendorProductId ?? throw new ArgumentNullException(nameof(oldSubVendorProductId)); //TODO
                ReplacementMode = replacementMode;
            }

            public override string ToString() => $"{nameof(OldSubVendorProductId)}: {OldSubVendorProductId}, " +
                       $"{nameof(ReplacementMode)}: {ReplacementMode}";
        }
    }
}

