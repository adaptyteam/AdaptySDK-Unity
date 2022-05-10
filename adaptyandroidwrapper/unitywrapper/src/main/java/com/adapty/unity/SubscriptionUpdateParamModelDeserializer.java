package com.adapty.unity;

import com.adapty.models.SubscriptionUpdateParamModel;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;

import java.lang.reflect.Type;

class SubscriptionUpdateParamModelDeserializer implements JsonDeserializer<SubscriptionUpdateParamModel> {
    @Override
    public SubscriptionUpdateParamModel deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
        try {
            JsonObject jsonObject = json.getAsJsonObject();
            String oldVendorProductId = jsonObject.has("old_sub_vendor_product_id") ? jsonObject.get("old_sub_vendor_product_id").getAsString() : null;
            if (oldVendorProductId == null) return null;
            SubscriptionUpdateParamModel.ProrationMode prorationMode = null;
            String prorationModeString = jsonObject.has("proration_mode") ? jsonObject.get("proration_mode").getAsString() : null;
            if (prorationModeString != null) {
                switch (prorationModeString) {
                    case "immediate_and_charge_full_price":
                        prorationMode = SubscriptionUpdateParamModel.ProrationMode.IMMEDIATE_AND_CHARGE_FULL_PRICE;
                        break;
                    case "deferred":
                        prorationMode = SubscriptionUpdateParamModel.ProrationMode.DEFERRED;
                        break;
                    case "immediate_without_proration":
                        prorationMode = SubscriptionUpdateParamModel.ProrationMode.IMMEDIATE_WITHOUT_PRORATION;
                        break;
                    case "immediate_and_charge_prorated_price":
                        prorationMode = SubscriptionUpdateParamModel.ProrationMode.IMMEDIATE_AND_CHARGE_PRORATED_PRICE;
                        break;
                    case "immediate_with_time_proration":
                        prorationMode = SubscriptionUpdateParamModel.ProrationMode.IMMEDIATE_WITH_TIME_PRORATION;
                        break;
                }
            }
            if (prorationMode != null) return new SubscriptionUpdateParamModel(oldVendorProductId, prorationMode);
        } catch (Exception e) { }
        return null;
    }
}