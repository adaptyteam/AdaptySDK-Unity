package com.adapty.unity;

import com.adapty.api.AdaptyError;
import com.adapty.api.entity.paywalls.PaywallModel;
import com.adapty.api.entity.paywalls.ProductModel;
import com.adapty.api.entity.paywalls.ProductModel.PeriodUnit;
import com.adapty.api.entity.paywalls.ProductModel.ProductSubscriptionPeriodModel;
import com.adapty.api.entity.paywalls.ProductModel.ProductDiscountModel;
import com.adapty.api.entity.paywalls.PromoModel;
import com.adapty.api.entity.profile.update.Date;
import com.adapty.api.entity.profile.update.Gender;
import com.adapty.api.entity.profile.update.ProfileParameterBuilder;
import com.adapty.api.entity.purchaserInfo.model.AccessLevelInfoModel;
import com.adapty.api.entity.purchaserInfo.model.NonSubscriptionInfoModel;
import com.adapty.api.entity.purchaserInfo.model.PurchaserInfoModel;
import com.adapty.api.entity.purchaserInfo.model.SubscriptionInfoModel;
import com.adapty.api.entity.validate.GoogleValidationResult;
import com.adapty.api.entity.validate.IntroductoryPriceInfo;
import com.adapty.api.entity.validate.Price;
import com.adapty.api.entity.validate.SubscriptionCancelSurveyResult;
import com.adapty.api.entity.validate.SubscriptionPriceChange;
import com.android.billingclient.api.SkuDetails;
import com.google.gson.Gson;

import java.math.BigDecimal;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.List;
import java.util.Map;

public class AdaptyModels {
    static Hashtable<String, SkuDetails> skuDetailsCache = new Hashtable<>();
    static String periodUnitToString(PeriodUnit unit){
        switch (unit) {
            case D:
                return "day";
            case W:
                return "week";
            case M:
                return "month";
            case Y:
                return "year";
            default:
                return "unknown";
        }
    }

    static PeriodUnit stringToPeriodUnit(String unit) {
        switch (unit) {
            default:
            case "day":
                return PeriodUnit.D;
            case "week":
                return PeriodUnit.W;
            case "month":
                return PeriodUnit.M;
            case "year":
                return PeriodUnit.Y;
        }
    }

    static Gender stringToGender(String gender) {
        switch (gender) {
            default:
            case "female":
                return Gender.FEMALE;
            case "male":
                return Gender.MALE;
            case "other":
                return Gender.OTHER;
        }
    }

