using System.Collections.Generic;
using AdaptySDK.SimpleJSON;

#if UNITY_IOS && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.iOS.AdaptyIOSCallbackAction;
#elif UNITY_ANDROID && !UNITY_EDITOR
using _AdaptyCallbackAction = AdaptySDK.Android.AdaptyAndroidCallbackAction;
#else
using _AdaptyCallbackAction = AdaptySDK.Noop.AdaptyNoopCallbackAction;
#endif

namespace AdaptySDK
{
    public interface AdaptyEventListener
    {
        void OnLoadLatestProfile(Adapty.Profile profile);
    }

    public static partial class Adapty
    {
        private static AdaptyEventListener m_Listener;

        public static void SetEventListener(AdaptyEventListener listener)
        {
            _AdaptyCallbackAction.InitializeOnce();
            m_Listener = listener;
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
            }
        }
    }
}