#import "AdaptyUnityUtils.mm"

static const NSString *tag = @"AdaptyiOSWrapper";

static NSString *Callback_identify = @"OnIdentify";
static NSString *Callback_logout = @"OnLogout";
static NSString *Callback_getPaywalls = @"OnGetPaywalls";
static NSString *Callback_makePurchase = @"OnMakePurchase";
static NSString *Callback_restorePurchases = @"OnRestorePurchases";
static NSString *Callback_validateReceipt = @"OnValidateAppleReceipt";
static NSString *Callback_getPurchaserInfo = @"OnGetPurchaserInfo";
static NSString *Callback_setOnPurchaserInfoUpdatedListener = @"OnPurchaserInfoUpdated";
static NSString *Callback_updateAttribution = @"OnUpdateAttribution";
static NSString *Callback_updateProfile = @"OnUpdateProfile";
static NSString *Callback_getPromo = @"OnGetPromo";
static NSString *Callback_setOnPromoReceivedListener = @"OnPromoReceived";
static NSString *Callback_makeDeferredPurchaseListener = @"OnMakeDeferredPurchase";

@interface Callback : NSObject
@property(retain, nonatomic) NSString *objectName, *method, *message;
@end

@implementation Callback
@end

static NSMutableArray *callbacks = [NSMutableArray array];

static void sendMessage(NSString *objectName, NSString *method, NSString *message) {
    if (objectName) {
        Callback *callback = [Callback alloc];
        callback.objectName = objectName;
        callback.method = method;
        callback.message = message;
        [callbacks addObject:callback];
    }
}

@interface AdaptyDelegateWrapper : NSObject<AdaptyDelegate>
@property(copy, nonatomic) NSString *promoReceivedListener, *purchaserInfoUpdatedListener, *makeDeferredPurchaseListener;
@end

@implementation AdaptyDelegateWrapper
-(void)didReceivePromo:(PromoModel *)promo {
    if (self.promoReceivedListener) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (promo) {
            message[@"promo"] = promoToDictionary(promo);
        }
        sendMessage(self.promoReceivedListener, Callback_setOnPromoReceivedListener, dictionaryToJson(message));
    }
}
-(void)didReceiveUpdatedPurchaserInfo:(PurchaserInfoModel *)purchaserInfo {
    if (self.purchaserInfoUpdatedListener){
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (purchaserInfo) {
            message[@"purchaserInfo"] = purchaserInfoToDictionary(purchaserInfo);
        }
        sendMessage(self.purchaserInfoUpdatedListener, Callback_setOnPurchaserInfoUpdatedListener, dictionaryToJson(message));
    }
}
-(void)paymentQueueWithShouldAddStorePaymentFor:(ProductModel *)product defermentCompletion:(void (^)(void (^ _Nullable)(PurchaserInfoModel * _Nullable, NSString * _Nullable, NSDictionary<NSString *,id> * _Nullable, ProductModel * _Nullable, AdaptyError * _Nullable)))makeDeferredPurchase {
    makeDeferredPurchase(^(PurchaserInfoModel * _Nullable purchaserInfo, NSString * _Nullable receipt, NSDictionary<NSString *,id> * _Nullable appleValidationResult, ProductModel * _Nullable _product, AdaptyError * _Nullable error) {
        if (self.makeDeferredPurchaseListener){
            NSMutableDictionary *message = [NSMutableDictionary dictionary];
            if (purchaserInfo) {
                message[@"purchaserInfo"] = purchaserInfoToDictionary(purchaserInfo);
            }
            if (receipt) {
                message[@"receipt"] = receipt;
            }
            if (appleValidationResult) {
                message[@"validationResult"] = appleValidationResult;
            }
            if (_product) {
                message[@"product"] = productToDictionary(_product);
            }
            if (error) {
                message[@"error"] = adaptyErrorToDictionary(error);
            }
            sendMessage(self.makeDeferredPurchaseListener, Callback_makeDeferredPurchaseListener, dictionaryToJson(message));
        }
    });
}
@end

static AdaptyDelegateWrapper *adaptyDelegateWrapper;

extern "C" bool AdaptyUnity_hasCallback() {
    return callbacks.count > 0;
}

