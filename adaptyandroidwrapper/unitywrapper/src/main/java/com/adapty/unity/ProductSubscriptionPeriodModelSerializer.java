package com.adapty.unity;

import com.adapty.models.ProductSubscriptionPeriodModel;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

class ProductSubscriptionPeriodModelSerializer implements JsonSerializer<ProductSubscriptionPeriodModel> {

    @Override
    public JsonElement serialize(ProductSubscriptionPeriodModel src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject jsonSubscriptionPeriod = new JsonObject();
        jsonSubscriptionPeriod.addProperty("unit", src.getUnit() != null ? src.getUnit().getPeriod() : "unknown");
        jsonSubscriptionPeriod.addProperty("number_of_units", src.getNumberOfUnits() != null ? src.getNumberOfUnits() : 0);
        return jsonSubscriptionPeriod;
    }
}