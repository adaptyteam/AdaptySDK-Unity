//
//  AdaptyOnboardingScreenParameters.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 20.12.2022.
//

namespace AdaptySDK
{

    public partial class AdaptyOnboardingScreenParameters
    {
        public string Name; //nullable
        public string ScreenName; //nullable
        public uint ScreenOrder;

        public AdaptyOnboardingScreenParameters(string name, string screenName, uint screenOrder)
        {
            Name = string.IsNullOrEmpty(name) ? null : name;
            ScreenName = string.IsNullOrEmpty(screenName) ? null : screenName;
            ScreenOrder = screenOrder;
        }

        public override string ToString() => 
            $"{nameof(Name)}: {Name}, " +
            $"{nameof(ScreenName)}: {ScreenName}, " +
            $"{nameof(ScreenOrder)}: {ScreenOrder}";
    }
}