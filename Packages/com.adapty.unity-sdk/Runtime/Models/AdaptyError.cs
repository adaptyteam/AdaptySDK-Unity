using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK {
    /// <summary>
    /// A failure reported by the native SDK. Every completion handler takes one, and it is null
    /// when the call succeeded.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyError {
        private AdaptyError() { }

        /// <summary>
        /// What went wrong. Branch on this rather than on <see cref="Message"/>, which is not a contract.
        /// </summary>
        [DataMember(Name = "adapty_code", IsRequired = true)]
        public readonly AdaptyErrorCode Code;
        /// <summary>
        /// A description of the failure, for a log. Not localized and not stable between versions.
        /// </summary>
        [DataMember(Name = "message", IsRequired = true)]
        public readonly string Message;
        /// <summary>
        /// What the native side added about this particular failure — the underlying exception, a store
        /// response. Null when there is nothing to add.
        /// </summary>
        [DataMember(Name = "detail")]
        public readonly string Detail; // nullable

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
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