    static Map<String, Object> adaptyErrorToHashtable(AdaptyError adaptyError) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        hashtable.put("message", adaptyError.getMessage());
        hashtable.put("code", adaptyError.getAdaptyErrorCode().getValue());
        return hashtable;
    }

    static Map<String, Object> subscriptionPeriodToHashtable(ProductSubscriptionPeriodModel subscriptionPeriod) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (subscriptionPeriod.getUnit() != null) {
            hashtable.put("unit", periodUnitToString(subscriptionPeriod.getUnit()));
        }
        if (subscriptionPeriod.getNumberOfUnits() != null) {
            hashtable.put("numberOfUnits", subscriptionPeriod.getNumberOfUnits());
        }
        return hashtable;
    }

    static Map<String, Object> discountToHashtable(ProductDiscountModel discount) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        hashtable.put("price", discount.getPrice().toString());
        hashtable.put("numberOfPeriods", discount.getNumberOfPeriods());
        hashtable.put("localizedPrice", discount.getLocalizedPrice());
        hashtable.put("subscriptionPeriod", subscriptionPeriodToHashtable(discount.getSubscriptionPeriod()));
        return hashtable;
    }

    static Map<String, Object> productToHashtable(ProductModel product) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (product.getVendorProductId() != null) {
            hashtable.put("vendorProductId", product.getVendorProductId());
        }
        if (product.getLocalizedTitle() != null) {
            hashtable.put("localizedTitle", product.getLocalizedTitle());
        }
        if (product.getLocalizedDescription() != null) {
            hashtable.put("localizedDescription", product.getLocalizedDescription());
        }
        if (product.getLocalizedPrice() != null) {
            hashtable.put("localizedPrice", product.getLocalizedPrice());
        }
        if (product.getPrice() != null) {
            hashtable.put("price", product.getPrice().toString());
        }
        if (product.getCurrencyCode() != null) {
            hashtable.put("currencyCode", product.getCurrencyCode());
        }
        if (product.getCurrencySymbol() != null) {
            hashtable.put("currencySymbol", product.getCurrencySymbol());
        }
        if (product.getSubscriptionPeriod() != null) {
            hashtable.put("subscriptionPeriod", subscriptionPeriodToHashtable(product.getSubscriptionPeriod()));
        }
        hashtable.put("introductoryOfferEligibility", product.getIntroductoryOfferEligibility());
        hashtable.put("promotionalOfferEligibility", product.getPromotionalOfferEligibility());
        if (product.getIntroductoryDiscount() != null) {
            hashtable.put("introductoryDiscount", discountToHashtable(product.getIntroductoryDiscount()));
        }
        if (product.getSkuDetails() != null) {
            hashtable.put("skuId", product.getSkuDetails().getSku());
            skuDetailsCache.put(product.getSkuDetails().getSku(), product.getSkuDetails());
        }
        return hashtable;
    }

    static Map<String, Object> paywallToHashtable(PaywallModel paywall) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (paywall.getDeveloperId() != null) {
            hashtable.put("developerId", paywall.getDeveloperId());
        }
        if (paywall.getVariationId() != null) {
            hashtable.put("variationId", paywall.getVariationId());
        }
        if (paywall.getRevision() != null) {
            hashtable.put("revision", paywall.getRevision());
        }
        if (paywall.isPromo() != null) {
            hashtable.put("isPromo", paywall.isPromo());
        }
        if (paywall.getCustomPayload() != null) {
            Gson gson = new Gson();
            hashtable.put("customPayload", gson.fromJson(paywall.getCustomPayload(), Map.class));
        }

        if (paywall.getProducts() != null) {
            ArrayList<Map<String, Object>> productsArray = new ArrayList<>();
            ArrayList<ProductModel> products = paywall.getProducts();
            if (products != null) {
                for (int i = 0; i < products.size(); ++i) {
                    if (products.get(i) != null) {
                        productsArray.add(productToHashtable(products.get(i)));
                    }
                }
            }
            hashtable.put("products", productsArray);
        }
        return hashtable;
    }

    static Map<String, Object> promoToHashtable(PromoModel promo) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (promo.getPromoType() != null) {
            hashtable.put("promoType", promo.getPromoType());
        }
        if (promo.getVariationId() != null) {
            hashtable.put("variationId", promo.getVariationId());
        }
        if (promo.getExpiresAt() != null) {
            hashtable.put("expiresAt", promo.getExpiresAt());
        }
        if (promo.getPaywall() != null) {
            hashtable.put("paywall", paywallToHashtable(promo.getPaywall()));
        }
        return hashtable;
    }

    static Map<String, Object> accessLevelInfoToHashtable(AccessLevelInfoModel accessLevel) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (accessLevel.isActive() != null) {
            hashtable.put("isActive", accessLevel.isActive());
        }
        if (accessLevel.getVendorProductId() != null) {
            hashtable.put("vendorProductId", accessLevel.getVendorProductId());
        }
        if (accessLevel.getVendorTransactionId() != null) {
            hashtable.put("vendorTransactionId", accessLevel.getVendorTransactionId());
        }
        if (accessLevel.getVendorOriginalTransactionId() != null) {
            hashtable.put("vendorOriginalTransactionId", accessLevel.getVendorOriginalTransactionId());
        }
        if (accessLevel.getStore() != null) {
            hashtable.put("store", accessLevel.getStore());
        }
        if (accessLevel.getActivatedAt() != null) {
            hashtable.put("activatedAt", accessLevel.getActivatedAt());
        }
        if (accessLevel.getStartsAt() != null) {
            hashtable.put("startsAt", accessLevel.getStartsAt());
        }
        if (accessLevel.getRenewedAt() != null) {
            hashtable.put("renewedAt", accessLevel.getRenewedAt());
        }
        if (accessLevel.getExpiresAt() != null) {
            hashtable.put("expiresAt", accessLevel.getExpiresAt());
        }
        if (accessLevel.isLifetime() != null) {
            hashtable.put("isLifetime", accessLevel.isLifetime());
        }
        if (accessLevel.getWillRenew() != null) {
            hashtable.put("willRenew", accessLevel.getWillRenew());
        }
        if (accessLevel.isInGracePeriod() != null) {
            hashtable.put("isInGracePeriod", accessLevel.isInGracePeriod());
        }
        if (accessLevel.getUnsubscribedAt() != null) {
            hashtable.put("unsubscribedAt", accessLevel.getUnsubscribedAt());
        }
        if (accessLevel.getBillingIssueDetectedAt() != null) {
            hashtable.put("billingIssueDetectedAt", accessLevel.getBillingIssueDetectedAt());
        }
        if (accessLevel.getCancellationReason() != null) {
            hashtable.put("cancellationReason", accessLevel.getCancellationReason());
        }
        if (accessLevel.isRefund() != null) {
            hashtable.put("isRefund", accessLevel.isRefund());
        }
        if (accessLevel.getActiveIntroductoryOfferType() != null) {
            hashtable.put("activeIntroductoryOfferType", accessLevel.getActiveIntroductoryOfferType());
        }
        if (accessLevel.getActivePromotionalOfferType() != null) {
            hashtable.put("activePromotionalOfferType", accessLevel.getActivePromotionalOfferType());
        }
        return hashtable;
    }

    static Map<String, Object> subscriptionInfoToHashtable(SubscriptionInfoModel subscription) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (subscription.isActive() != null) {
            hashtable.put("isActive", subscription.isActive());
        }
        if (subscription.getVendorProductId() != null) {
            hashtable.put("vendorProductId", subscription.getVendorProductId());
        }
        if (subscription.getStore() != null) {
            hashtable.put("store", subscription.getStore());
        }
        if (subscription.getActivatedAt() != null) {
            hashtable.put("activatedAt", subscription.getActivatedAt());
        }
        if (subscription.getStartsAt() != null) {
            hashtable.put("startsAt", subscription.getStartsAt());
        }
        if (subscription.getRenewedAt() != null) {
            hashtable.put("renewedAt", subscription.getRenewedAt());
        }
        if (subscription.getExpiresAt() != null) {
            hashtable.put("expiresAt", subscription.getExpiresAt());
        }
        if (subscription.isLifetime() != null) {
            hashtable.put("isLifetime", subscription.isLifetime());
        }
        if (subscription.getWillRenew() != null) {
            hashtable.put("willRenew", subscription.getWillRenew());
        }
        if (subscription.isInGracePeriod() != null) {
            hashtable.put("isInGracePeriod", subscription.isInGracePeriod());
        }
        if (subscription.getUnsubscribedAt() != null) {
            hashtable.put("unsubscribedAt", subscription.getUnsubscribedAt());
        }
        if (subscription.getBillingIssueDetectedAt() != null) {
            hashtable.put("billingIssueDetectedAt", subscription.getBillingIssueDetectedAt());
        }
        if (subscription.getCancellationReason() != null) {
            hashtable.put("cancellationReason", subscription.getCancellationReason());
        }
        if (subscription.isRefund() != null) {
            hashtable.put("isRefund", subscription.isRefund());
        }
        if (subscription.getActiveIntroductoryOfferType() != null) {
            hashtable.put("activeIntroductoryOfferType", subscription.getActiveIntroductoryOfferType());
        }
        if (subscription.getActivePromotionalOfferType() != null) {
            hashtable.put("activePromotionalOfferType", subscription.getActivePromotionalOfferType());
        }
        if (subscription.isSandbox() != null) {
            hashtable.put("isSandbox", subscription.isSandbox());
        }
        return hashtable;
    }

    static Map<String, Object> nonSubscriptionInfoToHashtable(NonSubscriptionInfoModel nonSubscription) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (nonSubscription.getPurchaseId() != null) {
            hashtable.put("purchaseId", nonSubscription.getPurchaseId());
        }
        if (nonSubscription.getVendorProductId() != null) {
            hashtable.put("vendorProductId", nonSubscription.getVendorProductId());
        }
        if (nonSubscription.getStore() != null) {
            hashtable.put("store", nonSubscription.getStore());
        }
        if (nonSubscription.getPurchasedAt() != null) {
            hashtable.put("purchasedAt", nonSubscription.getPurchasedAt());
        }
        if (nonSubscription.isRefund() != null) {
            hashtable.put("isRefund", nonSubscription.isRefund());
        }
        if (nonSubscription.isOneTime() != null) {
            hashtable.put("isOneTime", nonSubscription.isOneTime());
        }
        if (nonSubscription.isSandbox() != null) {
            hashtable.put("isSandbox", nonSubscription.isSandbox());
        }
        return hashtable;
    }

    static Map<String, Object> purchaserInfoToHashtable(PurchaserInfoModel purchaser) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (purchaser.getCustomerUserId() != null) {
            hashtable.put("customerUserId", purchaser.getCustomerUserId());
        }

        Hashtable<String, Object> accessLevels = new Hashtable<>();
        if (purchaser.getAccessLevels() != null) {
            for (Map.Entry<String, AccessLevelInfoModel> entry : purchaser.getAccessLevels().entrySet()) {
                accessLevels.put(entry.getKey(), accessLevelInfoToHashtable(entry.getValue()));
            }
        }
        hashtable.put("accessLevels", accessLevels);

        Hashtable<String, Object> subscriptions = new Hashtable<>();
        if (purchaser.getSubscriptions() != null) {
            for (Map.Entry<String, SubscriptionInfoModel> entry : purchaser.getSubscriptions().entrySet()) {
                subscriptions.put(entry.getKey(), subscriptionInfoToHashtable(entry.getValue()));
            }
        }
        hashtable.put("subscriptions", subscriptions);

        Hashtable<String, Object> nonSubscriptions = new Hashtable<>();
        if (purchaser.getNonSubscriptions() != null) {
            for (Map.Entry<String, List<NonSubscriptionInfoModel>> entry : purchaser.getNonSubscriptions().entrySet()) {
                ArrayList<Hashtable<String, Object>> array = new ArrayList<>();
                List<NonSubscriptionInfoModel> nonSubscriptionsArray = entry.getValue();
                for (int i = 0; i < nonSubscriptionsArray.size(); ++i) {
                    array.add((Hashtable<String, Object>) nonSubscriptionInfoToHashtable(nonSubscriptionsArray.get(i)));
                }
                nonSubscriptions.put(entry.getKey(), array);
            }
        }
        hashtable.put("nonSubscriptions", nonSubscriptions);
        return hashtable;
    }

    static Map<String, Object> googleValidationResultToHashtable(GoogleValidationResult googleValidationResult) {
        Hashtable<String, Object> hashtable = new Hashtable<>();
        if (googleValidationResult.getKind() != null) {
            hashtable.put("kind", googleValidationResult.getKind());
        }
        if (googleValidationResult.getStartTimeMillis() != null) {
            hashtable.put("startTimeMillis", googleValidationResult.getStartTimeMillis());
        }
        if (googleValidationResult.getAutoResumeTimeMillis() != null) {
            hashtable.put("autoResumeTimeMillis", googleValidationResult.getAutoResumeTimeMillis());
        }
        if (googleValidationResult.getAutoRenewing() != null) {
            hashtable.put("autoRenewing", googleValidationResult.getAutoRenewing());
        }
        if (googleValidationResult.getPriceCurrencyCode() != null) {
            hashtable.put("priceCurrencyCode", googleValidationResult.getPriceCurrencyCode());
        }
        if (googleValidationResult.getPriceAmountMicros() != null) {
            hashtable.put("priceAmountMicros", googleValidationResult.getPriceAmountMicros());
        }
        if (googleValidationResult.getIntroductoryPriceInfo() != null) {
            IntroductoryPriceInfo info = googleValidationResult.getIntroductoryPriceInfo();
            Hashtable<String, Object> subHashtable = new Hashtable<>();
            if (info.getIntroductoryPriceCurrencyCode() != null) {
                subHashtable.put("introductoryPriceCurrencyCode", info.getIntroductoryPriceCurrencyCode());
            }
            if (info.getIntroductoryPriceAmountMicros() != null) {
                subHashtable.put("introductoryPriceAmountMicros", info.getIntroductoryPriceAmountMicros());
            }
            if (info.getIntroductoryPricePeriod() != null) {
                subHashtable.put("introductoryPricePeriod", info.getIntroductoryPricePeriod());
            }
            if (info.getIntroductoryPriceCycles() != null) {
                subHashtable.put("introductoryPriceCycles", info.getIntroductoryPriceCycles());
            }
            hashtable.put("introductoryPriceInfo", subHashtable);
        }
        if (googleValidationResult.getCountryCode() != null) {
            hashtable.put("countryCode", googleValidationResult.getCountryCode());
        }
        if (googleValidationResult.getDeveloperPayload() != null) {
            hashtable.put("developerPayload", googleValidationResult.getDeveloperPayload());
        }
        if (googleValidationResult.getPaymentState() != null) {
            hashtable.put("paymentState", googleValidationResult.getPaymentState());
        }
        if (googleValidationResult.getCancelReason() != null) {
            hashtable.put("cancelReason", googleValidationResult.getCancelReason());
        }
        if (googleValidationResult.getUserCancellationTimeMillis() != null) {
            hashtable.put("userCancellationTimeMillis", googleValidationResult.getUserCancellationTimeMillis());
        }
        if (googleValidationResult.getCancelSurveyResult() != null) {
            SubscriptionCancelSurveyResult result = googleValidationResult.getCancelSurveyResult();
            Hashtable<String, Object> subHashtable = new Hashtable<>();
            if (result.getUserInputCancelReason() != null) {
                subHashtable.put("userInputCancelReason", result.getUserInputCancelReason());
            }
            if (result.getCancelSurveyReason() != null) {
                subHashtable.put("cancelSurveyReason", result.getCancelSurveyReason());
            }
            hashtable.put("cancelSurveyResult", subHashtable);
        }
        if (googleValidationResult.getOrderId() != null) {
            hashtable.put("orderId", googleValidationResult.getOrderId());
        }
        if (googleValidationResult.getLinkedPurchaseToken() != null) {
            hashtable.put("linkedPurchaseToken", googleValidationResult.getLinkedPurchaseToken());
        }
        if (googleValidationResult.getPurchaseType() != null) {
            hashtable.put("purchaseType", googleValidationResult.getPurchaseType());
        }
        if (googleValidationResult.getPriceChange() != null) {
            SubscriptionPriceChange change = googleValidationResult.getPriceChange();
            Hashtable<String, Object> subHashtable = new Hashtable<>();
            if (change.getNewPrice() != null) {
                Price newPrice = change.getNewPrice();
                Hashtable<String, Object> subSubHashtable = new Hashtable<>();
                if (newPrice.getPriceMicros() != null) {
                    subSubHashtable.put("priceMicros", newPrice.getPriceMicros());
                }
                if (newPrice.getCurrency() != null) {
                    subSubHashtable.put("currency", newPrice.getCurrency());
                }
                subHashtable.put("newPrice", subSubHashtable);
            }
            if (change.getState() != null) {
                subHashtable.put("state", change.getState());
            }
            hashtable.put("priceChange", subHashtable);
        }
        if (googleValidationResult.getProfileName() != null) {
            hashtable.put("profileName", googleValidationResult.getProfileName());
        }
        if (googleValidationResult.getEmailAddress() != null) {
            hashtable.put("emailAddress", googleValidationResult.getEmailAddress());
        }
        if (googleValidationResult.getGivenName() != null) {
            hashtable.put("givenName", googleValidationResult.getGivenName());
        }
        if (googleValidationResult.getFamilyName() != null) {
            hashtable.put("familyName", googleValidationResult.getFamilyName());
        }
        if (googleValidationResult.getProfileId() != null) {
            hashtable.put("profileId", googleValidationResult.getProfileId());
        }
        if (googleValidationResult.getAcknowledgementState() != null) {
            hashtable.put("acknowledgementState", googleValidationResult.getAcknowledgementState());
        }
        if (googleValidationResult.getExternalAccountId() != null) {
            hashtable.put("externalAccountId", googleValidationResult.getExternalAccountId());
        }
        if (googleValidationResult.getPromotionType() != null) {
            hashtable.put("promotionType", googleValidationResult.getPromotionType());
        }
        if (googleValidationResult.getPromotionCode() != null) {
            hashtable.put("promotionCode", googleValidationResult.getPromotionCode());
        }
        if (googleValidationResult.getObfuscatedExternalAccountId() != null) {
            hashtable.put("obfuscatedExternalAccountId", googleValidationResult.getObfuscatedExternalAccountId());
        }
        if (googleValidationResult.getObfuscatedExternalProfileId() != null) {
            hashtable.put("obfuscatedExternalProfileId", googleValidationResult.getObfuscatedExternalProfileId());
        }
        if (googleValidationResult.getPurchaseTimeMillis() != null) {
            hashtable.put("purchaseTimeMillis", googleValidationResult.getPurchaseTimeMillis());
        }
        if (googleValidationResult.getPurchaseState() != null) {
            hashtable.put("purchaseState", googleValidationResult.getPurchaseState());
        }
        if (googleValidationResult.getConsumptionState() != null) {
            hashtable.put("consumptionState", googleValidationResult.getConsumptionState());
        }
        if (googleValidationResult.getPurchaseToken() != null) {
            hashtable.put("purchaseToken", googleValidationResult.getPurchaseToken());
        }
        if (googleValidationResult.getProductId() != null) {
            hashtable.put("productId", googleValidationResult.getProductId());
        }
        if (googleValidationResult.getQuantity() != null) {
            hashtable.put("quantity", googleValidationResult.getQuantity());
        }
        if (googleValidationResult.getRegionCode() != null) {
            hashtable.put("regionCode", googleValidationResult.getRegionCode());
        }
        return hashtable;
    }

    static ProductSubscriptionPeriodModel hashtableToSubscriptionPeriod(Map<String, Object> hashtable) {
        if (hashtable.containsKey("unit") && hashtable.get("unit") != null && hashtable.containsKey("numberOfUnits") && hashtable.get("numberOfUnits") != null) {
            return new ProductSubscriptionPeriodModel(stringToPeriodUnit((String)hashtable.get("unit")), (int)((double)hashtable.get("numberOfUnits")));
        }
        return null;
    }

    static ProductDiscountModel hashtableToDiscount(Map<String, Object> hashtable) {
        ProductDiscountModel discount = null;
        if (hashtable.containsKey("price") && hashtable.get("price") != null
        && hashtable.containsKey("numberOfPeriods") && hashtable.get("numberOfPeriods") != null
        && hashtable.containsKey("localizedPrice") && hashtable.get("localizedPrice") != null
        && hashtable.containsKey("subscriptionPeriod") && hashtable.get("subscriptionPeriod") != null) {
            discount = new ProductDiscountModel(new BigDecimal((String)hashtable.get("price")), (int)((double)hashtable.get("numberOfPeriods")), (String)hashtable.get("localizedPrice"), hashtableToSubscriptionPeriod((Map<String, Object>)hashtable.get("subscriptionPeriod")));
        }
        return discount;
    }

    static ProductModel hashtableToProduct(Map<String, Object> hashtable) {
        ProductModel product = new ProductModel();
        if (hashtable.containsKey("vendorProductId") && hashtable.get("vendorProductId") != null) {
            product.setVendorProductId((String)hashtable.get("vendorProductId"));
        }
        if (hashtable.containsKey("localizedTitle") && hashtable.get("localizedTitle") != null) {
            product.setLocalizedTitle((String)hashtable.get("localizedTitle"));
        }
        if (hashtable.containsKey("localizedDescription") && hashtable.get("localizedDescription") != null) {
            product.setLocalizedDescription((String)hashtable.get("localizedDescription"));
        }
        if (hashtable.containsKey("localizedPrice") && hashtable.get("localizedPrice") != null) {
            product.setLocalizedPrice((String)hashtable.get("localizedPrice"));
        }
        if (hashtable.containsKey("price") && hashtable.get("price") != null) {
            product.setPrice(new BigDecimal((String)hashtable.get("price")));
        }
        if (hashtable.containsKey("currencyCode") && hashtable.get("currencyCode") != null) {
            product.setCurrencyCode((String)hashtable.get("currencyCode"));
        }
        if (hashtable.containsKey("currencySymbol") && hashtable.get("currencySymbol") != null) {
            product.setCurrencySymbol((String)hashtable.get("currencySymbol"));
        }
        if (hashtable.containsKey("subscriptionPeriod") && hashtable.get("subscriptionPeriod") != null) {
            product.setSubscriptionPeriod(hashtableToSubscriptionPeriod((Map<String, Object> )hashtable.get("subscriptionPeriod")));
        }
        if (hashtable.containsKey("introductoryOfferEligibility") && hashtable.get("introductoryOfferEligibility") != null) {
            product.setIntroductoryOfferEligibility((boolean)hashtable.get("introductoryOfferEligibility"));
        }
        if (hashtable.containsKey("promotionalOfferEligibility") && hashtable.get("promotionalOfferEligibility") != null) {
            product.setPromotionalOfferEligibility((boolean)hashtable.get("promotionalOfferEligibility"));
        }
        if (hashtable.containsKey("introductoryDiscount") && hashtable.get("introductoryDiscount") != null) {
            product.setIntroductoryDiscount(hashtableToDiscount((Map<String, Object> )hashtable.get("introductoryDiscount")));
        }
        if (hashtable.containsKey("skuId") && hashtable.get("skuId") != null) {
            String skuId = (String)hashtable.get("skuId");
            if (skuDetailsCache.containsKey(skuId) && skuDetailsCache.get(skuId) != null) {
                product.setDetails(skuDetailsCache.get(skuId));
            }
        }
        return product;
    }

    static ProfileParameterBuilder hashtableToProfileParameterBuilder(Map<String, Object> hashtable) {
        ProfileParameterBuilder params = new ProfileParameterBuilder();
        if (hashtable.containsKey("email") && hashtable.get("email") != null) {
            params = params.withEmail((String)hashtable.get("email"));
        }
        if (hashtable.containsKey("phoneNumber") && hashtable.get("phoneNumber") != null) {
            params = params.withPhoneNumber((String)hashtable.get("phoneNumber"));
        }
        if (hashtable.containsKey("facebookUserId") && hashtable.get("facebookUserId") != null) {
            params = params.withFacebookUserId((String)hashtable.get("facebookUserId"));
        }
        if (hashtable.containsKey("amplitudeUserId") && hashtable.get("amplitudeUserId") != null) {
            params = params.withAmplitudeUserId((String)hashtable.get("amplitudeUserId"));
        }
        if (hashtable.containsKey("amplitudeDeviceId") && hashtable.get("amplitudeDeviceId") != null) {
            params = params.withAmplitudeDeviceId((String)hashtable.get("amplitudeDeviceId"));
        }
        if (hashtable.containsKey("mixpanelUserId") && hashtable.get("mixpanelUserId") != null) {
            params = params.withMixpanelUserId((String)hashtable.get("mixpanelUserId"));
        }
        if (hashtable.containsKey("appmetricaProfileId") && hashtable.get("appmetricaProfileId") != null) {
            params = params.withAppmetricaProfileId((String)hashtable.get("appmetricaProfileId"));
        }
        if (hashtable.containsKey("appmetricaDeviceId") && hashtable.get("appmetricaDeviceId") != null) {
            params = params.withAppmetricaDeviceId((String)hashtable.get("appmetricaDeviceId"));
        }
        if (hashtable.containsKey("firstName") && hashtable.get("firstName") != null) {
            params = params.withFirstName((String)hashtable.get("firstName"));
        }
        if (hashtable.containsKey("lastName") && hashtable.get("lastName") != null) {
            params = params.withLastName((String)hashtable.get("lastName"));
        }
        if (hashtable.containsKey("gender") && hashtable.get("gender") != null) {
            params = params.withGender(stringToGender((String)hashtable.get("gender")));
        }
        if (hashtable.containsKey("birthday") && hashtable.get("birthday") != null) {
            SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
            try {
                java.util.Date javaDate = dateFormat.parse((String)hashtable.get("birthday"));
                params = params.withBirthday(new Date(javaDate.getYear(), javaDate.getMonth(), javaDate.getDay()));
            } catch (ParseException e) {
                e.printStackTrace();
            }
        }
        if (hashtable.containsKey("customAttributes") && hashtable.get("customAttributes") != null) {
            params = params.withCustomAttributes((Map<String, Object>)hashtable.get("customAttributes"));
        }
        return params;
    }
}
