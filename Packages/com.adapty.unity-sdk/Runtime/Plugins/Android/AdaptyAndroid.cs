using System;
using UnityEngine;

namespace AdaptySDK.Android
{
#if UNITY_ANDROID
    /// <summary>
    /// The Android bridge: <c>CrossplatformHelper</c> from <c>io.adapty.internal:crossplatform</c>,
    /// called through JNI with no Java of the SDK's own in between.
    /// </summary>
    /// <remarks>
    /// The helper's four callbacks are plain Java interfaces with a single <c>invoke</c>, which is
    /// the shape <see cref="AndroidJavaProxy"/> implements by method name. They arrive on the
    /// thread the native SDK calls from, never Unity's, and are posted to Unity's through
    /// <see cref="AdaptyAndroidLooper"/> - not through <see cref="Adapty.RunOnMainThread"/>, which
    /// waits for a frame that does not come while a flow is on screen.
    /// </remarks>
    internal static class AdaptyAndroid
    {
        private const string HelperClass = "com.adapty.internal.crossplatform.CrossplatformHelper";
        private const string UnityPlayerClass = "com.unity3d.player.UnityPlayer";

        private static AndroidJavaObject s_Helper;

        /// <summary>
        /// Initialises the helper before the first scene, so an event has somewhere to land.
        /// The null check is the idempotence guard: everything here runs on the main thread.
        /// </summary>
        internal static void InitializeOnce()
        {
            _ = Helper;
        }

        internal static void Invoke(string method, string request, Action<string> completionHandler) =>
            Helper.Call("onMethodCall", request, method, new ResultCallback(completionHandler));

        private static AndroidJavaObject Helper
        {
            get
            {
                if (s_Helper != null)
                {
                    return s_Helper;
                }

                if (!AdaptyAndroidLooper.Install())
                {
                    throw new InvalidOperationException(
                        "Adapty: the bridge could not register on the calling thread's native "
                            + "looper, so SDK callbacks cannot be delivered back to it. It is expected "
                            + "to run on Unity's scripting thread, which Adapty.InitializeTransport "
                            + "does before the first scene loads."
                    );
                }

                using var player = new AndroidJavaClass(UnityPlayerClass);
                using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                using var context = activity.Call<AndroidJavaObject>("getApplicationContext");
                using var helperClass = new AndroidJavaClass(HelperClass);

                // True on the first initialisation of the process only; the activity provider is
                // installed once, like the wrapper it replaces did.
                var first = helperClass.CallStatic<bool>(
                    "init",
                    context,
                    new EventCallback(),
                    new FileLocationTransformer()
                );
                s_Helper = helperClass.CallStatic<AndroidJavaObject>("getShared");
                if (first)
                {
                    s_Helper.Call("setActivity", new ActivityProvider());
                }

                return s_Helper;
            }
        }

        private static AndroidJavaObject CurrentActivity()
        {
            using var player = new AndroidJavaClass(UnityPlayerClass);
            return player.GetStatic<AndroidJavaObject>("currentActivity");
        }

        private sealed class EventCallback : AndroidJavaProxy
        {
            internal EventCallback()
                : base("com.adapty.internal.crossplatform.EventCallback") { }

            public void invoke(string id, string json) =>
                AdaptyAndroidLooper.Post(() => Adapty.OnMessage(id, json));
        }

        private sealed class ResultCallback : AndroidJavaProxy
        {
            private readonly Action<string> m_Handler;

            internal ResultCallback(Action<string> handler)
                : base("com.adapty.internal.crossplatform.ResultCallback")
            {
                m_Handler = handler;
            }

            public void invoke(string json) =>
                AdaptyAndroidLooper.Post(() =>
                {
                    try
                    {
                        m_Handler?.Invoke(json);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Failed to invoke callback with arg " + json + ": " + e);
                    }
                });
        }

        private sealed class ActivityProvider : AndroidJavaProxy
        {
            internal ActivityProvider()
                : base("com.adapty.internal.crossplatform.ActivityProvider") { }

            public AndroidJavaObject invoke() => CurrentActivity();
        }

        /// <summary>
        /// Maps a <c>StreamingAssets</c> path to the asset the native SDK reads: on Android that
        /// path is inside the APK, <c>jar:file://...!/assets/&lt;name&gt;</c>.
        /// </summary>
        private sealed class FileLocationTransformer : AndroidJavaProxy
        {
            private const string AssetsMarker = "!/assets/";

            internal FileLocationTransformer()
                : base("com.adapty.internal.crossplatform.FileLocationTransformer") { }

            public AndroidJavaObject invoke(string path)
            {
                var marker = path.LastIndexOf(AssetsMarker, StringComparison.Ordinal);
                var asset = marker < 0 ? path : path.Substring(marker + AssetsMarker.Length);
                using var fileLocation = new AndroidJavaClass("com.adapty.utils.FileLocation");
                return fileLocation.CallStatic<AndroidJavaObject>("fromAsset", asset);
            }
        }
    }
#endif
}
