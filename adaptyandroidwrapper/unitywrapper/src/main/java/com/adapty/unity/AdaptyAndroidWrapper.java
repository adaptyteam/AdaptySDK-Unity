package com.adapty.unity;

import static com.adapty.internal.utils.UtilsKt.DEFAULT_PAYWALL_TIMEOUT_MILLIS;
import static com.adapty.unity.Constants.ADAPTY_ERROR_CODE_DECODING_FAILED;
import static com.adapty.unity.Constants.ADAPTY_ERROR_CODE_KEY;
import static com.adapty.unity.Constants.ADAPTY_ERROR_DECODING_FAILED_MESSAGE;
import static com.adapty.unity.Constants.ADAPTY_ERROR_DETAIL_KEY;
import static com.adapty.unity.Constants.ADAPTY_ERROR_KEY;
import static com.adapty.unity.Constants.ADAPTY_ERROR_MESSAGE_KEY;
import static com.adapty.unity.Constants.ADAPTY_ONBOARDING_NAME_KEY;
import static com.adapty.unity.Constants.ADAPTY_ONBOARDING_SCREEN_NAME_KEY;
import static com.adapty.unity.Constants.ADAPTY_ONBOARDING_SCREEN_ORDER_KEY;
import static com.adapty.unity.Constants.ADAPTY_PROFILE_UPDATE_KEY;
import static com.adapty.unity.Constants.ADAPTY_SUCCESS_KEY;
import static com.adapty.unity.Constants.ADAPTY_SDK_VERSION;

import android.content.Context;
import android.os.Handler;

