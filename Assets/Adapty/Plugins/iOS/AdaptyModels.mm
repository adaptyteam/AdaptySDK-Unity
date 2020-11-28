#import "Foundation/Foundation.h"
#import "StoreKit/StoreKit.h"

@import Adapty;

static NSMutableDictionary *skProductCache = [NSMutableDictionary dictionary];

static NSString *dateToString(NSDate *date) {
    NSISO8601DateFormatter *formatter = [[NSISO8601DateFormatter alloc] init];
    return [formatter stringFromDate:date];
}

static NSString *periodUnitToString(PeriodUnit unit){
    NSString *result = nil;
    switch (unit) {
        case PeriodUnitDay:
            result = @"day";
            break;
        case PeriodUnitWeek:
            result = @"week";
            break;
        case PeriodUnitMonth:
            result = @"month";
            break;
        case PeriodUnitYear:
            result = @"year";
            break;
        default:
        case PeriodUnitUnknown:
            result = @"unknown";
            break;
    }
    return result;
}

static PeriodUnit stringToPeriodUnit(NSString *unit) {
    PeriodUnit result = PeriodUnitUnknown;
    if ([unit isEqualToString:@"day"]) {
        return PeriodUnitDay;
    } else if ([unit isEqualToString:@"week"]) {
        return PeriodUnitWeek;
    } else if ([unit isEqualToString:@"month"]) {
        return PeriodUnitMonth;
    } else if ([unit isEqualToString:@"year"]) {
        return PeriodUnitYear;
    }
    return result;
}

static NSString *paymentModeToString(PaymentMode mode){
    NSString *result = nil;
    switch (mode) {
        case PaymentModeFreeTrial:
            result = @"free_trial";
            break;
        case PaymentModePayAsYouGo:
            result = @"pay_as_you_go";
            break;
        case PaymentModePayUpFront:
            result = @"pay_up_front";
            break;
        default:
        case PaymentModeUnknown:
            result = @"unknown";
            break;
    }
    return result;
}

static PaymentMode stringToPaymentMode(NSString *mode) {
    PaymentMode result = PaymentModeUnknown;
    if ([mode isEqualToString:@"free_trial"]) {
        return PaymentModeFreeTrial;
    } else if ([mode isEqualToString:@"pay_as_you_go"]) {
        return PaymentModePayAsYouGo;
    } else if ([mode isEqualToString:@"pay_up_front"]) {
        return PaymentModePayUpFront;
    }
    return result;
}

static Gender stringToGender(NSString *mode) {
    Gender result = GenderOther;
    if ([mode isEqualToString:@"female"]) {
        return GenderFemale;
    } else if ([mode isEqualToString:@"male"]) {
        return GenderMale;
    } else if ([mode isEqualToString:@"other"]) {
        return GenderOther;
    }
    return result;
}

static NSDictionary *adaptyErrorToDictionary(AdaptyError *adaptyError) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    if (adaptyError.localizedDescription) {
        dictionary[@"message"] = adaptyError.localizedDescription;
    }
    dictionary[@"code"] = @(adaptyError.adaptyErrorCode);
    return dictionary;
}

static NSDictionary *subscriptionPeriodToDictionary(ProductSubscriptionPeriodModel *subscriptionPeriod) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"unit"] = periodUnitToString(subscriptionPeriod.unit);
    dictionary[@"numberOfUnits"] = @(subscriptionPeriod.numberOfUnits);
    return dictionary;
}

static NSDictionary *discountToDictionary(ProductDiscountModel *discount) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    if (discount.identifier) {
        dictionary[@"identifier"] = discount.identifier;
    }
    dictionary[@"price"] = [NSDecimalNumber decimalNumberWithDecimal:discount.price].stringValue;
    dictionary[@"numberOfPeriods"] = @(discount.numberOfPeriods);
    dictionary[@"paymentMode"] = paymentModeToString(discount.paymentMode);
    if (discount.localizedPrice) {
        dictionary[@"localizedPrice"] = discount.localizedPrice;
    }
    if (discount.localizedSubscriptionPeriod) {
        dictionary[@"localizedSubscriptionPeriod"] = discount.localizedSubscriptionPeriod;
    }
    if (discount.localizedNumberOfPeriods) {
        dictionary[@"localizedNumberOfPeriods"] = discount.localizedNumberOfPeriods;
    }
    dictionary[@"subscriptionPeriod"] = subscriptionPeriodToDictionary(discount.subscriptionPeriod);
    return dictionary;
}

