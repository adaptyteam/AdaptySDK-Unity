//
//  AdaptyUIIOSPresentationStyle.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2024.
//

using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    [Preserve]
    public enum AdaptyUIIOSPresentationStyle
    {
        [EnumMember(Value = "full_screen")]
        FullScreen = 0,
        [EnumMember(Value = "page_sheet")]
        PageSheet = 1,
    }
}
