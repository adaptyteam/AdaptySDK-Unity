package com.adapty.unity;

import com.adapty.errors.AdaptyError;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonSerializationContext;
import com.google.gson.JsonSerializer;

import java.lang.reflect.Type;

class AdaptyErrorSerializer implements JsonSerializer<AdaptyError> {

    @Override
    public JsonElement serialize(AdaptyError src, Type typeOfSrc, JsonSerializationContext context) {
        JsonObject jsonError = new JsonObject();
        jsonError.addProperty("message", src.getMessage());
        jsonError.addProperty("adapty_code", src.getAdaptyErrorCode().getValue());
        return jsonError;
    }
}