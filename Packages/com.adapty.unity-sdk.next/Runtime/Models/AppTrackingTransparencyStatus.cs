//
//  AppTrackingTransparencyStatus.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using UnityEngine.Scripting;

namespace AdaptySDK {

    [Preserve]
    public enum AppTrackingTransparencyStatus {
        NotDetermined,
        Restricted,
        Denied,
        Authorized
    }

}