static NSDictionary *productToDictionary(ProductModel *product) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"vendorProductId"] = product.vendorProductId;
    dictionary[@"localizedTitle"] = product.localizedTitle;
    dictionary[@"localizedDescription"] = product.localizedDescription;
    if (product.localizedPrice) {
        dictionary[@"localizedPrice"] = product.localizedPrice;
    }
    if (product.localizedSubscriptionPeriod) {
        dictionary[@"localizedSubscriptionPeriod"] = product.localizedSubscriptionPeriod;
    }
    dictionary[@"price"] = [NSDecimalNumber decimalNumberWithDecimal:product.price].stringValue;
    if (product.currencyCode) {
        dictionary[@"currencyCode"] = product.currencyCode;
    }
    if (product.currencySymbol) {
        dictionary[@"currencySymbol"] = product.currencySymbol;
    }
    if (product.regionCode) {
        dictionary[@"regionCode"] = product.regionCode;
    }
    if (product.subscriptionPeriod) {
        dictionary[@"subscriptionPeriod"] = subscriptionPeriodToDictionary(product.subscriptionPeriod);
    }
    if (product.subscriptionGroupIdentifier) {
        dictionary[@"subscriptionGroupIdentifier"] = product.subscriptionGroupIdentifier;
    }
    dictionary[@"introductoryOfferEligibility"] = @(product.introductoryOfferEligibility);
    dictionary[@"promotionalOfferEligibility"] = @(product.promotionalOfferEligibility);
    if (product.promotionalOfferId) {
        dictionary[@"promotionalOfferId"] = product.promotionalOfferId;
    }
    if (product.introductoryDiscount) {
        dictionary[@"introductoryDiscount"] = discountToDictionary(product.introductoryDiscount);
    }
    
    NSMutableArray<NSDictionary *> *discountsArray = [[NSMutableArray alloc] init];
    for (int i = 0; i < product.discounts.count; ++i) {
        if (product.discounts[i]) {
            [discountsArray addObject:discountToDictionary(product.discounts[i])];
        }
    }
    dictionary[@"discounts"] = discountsArray;
    if (product.skProduct) {
        dictionary[@"skuId"] = product.skProduct.productIdentifier;
        skProductCache[product.skProduct.productIdentifier] = product.skProduct;
    }
    return dictionary;
}

static NSDictionary *paywallToDictionary(PaywallModel *paywall) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"developerId"] = paywall.developerId;
    dictionary[@"variationId"] = paywall.variationId;
    dictionary[@"revision"] = @(paywall.revision);
    dictionary[@"isPromo"] = @(paywall.isPromo);
    dictionary[@"customPayload"] = paywall.customPayload;
    
    NSMutableArray<NSDictionary *> *productsArray = [[NSMutableArray alloc] init];
    for (int i = 0; i < paywall.products.count; ++i) {
        if (paywall.products[i]) {
            [productsArray addObject:productToDictionary(paywall.products[i])];
        }
    }
    dictionary[@"products"] = productsArray;
    return dictionary;
}

static NSDictionary *promoToDictionary(PromoModel *promo) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"promoType"] = promo.promoType;
    dictionary[@"variationId"] = promo.variationId;
    if (promo.expiresAt) {
        dictionary[@"expiresAt"] = dateToString(promo.expiresAt);
    }
    if (promo.paywall) {
        dictionary[@"paywall"] = paywallToDictionary(promo.paywall);
    }
    return dictionary;
}

static NSDictionary *accessLevelInfoToDictionary(AccessLevelInfoModel *accessLevel) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"id"] = accessLevel.id;
    dictionary[@"isActive"] = @(accessLevel.isActive);
    dictionary[@"vendorProductId"] = accessLevel.vendorProductId;
    if (accessLevel.vendorTransactionId) {
        dictionary[@"vendorTransactionId"] = accessLevel.vendorTransactionId;
    }
    if (accessLevel.vendorOriginalTransactionId) {
        dictionary[@"vendorOriginalTransactionId"] = accessLevel.vendorOriginalTransactionId;
    }
    dictionary[@"store"] = accessLevel.store;
    if (accessLevel.activatedAt) {
        dictionary[@"activatedAt"] = dateToString(accessLevel.activatedAt);
    }
    if (accessLevel.startsAt) {
        dictionary[@"startsAt"] = dateToString(accessLevel.startsAt);
    }
    if (accessLevel.renewedAt) {
        dictionary[@"renewedAt"] = dateToString(accessLevel.renewedAt);
    }
    if (accessLevel.expiresAt) {
        dictionary[@"expiresAt"] = dateToString(accessLevel.expiresAt);
    }
    dictionary[@"isLifetime"] = @(accessLevel.isLifetime);
    dictionary[@"willRenew"] = @(accessLevel.willRenew);
    dictionary[@"isInGracePeriod"] = @(accessLevel.isInGracePeriod);
    if (accessLevel.unsubscribedAt) {
        dictionary[@"unsubscribedAt"] = dateToString(accessLevel.unsubscribedAt);
    }
    if (accessLevel.billingIssueDetectedAt) {
        dictionary[@"billingIssueDetectedAt"] = dateToString(accessLevel.billingIssueDetectedAt);
    }
    if (accessLevel.cancellationReason) {
        dictionary[@"cancellationReason"] = accessLevel.cancellationReason;
    }
    dictionary[@"isRefund"] = @(accessLevel.isRefund);
    if (accessLevel.activeIntroductoryOfferType) {
        dictionary[@"activeIntroductoryOfferType"] = accessLevel.activeIntroductoryOfferType;
    }
    if (accessLevel.activePromotionalOfferType) {
        dictionary[@"activePromotionalOfferType"] = accessLevel.activePromotionalOfferType;
    }
    return dictionary;
}

