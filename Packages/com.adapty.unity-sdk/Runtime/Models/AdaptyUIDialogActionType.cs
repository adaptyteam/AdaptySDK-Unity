using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// Which button closed a dialog shown by <see cref="AdaptyUI.ShowDialog"/>.
    /// </summary>
    [Preserve]
    public enum AdaptyUIDialogActionType {
        /// <summary>
        /// The default action — the title given as the default one.
        /// </summary>
        [EnumMember(Value = "primary")]
        Primary = 0,
        /// <summary>
        /// The other action. Only reported when the dialog was configured with one.
        /// </summary>
        [EnumMember(Value = "secondary")]
        Secondary = 1,
    }
}