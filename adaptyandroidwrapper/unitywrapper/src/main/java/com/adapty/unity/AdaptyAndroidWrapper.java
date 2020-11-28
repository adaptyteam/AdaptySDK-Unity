package com.adapty.unity;

import android.app.Activity;
import android.content.Context;
import android.util.Log;

import com.adapty.Adapty;
import com.adapty.api.AdaptyError;
import com.adapty.api.AttributionType;
import com.adapty.api.entity.DataState;
import com.adapty.api.entity.paywalls.OnPromoReceivedListener;
import com.adapty.api.entity.paywalls.PaywallModel;
import com.adapty.api.entity.paywalls.ProductModel;
import com.adapty.api.entity.paywalls.PromoModel;
import com.adapty.api.entity.profile.update.ProfileParameterBuilder;
import com.adapty.api.entity.purchaserInfo.OnPurchaserInfoUpdatedListener;
import com.adapty.api.entity.purchaserInfo.model.PurchaserInfoModel;
import com.adapty.api.entity.validate.GoogleValidationResult;
import com.adapty.purchase.PurchaseType;
import com.adapty.utils.AdaptyLogLevel;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import org.jetbrains.annotations.NotNull;

import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;
import java.util.Stack;

import kotlin.Unit;

public class AdaptyAndroidWrapper {
    private final static String tag = "AdaptyAndroidWrapper";

    private static final String Callback_identify = "OnIdentify";
    private static final String Callback_logout = "OnLogout";
    private static final String Callback_getPaywalls = "OnGetPaywalls";
    private static final String Callback_makePurchase = "OnMakePurchase";
    private static final String Callback_restorePurchases = "OnRestorePurchases";
    private static final String Callback_syncPurchases = "OnSyncPurchases";
    private static final String Callback_validatePurchase = "OnValidateGooglePurchase";
    private static final String Callback_getPurchaserInfo = "OnGetPurchaserInfo";
    private static final String Callback_setOnPurchaserInfoUpdatedListener = "OnPurchaserInfoUpdated";
    private static final String Callback_updateAttribution = "OnUpdateAttribution";
    private static final String Callback_updateProfile = "OnUpdateProfile";
    private static final String Callback_getPromo = "OnGetPromo";
    private static final String Callback_setOnPromoReceivedListener = "OnPromoReceived";

    private static final Stack<Callback> callbacks = new Stack<>();

    public static Callback getCallback() {
        if (!callbacks.empty()) {
            return callbacks.pop();
        }
        return null;
    }

    public static void SendMessage(String objectName, String method, String message) {
        Callback callback = new Callback();
        callback.objectName = objectName;
        callback.method = method;
        callback.message = message;
        callbacks.push(callback);
    }

    public static void setLogLevel(int logLevel) {
        Log.d(tag, "setLogLevel");
        switch (logLevel) {
            case 0:
                Adapty.setLogLevel(AdaptyLogLevel.NONE);
                break;

            case 1:
                Adapty.setLogLevel(AdaptyLogLevel.ERROR);
                break;

            case 2:
                Adapty.setLogLevel(AdaptyLogLevel.VERBOSE);
                break;
        }
    }

    public static void activate(Context context, String key) {
        Log.d(tag, "activate");
        Adapty.activate(context, key);
    }

