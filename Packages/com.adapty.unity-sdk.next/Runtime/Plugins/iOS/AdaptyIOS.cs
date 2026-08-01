using System;
using System.Runtime.InteropServices;

namespace AdaptySDK.iOS
{
#if UNITY_IOS
    internal static class AdaptyIOS
    {
        [DllImport("__Internal", CharSet = CharSet.Ansi, EntryPoint = "AdaptyUnity_invoke")]
        private static extern void _Invoke(string method, string request, IntPtr callback);

        internal static void Invoke(string method, string request, Action<string> completionHandler)  => 
            _Invoke(method, request, AdaptyIOSCallbackAction.ActionToIntPtr(completionHandler));
    }
#endif
}