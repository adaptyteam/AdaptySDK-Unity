package com.adapty.unity;

import android.content.Context;

import com.adapty.push.AdaptyPushHandler;

public class AdaptyUnityPushHandler extends AdaptyPushHandler {

    private final String clickAction;
    private final int smallIconResId;

    public AdaptyUnityPushHandler(Context context, String clickAction, int smallIconResId) {
        super(context);
        this.clickAction = clickAction;
        this.smallIconResId = smallIconResId;
    }

    public String getClickAction() {
        return this.clickAction;
    }

    public int getSmallIconResId() {
        return this.smallIconResId;
    }
}
