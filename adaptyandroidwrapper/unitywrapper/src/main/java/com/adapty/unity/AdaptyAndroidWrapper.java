package com.adapty.unity;

import android.os.Handler;
import android.os.Looper;

import com.adapty.internal.crossplatform.CrossplatformHelper;
import com.adapty.utils.FileLocation;
import com.unity3d.player.UnityPlayer;

public class AdaptyAndroidWrapper {

    private static CrossplatformHelper helper = null;

    private static final Object lock = new Object();

    public static CrossplatformHelper getHelper() {
        if (helper != null)
            return helper;

        synchronized (lock) {
            if (helper != null)
                return helper;
            if (CrossplatformHelper.init(
                    UnityPlayer.currentActivity.getApplicationContext(),
                    (eventName, eventData) -> {
                        runOnUnityThread(() -> {
                            if (messageHandler != null) {
                                messageHandler.onMessage(eventName, eventData);
                            }
                        });
                    },
                    value -> {
                        int lastIndex = value.lastIndexOf("!/assets/");
                        String result = (lastIndex != -1) ? value.substring(lastIndex + 9) : value;
                        return FileLocation.fromAsset(result);
                    }
                )
            ) {
                CrossplatformHelper.getShared().setActivity(() -> UnityPlayer.currentActivity);
            }
            helper = CrossplatformHelper.getShared();
            return helper;
        }
    }
    private static Handler unityMainThreadHandler;
    private static AdaptyAndroidMessageHandler messageHandler;

    /**
     * Binds to the registering thread's Looper - Unity's, since the C# side registers from its
     * scripting thread before the first scene loads. Unity's player loop is not the Android UI
     * thread, so Looper.getMainLooper() would deliver every callback on the wrong one.
     */
    public static void registerMessageHandler(AdaptyAndroidMessageHandler handler) {
        messageHandler = handler;
        if(unityMainThreadHandler == null) {
            Looper looper = Looper.myLooper();
            if (looper == null) {
                throw new IllegalStateException(
                    "Adapty: registerMessageHandler was called from a thread with no Looper, so SDK "
                        + "callbacks cannot be delivered back to it. It is expected to be called "
                        + "from Unity's scripting thread, which Adapty.SetEventListener does."
                );
            }
            unityMainThreadHandler = new Handler(looper);
        }
    }

    public static void runOnUnityThread(Runnable runnable) {
        if(unityMainThreadHandler != null && runnable != null) {
            unityMainThreadHandler.post(runnable);
        }
    }

    public static void invokeRequest(String methodName, String argument, AdaptyAndroidCallback callback) {
        getHelper().onMethodCall(argument, methodName, message -> {
            runOnUnityThread(() -> callback.onHandleResult(message));
        });
    }
}