static NSDictionary *subscriptionInfoToDictionary(SubscriptionInfoModel *subscription) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"id"] = @(subscription.isActive);
    dictionary[@"isActive"] = @(subscription.isActive);
    dictionary[@"vendorProductId"] = subscription.vendorProductId;
    if (subscription.vendorTransactionId) {
        dictionary[@"vendorTransactionId"] = subscription.vendorTransactionId;
    }
    if (subscription.vendorOriginalTransactionId) {
        dictionary[@"vendorOriginalTransactionId"] = subscription.vendorOriginalTransactionId;
    }
    dictionary[@"store"] = subscription.store;
    if (subscription.activatedAt) {
        dictionary[@"activatedAt"] = dateToString(subscription.activatedAt);
    }
    if (subscription.startsAt) {
        dictionary[@"startsAt"] = dateToString(subscription.startsAt);
    }
    if (subscription.renewedAt) {
        dictionary[@"renewedAt"] = dateToString(subscription.renewedAt);
    }
    if (subscription.expiresAt) {
        dictionary[@"expiresAt"] = dateToString(subscription.expiresAt);
    }
    dictionary[@"isLifetime"] = @(subscription.isLifetime);
    dictionary[@"willRenew"] = @(subscription.willRenew);
    dictionary[@"isInGracePeriod"] = @(subscription.isInGracePeriod);
    if (subscription.unsubscribedAt) {
        dictionary[@"unsubscribedAt"] = dateToString(subscription.unsubscribedAt);
    }
    if (subscription.billingIssueDetectedAt) {
        dictionary[@"billingIssueDetectedAt"] = dateToString(subscription.billingIssueDetectedAt);
    }
    if (subscription.cancellationReason) {
        dictionary[@"cancellationReason"] = subscription.cancellationReason;
    }
    dictionary[@"isRefund"] = @(subscription.isRefund);
    if (subscription.activeIntroductoryOfferType) {
        dictionary[@"activeIntroductoryOfferType"] = subscription.activeIntroductoryOfferType;
    }
    if (subscription.activePromotionalOfferType) {
        dictionary[@"activePromotionalOfferType"] = subscription.activePromotionalOfferType;
    }
    dictionary[@"isSandbox"] = @(subscription.isSandbox);
    return dictionary;
}

static NSDictionary *nonSubscriptionInfoToDictionary(NonSubscriptionInfoModel *nonSubscription) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    dictionary[@"purchaseId"] = nonSubscription.purchaseId;
    dictionary[@"vendorProductId"] = nonSubscription.vendorProductId;
    if (nonSubscription.vendorTransactionId) {
        dictionary[@"vendorTransactionId"] = nonSubscription.vendorTransactionId;
    }
    dictionary[@"store"] = nonSubscription.store;
    if (nonSubscription.purchasedAt) {
        dictionary[@"purchasedAt"] = dateToString(nonSubscription.purchasedAt);
    }
    dictionary[@"isRefund"] = @(nonSubscription.isRefund);
    dictionary[@"isOneTime"] = @(nonSubscription.isOneTime);
    dictionary[@"isSandbox"] = @(nonSubscription.isSandbox);
    return dictionary;
}

