using System;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        private static readonly string NotAvailableResponse =
            "{\"error\":{\"adapty_code\":"
            + (int)AdaptyErrorCode.AdaptyNotInitialized
            + ",\"message\":\"Adapty SDK is not available in the Unity Editor. Build and run the app on an iOS or Android device to use it.\"}}";

        internal static void Invoke(string method, string request, Action<string> completionHandler)
        {
            completionHandler(NotAvailableResponse);
        }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}
