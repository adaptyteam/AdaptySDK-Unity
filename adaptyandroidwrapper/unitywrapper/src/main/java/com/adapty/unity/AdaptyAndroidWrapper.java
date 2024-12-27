package com.adapty.unity;

import android.os.Handler;

import com.adapty.internal.crossplatform.CrossplatformHelper;
import com.adapty.utils.FileLocation;
import com.unity3d.player.UnityPlayer;

public class AdaptyAndroidWrapper {

    public static CrossplatformHelper getHelper() {
        if (CrossplatformHelper.init(
                UnityPlayer.currentActivity.getApplicationContext(),
                (eventName, eventData) -> {
                    runOnUnityThread(() -> {
                        if (messageHandler != null) {
                            messageHandler.onMessage(eventName, eventData);
                        }
                    });
                },
                FileLocation::fromAsset
            )
        ) {
            CrossplatformHelper.getShared().setActivity(() -> UnityPlayer.currentActivity);
        }
        return CrossplatformHelper.getShared();
    }
    private static final Handler unityMainThreadHandler = new Handler();
    private static AdaptyAndroidMessageHandler messageHandler;

    public static void registerMessageHandler(AdaptyAndroidMessageHandler handler) {
        messageHandler = handler;
    }

    public static void runOnUnityThread(Runnable runnable) {
        if(runnable != null) {
            unityMainThreadHandler.post(runnable);
        }
    }

    public static void invokeRequest(String methodName, String argument, AdaptyAndroidCallback callback) {
        getHelper().onMethodCall(argument, methodName, message -> {
            runOnUnityThread(() -> callback.onHandleResult(message));
        });
    }
}
