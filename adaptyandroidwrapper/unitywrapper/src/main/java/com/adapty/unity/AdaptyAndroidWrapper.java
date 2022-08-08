package com.adapty.unity;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.os.Handler;

import com.adapty.Adapty;
import com.adapty.errors.AdaptyError;
import com.adapty.errors.AdaptyErrorCode;
import com.adapty.models.AttributionType;
import com.adapty.models.GoogleValidationResult;
import com.adapty.models.PaywallModel;
import com.adapty.models.ProductModel;
import com.adapty.models.ProductSubscriptionPeriodModel;
import com.adapty.models.PromoModel;
import com.adapty.models.PurchaserInfoModel;
import com.adapty.models.SubscriptionUpdateParamModel;
import com.adapty.utils.AdaptyLogLevel;
import com.adapty.utils.ProfileParameterBuilder;
import com.google.gson.FieldNamingPolicy;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.reflect.TypeToken;
import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.concurrent.locks.ReentrantReadWriteLock;

import kotlin.Unit;

public class AdaptyAndroidWrapper {

    private static final Gson gson =
            new GsonBuilder()
                    .setFieldNamingPolicy(FieldNamingPolicy.LOWER_CASE_WITH_UNDERSCORES)
                    .registerTypeAdapter(ProductSubscriptionPeriodModel.class, new ProductSubscriptionPeriodModelSerializer())
                    .registerTypeAdapter(ProductModel.class, new ProductModelSerializer())
                    .registerTypeAdapter(SubscriptionUpdateParamModel.class, new SubscriptionUpdateParamModelDeserializer())
                    .registerTypeAdapter(ProfileParameterBuilder.class, new ProfileParameterBuilderDeserializer())
                    .registerTypeAdapter(AdaptyError.class, new AdaptyErrorSerializer())
                    .create();

    private static final ReentrantReadWriteLock paywallsProductsLock = new ReentrantReadWriteLock();
    private static final List<PaywallModel> paywalls = new ArrayList<>();
    private static final Map<String, ProductModel> products = new HashMap<>();
    private static PaywallModel promoPaywall = null;
    private static volatile AdaptyUnityPushHandler pushHandler = null;

    private static Handler unityMainThreadHandler;
    private static AdaptyAndroidMessageHandler messageHandler;

    public static void registerMessageHandler(AdaptyAndroidMessageHandler handler) {
        messageHandler = handler;
        if(unityMainThreadHandler == null) {
            unityMainThreadHandler = new Handler();
            listenPurchaserInfoUpdates();
            listenPromoUpdates();
            listenRemoteConfigUpdates();
        }
    }

    public static void runOnUnityThread(Runnable runnable) {
        if(unityMainThreadHandler != null && runnable != null) {
            unityMainThreadHandler.post(runnable);
        }
    }

    public static void setLogLevel(String logLevel) {
        switch (logLevel) {
            case "errors":
                Adapty.setLogLevel(AdaptyLogLevel.ERROR);
                break;
            case "verbose":
                Adapty.setLogLevel(AdaptyLogLevel.VERBOSE);
                break;
            case "all":
                Adapty.setLogLevel(AdaptyLogLevel.ALL);
                break;
            default:
                Adapty.setLogLevel(AdaptyLogLevel.NONE);
                break;
        }
    }

    public static void activate(Context context, String key, String customerUserId, boolean observerMode) {
        Adapty.activate(context, key, customerUserId, observerMode);
    }