extern "C" const char *AdaptyUnity_getCallbackObjectName() {
    Callback *callback = (Callback *)callbacks.firstObject;
    if (callback) {
        return strdup(callback.objectName.UTF8String);
    }
    return nil;
}

extern "C" const char *AdaptyUnity_getCallbackMethod() {
    Callback *callback = (Callback *)callbacks.firstObject;
    if (callback) {
        return strdup(callback.method.UTF8String);
    }
    return nil;
}

extern "C" const char *AdaptyUnity_getCallbackMessageAndPop() {
    Callback *callback = (Callback *)callbacks.firstObject;
    if (callback) {
        [callbacks removeObjectAtIndex:0];
        return strdup(callback.message.UTF8String);
    }
    return nil;
}

extern "C" void AdaptyUnity_setLogLevel(int logLevel) {
    NSLog(@"%@ setLogLevel()", tag);
    switch (logLevel) {
        case 0:
            [Adapty setLogLevel:AdaptyLogLevelNone];
            break;
            
        case 1:
            [Adapty setLogLevel:AdaptyLogLevelErrors];
            break;
            
        case 2:
            [Adapty setLogLevel:AdaptyLogLevelVerbose];
            break;
    }
}

extern "C" void AdaptyUnity_activate(const char *key, bool observeMode) {
    NSLog(@"%@ activate()", tag);
	[Adapty activate:cstringToString(key) observerMode:observeMode];
    if (adaptyDelegateWrapper == nil) {
        adaptyDelegateWrapper = [[AdaptyDelegateWrapper alloc] init];
        [Adapty setDelegate:adaptyDelegateWrapper];
    }
}

