using System;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        private static readonly string NotAvailableResponse =
            "{\"error\":{\"adapty_code\":"
            + (int)AdaptyErrorCode.AdaptyNotInitialized
            + ",\"message\":\"Adapty SDK is not available in the Unity Editor. Build and run the app on an iOS or Android device to use it.\"}}";

        /// <summary>
        /// Replaces the canned reply, and sees the request that produced it.
        /// </summary>
        /// <remarks>
        /// The only seam into the transport when there is no native side: the request payload is
        /// assembled inside <c>Request.Send</c>, so this is where a test can read what would have
        /// gone over the bridge.
        /// </remarks>
        internal static Func<string, string, string> Handler;

        // Reset for the same reason as the listeners: a hook a previous Play Mode run installed
        // must not answer this one.
        [UnityEngine.RuntimeInitializeOnLoadMethod(
            UnityEngine.RuntimeInitializeLoadType.SubsystemRegistration
        )]
        internal static void ResetHandler() => Handler = null;

        internal static void Invoke(string method, string request, Action<string> completionHandler)
        {
            completionHandler(Handler?.Invoke(method, request) ?? NotAvailableResponse);
        }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}
