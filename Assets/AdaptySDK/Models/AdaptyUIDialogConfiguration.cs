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
        public string Title;
        public string Content;
        public string DefaultActionTitle;
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