    public static void identify(String customerUserId, String objectName) {
        Log.d(tag, "identify");
        Adapty.identify(customerUserId, (AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_identify, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void logout(String objectName) {
        Log.d(tag, "logout");
        Adapty.logout((AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_logout, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void getPaywalls(String objectName) {
        Log.d(tag, "getPaywalls");
        Adapty.getPaywalls((List<PaywallModel> paywalls, ArrayList<ProductModel> products, DataState state, AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (paywalls != null) {
                ArrayList<Map<String, Object>> paywallsArray = new ArrayList<>();
                for (PaywallModel paywall : paywalls) {
                    paywallsArray.add(AdaptyModels.paywallToHashtable(paywall));
                }
                message.put("paywalls", paywallsArray);
            }
            if (products != null) {
                ArrayList<Map<String, Object>> productsArray = new ArrayList<>();
                for (ProductModel product : products) {
                    productsArray.add(AdaptyModels.productToHashtable(product));
                }
                message.put("products", productsArray);
            }
            message.put("state", state.toString().toLowerCase());
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_getPaywalls, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void makePurchase(Activity activity, String productJson, String objectName) {
        Log.d(tag, "makePurchase");
        Gson gson = new Gson();
        ProductModel product = AdaptyModels.hashtableToProduct(gson.fromJson(productJson, new TypeToken<Map<String, Object>>(){}.getType()));
        Adapty.makePurchase(activity, product, (PurchaserInfoModel purchaserInfo, String purchaseToken, GoogleValidationResult googleValidationResult, ProductModel _product, AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (purchaserInfo != null) {
                message.put("purchaserInfo", AdaptyModels.purchaserInfoToHashtable(purchaserInfo));
            }
            if (purchaseToken != null) {
                message.put("purchaseToken", purchaseToken);
            }
            if (googleValidationResult != null) {
                message.put("validationResult", googleValidationResult);
            }
            if (_product != null) {
                message.put("product", AdaptyModels.productToHashtable(_product));
            }
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            SendMessage(objectName, Callback_makePurchase, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void restorePurchases(String objectName) {
        Log.d(tag, "restorePurchases");
        Adapty.restorePurchases((PurchaserInfoModel purchaserInfo, List<GoogleValidationResult> googleValidationResults, AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (purchaserInfo != null) {
                message.put("purchaserInfo", AdaptyModels.purchaserInfoToHashtable(purchaserInfo));
            }
            if (googleValidationResults != null) {
                ArrayList<Map<String, Object>> validationResultsArray = new ArrayList<>();
                    for (GoogleValidationResult googleValidationResult : googleValidationResults) {
                        validationResultsArray.add(AdaptyModels.googleValidationResultToHashtable(googleValidationResult));
                    }
                    message.put("validationResults",  validationResultsArray);
            }
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_restorePurchases, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void syncPurchases(String objectName) {
        Log.d(tag, "syncPurchases");
        Adapty.syncPurchases((AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_syncPurchases, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void validatePurchase(int purchaseTypeInt, String productId, String purchaseToken, String purchaseOrderId, String productJson, String objectName) {
        Log.d(tag, "validatePurchase");
        PurchaseType purchaseType = PurchaseType.INAPP;
        if (purchaseTypeInt == 1) {
            purchaseType = PurchaseType.SUBS;
        }
        Gson gson = new Gson();
        ProductModel product = AdaptyModels.hashtableToProduct(gson.fromJson(productJson, new TypeToken<Map<String, Object>>(){}.getType()));
        Adapty.validatePurchase(purchaseType, productId, purchaseToken, purchaseOrderId, product,
                (PurchaserInfoModel purchaserInfo, String _purchaseToken, GoogleValidationResult googleValidationResult, AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (purchaserInfo != null) {
                message.put("purchaserInfo", AdaptyModels.purchaserInfoToHashtable(purchaserInfo));
            }
            if (googleValidationResult != null) {
                message.put("validationResult", googleValidationResult);
            }
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            SendMessage(objectName, Callback_validatePurchase, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void getPurchaserInfo(String objectName) {
        Log.d(tag, "getPurchaserInfo");
        Adapty.getPurchaserInfo((PurchaserInfoModel purchaserInfo, DataState state, AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (purchaserInfo != null) {
                message.put("purchaserInfo", AdaptyModels.purchaserInfoToHashtable(purchaserInfo));
            }
            message.put("state", state.toString().toLowerCase());
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_getPurchaserInfo, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void setOnPurchaserInfoUpdatedListener(String objectName) {
        Log.d(tag, "setOnPurchaserInfoUpdatedListener");
        OnPurchaserInfoUpdatedListener listener = new OnPurchaserInfoUpdatedListener() {
            @Override
            public void onPurchaserInfoReceived(@NotNull PurchaserInfoModel purchaserInfo) {
                Hashtable<String, Object> message = new Hashtable<>();
                message.put("purchaserInfo", AdaptyModels.purchaserInfoToHashtable(purchaserInfo));
                Gson gson = new Gson();
                SendMessage(objectName, Callback_setOnPurchaserInfoUpdatedListener, gson.toJson(message));
            }
        };
        Adapty.setOnPurchaserInfoUpdatedListener(listener);
    }

    public static void updateAttribution(String attributionJson, int sourceInt, String networkUserId, String objectName) {
        Log.d(tag, "getPurchaserInfo");
        Gson gson = new Gson();
        Map<String, Object> attribution = gson.fromJson(attributionJson, new TypeToken<Map<String, Object>>(){}.getType());
        AttributionType source;
        switch (sourceInt) {
            default:
            case 0:
                source = AttributionType.APPSFLYER;
                break;

            case 1:
                source = AttributionType.ADJUST;
                break;

            case 2:
                source = AttributionType.BRANCH;
                break;

            case 3:
                source = AttributionType.CUSTOM;
                break;
        }
        Adapty.updateAttribution(attribution, source, networkUserId, (AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            SendMessage(objectName, Callback_updateAttribution, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void updateProfile(String paramsJson, String objectName) {
        Log.d(tag, "updateProfile");
        Gson gson = new Gson();
        Map<String, Object> paramsDictionary = gson.fromJson(paramsJson, new TypeToken<Map<String, Object>>(){}.getType());
        ProfileParameterBuilder params = AdaptyModels.hashtableToProfileParameterBuilder(paramsDictionary);

        Adapty.updateProfile(params, (AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            SendMessage(objectName, Callback_updateProfile, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void getPromo(String objectName) {
        Log.d(tag, "getPromo");
        Adapty.getPromo((PromoModel promo, AdaptyError error) -> {
            Hashtable<String, Object> message = new Hashtable<>();
            if (promo != null) {
                message.put("promo", AdaptyModels.promoToHashtable(promo));
            }
            if (error != null) {
                message.put("error", AdaptyModels.adaptyErrorToHashtable(error));
            }
            Gson gson = new Gson();
            SendMessage(objectName, Callback_getPromo, gson.toJson(message));
            return Unit.INSTANCE;
        });
    }

    public static void setOnPromoReceivedListener(String objectName) {
        Log.d(tag, "setOnPromoReceivedListener");
        OnPromoReceivedListener listener = new OnPromoReceivedListener() {
            @Override
            public void onPromoReceived(@NotNull PromoModel promo) {
                Hashtable<String, Object> message = new Hashtable<>();
                message.put("promo", AdaptyModels.promoToHashtable(promo));
                Gson gson = new Gson();
                SendMessage(objectName, Callback_setOnPromoReceivedListener, gson.toJson(message));
            }
        };
        Adapty.setOnPromoReceivedListener(listener);
    }
}