import com.adapty.Adapty;
import com.adapty.errors.AdaptyError;
import com.adapty.internal.crossplatform.CrossplatformHelper;
import com.adapty.internal.crossplatform.CrossplatformName;
import com.adapty.internal.crossplatform.MetaInfo;
import com.adapty.models.AdaptyAttributionSource;
import com.adapty.models.AdaptyPaywall;
import com.adapty.models.AdaptyPaywallProduct;
import com.adapty.models.AdaptyProfileParameters;
import com.adapty.models.AdaptySubscriptionUpdateParameters;
import com.adapty.utils.AdaptyLogLevel;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class AdaptyAndroidWrapper {

    private static final CrossplatformHelper helper;

    static {
        CrossplatformHelper.init(MetaInfo.from(CrossplatformName.UNITY, ADAPTY_SDK_VERSION));
        helper = CrossplatformHelper.getShared();
    }

    private static Handler unityMainThreadHandler;
    private static AdaptyAndroidMessageHandler messageHandler;

    public static AdaptyAndroidMessageHandler getMessageHandler() {
        return messageHandler;
    }

    public static void registerMessageHandler(AdaptyAndroidMessageHandler handler) {
        messageHandler = handler;
        if(unityMainThreadHandler == null) {
            unityMainThreadHandler = new Handler();
            handleProfileUpdates();
        }
    }

    public static void runOnUnityThread(Runnable runnable) {
        if(unityMainThreadHandler != null && runnable != null) {
            unityMainThreadHandler.post(runnable);
        }
    }

    public static void setLogLevel(String logLevelStr) {
        AdaptyLogLevel logLevel = null;

        try {
            logLevel = helper.toLogLevel(logLevelStr);
        } catch (Exception ignored) {}

        if (logLevel != null) {
            Adapty.setLogLevel(logLevel);
        }
    }

    public static void activate(Context context, String key, String customerUserId, boolean observerMode) {
        Adapty.activate(context, key, observerMode, customerUserId);
    }

    public static void identify(String customerUserId, AdaptyAndroidCallback callback) {
        Adapty.identify(customerUserId, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    public static void logout(AdaptyAndroidCallback callback) {
        Adapty.logout((AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    public static void getPaywall(String id, String locale, String fetchPolicyJson, Long loadTimeoutMillis, AdaptyAndroidCallback callback) {
        if (id == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("id", methodName, callback);
            return;
        }

        AdaptyPaywall.FetchPolicy fetchPolicy = parseJsonArgument(fetchPolicyJson, AdaptyPaywall.FetchPolicy.class);
        if (fetchPolicy == null)
            fetchPolicy = AdaptyPaywall.FetchPolicy.Default;

        int timeout = handleLoadTimeoutParam(loadTimeoutMillis);

        Adapty.getPaywall(id, locale, fetchPolicy, timeout, result -> {
            sendMessageWithResult(helper.toJson(result), callback);
        });
    }

    public static void getPaywallProducts(String paywallJson, AdaptyAndroidCallback callback) {
        AdaptyPaywall paywall = parseJsonArgument(paywallJson, AdaptyPaywall.class);
        if (paywall == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("paywallJson", methodName, callback);
            return;
        }

        Adapty.getPaywallProducts(paywall, result -> {
            sendMessageWithResult(helper.toJson(result), callback);
        });
    }

    public static void makePurchase(String productJson, String subscriptionUpdateParamsJson, boolean isOfferPersonalized, AdaptyAndroidCallback callback) {
        AdaptyPaywallProduct product = parseJsonArgument(productJson, AdaptyPaywallProduct.class);
        if (product == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("productJson", methodName, callback);
            return;
        }

        AdaptySubscriptionUpdateParameters subscriptionUpdateParams = isNullOrBlank(subscriptionUpdateParamsJson) ? null : parseJsonArgument(subscriptionUpdateParamsJson, AdaptySubscriptionUpdateParameters.class);

        Adapty.makePurchase(UnityPlayer.currentActivity, product, subscriptionUpdateParams, isOfferPersonalized, result -> {
            sendMessageWithResult(helper.toJson(result), callback);
        });
    }

    public static void restorePurchases(AdaptyAndroidCallback callback) {
        Adapty.restorePurchases(result -> {
            sendMessageWithResult(helper.toJson(result), callback);
        });
    }

    public static void getProfile(AdaptyAndroidCallback callback) {
        Adapty.getProfile(result -> {
            sendMessageWithResult(helper.toJson(result), callback);
        });
    }

    public static void handleProfileUpdates() {
        Adapty.setOnProfileUpdatedListener(profile -> {
            sendDataToMessageHandler(ADAPTY_PROFILE_UPDATE_KEY, helper.toJson(profile));
        });
    }

    public static void updateAttribution(String attributionJson, String sourceName, String networkUserId, AdaptyAndroidCallback callback) {
        Map<String, Object> attribution = null;
        try {
            attribution = helper.fromJson(attributionJson, HashMap.class);
        } catch (Exception ignored) {}

        if (attribution == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("attributionJson", methodName, callback);
            return;
        }

        AdaptyAttributionSource source = helper.toAttributionSourceType(sourceName);
        if (source == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("sourceName", methodName, callback);
            return;
        }

        Adapty.updateAttribution(attribution, source, networkUserId, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    public static void setVariationId(String transactionId, String variationId, AdaptyAndroidCallback callback) {
        if (isNullOrBlank(transactionId)) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("transactionId", methodName, callback);
            return;
        }
        if (isNullOrBlank(variationId)) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("variationId", methodName, callback);
            return;
        }
        Adapty.setVariationId(transactionId, variationId, error -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    public static void logShowPaywall(String paywallJson, AdaptyAndroidCallback callback) {
        AdaptyPaywall paywall = parseJsonArgument(paywallJson, AdaptyPaywall.class);

        if (paywall == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("paywallJson", methodName, callback);
            return;
        }

        Adapty.logShowPaywall(paywall, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    public static void logShowOnboarding(String onboardingParamsJson, AdaptyAndroidCallback callback) {
        HashMap<?, ?> onboardingParams = parseJsonArgument(onboardingParamsJson, HashMap.class);

        if (onboardingParams == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("onboardingParamsJson", methodName, callback);
            return;
        }

        int screenOrder = -1;
        if (onboardingParams.get(ADAPTY_ONBOARDING_SCREEN_ORDER_KEY) instanceof Number) {
            screenOrder = ((Number) onboardingParams.get(ADAPTY_ONBOARDING_SCREEN_ORDER_KEY)).intValue();
        }

        if (screenOrder == -1) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError(ADAPTY_ONBOARDING_SCREEN_ORDER_KEY, methodName, callback);
            return;
        }

        Adapty.logShowOnboarding(
                (String) onboardingParams.get(ADAPTY_ONBOARDING_NAME_KEY),
                (String) onboardingParams.get(ADAPTY_ONBOARDING_SCREEN_NAME_KEY),
                screenOrder,
                (AdaptyError error) -> {
                    sendEmptyResultOrError(error, callback);
                }
        );
    }

    public static void setFallbackPaywalls(String paywalls, AdaptyAndroidCallback callback) {
        if (paywalls == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("paywalls", methodName, callback);
            return;
        }

        Adapty.setFallbackPaywalls(paywalls, error -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    public static void updateProfile(String paramsJson, AdaptyAndroidCallback callback) {
        AdaptyProfileParameters params = parseJsonArgument(paramsJson, AdaptyProfileParameters.class);

        if (params == null) {
            String methodName = new Object() {}.getClass().getEnclosingMethod().getName();
            sendParameterError("paramsJson", methodName, callback);
            return;
        }

        Adapty.updateProfile(params, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
        });
    }

    private static int handleLoadTimeoutParam(Long loadTimeoutMillis) {
        if (loadTimeoutMillis == null) return DEFAULT_PAYWALL_TIMEOUT_MILLIS;
        return (loadTimeoutMillis > Integer.MAX_VALUE) ? Integer.MAX_VALUE : loadTimeoutMillis.intValue();
    }

    private static <T> T parseJsonArgument(String json, Class<T> clazz) {
        if (isNullOrBlank(json)) return null;

        T result = null;
        try {
            result = helper.fromJson(json, clazz);
        } catch (Exception e) { }

        return result;
    }

    private static boolean isNullOrBlank(String str) {
        return str == null || str.equals("") || str.trim().equals("");
    }

    private static void sendMessageWithResult(String message, AdaptyAndroidCallback callback) {
        runOnUnityThread(() -> callback.onHandleResult(message));
    }

    private static void sendParameterError(String paramKey, String methodName, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        Map<String, Object> errorMap = new HashMap<>();
        errorMap.put(ADAPTY_ERROR_MESSAGE_KEY, ADAPTY_ERROR_DECODING_FAILED_MESSAGE);
        errorMap.put(ADAPTY_ERROR_CODE_KEY, ADAPTY_ERROR_CODE_DECODING_FAILED);
        errorMap.put(ADAPTY_ERROR_DETAIL_KEY, createErrorDetailsString(paramKey, methodName));
        message.put(ADAPTY_ERROR_KEY, errorMap);
        sendMessageWithResult(helper.toJson(message), callback);
    }

    private static String createErrorDetailsString(String paramKey, String methodName) {
        return "AdaptyPluginError.decodingFailed(Error while parsing parameter: " + paramKey + ", method: " + methodName + ")";
    }

    private static void sendEmptyResultOrError(AdaptyError error, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        if (error != null) {
            message.put(ADAPTY_ERROR_KEY, error);
        } else {
            message.put(ADAPTY_SUCCESS_KEY, true);
        }
        sendMessageWithResult(helper.toJson(message), callback);
    }

    private static void sendDataToMessageHandler(String key, String data) {
        runOnUnityThread(() -> {
            if (messageHandler != null) {
                messageHandler.onMessage(key, data);
            }
        });
    }
}

class Constants {

    public static final String ADAPTY_SUCCESS_KEY = "success";
    public static final String ADAPTY_PROFILE_UPDATE_KEY = "did_load_latest_profile";
    public static final String ADAPTY_ONBOARDING_NAME_KEY = "onboarding_name";
    public static final String ADAPTY_ONBOARDING_SCREEN_NAME_KEY = "onboarding_screen_name";
    public static final String ADAPTY_ONBOARDING_SCREEN_ORDER_KEY = "onboarding_screen_order";
    public static final String ADAPTY_ERROR_KEY = "error";
    public static final String ADAPTY_ERROR_CODE_KEY = "adapty_code";
    public static final String ADAPTY_ERROR_MESSAGE_KEY = "message";
    public static final String ADAPTY_ERROR_DETAIL_KEY = "detail";
    public static final String ADAPTY_ERROR_DECODING_FAILED_MESSAGE = "Decoding failed";
    public static final int ADAPTY_ERROR_CODE_DECODING_FAILED = 2006;
    public static final String ADAPTY_SDK_VERSION = "2.9.0";
}
