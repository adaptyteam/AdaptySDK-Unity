//
//  AdaptyUIIOSPresentationStyle.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2024.
//

using System.Runtime.Serialization;

namespace AdaptySDK
{
    public enum AdaptyUIIOSPresentationStyle
    {
        [EnumMember(Value = "full_screen")]
        FullScreen,
        [EnumMember(Value = "page_sheet")]
        PageSheet,
    }
}
