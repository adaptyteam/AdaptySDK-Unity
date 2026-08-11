using UnityEngine.Scripting;
using System.Runtime.Serialization;

namespace AdaptySDK
{
    /// <summary>
    /// What <see cref="AdaptyUI.ShowDialog"/> puts on the dialog. Only the default action title
    /// is required; a dialog without a secondary one has a single button.
    /// </summary>
    [DataContract]
    [Preserve]
    public sealed class AdaptyUIDialogConfiguration
    {
        /// <summary>
        /// The title of the dialog.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title;

        /// <summary>
        /// Descriptive text that provides additional details about the reason for the dialog.
        /// </summary>
        [DataMember(Name = "content")]
        public string Content;

        /// <summary>
        /// The action title to display as part of the dialog. If you provide two actions, be sure the `defaultAction` cancels the operation and leaves things unchanged.
        /// </summary>
        [DataMember(Name = "default_action_title", IsRequired = true)]
        public string DefaultActionTitle;

        /// <summary>
        /// The secondary action title to display as part of the dialog.
        /// </summary>
        [DataMember(Name = "secondary_action_title")]
        public string SecondaryActionTitle;

        /// <summary>
        /// A description for logs and the debugger. The format is not part of the contract —
        /// read the members rather than parsing it.
        /// </summary>
        public override string ToString() =>
            $"{nameof(Title)}: {Title}, " +
            $"{nameof(Content)}: {Content}, " +
            $"{nameof(DefaultActionTitle)}: {DefaultActionTitle}, " +
            $"{nameof(SecondaryActionTitle)}: {SecondaryActionTitle}";


        /// <summary>Sets the title.</summary>
        /// <param name="title">The dialog's title.</param>
        public AdaptyUIDialogConfiguration SetTitle(string title)
        {
            Title = title;
            return this;
        }

        /// <summary>Sets the content.</summary>
        /// <param name="content">The body text.</param>
        public AdaptyUIDialogConfiguration SetContent(string content)
        {
            Content = content;
            return this;
        }

        /// <summary>Sets the default action title.</summary>
        /// <param name="defaultActionTitle">The label of the button reported as <see cref="AdaptyUIDialogActionType.Primary"/>.</param>
        public AdaptyUIDialogConfiguration SetDefaultActionTitle(string defaultActionTitle)
        {
            DefaultActionTitle = defaultActionTitle;
            return this;
        }

        /// <summary>Sets the secondary action title.</summary>
        /// <param name="secondaryActionTitle">The label of the button reported as <see cref="AdaptyUIDialogActionType.Secondary"/>. Leave it out for a one-button dialog.</param>
        public AdaptyUIDialogConfiguration SetSecondaryActionTitle(string secondaryActionTitle)
        {
            SecondaryActionTitle = secondaryActionTitle;
            return this;
        }

    }
}