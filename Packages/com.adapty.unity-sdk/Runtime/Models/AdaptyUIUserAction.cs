using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK {
    /// <summary>
    /// Something the user did in a flow view, reported through
    /// <c>FlowViewDidPerformAction</c>. Nothing is done for you — a close does not dismiss the
    /// view.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyUIUserAction {
        private AdaptyUIUserAction() { }

        /// <summary>
        /// Which action it was.
        /// </summary>
        [DataMember(Name = "type", IsRequired = true)]
        public AdaptyUIUserActionType Type;

        /// <summary>
        /// What the action carries: the URL for <see cref="AdaptyUIUserActionType.OpenUrl"/>, the flow's
        /// own identifier for <see cref="AdaptyUIUserActionType.Custom"/>. Null for the rest.
        /// </summary>
        [DataMember(Name = "value")]
        public string Value;

        /// <summary>
        /// Where the flow asked for the URL to open. Set for
        /// <see cref="AdaptyUIUserActionType.OpenUrl"/> only.
        /// </summary>
        [DataMember(Name = "open_in")]
        public AdaptyWebPresentation? OpenIn;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Type)}: {Type}, " +
            $"{nameof(Value)}: {Value}, " +
            $"{nameof(OpenIn)}: {OpenIn}";
    }
}