    public static void identify(String customerUserId, AdaptyAndroidCallback callback) {
        Adapty.identify(customerUserId, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
            return Unit.INSTANCE;
        });
    }

    public static void logout(AdaptyAndroidCallback callback) {
        Adapty.logout((AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
            return Unit.INSTANCE;
        });
    }

    public static void getPaywalls(boolean forceUpdate, AdaptyAndroidCallback callback) {
        Adapty.getPaywalls(forceUpdate, (List<PaywallModel> paywalls, List<ProductModel> products, AdaptyError error) -> {
            Map<String, Object> message = new HashMap<>();
            if (error != null) {
                message.put("error", error);
            } else {
                Map<String, Object> successMessage = new HashMap<>();
                if (paywalls != null) {
                    cachePaywalls(paywalls);
                    successMessage.put("paywalls", paywalls);
                }
                if (products != null) {
                    cacheProducts(products);
                    successMessage.put("products", products);
                }
                message.put("success", successMessage);
            }
            sendMessageWithResult(gson.toJson(message), callback);
            return Unit.INSTANCE;
        });
    }

    public static void makePurchase(String productId, String variationId, String subscriptionUpdateParamsJson, AdaptyAndroidCallback callback) {
        SubscriptionUpdateParamModel subscriptionUpdateParams = isNullOrBlank(subscriptionUpdateParamsJson) ? null : gson.fromJson(subscriptionUpdateParamsJson, SubscriptionUpdateParamModel.class);
        ProductModel product = findProduct(productId, variationId);
        if (product == null) {
            sendBridgeRelatedError(10001, "Not found product (id: " + productId + ", variationId: " + variationId + ")", callback);
            return;
        }
        Adapty.makePurchase(UnityPlayer.currentActivity, product, subscriptionUpdateParams, (PurchaserInfoModel purchaserInfo, String purchaseToken, GoogleValidationResult googleValidationResult, ProductModel _product, AdaptyError error) -> {
            Map<String, Object> message = new HashMap<>();
            if (error != null) {
                message.put("error", error);
            } else {
                Map<String, Object> successMessage = new HashMap<>();
                if (purchaserInfo != null) {
                    successMessage.put("purchaser_info", purchaserInfo);
                }
                if (purchaseToken != null) {
                    successMessage.put("purchase_token", purchaseToken);
                }
                if (_product != null) {
                    successMessage.put("product", _product);
                }
                message.put("success", successMessage);
            }
            sendMessageWithResult(gson.toJson(message), callback);
            return Unit.INSTANCE;
        });
    }

    public static void restorePurchases(AdaptyAndroidCallback callback) {
        Adapty.restorePurchases((PurchaserInfoModel purchaserInfo, List<GoogleValidationResult> googleValidationResults, AdaptyError error) -> {
            Map<String, Object> message = new HashMap<>();
            if (error != null) {
                message.put("error", error);
            } else {
                Map<String, Object> successMessage = new HashMap<>();
                if (purchaserInfo != null) {
                    successMessage.put("purchaser_info", purchaserInfo);
                }
                message.put("success", successMessage);
            }
            sendMessageWithResult(gson.toJson(message), callback);
            return Unit.INSTANCE;
        });
    }

    public static void getPurchaserInfo(boolean forceUpdate, AdaptyAndroidCallback callback) {
        Adapty.getPurchaserInfo(forceUpdate, (PurchaserInfoModel purchaserInfo, AdaptyError error) -> {
            if (purchaserInfo != null) {
                Map<String, Object> message = new HashMap<>();
                message.put("success", purchaserInfo);
                sendMessageWithResult(gson.toJson(message), callback);
            }
            if (error != null) {
                sendEmptyResultOrError(error, callback);
            }
            return Unit.INSTANCE;
        });
    }

    public static void listenPurchaserInfoUpdates() {
        Adapty.setOnPurchaserInfoUpdatedListener(purchaserInfo -> {
            sendDataToMessageHandler("purchaser_info_update", gson.toJson(purchaserInfo));
        });
    }

    public static void updateAttribution(String attributionJson, String sourceName, String networkUserId, AdaptyAndroidCallback callback) {
        Map<String, Object> attribution = gson.fromJson(attributionJson, new TypeToken<Map<String, Object>>(){}.getType());
        AttributionType source;
        switch (sourceName) {
            default:
            case "appsflyer":
                source = AttributionType.APPSFLYER;
                break;

            case "adjust":
                source = AttributionType.ADJUST;
                break;

            case "branch":
                source = AttributionType.BRANCH;
                break;

            case "custom":
                source = AttributionType.CUSTOM;
                break;
        }
        Adapty.updateAttribution(attribution, source, networkUserId, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
            return Unit.INSTANCE;
        });
    }

    public static void setExternalAnalyticsEnabled(boolean enabled, AdaptyAndroidCallback callback) {
        Adapty.setExternalAnalyticsEnabled(enabled, error -> {
            sendEmptyResultOrError(error, callback);
            return Unit.INSTANCE;
        });
    }

    public static void setTransactionVariationId(String transactionId, String variationId, AdaptyAndroidCallback callback) {
        if (isNullOrBlank(transactionId)) {
            sendMissingParamError("No transaction id passed", callback);
            return;
        }
        if (isNullOrBlank(variationId)) {
            sendMissingParamError("No variation id passed", callback);
            return;
        }
        Adapty.setTransactionVariationId(transactionId, variationId, error -> {
            sendEmptyResultOrError(error, callback);
            return Unit.INSTANCE;
        });
    }

    public static void logShowPaywall(String variationId, AdaptyAndroidCallback callback) {
        PaywallModel paywall = findPaywall(variationId);
        if (paywall != null) {
            Adapty.logShowPaywall(paywall);
            sendEmptyResultOrError(null, callback);
        } else {
            sendBridgeRelatedError(10002, "Not found paywall (with variationId: " + variationId + ")", callback);
        }
    }

    public static void setFallbackPaywalls(String paywalls, AdaptyAndroidCallback callback) {
        if (paywalls == null) {
            sendMissingParamError("No paywalls passed", callback);
        } else {
            Adapty.setFallbackPaywalls(paywalls, error -> {
                sendEmptyResultOrError(error, callback);
                return Unit.INSTANCE;
            });
        }
    }

    public static void updateProfile(String paramsJson, AdaptyAndroidCallback callback) {
        ProfileParameterBuilder params = gson.fromJson(paramsJson, ProfileParameterBuilder.class);

        Adapty.updateProfile(params, (AdaptyError error) -> {
            sendEmptyResultOrError(error, callback);
            return Unit.INSTANCE;
        });
    }

    public static void getPromo(AdaptyAndroidCallback callback) {
        Adapty.getPromo((PromoModel promo, AdaptyError error) -> {
            Map<String, Object> message = new HashMap<>();
            if (promo != null) {
                message.put("success", promo);
                sendMessageWithResult(gson.toJson(message), callback);
            } else if (error == null) {
                try {
                    paywallsProductsLock.writeLock().lock();
                    promoPaywall = null;
                } finally {
                    paywallsProductsLock.writeLock().unlock();
                }
                message.put("success", null);
                sendMessageWithResult(gson.toJson(message), callback);
            }
            if (error != null) {
                sendEmptyResultOrError(error, callback);
            }
            return Unit.INSTANCE;
        });
    }

    public static void listenPromoUpdates() {
        Adapty.setOnPromoReceivedListener(promo -> {
            try {
                paywallsProductsLock.writeLock().lock();
                promoPaywall = promo.getPaywall();
            } finally {
                paywallsProductsLock.writeLock().unlock();
            }
            sendDataToMessageHandler("promo_received", gson.toJson(promo));
        });
    }

    public static void listenRemoteConfigUpdates() {
        Adapty.setOnPaywallsForConfigReceivedListener(paywalls -> {
            sendDataToMessageHandler("remote_config_update", gson.toJson(paywalls));
        });
    }

    public static void newPushToken(String pushToken) {
        if (!isNullOrBlank(pushToken)) {
            Adapty.refreshPushToken(pushToken);
        }
    }

    public static void pushReceived(String pushMessageJson) {
        Map<String, String> data = gson.fromJson(pushMessageJson, new TypeToken<Map<String, String>>(){}.getType());
        AdaptyUnityPushHandler pushHandler = getOrCreatePushHandler();
        if (pushHandler != null) {
            pushHandler.handleNotification(data);
        }
    }

    public static void handlePromoIntent(Intent intent) {
        Adapty.handlePromoIntent(intent, (promoModel, adaptyError) -> Unit.INSTANCE);
    }

    private static ProductModel findProduct(String productId, String variationId) {
        try {
            paywallsProductsLock.readLock().lock();
            if (variationId != null) {
                PaywallModel paywall = findPaywall(variationId);
                if (paywall != null) {
                    for (ProductModel product: paywall.getProducts()) {
                        if (product.getVendorProductId().equals(productId)) {
                            return product;
                        }
                    }
                }
                return null;
            } else {
                return products.get(productId);
            }
        } finally {
            paywallsProductsLock.readLock().unlock();
        }
    }

    private static PaywallModel findPaywall(String variationId) {
        if (variationId == null) return null;
        try {
            paywallsProductsLock.readLock().lock();
            for (PaywallModel paywall: paywalls) {
                if (variationId.equals(paywall.getVariationId())) {
                    return paywall;
                }
            }
            return (promoPaywall != null && variationId.equals(promoPaywall.getVariationId())) ? promoPaywall : null;
        } finally {
            paywallsProductsLock.readLock().unlock();
        }
    }

    private static void cachePaywalls(List<PaywallModel> newPaywalls) {
        try {
            paywallsProductsLock.writeLock().lock();
            paywalls.clear();
            paywalls.addAll(newPaywalls);
        } finally {
            paywallsProductsLock.writeLock().unlock();
        }
    }

    private static void cacheProducts(List<ProductModel> newProducts) {
        try {
            paywallsProductsLock.writeLock().lock();
            products.clear();
            for (ProductModel product: newProducts) {
                products.put(product.getVendorProductId(), product);
            }
        } finally {
            paywallsProductsLock.writeLock().unlock();
        }
    }

    private static AdaptyUnityPushHandler getOrCreatePushHandler() {
        if (pushHandler == null) {
            synchronized (AdaptyAndroidWrapper.class) {
                if (pushHandler == null) {
                    Activity activity = UnityPlayer.currentActivity;
                    if (activity != null) {
                        try {
                            Bundle metadata = activity.getPackageManager()
                                    .getApplicationInfo(activity.getPackageName(), PackageManager.GET_META_DATA)
                                    .metaData;
                            AdaptyUnityPushHandler handler = new AdaptyUnityPushHandler(
                                    activity.getApplicationContext(),
                                    (metadata != null && metadata.getString("AdaptyNotificationClickAction") != null) ? metadata.getString("AdaptyNotificationClickAction") : "ADAPTY_PROMO_CLICK_ACTION",
                                    (metadata != null) ? metadata.getInt("AdaptyNotificationSmallIcon", R.drawable.ic_adapty_promo_push) : R.drawable.ic_adapty_promo_push
                            );
                            pushHandler = handler;
                        } catch (Exception e) { }
                    }
                }
            }
        }
        return pushHandler;
    }

    private static boolean isNullOrBlank(String str) {
        return str == null || str.equals("") || str.trim().equals("");
    }

    private static void sendMessageWithResult(String message, AdaptyAndroidCallback callback) {
        runOnUnityThread(() -> callback.onHandleResult(message));
    }

    private static void sendMissingParamError(String errorMessage, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        Map<String, Object> errorMap = new HashMap<>();
        errorMap.put("message", errorMessage);
        errorMap.put("adapty_code", Utils.intFromErrorCode(AdaptyErrorCode.MISSING_PARAMETER));
        message.put("error", errorMap);
        sendMessageWithResult(gson.toJson(message), callback);
    }

    private static void sendEmptyResultOrError(AdaptyError error, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        if (error != null) {
            message.put("error", error);
        } else {
            message.put("success", true);
        }
        sendMessageWithResult(gson.toJson(message), callback);
    }

    private static void sendBridgeRelatedError(int code, String text, AdaptyAndroidCallback callback) {
        Map<String, Object> message = new HashMap<>();
        Map<String, Object> error = new HashMap<>();
        error.put("adapty_code", code);
        error.put("message", text);
        message.put("error", error);
        sendMessageWithResult(gson.toJson(message), callback);
    }

    private static void sendDataToMessageHandler(String key, String data) {
        runOnUnityThread(() -> {
            if (messageHandler != null) {
                messageHandler.onMessage(key, data);
            }
        });
    }
}
