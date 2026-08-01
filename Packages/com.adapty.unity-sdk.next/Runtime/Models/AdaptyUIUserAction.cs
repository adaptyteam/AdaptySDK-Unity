//
//  AdaptyUIUserAction.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK {
    public partial class AdaptyUIUserAction {
        public AdaptyUIUserActionType Type;
        public string Value;
        public AdaptyWebPresentation? OpenIn;

        public override string ToString() =>
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Value)}: {Value}, " +
            $"{nameof(OpenIn)}: {OpenIn}";
    }
}
