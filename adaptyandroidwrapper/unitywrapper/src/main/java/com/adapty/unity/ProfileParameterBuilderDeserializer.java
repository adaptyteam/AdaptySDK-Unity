package com.adapty.unity;

import com.adapty.models.Gender;
import com.adapty.utils.ProfileParameterBuilder;
import com.google.gson.FieldNamingPolicy;
import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonDeserializationContext;
import com.google.gson.JsonDeserializer;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
import com.google.gson.JsonParseException;

import java.lang.reflect.Type;

class ProfileParameterBuilderDeserializer implements JsonDeserializer<ProfileParameterBuilder> {

    private final Gson gson = new GsonBuilder()
            .setFieldNamingPolicy(FieldNamingPolicy.LOWER_CASE_WITH_UNDERSCORES)
            .create();

    @Override
    public ProfileParameterBuilder deserialize(JsonElement json, Type typeOfT, JsonDeserializationContext context) throws JsonParseException {
        try {
            JsonObject jsonObject = json.getAsJsonObject();
            ProfileParameterBuilder builder = gson.fromJson(jsonObject, ProfileParameterBuilder.class);
            if (jsonObject.has("gender")) {
                String genderStr = jsonObject.get("gender").getAsString();
                if (genderStr != null && genderStr.length() > 0) {
                    switch (genderStr.charAt(0)) {
                        case 'f':
                        case 'F':
                            builder = builder.withGender(Gender.FEMALE);
                            break;
                        case 'm':
                        case 'M':
                            builder = builder.withGender(Gender.MALE);
                            break;
                        case 'o':
                        case 'O':
                            builder = builder.withGender(Gender.OTHER);
                            break;
                    }
                }
            }
            return builder;
        } catch (Exception e) { }
        return null;
    }
}