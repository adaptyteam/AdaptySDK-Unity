//
//  AdaptySubscriptionUpdateParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;
using System;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [DataContract]
    [Preserve]
    public partial class AdaptySubscriptionUpdateParameters
    {
        /// The product id for current subscription to change.
        [DataMember(Name = "old_sub_vendor_product_id", IsRequired = true)]
        public string OldSubVendorProductId;

        [DataMember(Name = "replacement_mode", IsRequired = true)]
        public AdaptySubscriptionUpdateReplacementMode ReplacementMode;

        public AdaptySubscriptionUpdateParameters(
            string oldSubVendorProductId,
            AdaptySubscriptionUpdateReplacementMode replacementMode
        )
        {
            OldSubVendorProductId =
                oldSubVendorProductId
                ?? throw new ArgumentNullException(nameof(oldSubVendorProductId)); //TODO
            ReplacementMode = replacementMode;
        }

        public override string ToString() =>
            $"{nameof(OldSubVendorProductId)}: {OldSubVendorProductId}, "
            + $"{nameof(ReplacementMode)}: {ReplacementMode}";
    }
}
