using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AdaptySDK.iOS
{
    internal static class ExceptionGetFullMessage
    {
        internal static string GetFullMessage(this Exception ex)
        {
            return ex.InnerException == null
                 ? ex.Message
                 : ex.Message + " --> " + ex.InnerException.GetFullMessage();
        }
    }

#if UNITY_IOS
    internal static class AdaptyIOSCallbackAction {
        private delegate void MessageDelegate(string type, string data);
        private delegate void CallbackDelegate(IntPtr actionPtr, string data);

      
        [AOT.MonoPInvokeCallback(typeof(MessageDelegate))]
        private static void OnMessage(string type, string json) {
            Adapty.OnMessage(type, json);
        }

        [AOT.MonoPInvokeCallback(typeof(CallbackDelegate))]
        private static void OnCallback(IntPtr actionPtr, string data) {
            if(IntPtr.Zero.Equals(actionPtr)) { return; }
            var action = IntPtrToObject(actionPtr, true);
            if(action == null) { return; }

            try {
                var paramTypes = action.GetType().GetGenericArguments();
                var arg = paramTypes.Length == 0 ? null : data;

                var invokeMethod = action.GetType().GetMethod("Invoke", paramTypes.Length == 0  ? new Type[0] : new []{ paramTypes[0] });
                if(invokeMethod != null) {
                    invokeMethod.Invoke(action, paramTypes.Length == 0 ? new object[] { } : new[] { arg });
                }
                else {
                    Debug.LogError("Failed to invoke callback " + action + " with arg " + arg + ": invoke method not found");
                }
            }
            catch(Exception e) {
                Debug.LogError("Failed to invoke callback " + action + " with arg " + data + ": " + e.GetFullMessage());
            }
        }

        internal static object IntPtrToObject(IntPtr handle, bool unpinHandle) {
            if(IntPtr.Zero.Equals(handle)) { return null; }
            var gcHandle = GCHandle.FromIntPtr(handle);
            var result = gcHandle.Target;
            if(unpinHandle) { gcHandle.Free(); }
            return result;
        }

        internal static IntPtr ObjectToIntPtr(object obj) {
            if(obj == null) { return IntPtr.Zero; }
            var handle = GCHandle.Alloc(obj);
            return GCHandle.ToIntPtr(handle);
        }
        internal static IntPtr ActionToIntPtr<T>(Action<T> action) {
            return ObjectToIntPtr(action);
        }


        private static readonly object m_Lock = new object();
        private static bool m_IsInitialized = false;

        internal static void InitializeOnce() {
            lock (m_Lock)
            {
                if (!m_IsInitialized)
                {
                    m_IsInitialized = true;
                    RegisterCallbackDelegate(OnMessage, OnCallback);
                }
            }
        }

        [DllImport("__Internal", EntryPoint = "AdaptyUnity_registerCallbackHandler")]
        private static extern void RegisterCallbackDelegate(MessageDelegate messageDelegate,CallbackDelegate callbackDelegate);
    }
#endif
}