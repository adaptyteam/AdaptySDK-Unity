using System;
using UnityEngine;
namespace AdaptySDK.Android
{
#if UNITY_ANDROID
    using AdaptyAndroidCallback = AdaptyAndroidCallbackAction;

    internal static class AdaptyAndroid
    {
        private static AndroidJavaClass AdaptyAndroidClass = new AndroidJavaClass("com.adapty.unity.AdaptyAndroidWrapper");

        internal static void Invoke(string method, string request, Action<string> completionHandler) =>
            AdaptyAndroidClass.CallStatic("invokeRequest", method, request , AdaptyAndroidCallback.Action(completionHandler));
        
    }
#endif
}