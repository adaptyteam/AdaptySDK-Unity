//
//  AdaptySubscriptionUpdateReplacementMode.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{
    public enum AdaptySubscriptionUpdateReplacementMode
    {
        WithTimeProration,
        ChargeProratedPrice,
        WithoutProration,
        Deferred,
        ChargeFullPrice
    }
}