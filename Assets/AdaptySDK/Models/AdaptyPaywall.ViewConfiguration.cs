//
//  AdaptyPaywall.ViewConfiguration.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 15.12.2024.
//

namespace AdaptySDK
{
    public partial class AdaptyPaywall
    {
        internal partial class ViewConfiguration
        {
            internal readonly string PaywallBuilderId;
            internal readonly string Lang;

            public override string ToString() => 
                $"{nameof(PaywallBuilderId)}: {PaywallBuilderId}, " +
                $"{nameof(Lang)}: {Lang}";
        }
    }
}