static NSDictionary *purchaserInfoToDictionary(PurchaserInfoModel *purchaser) {
    NSMutableDictionary *dictionary = [NSMutableDictionary dictionary];
    if (purchaser.customerUserId) {
        dictionary[@"customerUserId"] = purchaser.customerUserId;
    }
    
    NSMutableDictionary *accessLevels = [NSMutableDictionary dictionary];
    [purchaser.accessLevels enumerateKeysAndObjectsUsingBlock:^(NSString *key, AccessLevelInfoModel *accessLevel, BOOL *stop) {
        accessLevels[key] = accessLevelInfoToDictionary(accessLevel);
    }];
    dictionary[@"accessLevels"] = accessLevels;
    
    NSMutableDictionary *subscriptions = [NSMutableDictionary dictionary];
    [purchaser.subscriptions enumerateKeysAndObjectsUsingBlock:^(NSString *key, SubscriptionInfoModel *subscription, BOOL *stop) {
        subscriptions[key] = subscriptionInfoToDictionary(subscription);
    }];
    dictionary[@"subscriptions"] = subscriptions;
    
    NSMutableDictionary *nonSubscriptions = [NSMutableDictionary dictionary];
    [purchaser.nonSubscriptions enumerateKeysAndObjectsUsingBlock:^(NSString *key, NSArray<NonSubscriptionInfoModel *> *nonSubscriptionsArray, BOOL *stop) {
        NSMutableArray<NSDictionary *> *array = [[NSMutableArray alloc] init];
        for (int i = 0; i < nonSubscriptionsArray.count; ++i) {
            if (nonSubscriptionsArray[i]) {
                [array addObject:nonSubscriptionInfoToDictionary(nonSubscriptionsArray[i])];
            }
        }
        nonSubscriptions[key] = array;
    }];
    dictionary[@"nonSubscriptions"] = nonSubscriptions;
    return dictionary;
}

static ProductSubscriptionPeriodModel *dictionaryToSubscriptionPeriod(NSDictionary *dictionary) {
    ProductSubscriptionPeriodModel *subscriptionPeriod = [ProductSubscriptionPeriodModel alloc];
    if (dictionary[@"unit"]) {
        subscriptionPeriod.unit = stringToPeriodUnit(dictionary[@"unit"]);
    }
    if (dictionary[@"numberOfUnits"]) {
        subscriptionPeriod.numberOfUnits = ((NSNumber *)dictionary[@"numberOfUnits"]).intValue;
    }
    return subscriptionPeriod;
}

static ProductDiscountModel *dictionaryToDiscount(NSDictionary *dictionary) {
    ProductDiscountModel *discount = [ProductDiscountModel alloc];
    if (dictionary[@"identifier"]) {
        discount.identifier = dictionary[@"identifier"];
    }
    if (dictionary[@"price"]) {
        discount.price = [NSDecimalNumber decimalNumberWithString:dictionary[@"price"]].decimalValue;
    }
    if (dictionary[@"numberOfPeriods"]) {
        discount.numberOfPeriods = ((NSNumber *)dictionary[@"numberOfPeriods"]).intValue;
    }
    if (dictionary[@"paymentMode"]) {
        discount.paymentMode = stringToPaymentMode(dictionary[@"paymentMode"]);
    }
    if (dictionary[@"localizedPrice"]) {
        discount.localizedPrice = dictionary[@"localizedPrice"];
    }
    if (dictionary[@"localizedSubscriptionPeriod"]) {
        discount.localizedSubscriptionPeriod = dictionary[@"localizedSubscriptionPeriod"];
    }
    if (dictionary[@"localizedNumberOfPeriods"]) {
        discount.localizedNumberOfPeriods = dictionary[@"localizedNumberOfPeriods"];
    }
    if (dictionary[@"subscriptionPeriod"]) {
        discount.subscriptionPeriod = dictionaryToSubscriptionPeriod((NSDictionary *)dictionary[@"subscriptionPeriod"]);
    }
    return discount;
}

