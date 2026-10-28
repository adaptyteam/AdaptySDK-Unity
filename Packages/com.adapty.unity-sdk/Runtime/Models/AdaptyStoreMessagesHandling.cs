using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// Who decides when the store's own messages — a billing issue, a price increase — are
    /// shown. On iOS it needs iOS 16+.
    /// </summary>
    [Preserve]
    public enum AdaptyStoreMessagesHandling
    {
        /// <summary>
        /// The store shows them as they arrive. The native default.
        /// </summary>
        [EnumMember(Value = "auto")]
        Auto = 0,

        /// <summary>
        /// They are held until <see cref="Adapty.ShowStoreMessages(System.Action{AdaptyError})"/>.
        /// </summary>
        [EnumMember(Value = "manual")]
        Manual = 1,
    }
}