extern "C" void AdaptyUnity_identify(const char *customerUserId, const char *objectName) {
    NSLog(@"%@ identify()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty identify:cstringToString(customerUserId) completion:^(AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_identify, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_logout(const char *objectName) {
    NSLog(@"%@ logout()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty logout:^(AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_logout, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_getPaywalls(const char *objectName) {
    NSLog(@"%@ getPaywalls()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty getPaywalls:^(NSArray<PaywallModel *> * _Nullable paywalls, NSArray<ProductModel *> * _Nullable products, enum DataState state, AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (paywalls) {
            NSMutableArray<NSDictionary *> *array = [[NSMutableArray alloc] init];
            for (int i = 0; i < paywalls.count; ++i) {
                if (paywalls[i]) {
                    [array addObject:paywallToDictionary(paywalls[i])];
                }
            }
            message[@"paywalls"] = array;
        }
        if (products) {
            NSMutableArray<NSDictionary *> *array = [[NSMutableArray alloc] init];
            for (int i = 0; i < products.count; ++i) {
                if (products[i]) {
                    [array addObject:productToDictionary(products[i])];
                }
            }
            message[@"products"] = array;
        }
        if (state == DataStateCached) {
            message[@"state"] = @"cached";
        } else if (state == DataStateSynced) {
            message[@"state"] = @"synced";
        }
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_getPaywalls, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_makePurchase(const char *productJson, const char *offerId, const char *objectName) {
    NSLog(@"%@ makePurchase()", tag);
    NSString *objectNameString = cstringToString(objectName);
    ProductModel *product = dictionaryToProduct(jsonToDictionary(productJson));
    [Adapty makePurchaseWithProduct:product offerId:cstringToString(offerId) completion:^(PurchaserInfoModel * _Nullable purchaserInfo, NSString * _Nullable receipt, NSDictionary<NSString *,id> * _Nullable appleValidationResult, ProductModel * _Nullable _product, AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (purchaserInfo) {
            message[@"purchaserInfo"] = purchaserInfoToDictionary(purchaserInfo);
        }
        if (receipt) {
            message[@"receipt"] = receipt;
        }
        if (appleValidationResult) {
            message[@"validationResult"] = appleValidationResult;
        }
        if (_product) {
            message[@"product"] = productToDictionary(_product);
        }
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_makePurchase, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_restorePurchases(const char *objectName) {
    NSLog(@"%@ restorePurchases()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty restorePurchasesWithCompletion:^(PurchaserInfoModel * _Nullable purchaserInfo, NSString * _Nullable receipt, NSDictionary<NSString *,id> * _Nullable appleValidationResult, AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (purchaserInfo) {
            message[@"purchaserInfo"] = purchaserInfoToDictionary(purchaserInfo);
        }
        if (receipt) {
            message[@"receipt"] = receipt;
        }
        if (appleValidationResult) {
            message[@"validationResults"] = [NSArray arrayWithObject:appleValidationResult];
        }
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_restorePurchases, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_validateReceipt(const char *receipt, const char *objectName) {
    NSLog(@"%@ validateReceipt()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty validateReceipt:cstringToString(receipt) completion:^(PurchaserInfoModel * _Nullable purchaserInfo, NSDictionary<NSString *,id> * _Nullable appleValidationResult, AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (purchaserInfo) {
            message[@"purchaserInfo"] = purchaserInfoToDictionary(purchaserInfo);
        }
        if (appleValidationResult) {
            message[@"validationResult"] = appleValidationResult;
        }
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_validateReceipt, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_getPurchaserInfo(const char *objectName) {
    NSLog(@"%@ getPurchaserInfo()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty getPurchaserInfo:^(PurchaserInfoModel * _Nullable purchaserInfo, DataState state, AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (purchaserInfo) {
            message[@"purchaserInfo"] = purchaserInfoToDictionary(purchaserInfo);
        }
        if (state == DataStateCached) {
            message[@"state"] = @"cached";
        } else if (state == DataStateSynced) {
            message[@"state"] = @"synced";
        }
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_getPurchaserInfo, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_setOnPurchaserInfoUpdatedListener(const char *objectName) {
    NSLog(@"%@ setOnPurchaserInfoUpdatedListener()", tag);
    if (adaptyDelegateWrapper) {
        adaptyDelegateWrapper.purchaserInfoUpdatedListener = cstringToString(objectName);
    }
}

extern "C" void AdaptyUnity_updateAttribution(const char *attributionJson, int sourceInt, const char *networkUserId, const char *objectName) {
    NSLog(@"%@ updateAttribution()", tag);
    NSString *objectNameString = cstringToString(objectName);
    NSDictionary *attribution = jsonToDictionary(attributionJson);
    AttributionNetwork attributionNetwork;
    switch (sourceInt) {
        default:
        case 0:
            attributionNetwork = AttributionNetworkAppsflyer;
            break;
            
        case 1:
            attributionNetwork = AttributionNetworkAdjust;
            break;
            
        case 2:
            attributionNetwork = AttributionNetworkBranch;
            break;
            
        case 3:
            attributionNetwork = AttributionNetworkCustom;
            break;
    }
    [Adapty updateAttribution:attribution source:attributionNetwork networkUserId:cstringToString(networkUserId) completion:^(AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_updateAttribution, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_updateProfile(const char *paramsJson, const char *objectName) {
    NSLog(@"%@ updateProfile()", tag);
    NSString *objectNameString = cstringToString(objectName);
    ProfileParameterBuilder *params = dictionaryToProfileParameterBuilder(jsonToDictionary(paramsJson));
    [Adapty updateProfileWithParams:params completion:^(AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_updateProfile, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_getPromo(const char *objectName) {
    NSLog(@"%@ getPromo()", tag);
    NSString *objectNameString = cstringToString(objectName);
    [Adapty getPromo:^(PromoModel * _Nullable promo, AdaptyError * _Nullable error) {
        NSMutableDictionary *message = [NSMutableDictionary dictionary];
        if (promo) {
            message[@"promo"] = promoToDictionary(promo);
        }
        if (error) {
            message[@"error"] = adaptyErrorToDictionary(error);
        }
        sendMessage(objectNameString, Callback_getPromo, dictionaryToJson(message));
    }];
}

extern "C" void AdaptyUnity_setOnPromoReceivedListener(const char *objectName) {
    NSLog(@"%@ setOnPromoReceivedListener()", tag);
    if (adaptyDelegateWrapper) {
        adaptyDelegateWrapper.promoReceivedListener = cstringToString(objectName);
    }
}
