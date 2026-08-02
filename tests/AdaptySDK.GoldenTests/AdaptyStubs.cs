// Compiled into AdaptySDK.GoldenTests only, by the SDK project's default globbing - that suite
// links the current package's Models and JSON but not its Adapty.cs or IAdaptyEventListener.cs,
// which reach for P/Invoke and AndroidJavaClass. The models still reference the few members
// below, so they are stubbed here.
//
// AdaptySDK.NextTests links the real transport instead and does not include this file.

using System;

namespace AdaptySDK
{
    public static partial class Adapty
    {
        public static readonly string SDKVersion = "4.0.0";
    }

    public static class AdaptyUI
    {
        public static void PresentFlowView(
            AdaptyUIFlowView view,
            AdaptyUIIOSPresentationStyle style,
            Action<AdaptyError> handler
        ) => throw new NotSupportedException();

        public static void DismissFlowView(AdaptyUIFlowView view, Action<AdaptyError> handler) =>
            throw new NotSupportedException();

        public static void PresentOnboardingView(
            AdaptyUIOnboardingView view,
            AdaptyUIIOSPresentationStyle style,
            Action<AdaptyError> handler
        ) => throw new NotSupportedException();

        public static void DismissOnboardingView(
            AdaptyUIOnboardingView view,
            Action<AdaptyError> handler
        ) => throw new NotSupportedException();
    }
}
