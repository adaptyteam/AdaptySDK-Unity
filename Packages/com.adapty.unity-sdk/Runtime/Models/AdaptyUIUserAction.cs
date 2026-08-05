//
//  AdaptyUIUserAction.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

using UnityEngine.Scripting;

namespace AdaptySDK {
    using System.Runtime.Serialization;

    [DataContract]
    [Preserve]
    public partial class AdaptyUIUserAction {
        private AdaptyUIUserAction() { }

        [DataMember(Name = "type", IsRequired = true)]
        public AdaptyUIUserActionType Type;

        [DataMember(Name = "value")]
        public string Value;

        [DataMember(Name = "open_in")]
        public AdaptyWebPresentation? OpenIn;

        public override string ToString() =>
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Value)}: {Value}, " +
            $"{nameof(OpenIn)}: {OpenIn}";
    }
}
