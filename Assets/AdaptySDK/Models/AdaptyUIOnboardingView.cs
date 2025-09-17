//
//  AdaptyUIOnboardingView.cs
//  AdaptySDK
//
//  Created by Aleksei Valiano on 17.12.2024.
//

namespace AdaptySDK
{
    public partial class AdaptyUIOnboardingView
    {
        public string Id;
        public string PlacementId;
        public string PaywallVariationId;

        public override string ToString() =>
            $"{nameof(Id)}: {Id}, "
            + $"{nameof(PlacementId)}: {PlacementId}, "
            + $"{nameof(PaywallVariationId)}: {PaywallVariationId}";

        public void Present(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.PresentOnboardingView(this, completionHandler);

        public void Dismiss(System.Action<AdaptyError> completionHandler) =>
            AdaptyUI.DismissOnboardingView(this, completionHandler);
    }
}
