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

    public static void registerMessageHandler(AdaptyAndroidMessageHandler handler) {
        messageHandler = handler;
        if(unityMainThreadHandler == null)
            unityMainThreadHandler = new Handler(Looper.getMainLooper());
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
