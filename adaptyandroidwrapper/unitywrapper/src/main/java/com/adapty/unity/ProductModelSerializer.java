package com.adapty.unity;

import com.adapty.models.ProductDiscountModel;
import com.adapty.models.ProductModel;
import com.adapty.models.ProductSubscriptionPeriodModel;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

class ProductModelSerializer implements JsonSerializer<ProductModel> {

    @Override
    public JsonElement serialize(ProductModel src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject jsonProduct = new JsonObject();
        jsonProduct.addProperty("vendor_product_id", src.getVendorProductId());
        jsonProduct.addProperty("localized_title", src.getLocalizedTitle());
        jsonProduct.addProperty("localized_description", src.getLocalizedDescription());
        if (src.getPaywallName() != null)
            jsonProduct.addProperty("paywall_name", src.getPaywallName());
        if (src.getPaywallABTestName() != null)
            jsonProduct.addProperty("paywall_ab_test_name", src.getPaywallABTestName());
        if (src.getVariationId() != null)
            jsonProduct.addProperty("variation_id", src.getVariationId());
        jsonProduct.addProperty("price", src.getPrice());
        if (src.getLocalizedPrice() != null)
            jsonProduct.addProperty("localized_price", src.getLocalizedPrice());
        if (src.getCurrencyCode() != null)
            jsonProduct.addProperty("currency_code", src.getCurrencyCode());
        if (src.getCurrencySymbol() != null)
            jsonProduct.addProperty("currency_symbol", src.getCurrencySymbol());
        if (src.getSubscriptionPeriod() != null)
            jsonProduct.add("subscription_period", context.serialize(src.getSubscriptionPeriod(), ProductSubscriptionPeriodModel.class));
        if (src.getLocalizedSubscriptionPeriod() != null)
            jsonProduct.addProperty("localized_subscription_period", src.getLocalizedSubscriptionPeriod());
        jsonProduct.addProperty("introductory_offer_eligibility", src.getIntroductoryOfferEligibility());
        if (src.getIntroductoryDiscount() != null)
            jsonProduct.add("introductory_discount", context.serialize(src.getIntroductoryDiscount(), ProductDiscountModel.class));
        if (src.getFreeTrialPeriod() != null)
            jsonProduct.add("free_trial_period", context.serialize(src.getFreeTrialPeriod(), ProductSubscriptionPeriodModel.class));
        if (src.getLocalizedFreeTrialPeriod() != null)
            jsonProduct.addProperty("localized_free_trial_period", src.getLocalizedFreeTrialPeriod());
        return jsonProduct;
    }
}
