using System;

namespace AdaptySDK.Noop
{
    internal static class AdaptyNoop
    {
        internal static void Invoke(string method, string request, Action<string> completionHandler) { completionHandler(null); }
    }

    internal static class AdaptyNoopCallbackAction
    {
        internal static void InitializeOnce() { }
    }
}