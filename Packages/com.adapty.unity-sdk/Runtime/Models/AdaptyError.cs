using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK {
    [DataContract]
    [Preserve]
    public sealed class AdaptyError {
        private AdaptyError() { }

        [DataMember(Name = "adapty_code", IsRequired = true)]
        public readonly AdaptyErrorCode Code;
        [DataMember(Name = "message", IsRequired = true)]
        public readonly string Message;
        [DataMember(Name = "detail")]
        public readonly string Detail; // nullable

        public override string ToString() =>
            $"{nameof(Code)}: {Code}, " +
            $"{nameof(Message)}: {Message}, " +
            $"{nameof(Detail)}: {Detail}";

        internal AdaptyError(AdaptyErrorCode Code, string Message, string Detail) {
            this.Message = Message;
            this.Detail = Detail;
            this.Code = Code;
        }
    }
}