static ProductModel *dictionaryToProduct(NSDictionary *dictionary) {
    ProductModel *product = [ProductModel alloc];
    if (dictionary[@"vendorProductId"]) {
        product.vendorProductId = dictionary[@"vendorProductId"];
    }
    if (dictionary[@"localizedTitle"]) {
        product.localizedTitle = dictionary[@"localizedTitle"];
    }
    if (dictionary[@"localizedDescription"]) {
        product.localizedDescription = dictionary[@"localizedDescription"];
    }
    if (dictionary[@"localizedPrice"]) {
        product.localizedPrice = dictionary[@"localizedPrice"];
    }
    if (dictionary[@"localizedSubscriptionPeriod"]) {
        product.localizedSubscriptionPeriod = dictionary[@"localizedSubscriptionPeriod"];
    }
    if (dictionary[@"price"]) {
        product.price = [NSDecimalNumber decimalNumberWithString:dictionary[@"price"]].decimalValue;
    }
    if (dictionary[@"currencyCode"]) {
        product.currencyCode = dictionary[@"currencyCode"];
    }
    if (dictionary[@"currencySymbol"]) {
        product.currencySymbol = dictionary[@"currencySymbol"];
    }
    if (dictionary[@"regionCode"]) {
        product.regionCode = dictionary[@"regionCode"];
    }
    if (dictionary[@"subscriptionPeriod"]) {
        product.subscriptionPeriod = dictionaryToSubscriptionPeriod((NSDictionary *)dictionary[@"subscriptionPeriod"]);
    }
    if (dictionary[@"subscriptionGroupIdentifier"]) {
        product.subscriptionGroupIdentifier = dictionary[@"subscriptionGroupIdentifier"];
    }
    if (dictionary[@"introductoryOfferEligibility"]) {
        product.introductoryOfferEligibility = ((NSNumber*)dictionary[@"introductoryOfferEligibility"]).boolValue;
    }
    if (dictionary[@"promotionalOfferEligibility"]) {
        product.promotionalOfferEligibility = ((NSNumber*)dictionary[@"promotionalOfferEligibility"]).boolValue;
    }
    if (dictionary[@"promotionalOfferId"]) {
        product.promotionalOfferId = dictionary[@"promotionalOfferId"];
    }
    if (dictionary[@"introductoryDiscount"]) {
        product.introductoryDiscount = dictionaryToDiscount((NSDictionary *)dictionary[@"introductoryDiscount"]);
    }
    
    if (dictionary[@"discounts"]) {
        NSArray<NSDictionary *> *discounts = dictionary[@"discounts"];
        NSMutableArray<ProductDiscountModel *> *discountsArray = [[NSMutableArray alloc] init];
        for (int i = 0; i < discounts.count; ++i) {
            if (discounts[i]) {
                [discountsArray addObject:dictionaryToDiscount(discounts[i])];
            }
        }
        product.discounts = discountsArray;
    }
    if (dictionary[@"skuId"] && skProductCache[dictionary[@"skuId"]]) {
        product.skProduct = skProductCache[dictionary[@"skuId"]];
    }
    return product;
}

static ProfileParameterBuilder *dictionaryToProfileParameterBuilder(NSDictionary *dictionary) {
    ProfileParameterBuilder *params = [[ProfileParameterBuilder alloc] init];
    if (dictionary[@"email"]) {
        params = [params withEmail:dictionary[@"email"]];
    }
    if (dictionary[@"phoneNumber"]) {
        params = [params withPhoneNumber:dictionary[@"phoneNumber"]];
    }
    if (dictionary[@"facebookUserId"]) {
        params = [params withFacebookUserId:dictionary[@"facebookUserId"]];
    }
    if (dictionary[@"amplitudeUserId"]) {
        params = [params withAmplitudeUserId:dictionary[@"amplitudeUserId"]];
    }
    if (dictionary[@"amplitudeDeviceId"]) {
        params = [params withAmplitudeDeviceId:dictionary[@"amplitudeDeviceId"]];
    }
    if (dictionary[@"mixpanelUserId"]) {
        params = [params withMixpanelUserId:dictionary[@"mixpanelUserId"]];
    }
    if (dictionary[@"appmetricaProfileId"]) {
        params = [params withAppmetricaProfileId:dictionary[@"appmetricaProfileId"]];
    }
    if (dictionary[@"appmetricaDeviceId"]) {
        params = [params withAppmetricaDeviceId:dictionary[@"appmetricaDeviceId"]];
    }
    if (dictionary[@"firstName"]) {
        params = [params withFirstName:dictionary[@"firstName"]];
    }
    if (dictionary[@"lastName"]) {
        params = [params withLastName:dictionary[@"lastName"]];
    }
    if (dictionary[@"gender"]) {
        params = [params withGender:stringToGender(dictionary[@"gender"])];
    }
    if (dictionary[@"birthday"]) {
        NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];
        [dateFormat setDateFormat:@"yyyy-MM-dd"];
        NSDate *date = [dateFormat dateFromString:dictionary[@"birthday"]];
        params = [params withBirthday:date];
    }
    if (dictionary[@"appTrackingTransparencyStatus"]) {
        params = [params withAppTrackingTransparencyStatus:[dictionary[@"appTrackingTransparencyStatus"] unsignedIntegerValue]];
    }
    if (dictionary[@"customAttributes"]) {
        params = [params withCustomAttributes:dictionary[@"customAttributes"]];
    }
    return params;
}
