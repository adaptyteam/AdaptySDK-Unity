using System.Collections.Generic;

#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{
    using AdaptySDK.SimpleJSON;

    public interface AdaptyEventListener
    {
        void OnLoadLatestProfile(Adapty.Profile profile);
    }

    public interface AdaptyUnknownEventListener
    {
        void OnUnknownMessage(string type, JSONNode json);
    }

    public static partial class Adapty
    {
        private static AdaptyEventListener m_Listener;
        private static AdaptyUnknownEventListener m_UnknownListener;

        public static void SetEventListener(AdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
        }

        public static void SetUnknownEventListener(AdaptyUnknownEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_UnknownListener = listener;
        }

        internal static void OnMessage(string type, string json)
        {
            if (string.IsNullOrEmpty(json) || m_Listener == null) return;
            var response = JSONNode.Parse(json);
            if (response == null || response.IsNull) return;
            switch (type)
            {
                case "did_load_latest_profile":
                    if (!response.IsObject) return;
                    m_Listener.OnLoadLatestProfile(response.AsObject.GetProfile());
                    return;
                default:
                    if (m_UnknownListener == null) return;
                    m_UnknownListener.OnUnknownMessage(type, response);
                    return;
            }
        }
    }
}