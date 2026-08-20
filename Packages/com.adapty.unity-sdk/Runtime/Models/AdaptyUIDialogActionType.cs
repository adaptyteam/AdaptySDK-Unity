using System.Runtime.Serialization;
using UnityEngine.Scripting;

namespace AdaptySDK
{
    /// <summary>
    /// Which button closed a dialog shown by <see cref="AdaptyUI.ShowDialog(AdaptySDK.AdaptyUIFlowView, AdaptySDK.AdaptyUIDialogConfiguration, System.Action{AdaptySDK.AdaptyUIDialogActionType, AdaptySDK.AdaptyError})"/>.
    /// </summary>
    [Preserve]
    public enum AdaptyUIDialogActionType
    {
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
