//
//  AdaptySubscriptionUpdateParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK
{
    public partial class AdaptySubscriptionUpdateParameters
    {
        /// The product id for current subscription to change.
        public string OldSubVendorProductId;

        public AdaptySubscriptionUpdateReplacementMode ReplacementMode;

        public AdaptySubscriptionUpdateParameters(string oldSubVendorProductId, AdaptySubscriptionUpdateReplacementMode replacementMode)
        {
            OldSubVendorProductId = oldSubVendorProductId ?? throw new ArgumentNullException(nameof(oldSubVendorProductId)); //TODO
            ReplacementMode = replacementMode;
        }

        public override string ToString() => 
            $"{nameof(OldSubVendorProductId)}: {OldSubVendorProductId}, " +
            $"{nameof(ReplacementMode)}: {ReplacementMode}";
    }
}