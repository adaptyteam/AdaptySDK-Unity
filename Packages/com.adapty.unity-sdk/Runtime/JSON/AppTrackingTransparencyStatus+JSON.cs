//
//  AppTrackingTransparencyStatus+JSON.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

using System;

namespace AdaptySDK.SimpleJSON
{
    internal static partial class JSONNodeExtensions
    {
        internal static int ToJSON(this AppTrackingTransparencyStatus value)
        {
            switch (value)
            {
                case AppTrackingTransparencyStatus.NotDetermined: return 0;
                case AppTrackingTransparencyStatus.Restricted: return 1;
                case AppTrackingTransparencyStatus.Denied: return 2;
                case AppTrackingTransparencyStatus.Authorized: return 3;
                default: throw new Exception($"AppTrackingTransparencyStatus unknown value: {value}");
            }
        }
    }
}