//
//  AdaptyUIDialogConfiguration.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 07.09.2023.
//

namespace AdaptySDK
{
    public partial class AdaptyUIDialogConfiguration
    {
        /// <summary>   
        /// The title of the dialog.
        /// </summary>
        public string Title;

        /// <summary>
        /// Descriptive text that provides additional details about the reason for the dialog.
        /// </summary>
        public string Content;

        /// <summary>
        /// The action title to display as part of the dialog. If you provide two actions, be sure the `defaultAction` cancels the operation and leaves things unchanged.
        /// </summary>
        public string DefaultActionTitle;

        /// <summary>
        /// The secondary action title to display as part of the dialog.
        /// </summary>
        public string SecondaryActionTitle;

        public override string ToString() =>
            $"{nameof(Title)}: {Title}, " +
            $"{nameof(Content)}: {Content}, " +
            $"{nameof(DefaultActionTitle)}: {DefaultActionTitle}, " +
            $"{nameof(SecondaryActionTitle)}: {SecondaryActionTitle}";


        public AdaptyUIDialogConfiguration SetTitle(string title)
        {
            Title = title;
            return this;
        }

        public AdaptyUIDialogConfiguration SetContent(string content)
        {
            Content = content;
            return this;
        }

        public AdaptyUIDialogConfiguration SetDefaultActionTitle(string defaultActionTitle)
        {
            DefaultActionTitle = defaultActionTitle;
            return this;
        }

        public AdaptyUIDialogConfiguration SetSecondaryActionTitle(string secondaryActionTitle)
        {
            SecondaryActionTitle = secondaryActionTitle;
            return this;
        }

    }
}