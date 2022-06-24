using System;
using UnityEngine;

namespace AdaptySDK.Android
{
#if UNITY_ANDROID
    internal static class AdaptyAndroidCallbackAction
    {
        internal class MessageHandler : AndroidJavaProxy
        {
            internal MessageHandler() : base("com.adapty.unity.AdaptyAndroidMessageHandler") { }

            public void onMessage(string type, string json)
            {
                Adapty.OnMessage(type, json);
            }
        }

        internal class CallbackHandler : AndroidJavaProxy
        {
            private readonly Action<string> _resultHandler;

            internal CallbackHandler(Action<string> resultHandler) : base("com.adapty.unity.AdaptyAndroidCallback")
            {
                _resultHandler = resultHandler;
            }

            public void onHandleResult(string result)
            {
                if (_resultHandler == null) return;
                _resultHandler.Invoke(result);
            }
        }

        public static AndroidJavaProxy Action(Action<string> action)
        {
            return new CallbackHandler(action);
        }

        private static readonly object m_Lock = new object();

        private static bool m_IsInitialized = false;

        internal static void InitializeOnce()
        {
            lock(m_Lock) {  
                if (!m_IsInitialized) {
                    m_IsInitialized = true;
                    new AndroidJavaClass("com.adapty.unity.AdaptyAndroidWrapper").CallStatic("registerMessageHandler", new MessageHandler());
                }
            }
        }

    }
#endif
}